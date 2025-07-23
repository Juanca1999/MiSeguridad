Imports System.IO
Imports System.Net
Imports ImageMagick
Imports System.Drawing
Imports System.Net.Mail
Imports Newtonsoft.Json
Imports System.Net.Security
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Drawing.Imaging
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Control_Contratista
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

    Private Function RegistroExisteEnBD(Cedula As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function Tiene_Facial(Cedula As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    Dim result As Object = command.ExecuteScalar()

                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Return True ' Id_HikCamara tiene datos
                    Else
                        Return False ' Id_HikCamara es NULL o no existe
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ObtenerId_HikCamara(Cedula As String) As Long
        Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
        Dim Id_HikCamara As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Cedula", Cedula)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Tercero
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    Id_HikCamara = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Tercero obtenido
        Return Id_HikCamara
    End Function

    Dim errores As New List(Of String)

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
        (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Empresa, Id_Sede,  Estado) 
        VALUES 
        (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Empresa, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Id_HikCamara", If(String.IsNullOrWhiteSpace(Session("Id_HikCamara")), DBNull.Value, Session("Id_HikCamara")))
                    cmd.Parameters.AddWithValue("@Cedula", If(String.IsNullOrWhiteSpace(TxCedulaContratista.Text), DBNull.Value, TxCedulaContratista.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Nombres", TxNombreContratista.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", 5)
                    cmd.Parameters.AddWithValue("@Correo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(TxCargo.Text), DBNull.Value, TxCargo.Text.ToUpper()))
                    cmd.Parameters.AddWithValue("@Celular", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Empresa", If(IsNumeric(TxEmpresa.SelectedValue), Convert.ToInt32(TxEmpresa.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Id_Sede", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Rol(Cedula As String)
        Dim sql As String = "UPDATE Terceros SET Estado = 1, Id_Rol = 5 WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Cedula", Cedula)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Actualizar_Rol_Hikvision(Cedula As String)
        Dim sql As String = "UPDATE Terceros SET Estado = 1, Id_Rol = 5, Id_HikCamara = @Id_HikCamara WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                cmd.Parameters.AddWithValue("@Cedula", Cedula)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Crear_Usuario()

        ' Convertir las fechas de los controles a formato ISO 8601
        Dim fechaInicio As String = DateTime.Parse(TxFechaInicio.Text).ToString("yyyy-MM-dd") & "T00:00:00"
        Dim fechaFin As String = DateTime.Parse(TxFechaFin.Text).ToString("yyyy-MM-dd") & "T23:59:59"

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
              ""name"": """ & TxNombreContratista.Text.ToUpper() & """,
              ""userType"": ""normal"",
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": """ & fechaInicio & """,
                 ""endTime"": """ & fechaFin & """
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
            Dim nombreArchivo As String = TxCedulaContratista.Text & ".png"
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

    Private Sub Editar_Usuario_Actualizar()
        Try
            ' Convertir fechas a formato ISO 8601
            Dim fechaInicio As String = DateTime.Parse(TxFechaInicio.Text).ToString("yyyy-MM-dd") & "T00:00:00"
            Dim fechaFin As String = DateTime.Parse(TxFechaFin.Text).ToString("yyyy-MM-dd") & "T23:59:59"

            ' Obtener las IPs de las cámaras
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()
            Dim employeeNo As String = Session("Id_Empleado").ToString()

            For Each ipCamara As String In ipCamaras
                ' Verificar si el usuario existe en la cámara
                If Verificar_Si_Existe(ipCamara, employeeNo) Then
                    ' Construir URL de actualización
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    requestDocument.Credentials = New NetworkCredential("admin", "Miro-321")
                    requestDocument.PreAuthenticate = True

                    ' JSON del cuerpo
                    Dim jsonBody As String = "
                {
                   ""UserInfo"": {
                      ""employeeNo"": """ & employeeNo & """,
                      ""name"": """ & TxNombreContratista.Text.ToUpper() & """,
                      ""userType"": ""normal"",
                       ""Valid"": {
                         ""enable"": true,
                         ""beginTime"": """ & fechaInicio & """,
                         ""endTime"": """ & fechaFin & """
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

                    ' Enviar solicitud
                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                    requestDocument.ContentLength = byteArray.Length
                    Using dataStream As Stream = requestDocument.GetRequestStream()
                        dataStream.Write(byteArray, 0, byteArray.Length)
                    End Using

                    ' Leer respuesta
                    Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseJson As String = streamReader.ReadToEnd()
                        Dim jsonResponse As JObject = JObject.Parse(responseJson)
                        Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                    End Using
                Else
                    ' Usuario no existe en esta cámara, puedes registrar un log o continuar simplemente
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

    Private Sub Editar_Usuario_Crear(ByVal Id_HikCamara As Long)
        Try

            ' Convertir las fechas de los controles a formato ISO 8601
            Dim fechaInicio As String = DateTime.Parse(TxFechaInicio.Text).ToString("yyyy-MM-dd") & "T00:00:00"
            Dim fechaFin As String = DateTime.Parse(TxFechaFin.Text).ToString("yyyy-MM-dd") & "T23:59:59"

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
              ""employeeNo"": """ & Id_HikCamara & """,
              ""name"": """ & TxNombreContratista.Text.ToUpper() & """,
              ""userType"": ""normal"",
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": """ & fechaInicio & """,
                 ""endTime"": """ & fechaFin & """
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
            errores.Add("Error en ReducirPesoImagen()" & ex.Message)
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

    Private Sub Consultar_Tercero()
        Try
            sql = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Id_Rol IN (1,3,8) AND Nombres LIKE '%" + TxBuscarPersona.Text + "%' ORDER BY Nombres"
            Using cmd As New SqlCommand(sql, conn)
                cmd.CommandType = CommandType.Text
                Using mda As New SqlDataAdapter(cmd)
                    Using datos As New DataTable
                        mda.Fill(datos)
                        TxNombrePersona.DataSource = datos
                        TxNombrePersona.DataTextField = "Nombres"
                        TxNombrePersona.DataValueField = "Id_Tercero"
                        TxNombrePersona.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = "Private Sub Consultar_Tercero: " + Mid(ex.ToString, 1, 500) + "<br /><br />" + "Sintaxis Sql: " + sql
            EnviarCorreoError()
            Dim script As String = String.Format("swal('Error!', 'No se encontró la persona', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            Consultar_Tercero()
        End If
    End Sub

    Private Sub TxCedulaContratista_TextChanged(sender As Object, e As EventArgs) Handles TxCedulaContratista.TextChanged
        If TxCedulaContratista.Text <> "" Then
            Buscar_Persona(TxCedulaContratista.Text.Trim())
        End If
    End Sub

    Private Sub Buscar_Persona(ByVal Usuario As String)
        ' Consulta para obtener datos del tercero por cédula
        Dim query As String = "SELECT Id_Tercero, Cedula, Nombres FROM Terceros WHERE Cedula = @Usuario"

        ' Establecer la conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Usuario", Usuario)

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()
                        TxNombreContratista.Text = dr("Nombres").ToString()
                    Else
                        TxNombreContratista.Text = String.Empty
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub Consultar_Id_Control()
        Dim Valor As Integer
        Session("sql") = "SELECT MAX(Id_Control_Contratistas) FROM Adm_Control_Contratistas"
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Try
                Valor = dr(0)
            Catch ex As Exception
                Valor = 0
            End Try
            dr.Close()

        End If
        Session("Id_Control_Contratistas") = Valor + 1
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub Guardar_Control()
        Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(User.Identity.Name)
        Session("sql") = "INSERT INTO Adm_Control_Contratistas ("
        Session("sql") += "Id_Control_Contratistas, "
        Session("sql") += "Id_Quien_Registra, "
        Session("sql") += "Id_Quien_Autoriza, "
        Session("sql") += "Id_Inmueble, "
        Session("sql") += "Cedula_Contratista, "
        Session("sql") += "Fecha_Inicio, "
        Session("sql") += "Fecha_Fin, "
        Session("sql") += "Id_Sede, "
        If TxObservacion.Text <> "" Then
            Session("sql") += "Observaciones, "
        End If
        If FLARL.HasFile = True Then
            Session("sql") += "ARL, "
        End If
        If TxFechaARL.Text <> "" Then
            Session("sql") += "Fecha_ARL, "
        End If
        If FLSeguridadSocial.HasFile = True Then
            Session("sql") += "Seguridad_Social, "
        End If
        If TxFechaSeguridadSocial.Text <> "" Then
            Session("sql") += "Fecha_Seguridad_Social, "
        End If
        If FLTrabajoAlturas.HasFile = True Then
            Session("sql") += "Trabajo_Alturas, "
        End If
        If TxFechaTrabajoAlturas.Text <> "" Then
            Session("sql") += "Fecha_Trabajo_Alturas, "
        End If
        Session("sql") += "Lunes, "
        Session("sql") += "Martes, "
        Session("sql") += "Miercoles, "
        Session("sql") += "Jueves, "
        Session("sql") += "Viernes, "
        Session("sql") += "Sabado, "
        Session("sql") += "Domingo, "
        Session("sql") += "Festivo, "
        Session("sql") += "Estado) VALUES ("

        Session("sql") += "" & Session("Id_Control_Contratistas") & ", "
        Session("sql") += "" & idTercero_Quien_Registra & ", "
        Session("sql") += "" & TxNombrePersona.SelectedValue & ", "
        Session("sql") += "'" & TxInmueble.Text & "', "
        Session("sql") += "" & TxCedulaContratista.Text.Trim() & ", "
        Dim Fecha_Ini As Date = TxFechaInicio.Text
        Session("sql") += "'" & Fecha_Ini.ToString("yyyy-MM-dd") & "', "
        Dim Fecha_Fin As Date = TxFechaFin.Text
        Session("sql") += "'" & Fecha_Fin.ToString("yyyy-MM-dd") & "', "
        Session("sql") += "" & TxSede.SelectedValue & ", "
        If TxObservacion.Text <> "" Then
            Session("sql") += "'" & TxObservacion.Text & "', "
        End If
        If FLARL.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLARL.FileName)

            FLARL.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "ARL_" + TxCedulaContratista.Text.Trim() + Ext.ToString))
            Session("sql") += "'" & "ARL_" + TxCedulaContratista.Text.Trim() + Ext.ToString & "', "
        End If
        If TxFechaARL.Text <> "" Then
            Dim Fecha_ARL As Date = TxFechaARL.Text
            Session("sql") += "'" & Fecha_ARL.ToString("yyyy-MM-dd") & "', "
        End If
        If FLSeguridadSocial.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLSeguridadSocial.FileName)

            FLSeguridadSocial.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Seguridad_Social_" + TxCedulaContratista.Text.Trim() + Ext.ToString))
            Session("sql") += "'" & "Seguridad_Social_" + TxCedulaContratista.Text.Trim() + Ext.ToString & "', "
        End If
        If TxFechaSeguridadSocial.Text <> "" Then
            Dim Fecha_Seg As Date = TxFechaSeguridadSocial.Text
            Session("sql") += "'" & Fecha_Seg.ToString("yyyy-MM-dd") & "', "
        End If
        If FLTrabajoAlturas.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLTrabajoAlturas.FileName)

            FLTrabajoAlturas.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Trabajo_Alturas_" + TxCedulaContratista.Text.Trim() + Ext.ToString))
            Session("sql") += "'" & "Trabajo_Alturas_" + TxCedulaContratista.Text.Trim() + Ext.ToString & "', "
        End If
        If TxFechaTrabajoAlturas.Text <> "" Then
            Dim Fecha_ALT As Date = TxFechaTrabajoAlturas.Text
            Session("sql") += "'" & Fecha_ALT.ToString("yyyy-MM-dd") & "', "
        End If
        If TxLunes.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxMartes.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxMiercoles.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxJueves.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxViernes.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxSabado.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxDomingo.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxFestivo.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        Session("sql") += " 1) "
        Guardar_Datos()
    End Sub

    Private Sub Actualizar_Control()
        Try

            Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(Convert.ToInt64(User.Identity.Name))

            Dim query As New System.Text.StringBuilder("UPDATE dbo.Adm_Control_Contratistas SET ")

            query.Append("Id_Quien_Registra = @Id_Quien_Registra, ")
            query.Append("Id_Quien_Autoriza = @Id_Quien_Autoriza, ")
            query.Append("Id_Inmueble = @Id_Inmueble, ")
            query.Append("Fecha_Inicio = @Fecha_Inicio, ")
            query.Append("Fecha_Fin = @Fecha_Fin, ")
            If TxObservacion.Text <> "" Then
                query.Append("Observaciones = @Observaciones, ")
            Else
                query.Append("Observaciones = NULL, ")
            End If
            If FLARL.HasFile = True Then
                query.Append("ARL = @ARL, ")
            Else
                query.Append("ARL = NULL, ")
            End If
            If TxFechaARL.Text <> "" Then
                query.Append("Fecha_ARL = @Fecha_ARL, ")
            Else
                query.Append("Fecha_ARL = NULL, ")
            End If
            If FLSeguridadSocial.HasFile = True Then
                query.Append("Seguridad_Social = @Seguridad_Social, ")
            Else
                query.Append("Seguridad_Social = NULL, ")
            End If
            If TxFechaSeguridadSocial.Text <> "" Then
                query.Append("Fecha_Seguridad_Social = @Fecha_Seguridad_Social, ")
            Else
                query.Append("Fecha_Seguridad_Social = NULL, ")
            End If
            If FLTrabajoAlturas.HasFile = True Then
                query.Append("Trabajo_Alturas = @Trabajo_Alturas, ")
            Else
                query.Append("Trabajo_Alturas = NULL, ")
            End If
            If TxFechaTrabajoAlturas.Text <> "" Then
                query.Append("Fecha_Trabajo_Alturas = @Fecha_Trabajo_Alturas, ")
            Else
                query.Append("Fecha_Trabajo_Alturas = NULL, ")
            End If
            query.Append("Lunes = @Lunes, ")
            query.Append("Martes = @Martes, ")
            query.Append("Miercoles = @Miercoles, ")
            query.Append("Jueves = @Jueves, ")
            query.Append("Viernes = @Viernes, ")
            query.Append("Sabado = @Sabado, ")
            query.Append("Domingo = @Domingo, ")
            query.Append("Festivo = @Festivo ")
            query.Append("WHERE Id_Control_Contratistas = @Id_Control_Contratistas")

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query.ToString(), conn)
                    cmd.Parameters.AddWithValue("@Id_Quien_Registra", idTercero_Quien_Registra)
                    cmd.Parameters.AddWithValue("@Id_Quien_Autoriza", TxNombrePersona.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_Inmueble", TxInmueble.SelectedValue)
                    Dim Fecha_Ini As Date = TxFechaInicio.Text
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", Fecha_Ini.ToString("yyyy-MM-dd"))
                    Dim Fecha_Fin As Date = TxFechaFin.Text
                    cmd.Parameters.AddWithValue("@Fecha_Fin", Fecha_Fin.ToString("yyyy-MM-dd"))

                    If TxObservacion.Text <> "" Then
                        cmd.Parameters.AddWithValue("@Observaciones", TxObservacion.Text)
                    End If

                    If FLARL.HasFile = True Then
                        Dim Ext As String = System.IO.Path.GetExtension(FLARL.FileName)
                        FLARL.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "ARL_" + TxCedulaContratista.Text.Trim() + Ext.ToString))
                        cmd.Parameters.AddWithValue("@ARL", "ARL_" + TxCedulaContratista.Text.Trim() + Ext.ToString)
                    End If

                    If TxFechaARL.Text <> "" Then
                        Dim Fecha_ARL As Date = TxFechaARL.Text
                        cmd.Parameters.AddWithValue("@Fecha_ARL", Fecha_ARL.ToString("yyyy-MM-dd"))
                    End If

                    If FLSeguridadSocial.HasFile = True Then
                        Dim Ext As String = System.IO.Path.GetExtension(FLSeguridadSocial.FileName)
                        FLARL.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Seguridad_Social_" + TxCedulaContratista.Text.Trim() + Ext.ToString))
                        cmd.Parameters.AddWithValue("@Seguridad_Social", "Seguridad_Social_" + TxCedulaContratista.Text.Trim() + Ext.ToString)
                    End If

                    If TxFechaSeguridadSocial.Text <> "" Then
                        Dim Fecha_Seg As Date = TxFechaSeguridadSocial.Text
                        cmd.Parameters.AddWithValue("@Fecha_Seguridad_Social", Fecha_Seg.ToString("yyyy-MM-dd"))
                    End If

                    If FLTrabajoAlturas.HasFile = True Then
                        Dim Ext As String = System.IO.Path.GetExtension(FLTrabajoAlturas.FileName)
                        FLARL.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Trabajo_Alturas_" + TxCedulaContratista.Text.Trim() + Ext.ToString))
                        cmd.Parameters.AddWithValue("@Trabajo_Alturas_", "Trabajo_Alturas_" + TxCedulaContratista.Text.Trim() + Ext.ToString)
                    End If

                    If TxFechaTrabajoAlturas.Text <> "" Then
                        Dim Fecha_ALT As Date = TxFechaTrabajoAlturas.Text
                        cmd.Parameters.AddWithValue("@Fecha_Trabajo_Alturas", Fecha_ALT.ToString("yyyy-MM-dd"))
                    End If

                    If TxLunes.Checked = True Then
                        cmd.Parameters.AddWithValue("@Lunes", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Lunes", 0)
                    End If

                    If TxMartes.Checked = True Then
                        cmd.Parameters.AddWithValue("@Martes", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Martes", 0)
                    End If

                    If TxMiercoles.Checked = True Then
                        cmd.Parameters.AddWithValue("@Miercoles", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Miercoles", 0)
                    End If

                    If TxJueves.Checked = True Then
                        cmd.Parameters.AddWithValue("@Jueves", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Jueves", 0)
                    End If

                    If TxViernes.Checked = True Then
                        cmd.Parameters.AddWithValue("@Viernes", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Viernes", 0)
                    End If

                    If TxSabado.Checked = True Then
                        cmd.Parameters.AddWithValue("@Sabado", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Sabado", 0)
                    End If

                    If TxDomingo.Checked = True Then
                        cmd.Parameters.AddWithValue("@Domingo", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Domingo", 0)
                    End If

                    If TxFestivo.Checked = True Then
                        cmd.Parameters.AddWithValue("@Festivo", 1)
                    Else
                        cmd.Parameters.AddWithValue("@Festivo", 0)
                    End If

                    cmd.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = "Error en Actualizar_Control()" & ex.Message
        End Try
    End Sub

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

    Private Sub Consultar_Usuario(ByVal Id As Long)
        Dim query As String = "SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Id_Empresa, CC.Cedula_Contratista, T.Nombres AS Contratista, CC.Fecha_Inicio, CC.Fecha_Fin, CC.Observaciones, CC.Fecha_ARL, CC.Fecha_Seguridad_Social, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo FROM Adm_Control_Contratistas CC " &
                              "LEFT JOIN Terceros T ON T.Cedula = CONVERT(NVARCHAR, CC.Cedula_Contratista) " &
                              "LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza " &
                              "WHERE CC.Id_Control_Contratistas = @Id"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Id", Id)

                conn.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        TxBuscarPersona.Text = dr("Quien_Autoriza").ToString()
                        TxNombrePersona.Items.Clear()
                        TxNombrePersona.Items.Add(New ListItem(dr("Quien_Autoriza").ToString, dr("Id_Quien_Autoriza").ToString))
                        TxInmueble.Text = dr("Id_Inmueble").ToString()
                        TxCedulaContratista.Text = dr("Cedula_Contratista").ToString()
                        TxNombreContratista.Text = dr("Contratista").ToString()
                        If dr("Id_Empresa").ToString() <> "" Then
                            TxEmpresa.SelectedValue = dr("Id_Empresa").ToString()
                        End If
                        Dim Fecha_Ini As Date = dr("Fecha_Inicio").ToString()
                        TxFechaInicio.Text = Fecha_Ini.ToString("yyyy-MM-dd")
                        Dim Fecha_Fin As Date = dr("Fecha_Fin").ToString()
                        TxFechaFin.Text = Fecha_Fin.ToString("yyyy-MM-dd")
                        If dr("Observaciones").ToString() <> "" Then
                            TxObservacion.Text = dr("Observaciones").ToString()
                        End If
                        If dr("Fecha_ARL").ToString() <> "" Then
                            Dim Fecha_ARL As Date = dr("Fecha_ARL").ToString()
                            TxFechaARL.Text = Fecha_ARL.ToString("yyyy-MM-dd")
                        End If
                        If dr("Fecha_Seguridad_Social").ToString() <> "" Then
                            Dim Fecha_Seguridad As Date = dr("Fecha_Seguridad_Social").ToString()
                            TxFechaSeguridadSocial.Text = Fecha_Seguridad.ToString("yyyy-MM-dd")
                        End If
                        If dr("Fecha_Trabajo_Alturas").ToString() <> "" Then
                            Dim Fecha_Trab As Date = dr("Fecha_Trabajo_Alturas").ToString()
                            TxFechaTrabajoAlturas.Text = Fecha_Trab.ToString("yyyy-MM-dd")
                        End If
                        TxLunes.Checked = If(dr("Lunes").ToString() = "True", True, False)
                        TxMartes.Checked = If(dr("Martes").ToString() = "True", True, False)
                        TxMiercoles.Checked = If(dr("Miercoles").ToString() = "True", True, False)
                        TxJueves.Checked = If(dr("Jueves").ToString() = "True", True, False)
                        TxViernes.Checked = If(dr("Viernes").ToString() = "True", True, False)
                        TxSabado.Checked = If(dr("Sabado").ToString() = "True", True, False)
                        TxDomingo.Checked = If(dr("Domingo").ToString() = "True", True, False)

                        BtGuardar.Text = "ACTUALIZAR"
                    Else
                        TxBuscarPersona.Text = String.Empty
                        TxNombrePersona.Items.Clear()
                        TxInmueble.Text = String.Empty
                        TxCedulaContratista.Text = String.Empty
                        TxNombreContratista.Text = String.Empty
                        TxFechaInicio.Text = String.Empty
                        TxFechaFin.Text = String.Empty
                        TxObservacion.Text = String.Empty
                        TxLunes.Checked = False
                        TxMartes.Checked = False
                        TxMiercoles.Checked = False
                        TxJueves.Checked = False
                        TxViernes.Checked = False
                        TxSabado.Checked = False
                        TxDomingo.Checked = False
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub LvControlContratista_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvControlContratista.ItemDataBound
    End Sub

    Private Sub LvControlContratista_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvControlContratista.ItemCommand
        Dim Id_Control_ContratistasLabel As Label = DirectCast(e.Item.FindControl("Id_Control_ContratistasLabel"), Label)
        Dim Cedula_ContratistaLabel As Label = DirectCast(e.Item.FindControl("Cedula_ContratistaLabel"), Label)
        Dim Id_HikCamaraLabel As Label = DirectCast(e.Item.FindControl("Id_HikCamaraLabel"), Label)
        Session("Id_Control_Contratistas") = Id_Control_ContratistasLabel.Text
        Session("Id_Empleado") = Id_HikCamaraLabel.Text
        If e.CommandName = "Editar" Then
            Consultar_Usuario(Id_Control_ContratistasLabel.Text)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then

            If TxTipoRegistro.SelectedValue = "0" Then

                Dim usuarioCreado As Boolean = False
                Dim fotoAgregada As Boolean = False
                Dim mensaje As String = String.Empty
                Dim script As String = String.Empty

                If Not RegistroExisteEnBD(TxCedulaContratista.Text.Trim()) Then
                    Consultar_Id_Persona()
                    Consultar_Id_HikCamara()
                    Guardar_Persona()
                    Consultar_Id_Control()
                    Guardar_Control()
                    Crear_Usuario()

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
                    Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista.aspx")
                Else
                    If Not Tiene_Facial(TxCedulaContratista.Text.Trim()) Then
                        Consultar_Id_HikCamara()
                        Actualizar_Rol_Hikvision(TxCedulaContratista.Text.Trim())
                        Consultar_Id_Control()
                        Guardar_Control()
                        Crear_Usuario()

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
                        Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista.aspx")
                    Else
                        Actualizar_Rol(TxCedulaContratista.Text.Trim())
                        Consultar_Id_Control()
                        Guardar_Control()
                        Dim Id_HikCamara As Long = ObtenerId_HikCamara(TxCedulaContratista.Text.Trim())
                        Editar_Usuario_Crear(Id_HikCamara)

                        ' Verificar si el usuario fue creado
                        If Session("Status_Code_Editar") = "1" Then
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
                                Actualizar_Foto_Persona_Archivo()

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
                                Actualizar_Foto_Persona_Tomada()

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
                        Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista.aspx")
                    End If
                End If
            ElseIf TxTipoRegistro.SelectedValue = "1" Then
                Dim usuarioCreado As Boolean = False
                Dim fotoAgregada As Boolean = False
                Dim mensaje As String = String.Empty
                Dim script As String = String.Empty

                If Not RegistroExisteEnBD(TxCedulaContratista.Text.Trim()) Then
                    Consultar_Id_Persona()
                    Guardar_Persona()
                Else
                    Actualizar_Rol(TxCedulaContratista.Text.Trim())
                End If

                Consultar_Id_Control()
                Guardar_Control()

                ' Verificar si el usuario fue creado
                If ErrorOp = Nothing Then
                    usuarioCreado = True
                    mensaje = "Usuario creado correctamente."
                Else
                    mensaje = "Error al crear el contratista manual, volver a intentar."
                    script = $"swal('Error!', '{mensaje}', 'warning');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Si el usuario fue creado, proceder con la foto
                If usuarioCreado Then
                    ' Verificar si se subió una foto desde un archivo
                    If FlFoto.HasFile Then
                        FlFoto.SaveAs(Server.MapPath("../../Adjunto/Foto_Usuarios/" + TxCedulaContratista.Text + System.IO.Path.GetExtension(FlFoto.FileName).ToString))

                        ' Verificar el estado del proceso de agregar foto desde archivo
                        If ErrorOp = Nothing Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (archivo) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                            script = $"swal('Error!', '{mensaje}', 'warning');"
                        End If

                        ' Verificar si se subió una foto desde imagen tomada (base64)
                    ElseIf base64image.Value <> "" Then
                        GuardarImagen()

                        ' Verificar el estado del proceso de agregar foto tomada
                        If ErrorOp = Nothing Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (tomada) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto tomada."
                            script = $"swal('Error!', '{mensaje}', 'warning');"
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
                Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista.aspx")

            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            If TxTipoRegistro.SelectedValue = "0" Then

                Dim usuarioActualizado As Boolean = False
                Dim fotoAgregada As Boolean = False
                Dim mensaje As String = String.Empty
                Dim script As String = String.Empty

                Actualizar_Control()
                Editar_Usuario_Actualizar()

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
                Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista.aspx")
            ElseIf TxTipoRegistro.SelectedValue = "1" Then
                Dim usuarioActualizado As Boolean = False
                Dim fotoAgregada As Boolean = False
                Dim mensaje As String = String.Empty
                Dim script As String = String.Empty

                Actualizar_Control()

                If ErrorOp = Nothing Then
                    usuarioActualizado = True
                    mensaje = "Control contratista actualizado correctamente."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                Else
                    mensaje = " Ocurrió un error al actualizar el control contratista."
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    mensaje = "Error al crear el usuario."
                    Exit Sub
                End If

                ' Si el usuario fue actualizado, proceder con la foto
                If usuarioActualizado Then
                    ' Verificar si se subió una foto desde un archivo
                    If FlFoto.HasFile Then
                        FlFoto.SaveAs(Server.MapPath("../../Adjunto/Foto_Usuarios/" + TxCedulaContratista.Text + System.IO.Path.GetExtension(FlFoto.FileName).ToString))

                        ' Verificar el estado del proceso de agregar foto desde archivo
                        If ErrorOp = Nothing Then
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

                        ' Verificar el estado del proceso de agregar foto tomada
                        If ErrorOp = Nothing Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (tomada) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto tomada."
                            script = $"swal('Error!', '{mensaje}', 'warning');"
                        End If
                    End If

                    ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                    If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                        mensaje &= " OJO sin foto."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    End If
                End If

                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista.aspx")

            End If
        End If

    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = "" Then
            LvControlContratista.DataSourceID = "SqlControlContratista"
        Else
            LvControlContratista.DataSourceID = "SqlBuscarControlContratista"
        End If
        LvControlContratista.DataBind()
    End Sub

End Class