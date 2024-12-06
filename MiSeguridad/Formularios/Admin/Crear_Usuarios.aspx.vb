Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Usuarios
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
            msg.From = New MailAddress(CorreoFrom, "Error MiSeguridad")
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

    Private Sub Consultar_Persona(ByVal Cedula As Integer)
        Session("sql") = "SELECT Nombres, Cargo, Correo FROM Terceros where Id_Tercero = " & Cedula & ""
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Session("Nombres") = dr("Nombres").ToString
            Session("Cargo") = dr("Cargo").ToString
            If dr("Correo") <> "" Then
                Session("Correo") = dr("Correo").ToString
            Else
                Session("Correo") = Nothing
            End If

            dr.Close()
        Else
            Session("Correo") = Nothing
        End If
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer
        Session("sql") = "SELECT MAX(Id_Tercero) FROM Terceros"
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
        Session("Id_Tercero") = Valor + 1
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub Guardar_Persona()
        Session("sql") = "INSERT INTO dbo.Terceros ("
        Session("sql") += "Id_Tercero, "
        If TxCedula.Text <> Nothing Then
            Session("sql") += "Cedula, "
        End If
        Session("sql") += "Nombres, "
        If TxIdRol.SelectedValue <> Nothing Then
            Session("sql") += "Id_Rol, "
        End If
        If TxCorreo.Text <> Nothing Then
            Session("sql") += "Correo, "
        End If
        If TxUsuario.Text <> Nothing Then
            Session("sql") += "Usuario, "
        End If
        If TxPassword.Text <> Nothing Then
            Session("sql") += "Password, "
        End If
        If TxCargo.Text <> Nothing Then
            Session("sql") += "Cargo, "
        End If
        If TxCelular.Text <> Nothing Then
            Session("sql") += "Celular, "
        End If
        If TxSede.SelectedValue <> Nothing Then
            Session("sql") += "Id_Sede, "
        End If
        Session("sql") += "Estado) VALUES ("

        Session("sql") += "" & Session("Id_Tercero") & ", "
        If TxCedula.Text <> Nothing Then
            Session("sql") += "" & TxCedula.Text & ", "
        End If
        Session("sql") += "'" & TxNombres.Text.ToUpper() & "', "
        If TxIdRol.SelectedValue <> Nothing Then
            Session("sql") += "" & TxIdRol.SelectedValue & ", "
        End If
        If TxCorreo.Text <> Nothing Then
            Session("sql") += "'" & TxCorreo.Text & "', "
        End If
        If TxUsuario.Text <> Nothing Then
            Session("sql") += "'" & TxUsuario.Text & "', "
        End If
        If TxPassword.Text <> Nothing Then
            Session("sql") += "'" & TxPassword.Text & "', "
        End If
        If TxCargo.Text <> Nothing Then
            Session("sql") += "'" & TxCargo.Text & "', "
        End If
        If TxCelular.Text <> Nothing Then
            Session("sql") += "" & TxCelular.Text & ", "
        End If
        If TxSede.SelectedValue <> Nothing Then
            Session("sql") += "" & TxSede.SelectedValue & ", "
        End If
        Session("sql") += "'" & TxEstado.SelectedValue & "')"
        Guardar_Datos()
    End Sub

    Private Sub Actualizar_Persona()
        Session("sql") = "UPDATE dbo.Terceros SET "
        Session("sql") += "Nombres = '" & TxNombres.Text.ToUpper() & "', "
        If TxCorreo.Text <> Nothing Then
            Session("sql") += "Correo = '" & TxCorreo.Text & "', "
        Else
            Session("sql") += "Correo = NULL, "
        End If
        If TxIdRol.SelectedValue <> Nothing Then
            Session("sql") += "Id_Rol = " & TxIdRol.SelectedValue & ", "
        Else
            Session("sql") += "Id_Rol = NULL, "
        End If
        If TxUsuario.Text <> Nothing Then
            Session("sql") += "Usuario = '" & TxUsuario.Text & "', "
        Else
            Session("sql") += "Usuario = NULL, "
        End If
        If TxPassword.Text <> Nothing Then
            Session("sql") += "Password = '" & TxPassword.Text & "', "
        Else
            Session("sql") += "Password = NULL, "
        End If
        If TxCargo.Text <> Nothing Then
            Session("sql") += "Cargo = '" & TxCargo.Text & "', "
        Else
            Session("sql") += "Cargo = NULL, "
        End If
        If TxCelular.Text <> Nothing Then
            Session("sql") += "Celular = " & TxCelular.Text & ", "
        Else
            Session("sql") += "Celular = NULL, "
        End If
        If TxSede.SelectedValue <> Nothing Then
            Session("sql") += "Id_Sede = " & TxSede.SelectedValue & ", "
        Else
            Session("sql") += "Id_Sede = NULL, "
        End If
        Session("sql") += "Estado = '" & TxEstado.SelectedValue & "' "
        Session("sql") += "WHERE Cedula = " & Session("Id_Persona") & ""
        Guardar_Datos()
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If Session("Id_Persona") = Nothing Then
            Consultar_Id_Persona()
            Guardar_Persona()
        Else
            Actualizar_Persona()
        End If
        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Persona Guardada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Session("Id_Persona") = Nothing

            Timer1.Interval = 5000
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If

    End Sub

    Private Sub Buscar_Persona(ByVal Usuario)
        Session("sql") = "SELECT T.Id_Tercero, T.Cedula, T.Nombres, T.Id_Rol, R.Rol, T.Correo, T.Usuario, T.Password, T.Cargo, T.Celular, T.Id_Sede, T.Estado FROM Terceros T LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol WHERE T.Cedula = " & Usuario & ""
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()

            Session("Cedula") = Usuario.ToString
            TxNombres.Text = dr("Nombres").ToString
            If dr("Correo").ToString <> Nothing Then
                TxCorreo.Text = dr("Correo").ToString
            End If
            If dr("Usuario").ToString <> Nothing Then
                TxUsuario.Text = dr("Usuario").ToString
            End If
            If dr("Password").ToString <> Nothing Then
                TxPassword.Text = dr("Password").ToString
            End If
            If dr("Cargo").ToString <> Nothing Then
                TxCargo.Text = dr("Cargo").ToString
            End If
            If dr("Id_Rol").ToString <> Nothing Then
                TxIdRol.SelectedValue = dr("Id_Rol").ToString
            End If
            If dr("Celular").ToString <> Nothing Then
                TxCelular.Text = dr("Celular").ToString
            End If
            If dr("Estado").ToString = "True" Then
                TxEstado.SelectedValue = "1"
            ElseIf dr("Estado").ToString = "False" Then
                TxEstado.SelectedValue = "0"
            Else
                TxEstado.SelectedValue = ""
            End If
            If dr("Id_Sede").ToString <> Nothing Then
                TxSede.SelectedValue = dr("Id_Sede").ToString
            End If
            dr.Close()
        Else
            Session("Id_Persona") = Nothing
            TxNombres.Text = Nothing
            TxCorreo.Text = Nothing
            TxPassword.Text = Nothing
            TxCargo.Text = Nothing
            TxCelular.Text = Nothing
            TxEstado.SelectedValue = Nothing
        End If
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub TxCedula_TextChanged(sender As Object, e As EventArgs) Handles TxCedula.TextChanged
        If TxCedula.Text <> Nothing Then
            Buscar_Persona(TxCedula.Text)
        End If
    End Sub

    Private Sub LvUsuarios_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvUsuarios.ItemCommand
        Dim Id_Persona As Label = DirectCast(e.Item.FindControl("CedulaLabel"), Label)
        Session("Id_Persona") = Id_Persona.Text
        Dim Id_TerceroLabel As Label = DirectCast(e.Item.FindControl("Id_TerceroLabel"), Label)
        Session("Id_Tercero") = Id_TerceroLabel.Text
        If e.CommandName = "Editar" Then
            PPermisos.Visible = False
            PUsuario.Visible = True
            TxCedula.Text = Id_Persona.Text
            Buscar_Persona(Id_Persona.Text)
        ElseIf e.CommandName = "Permisos" Then
            PPermisos.Visible = True
            PUsuario.Visible = False
            TxModulos.SelectedValue = Nothing
            Session("Id_Formulario") = Nothing
        End If
    End Sub

    Private Sub LbNueva_Persona_Click(sender As Object, e As EventArgs) Handles LbNueva_Persona.Click
        Session("Id_Persona") = Nothing
        Session("Id_Formulario") = Nothing
        Response.Redirect("Crear_Usuarios.aspx")
    End Sub

    Private Sub TxModulos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TxModulos.SelectedIndexChanged
        Session("Id_Formulario") = Nothing
        LvSubmenu.DataBind()
    End Sub

    Private Sub LvFormularios_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvFormularios.ItemCommand
        Dim Id_Formulario As Label = DirectCast(e.Item.FindControl("Id_Formulario"), Label)
        Consultar_Permisos(Session("Id_Tercero"), Id_Formulario.Text, 0)
        Session("Id_Formulario") = Id_Formulario.Text
        LvSubmenu.DataBind()
    End Sub

    Private Sub LvSubmenu_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvSubmenu.ItemCommand
        Dim Id_submenu As Label = DirectCast(e.Item.FindControl("Id_submenu"), Label)
        Consultar_Permisos(Session("Id_Tercero"), Session("Id_Formulario"), Id_submenu.Text)
    End Sub

    Private Sub Guardar_Permiso(ByVal Id_Tercero As Long, ByVal Id_Formulario As String, ByVal Id_submenu As Integer)
        Session("sql") = "INSERT INTO dbo.Sys_Permisos ("
        Session("sql") += "Id_Tercero, "
        Session("sql") += "Id_Formulario, "
        Session("sql") += "Id_submenu) VALUES ("
        Session("sql") += "" & Id_Tercero & ", "
        Session("sql") += "'" & Id_Formulario & "', "
        Session("sql") += "" & Id_submenu & ")"
        Guardar_Datos()
    End Sub

    Private Sub Borrar_Permiso(ByVal Id_Tercero As Long, ByVal Id_Formulario As String, ByVal Id_submenu As Integer)
        Session("sql") = "DELETE FROM dbo.Sys_Permisos WHERE "
        Session("sql") += "Id_Tercero = " & Id_Tercero & " AND "
        Session("sql") += "Id_Formulario = '" & Id_Formulario & "' AND "
        Session("sql") += "Id_submenu = " & Id_submenu & ""
        Guardar_Datos()
    End Sub

    Private Sub Consultar_Permisos(ByVal Id_Tercero As Long, ByVal Id_Formulario As String, ByVal Id_submenu As Integer)
        Dim Resultado As Integer = 0

        Session("sql") = "SELECT Id_Tercero, Id_Formulario, Id_submenu FROM Sys_Permisos WHERE Id_Tercero = " & Id_Tercero & " AND Id_Formulario = '" & Id_Formulario & "' AND Id_submenu = " & Id_submenu & " "
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Resultado = 1
            dr.Close()
        Else
            Resultado = 0
        End If
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        If Resultado = 1 Then
            Borrar_Permiso(Id_Tercero, Id_Formulario, Id_submenu)
        Else
            Guardar_Permiso(Id_Tercero, Id_Formulario, Id_submenu)
        End If
        LvFormularios.DataBind()
        LvSubmenu.DataBind()
    End Sub

    Private Sub LvFormularios_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvFormularios.ItemDataBound
        Dim Formulario As Button = DirectCast(e.Item.FindControl("Formulario"), Button)
        Dim Id_Formulario As Label = DirectCast(e.Item.FindControl("Id_Formulario"), Label)
        Session("sql") = "SELECT Id_Tercero FROM Sys_Permisos WHERE Id_Tercero = " & Session("Id_Tercero") & " AND Id_Formulario = '" & Id_Formulario.Text & "' AND Id_submenu = 0"
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Formulario.CssClass = "btn bg-primary"
            dr.Close()
        Else
            Formulario.CssClass = "btn bg-danger"
        End If
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub LvSubmenu_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvSubmenu.ItemDataBound
        Dim Submenu As Button = DirectCast(e.Item.FindControl("Submenu"), Button)
        Dim Id_Formulario As Label = DirectCast(e.Item.FindControl("Id_Formulario"), Label)
        Dim Id_Submenu As Label = DirectCast(e.Item.FindControl("Id_Submenu"), Label)

        Session("sql") = "SELECT Id_Tercero FROM Sys_Permisos where Id_Tercero = " & Session("Id_Tercero") & " AND Id_Formulario = '" & Id_Formulario.Text & "' AND Id_submenu = " & Id_Submenu.Text & ""
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Submenu.CssClass = "btn bg-primary"
            dr.Close()
        Else
            Submenu.CssClass = "btn bg-danger"
        End If
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub LvUsuarios_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvUsuarios.ItemDataBound
        Dim Estado As Label = DirectCast(e.Item.FindControl("Estado"), Label)
        Dim LibEditar As LinkButton = DirectCast(e.Item.FindControl("LibEditar"), LinkButton)
        Dim LibPermisos As LinkButton = DirectCast(e.Item.FindControl("LibPermisos"), LinkButton)

        If Estado.Text = "True" Then
            LibEditar.CssClass = "text-primary"
            LibPermisos.CssClass = "text-primary"
        Else
            LibEditar.CssClass = "text-danger"
            LibPermisos.CssClass = "text-danger"
        End If
    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            LvUsuarios.DataSourceID = "SqlUsuarios"
        Else
            LvUsuarios.DataSourceID = "Sql_Buscar_Persona"
        End If
        LvUsuarios.DataBind()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Usuarios.aspx")
    End Sub

End Class