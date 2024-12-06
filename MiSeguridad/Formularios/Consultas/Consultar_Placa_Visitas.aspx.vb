Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Consultar_Placa_Visitas
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

    Private Sub BtBuscarPlaca_Click(sender As Object, e As EventArgs) Handles BtBuscarPlaca.Click
        If TxBuscarPlaca.Text <> "" Then
            Dim placaBuscado As String = TxBuscarPlaca.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Placa = @Placa ORDER BY V.Fecha_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Placa", placaBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Placa no encontrada', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub TxBuscarPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPlaca.TextChanged
        If TxBuscarPlaca.Text <> "" Then
            Dim placaBuscado As String = TxBuscarPlaca.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Placa = @Placa ORDER BY V.Fecha_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Placa", placaBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Placa no encontrada', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub LvVisitantes_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvVisitantes.ItemDataBound
        Dim Fecha_Inicio_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Inicio_VisitaLabel"), Label)
        Fecha_Inicio_VisitaLabel.Text = Mid(Fecha_Inicio_VisitaLabel.Text.ToString, 1, 10)
        Dim Fecha_Fin_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Fin_VisitaLabel"), Label)
        Fecha_Fin_VisitaLabel.Text = Mid(Fecha_Fin_VisitaLabel.Text.ToString, 1, 10)
    End Sub

    Protected Sub BtExportarExcel_Click(sender As Object, e As EventArgs) Handles BtExportarExcel.Click
        ' Encuentra el DataPager
        Dim pager As DataPager = TryCast(LvVisitantes.FindControl("DataPager1"), DataPager)

        ' Guarda el estado original de la paginación
        Dim originalPageSize = pager.PageSize
        Dim originalStartRowIndex = pager.StartRowIndex

        ' Desactiva la paginación
        pager.SetPageProperties(0, Integer.MaxValue, False)

        ' Actualiza el ListView
        LvVisitantes.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=Visitas_Por_Placa.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            ' Ahora LvFacturacionGeneral incluirá todos los datos
            LvVisitantes.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End Using

        ' Restaura el estado original de la paginación
        pager.SetPageProperties(originalStartRowIndex, originalPageSize, False)

        ' Actualiza el ListView
        LvVisitantes.DataBind()
    End Sub

End Class