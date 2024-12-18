Imports System.IO
Imports System.Net
Imports ImageMagick
Imports Newtonsoft.Json
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class Crear_Usuario_Camara_Masivo
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

            LlenarListView()
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

    Private Function ObtenerIdTercero(Cedula As Long) As Long
        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Cedula"
        Dim idTercero As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Cedula", Cedula)

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

    Private Sub Eliminar_Usuario(ByVal Id As String)
        Try
            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

            For Each ipCamara As String In ipCamaras
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Delete?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
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
                    Session("Status_Code_Eliminar") = jsonResponse("statusCode").ToString()
                End Using
            Next
        Catch ex As Exception
            errores.Add("Error en Eliminar_Usuario()" & ex.Message)
        End Try
    End Sub

    Protected Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click
        ' Obtener el ID del empleado desde el HiddenField
        Dim idEmpleado As String = HiddenEmployeeNo.Value

        ' Validar que el ID no esté vacío
        If Not String.IsNullOrEmpty(idEmpleado) Then
            Eliminar_Usuario(idEmpleado)

            If errores.Count = 0 And Session("Status_Code_Eliminar") = "1" Then
                Dim script As String = "swal('Excelente!', 'Usuario eliminado correctamente.', 'success');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 3000
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

    ' Clase para mapear los usuarios
    Public Class Usuarios
        Public Property employeeNo As String
        Public Property name As String
        Public Property userType As String
        Public Property gender As String
        Public Property numOfFace As Integer
    End Class

    Private Function GetUserList() As List(Of Usuarios)
        ' Recuperar la lista completa de usuarios desde la API
        Dim allUsers As New List(Of Usuarios)()

        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Search?format=json"
            Dim maxResultsPerPage As Integer = 30 ' Máximo permitido por la API
            Dim totalMatches As Integer = 0
            Dim position As Integer = 0

            Do
                ' Configurar la solicitud
                Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                request.Method = "POST"
                request.ContentType = "application/json"
                request.Accept = "application/json"
                request.Credentials = New NetworkCredential("admin", "Miro-321")
                request.PreAuthenticate = False

                ' JSON con paginación dinámica
                Dim jsonBody As String = "
            {
                ""UserInfoSearchCond"": {
                    ""searchID"": ""1"",
                    ""searchResultPosition"": " & position & ",
                    ""maxResults"": " & maxResultsPerPage & ",
                    ""EmployeeNoList"": []
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

                    ' Total de coincidencias (solo lo obtenemos una vez)
                    If totalMatches = 0 Then
                        totalMatches = CInt(userInfoSearch("totalMatches"))
                    End If

                    ' Obtener la lista de usuarios en la página actual
                    Dim userInfoList = JsonConvert.DeserializeObject(Of List(Of Usuarios))(userInfoSearch("UserInfo").ToString())
                    allUsers.AddRange(userInfoList)

                    ' Actualizar posición para la siguiente página
                    position += maxResultsPerPage
                End Using
            Loop While position < totalMatches

        Catch ex As Exception
            ' Manejo de errores
            allUsers = New List(Of Usuarios)()
        End Try

        ' Si hay un término de búsqueda, filtrar la lista de usuarios por el nombre
        Dim searchTerm As String = TxBuscar.Text.Trim().ToLower()
        If Not String.IsNullOrEmpty(searchTerm) Then
            allUsers = allUsers.Where(Function(u) u.name.ToLower().Contains(searchTerm)).ToList()
        End If

        Return allUsers
    End Function

    Private Sub LlenarListView()
        Try
            ' Obtener la lista de usuarios (filtrada si hay búsqueda)
            Dim allUsers As List(Of Usuarios) = GetUserList()

            ' Vincular los usuarios al ListView
            If allUsers.Count = 0 Then
                LvUsuarios.DataSource = Nothing
                LvUsuarios.DataBind()
            Else
                LvUsuarios.DataSource = allUsers
                LvUsuarios.DataBind()
            End If
        Catch ex As Exception
            ' Manejo de errores
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String = ex.Message
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Protected Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        LlenarListView()
    End Sub

    Dim errores As New List(Of String)

    Public Shared Function ObtenerEndPoint() As String
        Dim IP_EndPoint As String = String.Empty

        Dim sql As String = "SELECT TOP 1 IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Sede = @Id_Sede ORDER BY Id_Acceso ASC"

        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
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

    Private Sub LvUsuarios_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvUsuarios.ItemDataBound
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim genderLabel As Label = DirectCast(e.Item.FindControl("genderLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Dim numOfFaceLabel As Label = DirectCast(e.Item.FindControl("numOfFaceLabel"), Label)
        Dim Foto As LinkButton = DirectCast(e.Item.FindControl("Foto"), LinkButton)

        If Convert.ToInt32(numOfFaceLabel.Text) > 0 Then
            Foto.Visible = True
        Else
            Foto.Visible = False
        End If
    End Sub

    Private Sub LvUsuarios_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvUsuarios.ItemCommand
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim genderLabel As Label = DirectCast(e.Item.FindControl("genderLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Dim numOfFaceLabel As Label = DirectCast(e.Item.FindControl("numOfFaceLabel"), Label)
        Session("Id_Empleado") = employeeNoLabel.Text
    End Sub

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Sede = " & Session("Sucursal_Usuario").ToString & "")

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

    Private Sub Guardar_Persona(ByVal Cedula As String, ByVal Nombres As String, ByVal Id_Rol As String, ByVal Correo As String, ByVal Usuario As String, ByVal Password As String, ByVal Cargo As String, ByVal Celular As String, ByVal Sede As String)
        Try
            Dim query As String = "
        INSERT INTO dbo.Terceros 
        (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                    cmd.Parameters.AddWithValue("@Cedula", If(Cedula Is Nothing, DBNull.Value, Cedula))
                    cmd.Parameters.AddWithValue("@Nombres", Nombres.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", If(Id_Rol Is Nothing, DBNull.Value, Id_Rol))
                    cmd.Parameters.AddWithValue("@Correo", If(String.IsNullOrWhiteSpace(Correo), DBNull.Value, Correo))
                    cmd.Parameters.AddWithValue("@Usuario", If(String.IsNullOrWhiteSpace(Usuario), DBNull.Value, Usuario))
                    cmd.Parameters.AddWithValue("@Password", If(String.IsNullOrWhiteSpace(Password), DBNull.Value, Password))
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(Cargo), DBNull.Value, Cargo))
                    cmd.Parameters.AddWithValue("@Celular", If(String.IsNullOrWhiteSpace(Celular), DBNull.Value, Celular))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(Sede Is Nothing, DBNull.Value, Sede))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Crear_Usuario(ByVal Nombres As String, ByVal Genero As String, ByVal Tipo_Usuario As String)

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
              ""name"": """ & Nombres.ToUpper() & """,
              ""gender"": """ & Genero & """,
              ""userType"": """ & Tipo_Usuario & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2025-12-31T23:59:59""
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
                errores.Add("Error al crear el usuario " & Session("Id_HikCamara").ToString & "en hikvision en la cámara " & ipCamara & " :" & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Agregar_Foto_Persona_Ruta(ByVal rutaBase As String)
        ' Buscar el archivo con cualquier extensión en el directorio especificado
        Dim directorio As String = Path.GetDirectoryName(rutaBase)
        Dim nombreArchivoBase As String = Path.GetFileNameWithoutExtension(rutaBase)
        Dim rutaCompleta As String = Nothing

        ' Extensiones soportadas
        Dim extensiones As String() = {".png", ".jpg", ".jpeg"}

        ' Buscar el archivo con las extensiones soportadas
        For Each extension In extensiones
            Dim rutaCandidata As String = Path.Combine(directorio, nombreArchivoBase & extension)
            If File.Exists(rutaCandidata) Then
                rutaCompleta = rutaCandidata
                Exit For
            End If
        Next

        ' Si no se encuentra un archivo válido, salir del método
        If String.IsNullOrEmpty(rutaCompleta) Then
            Return
        End If

        ' Procesar el archivo encontrado
        ProcesarImagen(rutaCompleta)
    End Sub

    Private Sub ProcesarImagen(ByVal Ruta As String)
        ' Reducir el peso de la imagen antes de procesarla
        Dim rutaReducida As String = Path.Combine(Path.GetDirectoryName(Ruta), "Reducida_" & Path.GetFileName(Ruta))
        ReducirPesoImagen(Ruta, rutaReducida)

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
                        Session("Status_Code_Tomada") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Tomada") = 0
                errores.Add("Error al agregar la foto al usuario " & Session("Id_HikCamara").ToString & " en Hikvision en la cámara " & ipCamara & " :" & ex.Message)
            End Try
        Next

        ' Eliminar la imagen reducida después de usarla
        Try
            If File.Exists(rutaReducida) Then
                File.Delete(rutaReducida)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ReducirPesoImagen(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            ' Cargar la imagen con MagickImage
            Using imagen As New MagickImage(rutaEntrada)
                Dim maxAncho As UInteger = 800
                Dim maxAlto As UInteger = 800

                ' Redimensionar la imagen manteniendo la proporción (Fill, Fit, o ajusta según tu necesidad)
                imagen.Resize(New MagickGeometry(maxAncho, maxAlto) With {.IgnoreAspectRatio = False})

                ' Ajustar la calidad de la compresión
                imagen.Quality = 70 ' 70% de calidad

                ' Guardar la imagen comprimida
                imagen.Write(rutaSalida)
            End Using
        Catch ex As Exception
            ' Manejar excepciones
            Console.WriteLine("Error al reducir el peso de la imagen: " & ex.Message)
        End Try
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If FlFoto.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FlFoto.FileName)
            FlFoto.SaveAs(Server.MapPath("..\..\Adjunto\Temporal\Excel" & Ext))
            Dim filePath As String = Server.MapPath("..\..\Adjunto\Temporal\Excel" & Ext)

            ' Establecer el contexto de la licencia de EPPlus
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial

            ' Abrir el archivo Excel usando EPPlus dentro de un bloque Using
            Using package As New OfficeOpenXml.ExcelPackage(New FileInfo(filePath))
                Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets("Hoja1")

                Dim startRow As Integer = worksheet.Dimension.Start.Row
                Dim endRow As Integer = worksheet.Dimension.End.Row

                ' Recorre las filas, asumiendo que la primera es el encabezado
                For row As Integer = startRow + 1 To endRow


                    Consultar_Id_Persona()
                    Consultar_Id_HikCamara()
                    Guardar_Persona(worksheet.Cells(row, 2).Text, worksheet.Cells(row, 3).Text, worksheet.Cells(row, 4).Text, worksheet.Cells(row, 5).Text, worksheet.Cells(row, 6).Text, worksheet.Cells(row, 7).Text, worksheet.Cells(row, 8).Text, worksheet.Cells(row, 9).Text, worksheet.Cells(row, 10).Text)
                    Crear_Usuario(worksheet.Cells(row, 3).Text, worksheet.Cells(row, 14).Text, worksheet.Cells(row, 13).Text)
                    Agregar_Foto_Persona_Ruta(Server.MapPath("../../Adjunto/Temporal/Biometrico/" & worksheet.Cells(row, 2).Text))

                Next
            End Using

            File.Delete(filePath)

            If errores.Count = 0 Then
                Dim script As String = String.Format("swal('Excelente!', 'Usuarios Registrados Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 3000
            Else
                ' Une los errores en un texto con saltos de línea
                Dim erroresTexto As String = String.Join("<br>", errores)
                Dim script As String = $"swal('OJO!', 'Fallaron los siguientes usuarios :<br>{erroresTexto}', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            Dim script As String = String.Format("swal('OJO!', 'Debe agregar un archivo', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Usuario_Camara_Masivo.aspx")
    End Sub

End Class