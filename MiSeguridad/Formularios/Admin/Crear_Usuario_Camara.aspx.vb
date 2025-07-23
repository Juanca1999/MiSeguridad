Imports System.IO
Imports System.Net
Imports ImageMagick
Imports System.Drawing
Imports Newtonsoft.Json
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Drawing.Imaging

Public Class Crear_Usuario_Camara
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session("Usuario") = User.Identity.Name

            Consultar_Info_Usuario()

            If Session("Sucursal_Usuario") Is Nothing Then
                Dim script As String = "swal({
                title: 'OJO!',
                text: 'Debe estar registrado en alguna sucursal',
                type: 'warning',
                allowOutsideClick: false
                }).then(function() {
                    window.location.href = '../../Inicio.aspx';
                });"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If

            ' Cargar la lista de usuarios solo en la primera carga de la página
            Dim allUsers As List(Of Usuarios) = GetUserList()
            Session("Usuarios") = allUsers ' Guardar los usuarios en la sesión
            LlenarListView(allUsers)
        Else
            ' Reutilizar los datos de la sesión en los postbacks
            Dim allUsers As List(Of Usuarios) = TryCast(Session("Usuarios"), List(Of Usuarios))
            If allUsers IsNot Nothing Then
                ' Aplicar filtro si hay un término de búsqueda
                Dim searchTerm As String = TxBuscar.Text.Trim().ToLower()
                If Not String.IsNullOrEmpty(searchTerm) Then
                    allUsers = allUsers.Where(Function(u) u.name.ToLower().Contains(searchTerm)).ToList()
                End If
                LlenarListView(allUsers)
            End If
        End If
    End Sub

    Private Sub Consultar_Info_Usuario()
        Try
            Dim query As String = "SELECT Id_Tercero, Nombres, Id_Sede, Id_Acceso FROM Terceros WHERE Usuario = @Usuario"

            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Usuario", User.Identity.Name)

                    Using dr As SqlDataReader = command.ExecuteReader()
                        If dr.HasRows Then
                            dr.Read()

                            ' Asignación de Id_Sede verificando si es DBNull
                            If IsDBNull(dr("Id_Sede")) Then
                                Session("Sucursal_Usuario") = Nothing
                            Else
                                Session("Sucursal_Usuario") = dr("Id_Sede").ToString()
                            End If
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function Consultar_Control_LPR() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Tiene_LPR FROM Adm_Empresa"
                Using command As New SqlCommand(query, connection)

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado no es NULL y devolver el valor como Boolean
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return Convert.ToBoolean(result)
                    Else
                        ' Si el resultado es NULL, devolver False
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function Consultar_Control_Restaurante_Facial() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Control_Restaurante_Facial FROM Adm_Empresa"
                Using command As New SqlCommand(query, connection)

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado no es NULL y devolver el valor como Boolean
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return Convert.ToBoolean(result)
                    Else
                        ' Si el resultado es NULL, devolver False
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function RegistroExisteEnBD(Placa As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Adm_Vehiculo WHERE Placa = @Placa"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Placa", Placa)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    If count > 0 Then
                        Return True ' El Registro existe en la base de datos
                    Else
                        Return False ' La Registro no existe en la base de datos
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False ' Ocurrió un error al verificar la existencia de la carpeta
        End Try
    End Function

    Private Function ObtenerIdTercero(Cedula As Long) As Long
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE CONVERT(VARCHAR, Cedula) = @Cedula"
        Dim idTercero As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Cedula", Cedula.ToString())

                connection.Open()
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idTercero = Convert.ToInt64(result)
                End If
            End Using
        End Using

        Return idTercero
    End Function

    Private Function ObtenerIdTerceroHIK(Id_HikCamara As Long) As Long
        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Id_HikCamara = @Id_HikCamara"
        Dim idTercero As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Id_HikCamara", Id_HikCamara)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Tercero
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idTercero = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Tercero obtenido
        Return idTercero
    End Function

    ' Clase para mapear los usuarios
    Public Class Usuarios
        Public Property employeeNo As String
        Public Property name As String
        Public Property userType As String
        Public Property gender As String
        Public Property numOfFace As Integer
    End Class

    Private Function GetUserList() As List(Of Usuarios)
        Dim allUsers As New List(Of Usuarios)()

        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Search?format=json"
            Dim maxResultsPerPage As Integer = 30
            Dim totalMatches As Integer = 0
            Dim position As Integer = 0

            Do
                Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                request.Method = "POST"
                request.ContentType = "application/json"
                request.Accept = "application/json"
                request.Credentials = New NetworkCredential("admin", "Miro-321")
                request.PreAuthenticate = False

                Dim jsonBody As String = "
            {
                ""UserInfoSearchCond"": {
                    ""searchID"": ""1"",
                    ""searchResultPosition"": " & position & ",
                    ""maxResults"": " & maxResultsPerPage & ",
                    ""EmployeeNoList"": []
                }
            }"

                Using writer As New StreamWriter(request.GetRequestStream())
                    writer.Write(jsonBody)
                    writer.Flush()
                End Using

                Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseBody As String = reader.ReadToEnd()
                    Dim result = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseBody)
                    Dim userInfoSearch = result("UserInfoSearch")

                    If totalMatches = 0 Then
                        totalMatches = CInt(userInfoSearch("totalMatches"))
                    End If

                    Dim userInfoList = JsonConvert.DeserializeObject(Of List(Of Usuarios))(userInfoSearch("UserInfo").ToString())
                    allUsers.AddRange(userInfoList)
                    position += maxResultsPerPage
                End Using
            Loop While position < totalMatches

        Catch ex As Exception
            allUsers = New List(Of Usuarios)()
        End Try

        Dim searchTerm As String = TxBuscar.Text.Trim().ToLower()
        If Not String.IsNullOrEmpty(searchTerm) Then
            allUsers = allUsers.Where(Function(u) u.name.ToLower().Contains(searchTerm)).ToList()
        End If

        Return allUsers
    End Function

    Private Sub LlenarListView(allUsers As List(Of Usuarios))
        Try
            If allUsers Is Nothing OrElse allUsers.Count = 0 Then
                LvUsuarios.DataSource = Nothing
                LvUsuarios.DataBind()
            Else
                LvUsuarios.DataSource = allUsers
                LvUsuarios.DataBind()
            End If
        Catch ex As Exception
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String = ex.Message
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Protected Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        ' Recuperar la lista completa de usuarios desde la sesión
        Dim allUsers As List(Of Usuarios) = TryCast(Session("Usuarios"), List(Of Usuarios))

        If allUsers IsNot Nothing Then
            ' Filtrar los usuarios por el término de búsqueda
            Dim searchTerm As String = TxBuscar.Text.Trim().ToLower()
            Dim filteredUsers As List(Of Usuarios)

            If String.IsNullOrEmpty(searchTerm) Then
                ' Si no hay término de búsqueda, mostrar todos los usuarios
                filteredUsers = allUsers
            Else
                ' Filtrar la lista por coincidencias en el nombre
                filteredUsers = allUsers.Where(Function(u) u.name.ToLower().Contains(searchTerm)).ToList()
            End If

            ' Guardar los usuarios filtrados en la sesión
            Session("UsuariosFiltrados") = filteredUsers

            ' Llenar el ListView con los usuarios filtrados
            LlenarListView(filteredUsers)
        Else
            ' Manejo de casos donde la sesión esté vacía
            LlenarListView(New List(Of Usuarios)())
        End If
    End Sub

    Protected Sub LvUsuarios_PagePropertiesChanging(sender As Object, e As PagePropertiesChangingEventArgs) Handles LvUsuarios.PagePropertiesChanging
        ' Configura la nueva posición de la página en el DataPager
        Dim pager As DataPager = CType(LvUsuarios.FindControl("DataPager1"), DataPager)
        If pager IsNot Nothing Then
            pager.SetPageProperties(e.StartRowIndex, e.MaximumRows, False)
        End If

        ' Recuperar la lista filtrada o completa desde la sesión
        Dim filteredUsers As List(Of Usuarios) = TryCast(Session("UsuariosFiltrados"), List(Of Usuarios))
        Dim allUsers As List(Of Usuarios) = TryCast(Session("Usuarios"), List(Of Usuarios))

        ' Si hay usuarios filtrados, utiliza esa lista; de lo contrario, usa la lista completa
        If filteredUsers IsNot Nothing Then
            LlenarListView(filteredUsers)
        ElseIf allUsers IsNot Nothing Then
            LlenarListView(allUsers)
        Else
            ' Si ambas listas están vacías, limpiar el ListView
            LlenarListView(New List(Of Usuarios)())
        End If
    End Sub

    Dim errores As New List(Of String)

    Private Sub Consultar_Usuario(ByVal Id As Long)
        Dim query As String = "SELECT T.Id_Tercero, T.Cedula, T.Nombres, T.Id_Rol, T.Correo, T.Usuario, T.Password, T.Cargo, T.Celular, T.Foto, T.Estado, T.Id_Sede, T.Id_Acceso, T.Id_Empresa, T.Id_Responsable, R.Nombres AS Nombre_Responsable, T.Id_HikCamara FROM Terceros T LEFT JOIN Terceros R ON R.Id_Tercero = T.Id_Responsable " &
                          "WHERE T.Id_HikCamara = @Id"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Id", Id)

                conn.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        TxCedula.Text = dr("Cedula").ToString()
                        TxNombres.Text = dr("Nombres").ToString()
                        TxCorreo.Text = If(dr("Correo") IsNot DBNull.Value, dr("Correo").ToString(), String.Empty)
                        TxUsuario.Text = If(dr("Usuario") IsNot DBNull.Value, dr("Usuario").ToString(), String.Empty)
                        TxPassword.Text = If(dr("Password") IsNot DBNull.Value, dr("Password").ToString(), String.Empty)
                        TxCargo.Text = If(dr("Cargo") IsNot DBNull.Value, dr("Cargo").ToString(), String.Empty)
                        TxIdRol.SelectedValue = If(dr("Id_Rol") IsNot DBNull.Value, dr("Id_Rol").ToString(), String.Empty)
                        TxCelular.Text = If(dr("Celular") IsNot DBNull.Value, dr("Celular").ToString(), String.Empty)

                        If dr("Estado").ToString() = "True" Then
                            TxEstado.SelectedValue = "1"
                        ElseIf dr("Estado").ToString() = "False" Then
                            TxEstado.SelectedValue = "0"
                        Else
                            TxEstado.SelectedValue = String.Empty
                        End If
                        TxEmpresa.SelectedValue = If(dr("Id_Empresa") IsNot DBNull.Value, dr("Id_Empresa").ToString(), String.Empty)
                        TxSede.SelectedValue = If(dr("Id_Sede") IsNot DBNull.Value, dr("Id_Sede").ToString(), String.Empty)
                        TxNombreResponsable.Items.Clear()
                        Dim Id_Responsable As String = If(dr("Id_Responsable") IsNot DBNull.Value, dr("Id_Responsable").ToString(), "")
                        Dim Nombre_Responsable As String = If(dr("Nombre_Responsable") IsNot DBNull.Value, dr("Nombre_Responsable").ToString(), "")
                        If Not String.IsNullOrWhiteSpace(Id_Responsable) AndAlso Not String.IsNullOrWhiteSpace(Nombre_Responsable) Then
                            TxNombreResponsable.Items.Add(New ListItem(Nombre_Responsable, Id_Responsable))
                        End If
                        TxNombreResponsable.DataBind()
                        BtGuardar.Text = "ACTUALIZAR"
                    Else
                        TxCedula.Text = String.Empty
                        TxNombres.Text = String.Empty
                        TxCorreo.Text = String.Empty
                        TxPassword.Text = String.Empty
                        TxCargo.Text = String.Empty
                        TxIdRol.SelectedValue = String.Empty
                        TxCelular.Text = String.Empty
                        TxEstado.SelectedValue = String.Empty
                        TxSede.SelectedIndex = 0
                        TxEmpresa.SelectedIndex = 0
                        TxBuscarResponsable.Text = String.Empty
                        TxNombreResponsable.Items.Clear()
                    End If
                End Using
            End Using
        End Using
    End Sub

    Public Shared Function ObtenerEndPoint() As String
        Dim IP_EndPoint As String = String.Empty

        Dim sql As String = "SELECT TOP 1 IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Sede = @Id_Sede ORDER BY Id_Acceso ASC"

        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Sede", HttpContext.Current.Session("Sucursal_Usuario"))
                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            IP_EndPoint = reader("IP_Camara").ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try

        Return IP_EndPoint
    End Function

    <WebMethod>
    Public Shared Function Consultar_Foto(ByVal Id As String) As String
        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/Intelligent/FDLib/FDSearch?format=json"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json"
            request.Credentials = New NetworkCredential("admin", "Miro-321")
            request.PreAuthenticate = False

            ' JSON que se enviará en el cuerpo
            Dim jsonBody As String = "
        {
            ""searchResultPosition"": 0,
            ""maxResults"": 100,
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Id & """
        }"

            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(jsonBody)
            End Using

            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Dim responseText As String = String.Empty
            Using reader As New StreamReader(response.GetResponseStream())
                responseText = reader.ReadToEnd()
            End Using

            Dim jsonResponse As JObject = JObject.Parse(responseText)

            If jsonResponse("statusCode").ToString() = "1" AndAlso jsonResponse("numOfMatches").ToString() = "1" Then
                ' Obtener la URL de la imagen desde el JSON
                Dim faceUrl As String = jsonResponse("MatchList")(0)("faceURL").ToString()

                ' Definir el nombre y la ruta del archivo
                Dim fileName As String = "face_" & Id & ".jpg"
                Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)

                ' Verificar si el archivo ya existe
                If System.IO.File.Exists(serverPath) Then
                    ' Eliminar el archivo existente antes de descargar el nuevo
                    System.IO.File.Delete(serverPath)
                End If

                ' Descargar la nueva imagen
                Dim webClient As New WebClient()
                webClient.Credentials = New NetworkCredential("admin", "Miro-321")

                ' Descargar y guardar la imagen en el servidor
                webClient.DownloadFile(faceUrl, serverPath)

                ' Retornar la URL local del servidor para el frontend
                Return "../../Adjunto/Foto_Usuarios/" & fileName
            Else
                ' Retornar mensaje de error
                Return "Foto no encontrada."
            End If
        Catch ex As Exception
            ' Retornar mensaje de error
            Return "Error: " & ex.Message
        End Try
    End Function

    Private Sub Consultar_Tercero()
        Try
            Dim sqlQuery As String = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Id_Sede = @Sede AND Estado = 1 AND Nombres LIKE '%' + @Nombre + '%' ORDER BY Nombres ASC"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Nombre", TxBuscarResponsable.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxNombreResponsable.DataSource = datos
                            TxNombreResponsable.DataTextField = "Nombres"
                            TxNombreResponsable.DataValueField = "Id_Tercero"
                            TxNombreResponsable.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró la persona con el nombre ingresado, verificar y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarResponsable_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarResponsable.TextChanged
        If TxBuscarResponsable.Text <> "" Then
            Consultar_Tercero()
        Else
            TxNombreResponsable.Items.Clear()
        End If
    End Sub

    Private Sub LvUsuarios_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvUsuarios.ItemDataBound
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim genderLabel As Label = DirectCast(e.Item.FindControl("genderLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Dim numOfFaceLabel As Label = DirectCast(e.Item.FindControl("numOfFaceLabel"), Label)
        Dim Foto As LinkButton = DirectCast(e.Item.FindControl("Foto"), LinkButton)
        Dim Tiene_Vehiculo As ImageButton = DirectCast(e.Item.FindControl("Tiene_Vehiculo"), ImageButton)
        Dim Tiene_Locker As ImageButton = DirectCast(e.Item.FindControl("Tiene_Locker"), ImageButton)
        Dim Tiene_Equipo As ImageButton = DirectCast(e.Item.FindControl("Tiene_Equipo"), ImageButton)
        Dim Tiene_Responsable As ImageButton = DirectCast(e.Item.FindControl("Tiene_Responsable"), ImageButton)

        Foto.Visible = (Convert.ToInt32(numOfFaceLabel.Text) > 0)
        Dim idHikCamara As String = employeeNoLabel.Text

        ' Verificar y actualizar imagenes
        If TieneVehiculo(idHikCamara) Then Tiene_Vehiculo.ImageUrl = "~/Diseno/Imagenes/Verde.png"
        If TieneLocker(idHikCamara) Then Tiene_Locker.ImageUrl = "~/Diseno/Imagenes/Verde.png"
        If TieneEquipo(idHikCamara) Then Tiene_Equipo.ImageUrl = "~/Diseno/Imagenes/Verde.png"
        If TieneResponsable(idHikCamara) Then Tiene_Responsable.ImageUrl = "~/Diseno/Imagenes/Verde.png"
    End Sub

    Private Sub LvUsuarios_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvUsuarios.ItemCommand
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim genderLabel As Label = DirectCast(e.Item.FindControl("genderLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Dim numOfFaceLabel As Label = DirectCast(e.Item.FindControl("numOfFaceLabel"), Label)
        Session("Id_Empleado") = employeeNoLabel.Text
        If e.CommandName = "Editar" Then
            TxTipoUsuario.SelectedValue = userTypelabel.Text
            TxGenero.SelectedValue = genderLabel.Text
            Consultar_Usuario(employeeNoLabel.Text)
            Consultar_Vehiculos(employeeNoLabel.Text)
            Consultar_Lockers(employeeNoLabel.Text)
            Consultar_Equipos(employeeNoLabel.Text)
        End If
    End Sub

    Private Sub Consultar_Vehiculos(ByVal Id As String)
        Vehiculos.Visible = True
        SqlVehiculosAsignados.SelectCommand = "SELECT VP.Id_Tercero, VP.Placa , TV.Tipo_Vehiculo, V.Marca, V.Modelo, V.Color FROM Adm_Vehiculo_Persona VP LEFT JOIN Adm_Vehiculo V ON V.Placa = VP.Placa LEFT JOIN Adm_Tipo_Vehiculo TV ON TV.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo LEFT JOIN Terceros T ON T.Id_Tercero = VP.Id_Tercero WHERE V.Estado = 1 AND T.Id_HikCamara = " + Id + " "
        LvVehiculosAsignados.DataBind()
    End Sub

    Private Sub Consultar_Lockers(ByVal Id As String)
        Lockers.Visible = True
        SqlLockersAsignados.SelectCommand = "SELECT TL.Id_Tercero, T.Cedula, T.Nombres, TL.Id_Locker, V.Nombre_Vestier FROM Adm_Tercero_Locker TL INNER JOIN Terceros T ON T.Id_Tercero = TL.Id_Tercero INNER JOIN Adm_Lockers L ON L.Id_Locker = TL.Id_Locker AND L.Id_Vestier = TL.Id_Vestier INNER JOIN Adm_Vestier V ON V.Id_Vestier = L.Id_Vestier WHERE L.Estado = 1 AND T.Id_HikCamara = " + Id + " "
        LvLockerAsignados.DataBind()
    End Sub

    Private Sub Consultar_Equipos(ByVal Id As String)
        Equipos.Visible = True
        SqlEquipos.SelectCommand = "SELECT TE.Detalle_Equipo, TE.Marca, TE.Serial FROM Adm_Tercero_Equipo TE LEFT JOIN Terceros T ON T.Id_Tercero = TE.Id_Tercero WHERE T.Id_HikCamara = " + Id + " "
        LvEquipos.DataBind()
    End Sub

    Private Function TieneVehiculo(idHikCamara As String) As Boolean
        Dim query As String = "
        SELECT TOP 1 VP.Id_Tercero 
        FROM Adm_Vehiculo_Persona VP
        LEFT JOIN Terceros T ON T.Id_Tercero = VP.Id_Tercero 
        WHERE T.Id_HikCamara = @Id"
        Return ExisteDato(query, idHikCamara)
    End Function

    Private Function TieneLocker(idHikCamara As String) As Boolean
        Dim query As String = "
        SELECT TOP 1 TL.Id_Tercero 
        FROM Adm_Tercero_Locker TL
        LEFT JOIN Terceros T ON T.Id_Tercero = TL.Id_Tercero 
        WHERE T.Id_HikCamara = @Id"
        Return ExisteDato(query, idHikCamara)
    End Function

    Private Function TieneEquipo(idHikCamara As String) As Boolean
        Dim query As String = "
        SELECT TOP 1 TE.Id_Tercero 
        FROM Adm_Tercero_Equipo TE
        LEFT JOIN Terceros T ON T.Id_Tercero = TE.Id_Tercero 
        WHERE T.Id_HikCamara = @Id"
        Return ExisteDato(query, idHikCamara)
    End Function

    Private Function TieneResponsable(idHikCamara As String) As Boolean
        Dim query As String = "
        SELECT Id_Responsable 
        FROM Terceros 
        WHERE Id_HikCamara = @Id"
        Dim result = EjecutarEscalar(query, idHikCamara)
        Return result IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(result.ToString())
    End Function

    Private Function ExisteDato(query As String, idHikCamara As String) As Boolean
        Dim result = EjecutarEscalar(query, idHikCamara)
        Return result IsNot Nothing
    End Function

    Private Function EjecutarEscalar(query As String, idHikCamara As String) As Object
        Dim connStr As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()
        Using conn As New SqlConnection(connStr)
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Id", idHikCamara)
                conn.Open()
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 1")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras:" & ex.Message)
        End Try
        Return ipCamaras
    End Function

    Private Function ObtenerIPCamaraDesdeBaseDeDatosAccesos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 1 AND Id_Inmueble <> 'RESTAURANTE'")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras:" & ex.Message)
        End Try
        Return ipCamaras
    End Function

    Private Function ObtenerIPCamaraDesdeBaseDeDatosRestaurante() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 1 AND Id_Inmueble = 'RESTAURANTE'")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras:" & ex.Message)
        End Try
        Return ipCamaras
    End Function

    ' Método para ejecutar una consulta SQL y retornar un DataTable
    Private Function EjecutarConsultaSQL(query As String) As DataTable
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    Try
                        conn.Open()
                        da.Fill(dt)
                    Catch ex As Exception
                        errores.Add("Error al ejecutar la consulta SQL:" & ex.Message)
                    End Try
                    Return dt
                End Using
            End Using
        End Using
    End Function

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Consultar_Id_HikCamara()
        Dim Valor As Integer = ObtenerMaxId("Id_HikCamara", "Terceros")
        Session("Id_HikCamara") = Valor + 1
    End Sub

    Private Sub Consultar_Id_Empresa()
        Dim Valor As Integer = ObtenerMaxId("Id_Empresa", "Empresas")
        Session("Id_Empresa") = Valor + 1
    End Sub

    Private Function ObtenerMaxId(campo As String, tabla As String) As Integer
        Dim Valor As Integer = 0
        Dim query As String = $"SELECT MAX({campo}) FROM {tabla}"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Dim resultado = cmd.ExecuteScalar()
                If resultado IsNot Nothing AndAlso Not IsDBNull(resultado) Then
                    Valor = Convert.ToInt32(resultado)
                End If
            End Using
        End Using

        Return Valor
    End Function

    Private Sub Guardar_Empresa()
        Try
            Dim query As String = "
        INSERT INTO dbo.Empresas 
        (Id_Empresa, Empresa) 
        VALUES 
        (@Id_Empresa, @Empresa)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Empresa", Session("Id_Empresa"))
                    cmd.Parameters.AddWithValue("@Empresa", TxNombreEmpresa.Text.ToUpper())

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Guardar_Persona()
        Try
            Dim query As String = "
        INSERT INTO dbo.Terceros 
        (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Empresa, Id_Responsable, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Empresa, @Id_Responsable, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                    cmd.Parameters.AddWithValue("@Cedula", If(String.IsNullOrWhiteSpace(TxCedula.Text.Trim()), DBNull.Value, TxCedula.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", If(IsNumeric(TxIdRol.SelectedValue), Convert.ToInt32(TxIdRol.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Correo", If(String.IsNullOrWhiteSpace(TxCorreo.Text), DBNull.Value, TxCorreo.Text))
                    cmd.Parameters.AddWithValue("@Usuario", If(String.IsNullOrWhiteSpace(TxUsuario.Text), DBNull.Value, TxUsuario.Text))
                    cmd.Parameters.AddWithValue("@Password", If(String.IsNullOrWhiteSpace(TxPassword.Text), DBNull.Value, TxPassword.Text))
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(TxCargo.Text), DBNull.Value, TxCargo.Text))
                    cmd.Parameters.AddWithValue("@Celular", If(String.IsNullOrWhiteSpace(TxCelular.Text), DBNull.Value, TxCelular.Text))
                    cmd.Parameters.AddWithValue("@Id_Empresa", If(IsNumeric(TxEmpresa.SelectedValue), Convert.ToInt32(TxEmpresa.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Id_Responsable", If(IsNumeric(TxNombreResponsable.SelectedValue), Convert.ToInt32(TxNombreResponsable.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(IsNumeric(TxSede.SelectedValue), Convert.ToInt32(TxSede.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Estado", If(IsNumeric(TxEstado.SelectedValue), Convert.ToInt32(TxEstado.SelectedValue), DBNull.Value))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Id_Hikvision(Id_HikCamara As String, Id_Tercero As String)
        Dim sql As String = "UPDATE Terceros SET Estado = 1, Id_HikCamara = @Id WHERE Id_Tercero = @Id_Tercero"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id", Id_HikCamara)
                cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Crear_Usuario()

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & TxTipoUsuario.SelectedValue & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": ""1"",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al crear usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Crear_Usuario_Accesos()

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatosAccesos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & TxTipoUsuario.SelectedValue & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": ""1"",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al crear usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Crear_Usuario_Restaurante_Sin_Acceso()

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatosRestaurante()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & TxTipoUsuario.SelectedValue & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": """",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al crear usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Agregar_Foto_Persona_Archivo()

        ' Rutas temporales
        Dim rutaEntrada As String = Path.Combine(Server.MapPath("../../Adjunto/Temporal/"), Session("Id_HikCamara").ToString() & Path.GetExtension(FlFoto.FileName))
        Dim rutaReducida As String = Path.Combine(Server.MapPath("../../Adjunto/Temporal/"), "Reducida_" & Session("Id_HikCamara").ToString() & Path.GetExtension(FlFoto.FileName))

        ' Guardar el archivo original en la ruta temporal
        Try
            FlFoto.SaveAs(rutaEntrada)

            ' Reducir el peso del archivo
            ReducirPesoImagen(rutaEntrada, rutaReducida)
        Catch ex As Exception
            errores.Add("Error al guardar o reducir la imagen: " & ex.Message)
            Exit Sub
        End Try

        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_HikCamara").ToString & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo desde la ruta reducida)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                         "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                         "Content-Type: {2}" & vbCrLf & vbCrLf,
                                         boundary,
                                         Path.GetFileName(rutaReducida),
                                         "image/jpeg")

                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo desde la ruta reducida
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaReducida)

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Archivo") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Archivo") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next

        ' Eliminar los archivos temporales
        Try
            If File.Exists(rutaEntrada) Then File.Delete(rutaEntrada)
            If File.Exists(rutaReducida) Then File.Delete(rutaReducida)
        Catch ex As Exception
            errores.Add("Error al eliminar los archivos temporales: " & ex.Message)
        End Try
    End Sub

    Private Sub Agregar_Foto_Persona_Tomada()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_HikCamara").ToString() & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: image/png" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  Path.GetFileName(Session("RutaImagen").ToString()))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo desde la ruta guardada
                Dim rutaArchivo As String = Session("RutaImagen").ToString()
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaArchivo)

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Tomada") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Tomada") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Sub Actualizar_Foto_Persona_Archivo()

        ' Validar extensión permitida
        Dim extensionesPermitidas As String() = {".jpg", ".jpeg", ".png"}
        If Not extensionesPermitidas.Contains(Path.GetExtension(FlFoto.FileName).ToLower()) Then
            errores.Add("Formato de archivo no permitido. Solo se aceptan imágenes en formato JPG o PNG.")
            Exit Sub
        End If

        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Rutas temporales
        Dim rutaEntrada As String = Path.Combine(Server.MapPath("../../Adjunto/Temporal/"), Session("Id_Empleado").ToString() & Path.GetExtension(FlFoto.FileName))
        Dim rutaReducida As String = Path.Combine(Server.MapPath("../../Adjunto/Temporal/"), "Reducida_" & Session("Id_Empleado").ToString() & Path.GetExtension(FlFoto.FileName))

        ' Guardar el archivo original en la ruta temporal
        Try
            FlFoto.SaveAs(rutaEntrada)

            ' Reducir el peso del archivo
            ReducirPesoImagen(rutaEntrada, rutaReducida)
        Catch ex As Exception
            errores.Add("Error al guardar o reducir la imagen: " & ex.Message)
            Exit Sub
        End Try

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FDSetUp?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_Empleado").ToString & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo reducido)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                     "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                     "Content-Type: image/png" & vbCrLf & vbCrLf,
                                                     boundary,
                                                     Path.GetFileName(rutaReducida))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo reducido desde la ruta guardada
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaReducida)

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Actualizar_Archivo") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Actualizar_Archivo") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next

        ' Eliminar los archivos temporales
        Try
            If File.Exists(rutaEntrada) Then File.Delete(rutaEntrada)
            If File.Exists(rutaReducida) Then File.Delete(rutaReducida)
        Catch ex As Exception
            errores.Add("Error al eliminar los archivos temporales: " & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Foto_Persona_Tomada()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FDSetUp?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_Empleado").ToString() & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: image/png" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  Path.GetFileName(Session("RutaImagen").ToString()))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo desde la ruta guardada
                Dim rutaArchivo As String = Session("RutaImagen").ToString()
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaArchivo)

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Actualizar_Tomada") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Actualizar_Tomada") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Sub GuardarImagen()
        Try
            ' Obtener la cadena Base64
            Dim base64Foto As String = base64image.Value

            ' Validar si la imagen corresponde al valor por defecto
            If String.IsNullOrEmpty(base64Foto) Then
                Throw New Exception("No se puede guardar la imagen predeterminada. Seleccione una imagen válida.")
            End If

            ' Definir la ruta de la carpeta
            Dim rutaCarpeta As String = Server.MapPath("../../Adjunto/Foto_Usuarios/")

            ' Nombre único para el archivo
            Dim nombreArchivo As String = TxCedula.Text & ".png"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, nombreArchivo)

            ' Verificar si el archivo ya existe
            If File.Exists(rutaArchivo) Then
                ' Eliminar el archivo existente antes de guardar el nuevo
                File.Delete(rutaArchivo)
            End If

            ' Convertir Base64 a archivo y guardar temporalmente
            Dim imageBytes As Byte() = Convert.FromBase64String(base64Foto.Replace("data:image/png;base64,", ""))
            Dim rutaArchivoTemp As String = Path.Combine(rutaCarpeta, "temp_" & nombreArchivo)
            File.WriteAllBytes(rutaArchivoTemp, imageBytes)

            ' Reducir el peso de la imagen y guardar la versión optimizada
            ReducirPesoImagenTomada(rutaArchivoTemp, rutaArchivo)

            ' Eliminar la imagen temporal
            If File.Exists(rutaArchivoTemp) Then
                File.Delete(rutaArchivoTemp)
            End If

            ' Guardar la ruta del archivo en la sesión
            Session("RutaImagen") = rutaArchivo
        Catch ex As Exception
            errores.Add("Error en GuardarImagen()" & ex.Message)
        End Try
    End Sub

    Private Sub Editar_Usuario()
        Try
            ' Obtener estado y tipo de usuario
            Dim estadoNuevo As String = TxEstado.SelectedValue
            Dim tipo_Usuario As String = If(estadoNuevo = "0", "blackList", TxTipoUsuario.SelectedValue)

            ' Obtener lista de cámaras
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()
            Dim employeeNo As String = Session("Id_Empleado").ToString()

            For Each ipCamara As String In ipCamaras
                ' Verificar si el usuario existe en la cámara antes de modificar
                If Verificar_Si_Existe(ipCamara, employeeNo) Then
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    requestDocument.Credentials = New NetworkCredential("admin", "Miro-321")
                    requestDocument.PreAuthenticate = True

                    ' JSON con datos del usuario
                    Dim jsonBody As String = "
                {
                   ""UserInfo"": {
                      ""employeeNo"": """ & employeeNo & """,
                      ""name"": """ & TxNombres.Text.ToUpper() & """,
                      ""gender"": """ & TxGenero.SelectedValue & """,
                      ""userType"": """ & tipo_Usuario & """,
                       ""Valid"": {
                         ""enable"": true,
                         ""beginTime"": ""2024-12-04T00:00:00"",
                         ""endTime"": ""2030-12-31T23:59:59""
                      },
                      ""doorRight"": ""1"",
                      ""RightPlan"": [
                         {
                            ""doorNo"": 1,
                            ""planTemplateNo"": ""1""
                         }
                      ]
                   }
                }"

                    ' Enviar solicitud PUT
                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                    requestDocument.ContentLength = byteArray.Length
                    Using dataStream As Stream = requestDocument.GetRequestStream()
                        dataStream.Write(byteArray, 0, byteArray.Length)
                    End Using

                    Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseJson As String = streamReader.ReadToEnd()
                        Dim jsonResponse As JObject = JObject.Parse(responseJson)
                        Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                    End Using
                Else
                    ' Usuario no existe en esta cámara, puedes registrar log si quieres
                    Continue For
                End If
            Next

        Catch ex As Exception
            Session("Status_Code_Editar") = 0
            errores.Add("Error en Editar_Usuario(): " & ex.Message)
        End Try
    End Sub

    Private Function Verificar_Si_Existe(IP_EndPoint As String, Id_Empleado As String) As Boolean
        Try
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Search?format=json"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json"
            request.Credentials = New NetworkCredential("admin", "Miro-321")
            request.PreAuthenticate = False

            ' JSON que se enviará en el cuerpo
            Dim jsonBody As String = "
        {
            ""UserInfoSearchCond"": {
                ""searchID"": """ & Id_Empleado & """,
                ""searchResultPosition"": 0,
                ""maxResults"": 200,
                ""EmployeeNoList"": [
                    { ""employeeNo"": """ & Id_Empleado & """ }
                ]
            }
        }"

            ' Escribir el JSON en el cuerpo de la solicitud
            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(jsonBody)
                writer.Flush()
            End Using

            ' Obtener y procesar la respuesta
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Using reader As New StreamReader(response.GetResponseStream())
                Dim responseBody As String = reader.ReadToEnd()

                ' Deserializar la respuesta
                Dim result = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseBody)
                Dim userInfoSearch = result("UserInfoSearch")

                ' Verificar si hay usuarios
                If userInfoSearch("numOfMatches") = 0 Then
                    Return False ' Usuario no encontrado
                Else
                    Return True ' Usuario encontrado
                End If
            End Using
        Catch ex As Exception
            Return False ' Ocurrió un error al verificar la existencia
        End Try
    End Function

    Private Sub Editar_Usuario_Accesos()
        Try
            ' Obtener directamente el estado seleccionado: 0 = inactivo, 1 = activo
            Dim estadoNuevo As String = TxEstado.SelectedValue
            Dim tipo_Usuario As String = If(estadoNuevo = "0", "blackList", TxTipoUsuario.SelectedValue)

            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatosAccesos()

            For Each ipCamara As String In ipCamaras
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_Empleado").ToString & """,
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & tipo_Usuario & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": ""1"",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                End Using
            Next

        Catch ex As Exception
            Session("Status_Code_Editar") = 0
            errores.Add("Error en Editar_Usuario()" & ex.Message)
        End Try
    End Sub

    Private Sub Editar_Usuario_Restaurante_Sin_Acceso()
        Try
            ' Obtener directamente el estado seleccionado: 0 = inactivo, 1 = activo
            Dim estadoNuevo As String = TxEstado.SelectedValue
            Dim tipo_Usuario As String = If(estadoNuevo = "0", "blackList", TxTipoUsuario.SelectedValue)

            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatosRestaurante()

            For Each ipCamara As String In ipCamaras
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_Empleado").ToString & """,
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & tipo_Usuario & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": """",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                End Using
            Next

        Catch ex As Exception
            Session("Status_Code_Editar") = 0
            errores.Add("Error en Editar_Usuario()" & ex.Message)
        End Try
    End Sub

    Private Sub Eliminar_Usuario(ByVal Id As String)
        Try
            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

            For Each ipCamara As String In ipCamaras
                ' Verificar si el usuario existe en esta cámara antes de intentar eliminarlo
                If Verificar_Si_Existe(ipCamara, Id) Then
                    ' Construir la URL de eliminación
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Delete?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    requestDocument.Credentials = New NetworkCredential("admin", "Miro-321")
                    requestDocument.PreAuthenticate = True

                    ' JSON para eliminar usuario
                    Dim jsonBody As String = "
                {
                   ""UserInfoDelCond"": {
                      ""EmployeeNoList"": [
                         {
                            ""employeeNo"": """ & Id & """
                         }
                      ]
                   }
                }"

                    ' Enviar solicitud
                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                    requestDocument.ContentLength = byteArray.Length

                    Using dataStream As Stream = requestDocument.GetRequestStream()
                        dataStream.Write(byteArray, 0, byteArray.Length)
                    End Using

                    Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseJson As String = streamReader.ReadToEnd()
                        Dim jsonResponse As JObject = JObject.Parse(responseJson)
                        Session("Status_Code_Eliminar") = jsonResponse("statusCode").ToString()
                    End Using
                Else
                    ' Usuario no existe en esta cámara, continuar con la siguiente
                    Continue For
                End If
            Next

        Catch ex As Exception
            errores.Add("Error en Eliminar_Usuario(): " & ex.Message)
        End Try
    End Sub

    Private Sub Inactivar_Persona(ByVal Id As Long)
        Try
            Dim query As String = "UPDATE Terceros SET Estado = 0, Id_HikCamara = NULL WHERE Id_HikCamara = @Id"

            ' Configuración de la conexión a la base de datos
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id", Id)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error en Inactivar_Persona()" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Persona()
        Try
            Dim query As New System.Text.StringBuilder("UPDATE dbo.Terceros SET ")

            query.Append("Nombres = @Nombres, ")

            If Not String.IsNullOrEmpty(TxCorreo.Text) Then
                query.Append("Correo = @Correo, ")
            Else
                query.Append("Correo = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxIdRol.SelectedValue) Then
                query.Append("Id_Rol = @IdRol, ")
            Else
                query.Append("Id_Rol = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxUsuario.Text) Then
                query.Append("Usuario = @Usuario, ")
            Else
                query.Append("Usuario = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxPassword.Text) Then
                query.Append("Password = @Password, ")
            Else
                query.Append("Password = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxCargo.Text) Then
                query.Append("Cargo = @Cargo, ")
            Else
                query.Append("Cargo = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxCelular.Text) Then
                query.Append("Celular = @Celular, ")
            Else
                query.Append("Celular = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxEmpresa.SelectedValue) Then
                query.Append("Id_Empresa = @Id_Empresa, ")
            Else
                query.Append("Id_Empresa = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxNombreResponsable.SelectedValue) Then
                query.Append("Id_Responsable = @Id_Responsable, ")
            Else
                query.Append("Id_Responsable = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxSede.SelectedValue) Then
                query.Append("Id_Sede = @IdSede, ")
            Else
                query.Append("Id_Sede = NULL, ")
            End If

            query.Append("Estado = @Estado ")
            query.Append("WHERE Id_HikCamara = @Id_HikCamara")

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query.ToString(), conn)
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())

                    If Not String.IsNullOrEmpty(TxCorreo.Text) Then
                        cmd.Parameters.AddWithValue("@Correo", TxCorreo.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxIdRol.SelectedValue) Then
                        cmd.Parameters.AddWithValue("@IdRol", TxIdRol.SelectedValue)
                    End If

                    If Not String.IsNullOrEmpty(TxUsuario.Text) Then
                        cmd.Parameters.AddWithValue("@Usuario", TxUsuario.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxPassword.Text) Then
                        cmd.Parameters.AddWithValue("@Password", TxPassword.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxCargo.Text) Then
                        cmd.Parameters.AddWithValue("@Cargo", TxCargo.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxCelular.Text) Then
                        cmd.Parameters.AddWithValue("@Celular", TxCelular.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxEmpresa.SelectedValue) Then
                        cmd.Parameters.AddWithValue("@Id_Empresa", TxEmpresa.SelectedValue)
                    End If

                    If Not String.IsNullOrEmpty(TxNombreResponsable.SelectedValue) Then
                        cmd.Parameters.AddWithValue("@Id_Responsable", TxNombreResponsable.SelectedValue)
                    End If

                    If Not String.IsNullOrEmpty(TxSede.SelectedValue) Then
                        cmd.Parameters.AddWithValue("@IdSede", TxSede.SelectedValue)
                    End If

                    cmd.Parameters.AddWithValue("@Estado", TxEstado.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_Empleado"))

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error en Actualizar_Persona()" & ex.Message)
        End Try
    End Sub

    Public Sub ReducirPesoImagen(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            ' Cargar la imagen con MagickImage
            Using imagen As New MagickImage(rutaEntrada)
                Dim maxAncho As UInteger = 450
                Dim maxAlto As UInteger = 450

                ' Redimensionar la imagen manteniendo la proporción (Fill, Fit, o ajusta según tu necesidad)
                imagen.Resize(New MagickGeometry(maxAncho, maxAlto) With {.IgnoreAspectRatio = False})

                ' Ajustar la calidad de la compresión
                imagen.Quality = 70 ' 70% de calidad

                ' Guardar la imagen comprimida
                imagen.Write(rutaSalida)
            End Using
        Catch ex As Exception
            errores.Add("Error en ReducirPesoImagen()" & ex.Message)
        End Try
    End Sub

    Private Sub ReducirPesoImagenTomada(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            Using imgOriginal As Bitmap = New Bitmap(rutaEntrada)
                ' Definir la calidad de compresión
                Dim codecInfo As ImageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg)
                Dim qualityParam As New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L) ' 80% de calidad
                Dim encoderParams As New EncoderParameters(1)
                encoderParams.Param(0) = qualityParam

                ' Guardar la imagen con el nivel de compresión especificado
                imgOriginal.Save(rutaSalida, codecInfo, encoderParams)
            End Using
        Catch ex As Exception
            errores.Add("Error en ReducirPesoImagenTomada()" & ex.Message)
        End Try
    End Sub

    Private Function GetEncoderInfo(ByVal format As ImageFormat) As ImageCodecInfo
        For Each codec As ImageCodecInfo In ImageCodecInfo.GetImageEncoders()
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    Private Sub BtCrearEmpresa_Click(sender As Object, e As EventArgs) Handles BtCrearEmpresa.Click
        If TxNombreEmpresa.Text <> "" Then
            Consultar_Id_Empresa()
            Guardar_Empresa()

            If errores.Count = 0 Then
                Dim script As String = "swal('Excelente!', 'Empresa Guardada Correctamente.', 'success');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                TxEmpresa.Items.Clear()
                TxEmpresa.Items.Add(New ListItem("Empresa", ""))
                TxEmpresa.DataBind()
            Else
                Dim mensajeError As String = String.Join("<br/>", errores)
                Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        End If
    End Sub

    Protected Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click
        ' Obtener el ID del empleado desde el HiddenField
        Dim idEmpleado As String = HiddenEmployeeNo.Value

        ' Validar que el ID no esté vacío
        If Not String.IsNullOrEmpty(idEmpleado) Then
            Eliminar_Usuario(idEmpleado)
            If Session("Status_Code_Eliminar") = "1" Then
                Inactivar_Persona(idEmpleado)
            End If

            If errores.Count = 0 And Session("Status_Code_Eliminar") = "1" Then
                Dim script As String = "swal('Excelente!', 'Usuario eliminado correctamente.', 'success');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")
            Else
                Dim mensajeError As String = String.Join("<br/>", errores)
                Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            ' Manejo en caso de que no haya un valor
            Dim script As String = "swal('Error!', 'No se pudo identificar el empleado.', 'error');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If
    End Sub

    Private Function ObtenerIPLPRDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 2")
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras: " & ex.Message)
        End Try
        Return ipCamaras
    End Function

    Private Function Crear_Placa() As Boolean
        Dim ipCamaras As List(Of String) = ObtenerIPLPRDesdeBaseDeDatos()
        Dim todasExitosas As Boolean = True

        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Traffic/channels/1/licensePlateAuditData/record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                Dim jsonBody As String = "
            {
                ""LicensePlateInfoList"": [
                    {
                        ""id"": ""1"",
                        ""listType"": ""allowList"",
                        ""LicensePlate"": """ & TxPlaca.Text.Trim.ToUpper() & """
                    }
                ]
            }"

                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                requestDocument.ContentLength = byteArray.Length

                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Dim statusCode As String = jsonResponse("statusCode").ToString()
                    ' Aquí puedes guardar el resultado por IP si lo deseas
                    If statusCode <> "1" Then
                        errores.Add("Cámara " & ipCamara & " no aceptó la placa. Código: " & statusCode)
                        todasExitosas = False
                    End If
                End Using

            Catch ex As Exception
                errores.Add("Error en cámara " & ipCamara & ": " & ex.Message)
                todasExitosas = False
            End Try
        Next

        ' Al final del recorrido, retornamos si todo fue exitoso
        Return todasExitosas
    End Function

    Private Sub Guardar_Vehiculo_Persona(ByVal Id_Tercero As String)
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Vehiculo_Persona 
        (Id_Tercero, Placa, Id_Sede) 
        VALUES 
        (@Id_Tercero, @Placa, @Id_Sede)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Vehiculo_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub Guardar_Vehiculo()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Vehiculo 
        (Placa, Id_Tipo_Vehiculo, Marca, Modelo, Color, Estado) 
        VALUES 
        (@Placa, @Id_Tipo_Vehiculo, @Marca, @Modelo, @Color, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Tipo_Vehiculo", TxTipoVehiculo.SelectedValue)

                    cmd.Parameters.AddWithValue("@Marca", If(String.IsNullOrWhiteSpace(TxMarca.Text), DBNull.Value, TxMarca.Text.ToUpper()))
                    cmd.Parameters.AddWithValue("@Modelo", If(String.IsNullOrWhiteSpace(TxModelo.Text), DBNull.Value, TxModelo.Text.ToUpper()))
                    cmd.Parameters.AddWithValue("@Color", If(String.IsNullOrWhiteSpace(TxColor.Text), DBNull.Value, TxColor.Text.ToUpper()))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Vehiculo(): " & ex.Message)
        End Try
    End Sub

    Private Sub BtGuardarVehiculo_Click(sender As Object, e As EventArgs) Handles BtGuardarVehiculo.Click
        Dim idEmpleado As String = HiddenFieldVehiculo.Value

        ' Validar que el ID no esté vacío
        If Not String.IsNullOrEmpty(idEmpleado) Then

            Dim IdTercero As Long = ObtenerIdTerceroHIK(idEmpleado)

            Dim vehiculoCreado As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            If RegistroExisteEnBD(TxPlaca.Text.Trim.ToUpper()) Then
                mensaje = "Este vehiculo ya existe en el sistema."
                script = $"swal('OJO!', '{mensaje}', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "alerta", script, True)
                Exit Sub
            End If

            ' Paso 1: Crear placa
            If Consultar_Control_LPR() Then
                If Not Crear_Placa() Then
                    mensaje = "No se pudo registrar la placa en la cámara Hikvision."
                    script = $"swal('Error!', '{mensaje}', 'error');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "errorHikvision", script, True)
                    Exit Sub
                End If
            End If

            ' Paso 2: Guardar en BD si placa fue creada exitosamente
            Guardar_Vehiculo()
            Guardar_Vehiculo_Persona(IdTercero)

            ' Verificar si hubo errores
            If errores.Count = 0 Then
                mensaje = "Vehículo creado correctamente."
                script = $"swal('Excelente!', '{mensaje}', 'success');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "ok", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")
            Else
                mensaje = String.Join("<br/>", errores)
                script = $"swal('Error!', '{mensaje}', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "errorBD", script, True)
            End If
        Else
            ' Manejo en caso de que no haya un valor
            Dim script As String = "swal('Error!', 'No se pudo identificar la persona.', 'error');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Buscar_Locker()
        Try
            If String.IsNullOrEmpty(TxVestier.SelectedValue) Then
                TxLocker.Items.Clear()
                TxLocker.Items.Add(New ListItem("Locker", ""))
                Return
            End If

            Dim sqlQuery As String = "SELECT L.Id_Locker FROM Adm_Lockers L LEFT JOIN Adm_Tercero_Locker TL ON TL.Id_Locker = L.Id_Locker WHERE L.Id_Vestier = @Id_Vestier AND TL.Id_Tercero IS NULL AND L.Id_Locker LIKE @Locker ORDER BY CAST(L.Id_Locker AS INT) ASC"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Id_Vestier", TxVestier.SelectedValue)
                    cmd.Parameters.AddWithValue("@Locker", "%" & TxBuscarLocker.Text.Trim() & "%")
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)

                            TxLocker.Items.Clear()
                            TxLocker.Items.Add(New ListItem("Locker", ""))

                            TxLocker.DataSource = datos
                            TxLocker.DataTextField = "Id_Locker"
                            TxLocker.DataValueField = "Id_Locker"
                            TxLocker.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró el locker ingresado, verificar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub Buscar_Lockers_Vestier()
        If String.IsNullOrEmpty(TxVestier.SelectedValue) Then
            TxLocker.Items.Clear()
            TxLocker.Items.Add(New ListItem("Locker", ""))
            Return
        End If

        Dim sqlQuery As String = "SELECT L.Id_Locker FROM Adm_Lockers L LEFT JOIN Adm_Tercero_Locker TL ON TL.Id_Locker = L.Id_Locker WHERE L.Id_Vestier = @Id_Vestier AND TL.Id_Tercero IS NULL ORDER BY CAST(L.Id_Locker AS INT) ASC"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(sqlQuery, conn)
                cmd.Parameters.AddWithValue("@Id_Vestier", TxVestier.SelectedValue)

                Using mda As New SqlDataAdapter(cmd)
                    Using datos As New DataTable
                        mda.Fill(datos)

                        TxLocker.Items.Clear()
                        TxLocker.Items.Add(New ListItem("Locker", ""))

                        TxLocker.DataSource = datos
                        TxLocker.DataTextField = "Id_Locker"
                        TxLocker.DataValueField = "Id_Locker"
                        TxLocker.DataBind()
                    End Using
                End Using
            End Using
        End Using
    End Sub

    Private Sub TxVestierSelectedIndexChanged(sender As Object, e As EventArgs) Handles TxVestier.SelectedIndexChanged
        Buscar_Lockers_Vestier()
    End Sub

    Private Sub TxBuscarLocker_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarLocker.TextChanged
        Buscar_Locker()
    End Sub

    Private Sub Asignar_Persona_Locker(ByVal Id_Tercero As String)

        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Tercero_Locker 
        (Id_Tercero, Id_Locker, Id_Vestier) 
        VALUES 
        (@Id_Tercero, @Id_Locker, @Id_Vestier)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)
                    cmd.Parameters.AddWithValue("@Id_Locker", TxLocker.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_Vestier", TxVestier.SelectedValue)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Asignar_Persona_Locker(): " & ex.Message)
        End Try
    End Sub

    Private Sub BtAsignarLocker_Click(sender As Object, e As EventArgs) Handles BtAsignarLocker.Click
        Dim idEmpleado As String = HiddenFieldLocker.Value

        ' Validar que el ID no esté vacío
        If Not String.IsNullOrEmpty(idEmpleado) Then

            Dim IdTercero As Long = ObtenerIdTerceroHIK(idEmpleado)

            Asignar_Persona_Locker(IdTercero)

            If errores.Count = 0 Then
                Dim script As String = String.Format("swal('Excelente!', 'Locker Asignado Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            ' Manejo en caso de que no haya un valor
            Dim script As String = "swal('Error!', 'No se pudo identificar la persona.', 'error');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Guardar_Equipo_Persona(ByVal Id_Tercero As String)
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Tercero_Equipo 
        (Id_Tercero, Marca, Serial, Detalle_Equipo) 
        VALUES 
        (@Id_Tercero, @Marca, @Serial, @Detalle_Equipo)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)
                    cmd.Parameters.AddWithValue("@Marca", If(String.IsNullOrWhiteSpace(TxMarcaEquipo.Text), DBNull.Value, TxMarcaEquipo.Text.Trim.ToUpper()))
                    cmd.Parameters.AddWithValue("@Serial", If(String.IsNullOrWhiteSpace(TxSerial.Text), DBNull.Value, TxSerial.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Detalle_Equipo", TxDetalleEquipo.Text)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Equipo_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub BtAsignarEquipo_Click(sender As Object, e As EventArgs) Handles BtAsignarEquipo.Click
        Dim idEmpleado As String = HiddenFieldEquipo.Value

        ' Validar que el ID no esté vacío
        If Not String.IsNullOrEmpty(idEmpleado) Then

            Dim IdTercero As Long = ObtenerIdTerceroHIK(idEmpleado)

            Guardar_Equipo_Persona(IdTercero)

            If errores.Count = 0 Then
                Dim script As String = String.Format("swal('Excelente!', 'Equipo Asignado Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            ' Manejo en caso de que no haya un valor
            Dim script As String = "swal('Error!', 'No se pudo identificar la persona.', 'error');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then
            Dim usuarioCreado As Boolean = False
            Dim fotoAgregada As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            Dim IdTercero As Long = ObtenerIdTercero(TxCedula.Text.Trim())

            If IdTercero = 0 Then

                Consultar_Id_Persona()
                Consultar_Id_HikCamara()
                Guardar_Persona()
                If Consultar_Control_Restaurante_Facial() Then
                    If TxIdRol.SelectedValue <> "8" Then
                        Crear_Usuario_Accesos()
                        Crear_Usuario_Restaurante_Sin_Acceso()
                    Else
                        Crear_Usuario()
                    End If
                Else
                    Crear_Usuario()
                End If

                ' Verificar si el usuario fue creado
                If Session("Status_Code_Usuario") = "1" Then
                    usuarioCreado = True
                    mensaje = "Usuario creado correctamente."
                Else
                    mensaje = "Error al crear el usuario."
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Si el usuario fue creado, proceder con la foto
                If usuarioCreado Then
                    ' Verificar si se subió una foto desde un archivo
                    If FlFoto.HasFile Then
                        Agregar_Foto_Persona_Archivo()

                        ' Verificar el estado del proceso de agregar foto desde archivo
                        If Session("Status_Code_Archivo") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (archivo) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If

                        ' Verificar si se subió una foto desde imagen tomada (base64)
                    ElseIf base64image.Value <> "" Then
                        GuardarImagen()
                        Agregar_Foto_Persona_Tomada()

                        ' Verificar el estado del proceso de agregar foto tomada
                        If Session("Status_Code_Tomada") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (tomada) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto tomada."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If
                    End If

                    ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                    If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                        mensaje &= " OJO sin foto."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    End If
                End If

                ' Mostrar mensaje al usuario con SweetAlert
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")

            Else

                Consultar_Id_HikCamara()
                Actualizar_Id_Hikvision(Session("Id_HikCamara").ToString, IdTercero)
                If Consultar_Control_Restaurante_Facial() Then
                    If TxIdRol.SelectedValue <> "8" Then
                        Crear_Usuario_Accesos()
                        Crear_Usuario_Restaurante_Sin_Acceso()
                    Else
                        Crear_Usuario()
                    End If
                Else
                    Crear_Usuario()
                End If

                ' Verificar si el usuario fue creado
                If Session("Status_Code_Usuario") = "1" Then
                    usuarioCreado = True
                    mensaje = "Usuario creado correctamente."
                Else
                    mensaje = "Error al crear el usuario."
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Si el usuario fue creado, proceder con la foto
                If usuarioCreado Then
                    ' Verificar si se subió una foto desde un archivo
                    If FlFoto.HasFile Then
                        Agregar_Foto_Persona_Archivo()

                        ' Verificar el estado del proceso de agregar foto desde archivo
                        If Session("Status_Code_Archivo") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (archivo) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If

                        ' Verificar si se subió una foto desde imagen tomada (base64)
                    ElseIf base64image.Value <> "" Then
                        GuardarImagen()
                        Agregar_Foto_Persona_Tomada()

                        ' Verificar el estado del proceso de agregar foto tomada
                        If Session("Status_Code_Tomada") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (tomada) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto tomada."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If
                    End If

                    ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                    If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                        mensaje &= " OJO sin foto."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    End If
                End If

                ' Mostrar mensaje al usuario con SweetAlert
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")
            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            Dim usuarioActualizado As Boolean = False
            Dim fotoAgregada As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            Actualizar_Persona()
            If Consultar_Control_Restaurante_Facial() Then
                If TxIdRol.SelectedValue <> "8" Then
                    Editar_Usuario_Accesos()
                    Editar_Usuario_Restaurante_Sin_Acceso()
                Else
                    Editar_Usuario()
                End If
            Else
                Editar_Usuario()
            End If

            If Session("Status_Code_Editar") = "1" Then
                usuarioActualizado = True
                mensaje = "Usuario actualizado correctamente."
                script = $"swal('Excelente!', '{mensaje}', 'success');"
            Else
                mensaje = " Ocurrió un error al actualizar el usuario."
                Dim detallesError As String = String.Join("<br>", errores)
                script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                mensaje = "Error al crear el usuario."
                Exit Sub
            End If

            ' Si el usuario fue actualizado, proceder con la foto
            If usuarioActualizado Then
                ' Verificar si se subió una foto desde un archivo
                If FlFoto.HasFile Then
                    Actualizar_Foto_Persona_Archivo()

                    ' Verificar el estado del proceso de agregar foto desde archivo
                    If Session("Status_Code_Actualizar_Archivo") = "1" Then
                        fotoAgregada = True
                        mensaje = "Usuario con foto (archivo) actualizado correctamente."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    Else
                        mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                        Dim detallesError As String = String.Join("<br>", errores)
                        script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    End If

                    ' Verificar si se subió una foto desde imagen tomada (base64)
                ElseIf base64image.Value <> "" Then
                    GuardarImagen()
                    Actualizar_Foto_Persona_Tomada()

                    ' Verificar el estado del proceso de agregar foto tomada
                    If Session("Status_Code_Actualizar_Tomada") = "1" Then
                        fotoAgregada = True
                        mensaje = "Usuario con foto (tomada) actualizada correctamente."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    Else
                        mensaje &= " Sin embargo, ocurrió un error al actualizar la foto tomada."
                        Dim detallesError As String = String.Join("<br>", errores)
                        script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    End If
                End If

                ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                    mensaje &= " OJO sin foto."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                End If
            End If

            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Crear_Usuario_Camara.aspx")
        End If
    End Sub

End Class