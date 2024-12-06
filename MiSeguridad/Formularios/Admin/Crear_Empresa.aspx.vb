Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Empresa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
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

    Private Sub BtCrearEmpresa_ServerClick(sender As Object, e As EventArgs) Handles BtCrearEmpresa.ServerClick
        Response.Redirect("Crear_Empresa.aspx")
    End Sub

    Private Sub BtCrearSedes_ServerClick(sender As Object, e As EventArgs) Handles BtCrearSedes.ServerClick
        Response.Redirect("Crear_Sedes.aspx")
    End Sub

    Private Sub BtCrearAccesos_ServerClick(sender As Object, e As EventArgs) Handles BtCrearAccesos.ServerClick
        Response.Redirect("Crear_Accesos.aspx")
    End Sub

    Private Sub Guardar_Empresa()
        Session("sql") = "INSERT INTO Adm_Empresa ("
        Session("sql") += "Id_Empresa, "
        Session("sql") += "Empresa, "
        Session("sql") += "Direccion, "
        Session("sql") += "Telefono, "
        Session("sql") += "Administrador, "
        Session("sql") += "Titulo_Personas, "
        If FLogo.HasFile = True Then
            Session("sql") += "Logo, "
        End If
        Session("sql") += "Tel_Administrador) VALUES ("

        Session("sql") += "" & TxNitEmpresa.Text & ", "
        Session("sql") += "UPPER('" & TxNombreEmpresa.Text & "'), "
        Session("sql") += "'" & TxDireccion.Text & "', "
        Session("sql") += "" & TxTelefono.Text & ", "
        Session("sql") += "UPPER('" & TxAdministrador.Text & "'), "
        Session("sql") += "UPPER('" & TxTituloPersonal.Text & "'), "
        If FLogo.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLogo.FileName)

            FLogo.SaveAs(Server.MapPath("../../Adjunto/Logo/" + TxNitEmpresa.Text + Ext.ToString))
            Session("sql") += "'" & TxNitEmpresa.Text + Ext.ToString & "', "
        End If
        Session("sql") += "" & TxTelefonoAdministrador.Text & ") "
        Guardar_Datos()
    End Sub

    Private Sub Actualizar_Empresa()
        Session("sql") = "UPDATE dbo.Adm_Empresa SET "
        Session("sql") += "Id_Empresa = " & TxNitEmpresa.Text & ", "
        Session("sql") += "Empresa = UPPER('" & TxNombreEmpresa.Text & "'), "
        Session("sql") += "Titulo_Personas = UPPER('" & TxTituloPersonal.Text & "'), "
        Session("sql") += "Direccion = '" & TxDireccion.Text & "', "
        Session("sql") += "Telefono = " & TxTelefono.Text & ", "
        Session("sql") += "Administrador = UPPER('" & TxAdministrador.Text & "'), "
        If FLogo.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLogo.FileName)

            FLogo.SaveAs(Server.MapPath("../../Adjunto/Logo/" + TxNitEmpresa.Text + Ext.ToString))
            Session("sql") += "Logo = '" & TxNitEmpresa.Text + Ext.ToString & "', "
        End If
        Session("sql") += "Tel_Administrador = " & TxTelefonoAdministrador.Text & " "
        Session("sql") += "WHERE Id_Empresa = " & Session("Id_Empresa") & ""
        Guardar_Datos()
    End Sub

    Private Sub Consultar_Empresa(ByVal Nit As Long)
        Session("sql") = "SELECT Id_Empresa, Empresa, Direccion, Telefono, Logo, Administrador, Titulo_Personas, Tel_Administrador FROM Adm_Empresa "
        Session("sql") += "WHERE Id_Empresa = " & Nit & " "

        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            TxNitEmpresa.Text = dr("Id_Empresa").ToString
            TxNombreEmpresa.Text = dr("Empresa").ToString
            TxTituloPersonal.Text = dr("Titulo_Personas").ToString
            TxDireccion.Text = dr("Direccion").ToString
            TxTelefono.Text = dr("Telefono").ToString
            TxAdministrador.Text = dr("Administrador").ToString
            TxTelefonoAdministrador.Text = dr("Tel_Administrador").ToString
            BtGuardar.Text = "ACTUALIZAR"
            dr.Close()
        End If

        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub LvEmpresa_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvEmpresa.ItemCommand
        Dim Id_EmpresaLabel As Label = DirectCast(e.Item.FindControl("Id_EmpresaLabel"), Label)
        Session("Id_Empresa") = Id_EmpresaLabel.Text
        If e.CommandName = "Editar" Then
            Consultar_Empresa(Id_EmpresaLabel.Text)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then
            If HayRegistrosEnAdmEmpresa() = True Then
                ' Si ya hay registros, mostrar un mensaje de advertencia
                Dim script As String = String.Format("swal('OJO!', 'Ya hay una empresa creada', 'warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Else
                ' Si no hay registros, proceder a guardar la empresa
                Guardar_Empresa()
                Dim script As String = String.Format("swal('Excelente!', 'Empresa Guardada Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer2.Interval = 3000
            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            Actualizar_Empresa()
        End If

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Empresa Guardada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Timer1.Interval = 3000
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Empresa.aspx")
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Sedes.aspx")
    End Sub

End Class