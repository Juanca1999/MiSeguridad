Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Inmueble
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

    Private Sub LbAdjunto_Click(sender As Object, e As EventArgs) Handles LbAdjunto.Click
        If FlAdjunto.HasFile Then
            Dim Ext As String = System.IO.Path.GetExtension(FlAdjunto.FileName)
            Session("Id_Adjunto") = Date.Now.Day.ToString + Date.Now.Month.ToString + Date.Now.Year.ToString + Date.Now.Hour.ToString + Date.Now.Minute.ToString + Date.Now.Second.ToString
            FlAdjunto.SaveAs(Server.MapPath("..\..\Adjunto\Adjunto_Inmueble\" & Session("Id_Adjunto").ToString & Ext))
            Guardar_Excel()

            If ErrorOp = Nothing Then
                Dim script As String = String.Format("swal('Excelente!', 'Inmueble Importado Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 2000
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                EnviarCorreoError()
            End If
        Else
            Dim script As String = String.Format("swal('Excel Necesario!', 'No se encuentran datos para importar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If
    End Sub

    Private Sub Guardar_Excel()

        Dim Ext As String = System.IO.Path.GetExtension(FlAdjunto.FileName)
        Dim filePath As String = Server.MapPath("..\..\Adjunto\Adjunto_Inmueble\" & Session("Id_Adjunto").ToString & Ext)

        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial

        Using package As New OfficeOpenXml.ExcelPackage(New FileInfo(filePath))
            Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets("Hoja1")

            Dim startRow As Integer = worksheet.Dimension.Start.Row
            Dim endRow As Integer = worksheet.Dimension.End.Row

            For row As Integer = startRow + 1 To endRow ' Start from 2nd row assuming the first row is header
                Session("sql") = "INSERT INTO dbo.Adm_Inmueble (Id_inmueble, Id_Tipo_Inmueble, Telefono, Id_Sede) VALUES ("
                Session("sql") += "'" & worksheet.Cells(row, 1).Text & "', "
                Session("sql") += "'" & worksheet.Cells(row, 2).Text & "', "
                Session("sql") += "'" & worksheet.Cells(row, 3).Text & "', "
                Session("sql") += "'" & worksheet.Cells(row, 4).Text & "') "
                Guardar_Datos()
            Next
        End Using
    End Sub

    Private Sub Guardar_Inmueble()
        Session("sql") = "INSERT INTO Adm_Inmueble ("
        Session("sql") += "Id_inmueble, "
        Session("sql") += "Id_Tipo_Inmueble, "
        Session("sql") += "Id_Sede, "
        Session("sql") += "Telefono) VALUES ("

        Session("sql") += "'" & TxIdInmueble.Text & "', "
        Session("sql") += "" & TxTipoInmueble.SelectedValue & ", "
        Session("sql") += "" & TxSede.SelectedValue & ", "
        Session("sql") += "" & TxTelefono.Text & ") "
        Guardar_Datos()
    End Sub

    Private Sub Actualizar_Inmueble()
        Session("sql") = "UPDATE dbo.Adm_Inmueble SET "
        Session("sql") += "Id_inmueble = '" & TxIdInmueble.Text & "', "
        Session("sql") += "Id_Tipo_Inmueble = " & TxTipoInmueble.SelectedValue & ", "
        Session("sql") += "Id_Sede = " & TxSede.SelectedValue & ", "
        Session("sql") += "Telefono = " & TxTelefono.Text & " "
        Session("sql") += "WHERE Id_inmueble = '" & Session("Id_inmueble") & "' "
        Guardar_Datos()
    End Sub

    Private Sub Eliminar_Inmueble(ByVal ID As String)
        Session("sql") = "DELETE FROM dbo.Adm_Inmueble WHERE Id_inmueble = '" & ID & "' "
        Guardar_Datos()
    End Sub

    Private Sub Consultar_Inmueble(ByVal ID As String)
        Session("sql") = "SELECT Id_inmueble, Id_Tipo_Inmueble, Telefono, Id_Sede FROM Adm_Inmueble "
        Session("sql") += "WHERE Id_inmueble = '" & ID & "' "

        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            TxIdInmueble.Text = dr("Id_inmueble").ToString
            TxTipoInmueble.SelectedValue = dr("Id_Tipo_Inmueble").ToString
            TxSede.SelectedValue = dr("Id_Sede").ToString
            TxTelefono.Text = dr("Telefono").ToString
            BtGuardar.Text = "ACTUALIZAR"
            dr.Close()
        End If

        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub Lvinmuebles_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles Lvinmuebles.ItemCommand
        Dim Id_inmuebleLabel As Label = DirectCast(e.Item.FindControl("Id_inmuebleLabel"), Label)
        Session("Id_inmueble") = Id_inmuebleLabel.Text
        If e.CommandName = "Editar" Then
            Consultar_Inmueble(Id_inmuebleLabel.Text)
        ElseIf e.CommandName = "Eliminar" Then
            Eliminar_Inmueble(Id_inmuebleLabel.Text)
            Lvinmuebles.DataBind()
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then
            Guardar_Inmueble()
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            Actualizar_Inmueble()
        End If

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Inmueble Guardado Correctamente', 'success');")
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
            Lvinmuebles.DataSourceID = "SqlInmuebles"
        Else
            Lvinmuebles.DataSourceID = "Sql_Buscar_Inmueble"
        End If
        Lvinmuebles.DataBind()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Inmueble.aspx")
    End Sub

End Class