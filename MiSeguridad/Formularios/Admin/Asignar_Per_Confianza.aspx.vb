Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Per_Confianza
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

    Private Sub Consultar_Tercero()
        Try
            sql = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Nombres LIKE '%" + TxBuscarPersona.Text + "%' ORDER BY Nombres"
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

    Protected Sub TxBuscarInmueble_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarInmueble.TextChanged
        Dim buscarTexto As String = TxBuscarInmueble.Text.Trim().ToLower()

        ' Clear the DropDownList
        TxInmueble.Items.Clear()

        ' Add the default option
        TxInmueble.Items.Add(New ListItem("Inmueble", ""))

        ' Retrieve the data (this example assumes you have a method to get the data)
        Dim dt As DataTable = GetInmuebles()

        ' Filter and bind the data to the DropDownList
        For Each row As DataRow In dt.Rows
            Dim idInmueble As String = row("Id_inmueble").ToString()
            If idInmueble.ToLower().Contains(buscarTexto) Then
                TxInmueble.Items.Add(New ListItem(idInmueble, idInmueble))
            End If
        Next
    End Sub

    Private Function GetInmuebles() As DataTable
        Dim connStr As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ConnectionString
        Dim query As String = "SELECT Id_inmueble FROM Adm_Inmueble ORDER BY Id_inmueble"
        Dim dt As New DataTable()

        Using conn As New SqlConnection(connStr)
            Using cmd As New SqlCommand(query, conn)
                Using adapter As New SqlDataAdapter(cmd)
                    conn.Open()
                    adapter.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Private Sub Guardar_Per_Confianza()
        Session("sql") = "INSERT INTO Adm_Inmueble_Per_Confianza ("
        Session("sql") += "Id_tercero, "
        Session("sql") += "Id_inmueble, "
        Session("sql") += "Id_Parentesco, "
        Session("sql") += "Estado) VALUES ("

        Session("sql") += "" & TxNombrePersona.SelectedValue & ", "
        Session("sql") += "'" & TxInmueble.SelectedValue & "', "
        Session("sql") += "" & TxParentesco.SelectedValue & ", "
        Session("sql") += "1) "
        Guardar_Datos()
    End Sub

    Private Sub LvPersonalConfianza_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvPersonalConfianza.ItemCommand
        Dim Id_inmuebleLabel As Label = DirectCast(e.Item.FindControl("Id_inmuebleLabel"), Label)
        Session("Id_inmueble") = Id_inmuebleLabel.Text
        Dim Id_terceroLabel As Label = DirectCast(e.Item.FindControl("Id_terceroLabel"), Label)
        Session("Id_tercero") = Id_terceroLabel.Text
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Guardar_Per_Confianza()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Persona de Confianza Guardada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Timer1.Interval = 3000
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If

    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            LvPersonalConfianza.DataSourceID = "SqlPersonalConfianza"
        Else
            LvPersonalConfianza.DataSourceID = "Sql_Buscar_Persona"
        End If
        LvPersonalConfianza.DataBind()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Asignar_Per_Confianza.aspx")
    End Sub

End Class