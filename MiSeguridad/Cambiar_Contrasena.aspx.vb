Imports System.IO
Imports System.Net
Imports ImageMagick
Imports System.Drawing
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Drawing.Imaging
Imports System.Security.Cryptography.X509Certificates

Public Class Cambiar_Contrasena
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session("Usuario") = User.Identity.Name
        End If
    End Sub

    Private errores As New List(Of String)

    Private Sub EnviarCorreoError()
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Error Miseguridad")
            msg.To.Add(New MailAddress("juan.aristizabal@miroseguridad.com"))
            msg.Subject = "Error Miseguridad"
            msg.Body = "Error al usuario: " + User.Identity.Name + "<br />" + "Nombre: " + Session("Nombres") + "<br />" + "Formulario: " + Request.Url.ToString + "<br /><br />"
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

    Private Function RegistroExisteEnBD(Cedula As Long) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

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

    Protected Sub BtnCambiarClave_Click(sender As Object, e As EventArgs) Handles BtnCambiarClave.Click

        Dim usuario As String = Session("Usuario")
        Dim contraseñaActual As String = TxComtraseñaActual.Text.Trim()
        Dim nuevaContraseña As String = TxNuevaContraseña.Text.Trim()
        Dim confirmarNueva As String = TxConfirmarNuevaContraseña.Text.Trim()

        ' Validaciones
        If String.IsNullOrEmpty(contraseñaActual) Then
            errores.Add("Debe ingresar la contraseña actual.")
        End If

        If String.IsNullOrEmpty(nuevaContraseña) Then
            errores.Add("Debe ingresar la nueva contraseña.")
        End If

        If String.IsNullOrEmpty(confirmarNueva) Then
            errores.Add("Debe confirmar la nueva contraseña.")
        End If

        If nuevaContraseña <> confirmarNueva Then
            errores.Add("La nueva contraseña y la confirmación no coinciden.")
        End If

        If errores.Count = 0 Then
            Dim contraseñaBD As String = ""

            ' Obtener contraseña actual de la BD
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Dim cmd As New SqlCommand("SELECT Password FROM Terceros WHERE Usuario = @Usuario", conn)
                cmd.Parameters.AddWithValue("@Usuario", usuario)

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    contraseñaBD = result.ToString()
                Else
                    errores.Add("Usuario no encontrado.")
                End If
            End Using

            ' Comparar contraseña actual
            If contraseñaBD <> contraseñaActual Then
                errores.Add("La contraseña actual no es correcta.")
            End If
        End If

        ' Si no hay errores, actualizar
        If errores.Count = 0 Then
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Dim cmd As New SqlCommand("UPDATE Terceros SET Password = @NuevaPassword WHERE Usuario = @Usuario", conn)
                cmd.Parameters.AddWithValue("@NuevaPassword", nuevaContraseña)
                cmd.Parameters.AddWithValue("@Usuario", usuario)

                Dim filasAfectadas = cmd.ExecuteNonQuery()
                If filasAfectadas > 0 Then
                    Dim script As String = "swal('Excelente!', 'Contraseña actualizada correctamente.', 'success');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "exito", script, True)
                    Response.AddHeader("Refresh", "2; URL=Cambiar_Contrasena.aspx")
                Else
                    errores.Add("No se pudo actualizar la contraseña.")
                End If
            End Using
        End If

        ' Mostrar errores con SweetAlert
        If errores.Count > 0 Then
            Dim mensajeError As String = String.Join("<br/>", errores)
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError.Replace("'", "\'"))
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "error", script, True)
        End If
    End Sub

End Class