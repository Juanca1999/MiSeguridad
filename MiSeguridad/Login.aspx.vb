Imports System.IO
Imports System.Data.SqlClient
Imports System.Security.Cryptography

Public Class Login

    Inherits System.Web.UI.Page

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session("dr") = dr
        Session("conn") = conn
    End Sub

    Public UsuarioLoginLogin As String
    Public AreaLoginLogin As String
    Public IdAreaLogin As String
    Public IdNombreLogin As String

    Private Function ValidateUser(ByVal userName As String, ByVal passWord As String) As Boolean

        Dim lookupPassword As String

        lookupPassword = Nothing

        ' Check for an invalid userName.
        ' userName  must not be set to nothing and must be between one and 15 characters.
        If ((userName Is Nothing)) Then
            System.Diagnostics.Trace.WriteLine("[ValidateUser] Input validation of userName failed.")
            Return False
        End If
        If ((userName.Length = 0) Or (userName.Length > 15)) Then
            System.Diagnostics.Trace.WriteLine("[ValidateUser] Input validation of userName failed.")
            Return False
        End If

        ' Check for invalid passWord.
        ' passWord must not be set to nothing and must be between one and 25 characters.
        If (passWord Is Nothing) Then
            System.Diagnostics.Trace.WriteLine("[ValidateUser] Input validation of passWord failed.")
            Return False
        End If
        If ((passWord.Length = 0) Or (passWord.Length > 25)) Then
            System.Diagnostics.Trace.WriteLine("[ValidateUser] Input validation of passWord failed.")
            Return False
        End If

        Try
            ' Consult with your SQL Server administrator for an appropriate connection
            ' string to use to connect to your local SQL Server.

            Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Dim cmdo As New SqlCommand
            con.Open()

            ' Create SqlCommand to select pwd field from the users table given a supplied userName.
            cmdo = New SqlCommand("SELECT Password FROM Terceros WHERE Usuario = @userName AND Estado = 1", con)
            cmdo.Parameters.Add("@userName", SqlDbType.VarChar, 25)
            cmdo.Parameters("@userName").Value = userName

            ' Execute command and fetch pwd field into lookupPassword string.
            lookupPassword = cmdo.ExecuteScalar()

            ' Cleanup command and connection objects.
            cmdo.Dispose()
            con.Dispose()
        Catch ex As Exception
            ' Add error handling here for debugging.
            ' This error message should not be sent back to the caller.
            System.Diagnostics.Trace.WriteLine("[ValidateUser] Exception " & ex.Message)
        End Try

        ' If no password found, return false.
        If (lookupPassword Is Nothing) Then
            ' You could write failed login attempts here to the event log for additional security.
            Return False
        End If

        ' Compare lookupPassword and input passWord by using a case-sensitive comparison.
        Return (String.Compare(lookupPassword, passWord, False) = 0)

    End Function

    Protected Sub BtIniciar_Click(sender As Object, e As EventArgs) Handles BtIniciar.Click
        If ValidateUser(TxNombreUsuario.Text, TxContraseña.Text) Then
            Dim tkt As FormsAuthenticationTicket
            Dim cookiestr As String
            Dim ck As HttpCookie

            tkt = New FormsAuthenticationTicket(1, TxNombreUsuario.Text, DateTime.Now(),
            DateTime.Now.AddMinutes(720), chkPersistCookie.Checked, "your custom data")
            cookiestr = FormsAuthentication.Encrypt(tkt)
            ck = New HttpCookie(FormsAuthentication.FormsCookieName(), cookiestr)
            If (chkPersistCookie.Checked) Then ck.Expires = tkt.Expiration
            ck.Path = FormsAuthentication.FormsCookiePath()
            Response.Cookies.Add(ck)

            Dim strRedirect As String
            strRedirect = Request("ReturnURL")
            If strRedirect <> "" Then
                Dim rutaArchivo As String = Server.MapPath("Diseno/2024/li.dat")
                Dim password As String = "M1r0@cc3$$P0int!2024"

                Dim fechaLicencia As String = DesencriptarArchivo(rutaArchivo, password)

                Dim fechaExpiracion As DateTime
                If DateTime.TryParse(fechaLicencia, fechaExpiracion) Then

                    Dim fechaActual As DateTime = DateTime.Now.Date
                    Dim fechaExpiracionSoloFecha As DateTime = fechaExpiracion.Date

                    Dim diasRestantes As Integer = (fechaExpiracionSoloFecha - fechaActual).Days

                    If fechaActual > fechaExpiracionSoloFecha Then
                        Response.Redirect("RenovarLicencia.aspx")
                    ElseIf diasRestantes <= 14 Then
                        ' Avisar cuando queden 14 días o menos
                        Session("MensajeLicencia") = "Atención: La licencia caduca en " & diasRestantes & " días, el " & fechaExpiracionSoloFecha.ToString("dd/MM/yyyy")
                        Response.Redirect(strRedirect, True)
                    Else
                        Response.Redirect(strRedirect, True)
                    End If
                Else
                    Label4.Text = "Error al validar la licencia, Consultar con el administrador"
                    Label4.Attributes.Add("style", "color:Red")
                End If
            Else
                Dim rutaArchivo As String = Server.MapPath("Diseno/2024/li.dat")
                Dim password As String = "M1r0@cc3$$P0int!2024"

                Dim fechaLicencia As String = DesencriptarArchivo(rutaArchivo, password)

                Dim fechaExpiracion As DateTime
                If DateTime.TryParse(fechaLicencia, fechaExpiracion) Then

                    Dim fechaActual As DateTime = DateTime.Now.Date
                    Dim fechaExpiracionSoloFecha As DateTime = fechaExpiracion.Date

                    Dim diasRestantes As Integer = (fechaExpiracionSoloFecha - fechaActual).Days

                    If fechaActual > fechaExpiracionSoloFecha Then
                        Response.Redirect("RenovarLicencia.aspx")
                    ElseIf diasRestantes <= 14 Then
                        Session("MensajeLicencia") = "Atención: La licencia caduca en " & diasRestantes & " días, el " & fechaExpiracionSoloFecha.ToString("dd/MM/yyyy")
                        SqlConnection.ClearPool(conn)
                        strRedirect = "Inicio.aspx"
                        Response.Redirect(strRedirect, True)
                    Else
                        SqlConnection.ClearPool(conn)
                        strRedirect = "Inicio.aspx"
                        Response.Redirect(strRedirect, True)
                    End If
                Else
                    Label4.Text = "Error al validar la licencia, Consultar con el administrador"
                    Label4.Attributes.Add("style", "color:Red")
                End If

            End If
        Else
            Label4.Text = "Usuario o Contraseña Incorrecta"
            Label4.Attributes.Add("style", "color:Red")
        End If
    End Sub

    Private Verificado As Boolean

    Public Function DesencriptarArchivo(ByVal inputFile As String, ByVal password As String) As String
        Dim key As Byte() = Encoding.UTF8.GetBytes(password.PadRight(32))

        Using aes As Aes = aes.Create()
            aes.Key = key

            Using fileStream As New FileStream(inputFile, FileMode.Open)
                Dim iv(15) As Byte
                fileStream.Read(iv, 0, iv.Length)

                aes.IV = iv
                Using cryptoStream As New CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read)
                    Using reader As New StreamReader(cryptoStream)
                        Return reader.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using
    End Function

End Class