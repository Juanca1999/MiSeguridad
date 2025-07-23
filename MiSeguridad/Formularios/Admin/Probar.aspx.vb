
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Probar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name

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
    Shared errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Shared Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 1 AND Tipo_Evento = 0")

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
    Shared Function EjecutarConsultaSQL(query As String) As DataTable
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

    Private Sub Crear_Placa()

        ' ⚠️ Aceptar certificados autofirmados (solo si confías en el dispositivo)
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        ServicePointManager.ServerCertificateValidationCallback = Function(sender, cert, chain, errors) True

        Try
            Dim HikvisionURL As String = "https://172.16.20.19/ISAPI/Traffic/channels/1/licensePlateAuditData/record?format=json"
            Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            requestDocument.Method = "PUT"
            requestDocument.ContentType = "application/json"

            ' Autenticación básica
            Dim authInfo As String = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:Miro-321"))
            requestDocument.Headers("Authorization") = "Basic " & authInfo
            requestDocument.Accept = "application/json"
            requestDocument.UserAgent = "Mozilla/5.0"

            ' JSON a enviar
            Dim jsonBody As String = "
            {
                ""LicensePlateInfoList"": [
                    {
                        ""id"": """",
                        ""listType"": ""allowList"",
                        ""LicensePlate"": ""FFF333""
                    }
                ]
            }"

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
            requestDocument.ContentLength = byteArray.Length

            Using dataStream As Stream = requestDocument.GetRequestStream()
                dataStream.Write(byteArray, 0, byteArray.Length)
            End Using

            Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Placa") = jsonResponse("statusCode").ToString()
                End Using
            End Using

        Catch ex As Exception
            Session("Status_Code_Placa") = 0
            errores.Add("Error al crear placa en hikvision " & ex.Message)
        End Try

    End Sub

    Shared Function Obtener_IdHikvision_Estudiantes_Desde_BaseDeDatos() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Id_HikCamara IS NOT NULL AND Id_Rol = 11")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
    End Function

    Shared Sub Activar_Accesos_PorLista(idUsuarios As List(Of String))
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each idUsuario As String In idUsuarios
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
                    End Using

                Catch ex As Exception
                    errores.Add("Error al ACTIVAR acceso del usuario " & idUsuario & " en cámara " & ipCamara & ": " & ex.Message)
                End Try
            Next
        Next
    End Sub


    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click

        Dim todosLosEstudiantes As List(Of String) = Obtener_IdHikvision_Estudiantes_Desde_BaseDeDatos()
        Activar_Accesos_PorLista(todosLosEstudiantes)

        If errores.Count > 0 Then

            Dim erroresLista As String = String.Join(", ", errores)
            Dim script As String = String.Format("swal('OJO!', 'Los siguientes usuarios tuvieron problemas: {0}', 'warning');", erroresLista)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "errorCorreos", script, True)
        Else
            Dim script As String = String.Format("swal('Excelente!', 'Camaras actualizadas correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If
    End Sub

End Class