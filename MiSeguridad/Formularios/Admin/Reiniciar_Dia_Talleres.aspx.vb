
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Reiniciar_Dia_Talleres
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name

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
    Shared errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Shared Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Inmueble <> 'COMEDOR'")

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

    ' Método para obtener las Id_HikVision de empleados de las cámaras desde la base de datos
    Shared Function ObtenerIdHikvisionEmpleadosDesdeBaseDeDatos() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Id_HikCamara IS NOT NULL AND Id_Rol IN (8,1,5)")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
    End Function

    Shared Sub Activar_Accesos_Empleados()

        ' Listas de cámaras y usuarios obtenidas desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()
        Dim Id_HikVision As List(Of String) = ObtenerIdHikvisionEmpleadosDesdeBaseDeDatos()

        ' Iterar sobre cada usuario
        For Each idUsuario As String In Id_HikVision
            ' Iterar sobre cada cámara
            For Each ipCamara As String In ipCamaras
                Try
                    ' Construir la URL de la solicitud para la cámara
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    Dim credentials As New NetworkCredential("admin", "Miro-321")
                    requestDocument.Credentials = credentials
                    requestDocument.PreAuthenticate = True

                    ' JSON que se enviará en el cuerpo de la solicitud
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

                    ' Convertir el JSON a un array de bytes
                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                    ' Establecer la longitud del cuerpo de la solicitud
                    requestDocument.ContentLength = byteArray.Length

                    ' Escribir en el cuerpo de la solicitud
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
                    End Using

                Catch ex As Exception
                    errores.Add("Error al dar acceso al usuario " & idUsuario & " en la cámara " & ipCamara & ": " & ex.Message)
                End Try
            Next
        Next
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click

        Activar_Accesos_Empleados()

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