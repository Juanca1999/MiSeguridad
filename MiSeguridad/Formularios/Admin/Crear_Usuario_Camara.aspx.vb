Imports System.IO
Imports System.Net
Imports System.Drawing
Imports System.Net.Mail
Imports Newtonsoft.Json
Imports System.Net.Security
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates
Imports System.Drawing.Imaging

Public Class Crear_Usuario_Camara
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        LlenarListView()
    End Sub

    Public Function HayRegistrosEnAdmEmpresa() As Boolean
        Dim hayRegistros As Boolean = False

        ' Cadena de conexión a tu base de datos
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para verificar si existe al menos un registro en la tabla Adm_Empresa
        Dim query As String = "SELECT COUNT(*) FROM Adm_Empresa"

        ' Crear la conexión y el comando SQL
        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                Try
                    ' Abrir la conexión
                    connection.Open()

                    ' Ejecutar el comando y obtener el número de registros
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    ' Si el recuento es mayor que 0, entonces hay registros
                    If count > 0 Then
                        hayRegistros = True
                    End If
                Catch ex As Exception
                Finally
                    ' Asegúrate de cerrar la conexión
                    connection.Close()
                End Try
            End Using
        End Using

        Return hayRegistros
    End Function

    Private Sub Guardar_Datos()
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        cmd.CommandType = CommandType.Text
        Try
            cmd.Connection = conn_Administracion1
            cmd.CommandText = Session("sql")
            conn_Administracion1.Open()
            cmd.ExecuteNonQuery()
            conn_Administracion1.Close()
            Session("Mensaje") = "Guardado Correctamente"
        Catch ex As Exception
            If ex.ToString.Contains("PRIMARY") Then
            Else
                Try
                    cmd.Connection = conn_Administracion2
                    cmd.CommandText = Session("sql")
                    conn_Administracion2.Open()
                    cmd.ExecuteNonQuery()
                    conn_Administracion2.Close()
                    Session("Mensaje") = "Guardado Correctamente"
                Catch ex1 As Exception
                    ErrorOp = "Private Sub Guardar_Datos:  " + Mid(ex1.ToString, 1, 300) + "<br /><br />" + "Sintaxis Sql: " + Session("sql")
                    conn_Administracion1.Close()
                    conn_Administracion2.Close()
                    EnviarCorreoError()
                    Session("Mensaje") = "!Error al Guardar, Verificar o llamar Administrador del sistema¡"
                End Try
            End If

        End Try

    End Sub

    Private Sub Ejecutar_Query()
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        cmd.CommandType = CommandType.Text
        Try
            cmd.Connection = conn_Administracion1
            cmd.CommandText = Session("sql")
            conn_Administracion1.Open()
            dr = cmd.ExecuteReader
        Catch ex As Exception
            Try
                cmd.Connection = conn_Administracion2
                cmd.CommandText = Session("sql")
                conn_Administracion2.Open()
                dr = cmd.ExecuteReader
            Catch ex1 As Exception
                ErrorOp = "Private Sub Ejecutar_Query: " + Mid(ex1.ToString, 1, 300) + "<br /><br />" + "Sintaxis Sql: " + Session("sql")
                conn_Administracion1.Close()
                conn_Administracion2.Close()
                EnviarCorreoError()
            End Try
        End Try

    End Sub

    Private ErrorOp As String

    Dim errores As New List(Of String)

    Private Sub EnviarCorreoError()
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Error Miseguridad")
            msg.To.Add(New MailAddress("juan.aristizabal@miroseguridad.com"))
            msg.Subject = "Error Miseguridad"
            msg.Body = "Error al usuario: " + User.Identity.Name + "<br />" + "Nombre: " + Session("Nombres") + "<br />" + "Formulario: " + Request.Url.ToString + "<br /><br />" + "Error: " + ErrorOp
            msg.IsBodyHtml = True

            Dim smtp = New SmtpClient
            smtp.Host = SmtpHost
            smtp.Port = 587
            smtp.UseDefaultCredentials = False
            smtp.Credentials = New NetworkCredential(UsuarioCredentials, ContrasenaCredentials)
            smtp.EnableSsl = True

            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            smtp.Send(msg)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Consultar_Usuario(ByVal Id As Long)
        Dim query As String = "SELECT Id_Tercero, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Foto, Estado, Id_Sede, Id_Acceso, Id_HikCamara FROM Terceros " &
                          "WHERE Id_HikCamara = @Id"

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
                        TxSede.SelectedValue = If(dr("Id_Sede") IsNot DBNull.Value, dr("Id_Sede").ToString(), String.Empty)
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
                        TxSede.SelectedIndex = String.Empty
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Function ObtenerEndPoint() As String
        Dim IP_EndPoint As String = String.Empty

        Dim sql As String = "SELECT TOP 1 IP_Camara FROM Adm_Accesos ORDER BY Id_Acceso ASC"

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

    ' Clase para mapear los usuarios
    Public Class Usuarios
        Public Property employeeNo As String
        Public Property name As String
        Public Property userType As String
    End Class

    Private Sub LlenarListView()
        Try
            Dim IP_EndPoint = ObtenerEndPoint()
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
                ""searchID"": ""1"",
                ""searchResultPosition"": 0,
                ""maxResults"": 200,
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

                ' Verificar si hay usuarios
                If userInfoSearch("numOfMatches") = 0 Then
                    ' No hay usuarios, limpiar y mostrar mensaje
                    LvUsuarios.DataSource = Nothing
                    LvUsuarios.DataBind()
                Else
                    ' Hay usuarios, deserializar y vincular
                    Dim userInfoList = JsonConvert.DeserializeObject(Of List(Of Usuarios))(userInfoSearch("UserInfo").ToString())
                    LvUsuarios.DataSource = userInfoList
                    LvUsuarios.DataBind()
                End If
            End Using
        Catch ex As UriFormatException
            ' Error específico de URI no válido
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String = "URI no válido: No se puede analizar el nombre de host."
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)

        Catch ex As WebException
            ' Error específico de solicitud web
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String
            If ex.Response IsNot Nothing Then
                Using reader As New StreamReader(ex.Response.GetResponseStream())
                    mensajeError = reader.ReadToEnd()
                End Using
            Else
                mensajeError = ex.Message
            End If
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)

        Catch ex As Exception
            ' Cualquier otro error general
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String = ex.Message
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub LvUsuarios_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvUsuarios.ItemCommand
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Session("Id_Empleado") = employeeNoLabel.Text
        If e.CommandName = "Editar" Then
            TxTipoUsuario.SelectedValue = userTypelabel.Text
            Consultar_Usuario(employeeNoLabel.Text)
        ElseIf e.CommandName = "Eliminar" Then
            Eliminar_Usuario(employeeNoLabel.Text)
            Eliminar_Persona(employeeNoLabel.Text)
        End If
    End Sub

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            ErrorOp = "Error al obtener las IPs de las cámaras: " & ex.Message
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
                        ErrorOp = "Error al ejecutar la consulta SQL: " & ex.Message
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

    Private Sub Guardar_Persona()
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
                    cmd.Parameters.AddWithValue("@Cedula", If(String.IsNullOrWhiteSpace(TxCedula.Text), DBNull.Value, TxCedula.Text))
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", If(TxIdRol.SelectedValue Is Nothing, DBNull.Value, TxIdRol.SelectedValue))
                    cmd.Parameters.AddWithValue("@Correo", If(String.IsNullOrWhiteSpace(TxCorreo.Text), DBNull.Value, TxCorreo.Text))
                    cmd.Parameters.AddWithValue("@Usuario", If(String.IsNullOrWhiteSpace(TxUsuario.Text), DBNull.Value, TxUsuario.Text))
                    cmd.Parameters.AddWithValue("@Password", If(String.IsNullOrWhiteSpace(TxPassword.Text), DBNull.Value, TxPassword.Text))
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(TxCargo.Text), DBNull.Value, TxCargo.Text))
                    cmd.Parameters.AddWithValue("@Celular", If(String.IsNullOrWhiteSpace(TxCelular.Text), DBNull.Value, TxCelular.Text))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(TxSede.SelectedValue Is Nothing, DBNull.Value, TxSede.SelectedValue))
                    cmd.Parameters.AddWithValue("@Estado", TxEstado.SelectedValue)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
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
            End Try
        Next
    End Sub

    ' Método para agregar la foto de una persona desde un archivo usando las IPs obtenidas desde la base de datos
    Private Sub Agregar_Foto_Persona_Archivo()
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

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: {2}" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  FlFoto.FileName,
                                                  FlFoto.PostedFile.ContentType)

                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo cargado en FileUpload en un byte array
                Dim imageBytes As Byte() = FlFoto.FileBytes

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
            End Try
        Next
    End Sub

    ' Método para agregar la foto de una persona tomada usando las IPs obtenidas desde la base de datos
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

            ' Guardar el archivo en el servidor
            Dim rutaCarpeta As String = Server.MapPath("../../Adjunto/Foto_Usuarios/")
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Nombre único para el archivo
            Dim nombreArchivo As String = TxCedula.Text & ".png"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, nombreArchivo)

            ' Convertir Base64 a archivo y guardar temporalmente
            Dim imageBytes As Byte() = Convert.FromBase64String(base64Foto.Replace("data:image/png;base64,", ""))
            Dim rutaArchivoTemp As String = Path.Combine(rutaCarpeta, "temp_" & nombreArchivo)
            File.WriteAllBytes(rutaArchivoTemp, imageBytes)

            ' Reducir el peso de la imagen y guardar la versión optimizada
            ReducirPesoImagen(rutaArchivoTemp, rutaArchivo)

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
            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

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
              ""userType"": """ & TxTipoUsuario.SelectedValue & """,
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
                    Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                End Using
            Next

        Catch ex As Exception
            Session("Status_Code_Editar") = 0
        End Try
    End Sub

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

                    If Session("Status_Code_Eliminar") = "1" Then
                        Dim script As String = String.Format("swal('Excelente!', 'Usuario Eliminado correctamente.', 'success');")
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        Timer1.Interval = 3000
                    End If
                End Using
            Next

        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub Eliminar_Persona(ByVal Id As Long)
        Dim query As String = "DELETE FROM Terceros WHERE Id_HikCamara = @Id"

        ' Configuración de la conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Id", Id)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
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
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub ReducirPesoImagen(ByVal rutaEntrada As String, ByVal rutaSalida As String)
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
            Throw New Exception("Error al reducir el peso de la imagen: " & ex.Message)
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


    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Dim usuarioCreado As Boolean = False
        Dim fotoAgregada As Boolean = False

        If BtGuardar.Text = "GUARDAR" Then
            Try
                ' Crear usuario
                Consultar_Id_Persona()
                Consultar_Id_HikCamara()
                Guardar_Persona()
                Crear_Usuario()
                If Session("Status_Code_Usuario") = "1" Then
                    usuarioCreado = True
                Else
                    errores.Add("No se pudo crear el usuario. Verifique los datos e intente nuevamente.")
                End If
            Catch ex As Exception
                errores.Add("Error al crear el usuario: " & ex.Message)
            End Try

            ' Agregar foto si el usuario fue creado correctamente
            If usuarioCreado Then
                ' Obtener las IPs de las cámaras desde la base de datos
                Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

                For Each ipCamara As String In ipCamaras
                    If FlFoto.HasFile Then
                        Try
                            ' Agregar foto a la cámara actual
                            Session("IP_EndPoint") = ipCamara ' Asignar IP de la cámara actual
                            Agregar_Foto_Persona_Archivo()
                            If Session("Status_Code_Archivo") = "1" Then
                                fotoAgregada = True
                            Else
                                errores.Add("No se pudo agregar la foto en la cámara " & ipCamara & ".")
                            End If
                        Catch ex As Exception
                            errores.Add("Error al agregar la foto en la cámara " & ipCamara & ": " & ex.Message)
                        End Try
                    ElseIf base64image.Value <> "" Then
                        Try
                            ' Agregar foto desde base64 a la cámara actual
                            Session("IP_EndPoint") = ipCamara ' Asignar IP de la cámara actual
                            GuardarImagen()
                            Agregar_Foto_Persona_Tomada()
                            If Session("Status_Code_Tomada") = "1" Then
                                fotoAgregada = True
                            Else
                                errores.Add("No se pudo agregar la foto en la cámara " & ipCamara & ".")
                            End If
                        Catch ex As Exception
                            errores.Add("Error al agregar la foto en la cámara " & ipCamara & ": " & ex.Message)
                        End Try
                    End If
                Next
            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            ' Lógica para actualizar el usuario
            Try
                Actualizar_Persona()
                Editar_Usuario()

                ' Si no hay errores, el usuario se actualizó correctamente
                If Session("Status_Code_Editar") = "1" Then
                    errores.Add("Usuario actualizado correctamente.")
                Else
                    errores.Add("No se pudo actualizar el usuario. Verifique los datos e intente nuevamente.")
                End If
            Catch ex As Exception
                errores.Add("Error al actualizar el usuario: " & ex.Message)
            End Try
        End If

        ' Mostrar resultados al usuario
        If errores.Count = 0 Then
            Dim script As String = String.Format("swal('Excelente!', 'Usuario registrado correctamente.', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Timer1.Interval = 3000
        Else
            ' Consolidar los errores en un mensaje para el usuario
            Dim mensajeError As String = String.Join("<br/>", errores)
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Usuario_Camara.aspx")
    End Sub

End Class