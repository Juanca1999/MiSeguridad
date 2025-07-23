Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class Crear_Control_Restaurante2
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

    Private ErrorOp As String

    Private Function ObtenerIdTercero(usuario As String) As Long
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Usuario"
        Dim idTercero As Long = 0

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Usuario", usuario)

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

    Private Function RegistroExisteEnBD(Cedula As Long) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE CONVERT(VARCHAR, Cedula) = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula.ToString())

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

    Dim errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Inmueble = 'RESTAURANTE'")

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

    Private Sub Consultar_Id_Control()
        Dim Valor As Integer = ObtenerMaxId("Id_Control", "Adm_Control_Restaurante")
        Session("Id_Control") = Valor + 1
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

    Private Sub Guardar_Control_Restaurante()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Control_Restaurante 
        (Id_Control, Cedula, Id_Quien_Autoriza, Fecha_Activacion, Cantidad_Accesos, Codigo_QR, Id_HikCamara, Numero_Factura) 
        VALUES 
        (@Id_Control, @Cedula, @Id_Quien_Autoriza, @Fecha_Activacion, @Cantidad_Accesos, @Codigo_QR, @Id_HikCamara, @Numero_Factura)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_Control", Session("Id_Control"))
                    cmd.Parameters.AddWithValue("@Cedula", TxCedula.Text.Trim())
                    cmd.Parameters.AddWithValue("@Id_Quien_Autoriza", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Fecha_Activacion", Date.Now.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@Cantidad_Accesos", TxCantidadAccesos.Text)
                    cmd.Parameters.AddWithValue("@Codigo_QR", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_HikCamara", TxId_Hikvision.Text)
                    cmd.Parameters.AddWithValue("@Numero_Factura", If(String.IsNullOrWhiteSpace(TxNumeroFactura.Text), DBNull.Value, TxNumeroFactura.Text))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD Guardar_Control_Restaurante():" & ex.Message)
        End Try
    End Sub

    Private Sub Buscar_Persona(ByVal Usuario As String)
        Dim query As String = "SELECT Id_Tercero, Cedula, Nombres, Correo, Usuario, Password, Cargo, Celular, Id_Sede, Id_HikCamara, Estado FROM Terceros WHERE Id_HikCamara IS NOT NULL AND Cedula = '" & Usuario & "'"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        Session("Cedula") = Usuario.ToString()
                        TxNombres.Text = dr("Nombres").ToString()
                        TxCorreo.Text = If(dr("Correo").ToString() <> Nothing, dr("Correo").ToString(), String.Empty)
                        TxCelular.Text = If(dr("Celular").ToString() <> Nothing, dr("Celular").ToString(), String.Empty)
                        TxId_Hikvision.Text = dr("Id_HikCamara").ToString()
                    Else
                        Session("Id_Persona") = Nothing
                        TxNombres.Text = String.Empty
                        TxCorreo.Text = String.Empty
                        TxCelular.Text = String.Empty
                        TxId_Hikvision.Text = String.Empty
                        Dim script As String = String.Format("swal('OJO!', 'Usuario no registrado. Dirigirse al departamento de seguridad para su registro.', 'Warning');")
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub TxCedula_TextChanged(sender As Object, e As EventArgs) Handles TxCedula.TextChanged
        If TxCedula.Text <> Nothing Then
            Buscar_Persona(TxCedula.Text.Trim())
        End If
    End Sub

    Private Sub Activar_Acceso(idUsuario As String)
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                Dim jsonBody As String = "
                {
                   ""UserInfo"": {
                      ""employeeNo"": """ & idUsuario & """,
                      ""doorRight"": ""1"",
                      ""RightPlan"": [
                         {
                            ""doorNo"": 1,
                            ""planTemplateNo"": ""1""
                         }
                      ]
                   }
                }"

                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                requestDocument.ContentLength = byteArray.Length

                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()
                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al ACTIVAR acceso del usuario " & idUsuario & " en cámara " & ipCamara & ": " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub LvControlRestaurante_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvControlRestaurante.ItemDataBound
    End Sub

    Private Sub LvControlRestaurante_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvControlRestaurante.ItemCommand
    End Sub

    Private Function ObtenerCantidadVeces(employeeNo As String) As Long
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim Cantidad As Long = 0

        ' Cadena SQL para obtener el último Id_Visita
        Dim sql As String = "SELECT COUNT(V.Id_Visita) AS Cantidad FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa CROSS APPLY ( SELECT TOP 1 Fecha_Activacion FROM Adm_Control_Restaurante R WHERE R.Cedula = T.Cedula ORDER BY R.Id_Control DESC ) UltimoControl WHERE V.Id_Acceso_Entrada IN (22) AND V.Id_Quien_Ingresa = @Id_Quien_Ingresa AND V.Fecha_Inicio_Visita >= UltimoControl.Fecha_Activacion"

        ' Conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Crear el comando SQL
            Using cmd As New SqlCommand(sql, conn)
                ' Agregar parámetro para Id_Quien_Ingresa
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Cantidad = Convert.ToInt64(reader("Cantidad"))
                    End If
                End Using
            End Using
        End Using

        Return Cantidad
    End Function

    Private Function ObtenerCantidadVecesControl(employeeNo As String) As Long
        Dim CantidadControl As Long = 0

        ' Cadena SQL para obtener el último Id_Visita
        Dim sql As String = "SELECT TOP 1 Cantidad_Accesos FROM Adm_Control_Restaurante WHERE Id_HikCamara = @Id_HikCamara ORDER BY Id_Control DESC"

        ' Conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Crear el comando SQL
            Using cmd As New SqlCommand(sql, conn)
                ' Agregar parámetro para Id_Quien_Ingresa
                cmd.Parameters.AddWithValue("@Id_HikCamara", employeeNo)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        CantidadControl = Convert.ToInt64(reader("Cantidad_Accesos"))
                    End If
                End Using
            End Using
        End Using

        Return CantidadControl
    End Function

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Dim usuarioCreado As Boolean = False
        Dim QRAgregado As Boolean = False
        Dim mensaje As String = String.Empty
        Dim script As String = String.Empty

        If RegistroExisteEnBD(TxCedula.Text.Trim()) Then

            ' Verificar si la persona ya tiene un control activo
            Dim cantidadVeces As Long = ObtenerCantidadVeces(TxId_Hikvision.Text)
            Dim cantidadEntradasControl As Long = ObtenerCantidadVecesControl(TxId_Hikvision.Text)

            If cantidadVeces < cantidadEntradasControl Then
                Dim ingresosRestantes As Long = cantidadEntradasControl - cantidadVeces
                mensaje = $"Aún le quedan {ingresosRestantes} ingreso(s) disponibles. No se puede crear un nuevo control hasta que los consuma."
                script = $"swal('Acceso Denegado', '{mensaje}', 'info');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "mensajeBloqueo", script, True)
                Exit Sub
            End If

            ' Si ya agotó los accesos, puede crear uno nuevo
            Consultar_Id_Control()
            Guardar_Control_Restaurante()
            Activar_Acceso(TxId_Hikvision.Text)

            ' Verificar si el usuario fue creado
            If Session("Status_Code_Usuario") = "1" Then

                mensaje = "Error al crear el usuario."
                Dim detallesError As String = String.Join("<br>", errores)
                script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Exit Sub
            End If

            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "success", script, True)
            Response.AddHeader("Refresh", "3; URL=Crear_Control_Restaurante2.aspx")
        End If
    End Sub

End Class