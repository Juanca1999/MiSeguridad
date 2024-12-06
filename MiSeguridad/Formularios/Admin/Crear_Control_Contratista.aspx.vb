Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Control_Contratista
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
        Dim sql As String = "INSERT INTO dbo.Terceros (Id_Tercero, Cedula, Nombres, Id_Rol, Usuario, Password, Foto, Estado) " &
                        "VALUES (@Id_Tercero, @Cedula, @Nombres, @Id_Rol, @Usuario, @Password, @Foto, @Estado)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))

                If Not String.IsNullOrEmpty(TxCedulaContratista.Text) Then
                    cmd.Parameters.AddWithValue("@Cedula", TxCedulaContratista.Text)
                    cmd.Parameters.AddWithValue("@Usuario", Convert.ToInt64(TxCedulaContratista.Text))
                    cmd.Parameters.AddWithValue("@Password", Convert.ToInt64(TxCedulaContratista.Text))
                Else
                    cmd.Parameters.AddWithValue("@Cedula", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Nombres", TxNombreContratista.Text.ToUpper())
                cmd.Parameters.AddWithValue("@Id_Rol", 5)
                cmd.Parameters.AddWithValue("@Foto", DBNull.Value)
                cmd.Parameters.AddWithValue("@Estado", 1)

                ' Abrir la conexión y ejecutar el comando
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

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
        Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(Convert.ToInt64(User.Identity.Name))
        Session("sql") = "INSERT INTO Adm_Control_Contratistas ("
        Session("sql") += "Id_Control_Contratistas, "
        Session("sql") += "Id_Quien_Registra, "
        Session("sql") += "Id_Quien_Autoriza, "
        Session("sql") += "Id_Inmueble, "
        Session("sql") += "Cedula_Contratista, "
        Session("sql") += "Fecha_Inicio, "
        Session("sql") += "Fecha_Fin, "
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
            Session("sql") += "Fecha_Seguridad_Social, "
        End If
        If TxFechaARL.Text <> "" Then
            Session("sql") += "Fecha_Trabajo_Alturas, "
        End If
        Session("sql") += "Lunes, "
        Session("sql") += "Martes, "
        Session("sql") += "Miercoles, "
        Session("sql") += "Jueves, "
        Session("sql") += "Viernes, "
        Session("sql") += "Sabado, "
        Session("sql") += "Domingo, "
        Session("sql") += "Festivo) VALUES ("

        Session("sql") += "" & Session("Id_Control_Contratistas") & ", "
        Session("sql") += "" & idTercero_Quien_Registra & ", "
        Session("sql") += "" & TxNombrePersona.SelectedValue & ", "
        Session("sql") += "'" & TxInmueble.Text & "', "
        Session("sql") += "" & TxCedulaContratista.Text & ", "
        Dim Fecha_Ini As Date = TxFechaInicio.Text
        Session("sql") += "'" & Fecha_Ini.ToString("yyyy-MM-dd") & "', "
        Dim Fecha_Fin As Date = TxFechaFin.Text
        Session("sql") += "'" & Fecha_Fin.ToString("yyyy-MM-dd") & "', "
        If FLARL.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLARL.FileName)

            FLARL.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "ARL_" + TxNombrePersona.SelectedValue + Ext.ToString))
            Session("sql") += "'" & "ARL_" + TxNombrePersona.SelectedValue + Ext.ToString & "', "
        End If
        If TxFechaARL.Text <> "" Then
            Dim Fecha_ARL As Date = TxFechaARL.Text
            Session("sql") += "'" & Fecha_ARL.ToString("yyyy-MM-dd") & "', "
        End If
        If FLSeguridadSocial.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLSeguridadSocial.FileName)

            FLSeguridadSocial.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Seguridad_Social_" + TxNombrePersona.SelectedValue + Ext.ToString))
            Session("sql") += "'" & "Seguridad_Social_" + TxNombrePersona.SelectedValue + Ext.ToString & "', "
        End If
        If TxFechaSeguridadSocial.Text <> "" Then
            Dim Fecha_Seg As Date = TxFechaSeguridadSocial.Text
            Session("sql") += "'" & Fecha_Seg.ToString("yyyy-MM-dd") & "', "
        End If
        If FLTrabajoAlturas.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLTrabajoAlturas.FileName)

            FLTrabajoAlturas.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Trabajo_Alturas_" + TxNombrePersona.SelectedValue + Ext.ToString))
            Session("sql") += "'" & "Trabajo_Alturas_" + TxNombrePersona.SelectedValue + Ext.ToString & "', "
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
            Session("sql") += "1) "
        Else
            Session("sql") += "0) "
        End If
        Guardar_Datos()
    End Sub

    Private Sub LvControlContratista_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvControlContratista.ItemDataBound
        Dim Fecha_InicioLabel As Label = DirectCast(e.Item.FindControl("Fecha_InicioLabel"), Label)
        Fecha_InicioLabel.Text = Mid(Fecha_InicioLabel.Text.ToString, 1, 10)
        Dim Fecha_FinLabel As Label = DirectCast(e.Item.FindControl("Fecha_FinLabel"), Label)
        Fecha_FinLabel.Text = Mid(Fecha_FinLabel.Text.ToString, 1, 10)
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If Not RegistroExisteEnBD(TxCedulaContratista.Text) Then
            Consultar_Id_Persona()
            Guardar_Persona()
        End If
        Consultar_Id_Control()
        Guardar_Control()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Control Contratista Guardado Correctamente', 'success');")
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
        Response.Redirect("Crear_Control_Contratista.aspx")
    End Sub

End Class