Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Previsita
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            EstablecerLogo()
        End If
    End Sub

    Private Sub EstablecerLogo()
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener la ruta del logo
        Dim query As String = "SELECT TOP 1 Logo FROM Adm_Empresa"

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener la ruta de la imagen
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing Then
                    Logo.Src = "/../../../Adjunto/Logo/" + result.ToString()
                Else
                    Logo.Src = "/../../../Adjunto/Logo/miro.png"
                End If
            End Using
        End Using
    End Sub

    Dim errores As New List(Of String)

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

    Private Sub Correo_Notificacion(ByVal Correo_Notificacion As String)
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Registro Previsita")
            msg.To.Add(New MailAddress(Correo_Notificacion))
            msg.Subject = "Registro Previsita"
            msg.Body = "<table style='width: 100%; padding: 10px; padding-top: 30px; padding-bottom: 30px; background-color: #7e8893; border-radius: 12px;'>
          <tr>
            <td style='border-radius: 12px; box-shadow: 0 10px 20px 0px rgba(0, 0, 0, 0.56), 0 7px 20px -5px rgba(26, 30, 36, 0.40); padding: 12px; width: 70%; background-color: white'>
                <p style='font-size: 11pt; font-family: Trebuchet MS; color: #1F4E79;'>
                   <b>NOMBRE:</b> " & TxNombres.Text & " 
                   <br />
                   <br />
                   <b>EMPRESA:</b> " & TxEmpresa.Text & "
                   <br />
                   <br />
                   <b>SEDE QUE VISITA:</b> " & TxSede.SelectedItem.Text & "
                   <br />
                   <br />
                   <b>FECHA INGRESO:</b> " & TxFechaIngreso.Text & "
                   <br />
                   <br />
                   <b>INGRESA EQUIPO DE COMPUTO:</b> " & If(CbSi.Checked, "SI", "NO") & "
                   <br />
                   <br />
                   <b>SERIAL DEL EQUIPO DE COMPUTO:</b> " & TxSerialEquipo.Text & "
                   <br />
                   <br />
                   <b>TIPO DE VEHICULO:</b> " & TxTipoVehiculo.SelectedItem.Text & "
                   <br />
                   <br />
                   <b>PLACA:</b> " & TxPlaca.Text & "
                   <br />
                   <br />
                   <a href='https://biometrico.auteco.com.co/Formularios/Admin/Consultar_Previsita.aspx' style='text-align: center; width: 200px; height: 100px'>CONTINUAR PROCESO EN ACCESS POINT</a>
                   <br />
                </p>
            </td>
            <td style='width: 30%; text-align: center; vertical-align: middle'>
            <img alt='' src='https://biometrico.auteco.com.co/Adjunto/Logo/901249413.png' width='150' />
            </td>
        </tr>
      <tr>
</tr>
    </table>"
            msg.IsBodyHtml = True

            Dim smtp = New SmtpClient
            smtp.Host = SmtpHost
            smtp.Credentials = New NetworkCredential(UsuarioCredentials, ContrasenaCredentials)
            smtp.Port = Port
            smtp.EnableSsl = True

            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            smtp.Send(msg)
        Catch ex As Exception
            ErrorOp = "Private Sub Correo_Notificacion: " + Mid(ex.ToString, 1, 900)
            EnviarCorreoError()
        End Try
    End Sub

    Private Sub Consultar_Tercero()
        Try
            sql = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Estado = 1 AND Id_Rol = 8 AND Nombres LIKE '%" + TxBuscarPersona.Text + "%' ORDER BY Nombres"
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
            Dim script As String = String.Format("swal('Error!', 'No se encontró la persona', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            Consultar_Tercero()
        End If
    End Sub

    Private Sub Buscar_Persona(ByVal Usuario As String)
        Dim query As String = "SELECT Id_Tercero, Cedula, Nombres FROM Terceros WHERE Cedula = " & Usuario & ""

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()
                        TxNombres.Text = dr("Nombres").ToString()
                    Else
                        TxNombres.Text = String.Empty
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub TxCedula_TextChanged(sender As Object, e As EventArgs) Handles TxCedula.TextChanged
        If TxCedula.Text <> Nothing Then
            Buscar_Persona(TxCedula.Text.Trim())
        End If
    End Sub

    Private Sub Consultar_Correo_Persona(Id_Tercero As Long)
        Using conexion As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using comando As New SqlCommand("SELECT Correo FROM Terceros WHERE Id_Tercero = @Id_Tercero", conexion)
                comando.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

                Try
                    conexion.Open()
                    Using reader As SqlDataReader = comando.ExecuteReader()
                        If reader.Read() AndAlso Not Convert.IsDBNull(reader("Correo")) Then
                            Session("Correo_Quien_Autoriza") = reader("Correo").ToString()
                        Else
                            Session("Correo_Quien_Autoriza") = ""
                        End If
                    End Using
                Catch ex As Exception
                    Session("Correo_Quien_Autoriza") = ""
                End Try
            End Using
        End Using
    End Sub

    Protected Sub TxTipoVehiculo_TextChanged(sender As Object, e As EventArgs) Handles TxTipoVehiculo.TextChanged
        If TxTipoVehiculo.SelectedValue = "1" Or TxTipoVehiculo.SelectedValue = "2" Then
            Placa.Style("display") = "block"
            TxPlaca.Enabled = True
            TxPlaca.Attributes("required") = "1"
            lblPlacaError.Style("display") = "none"

            ' Si ya hay una placa ingresada, validar automáticamente
            If Not String.IsNullOrEmpty(TxPlaca.Text.Trim()) Then
                TxPlaca_TextChanged(TxPlaca, EventArgs.Empty)
            End If
        Else
            Placa.Style("display") = "none"
            TxPlaca.Text = ""
            TxPlaca.Attributes.Remove("required")
            lblPlacaError.Style("display") = "none"
            BtGuardar.Enabled = True
        End If
    End Sub

    Protected Sub TxPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxPlaca.TextChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue
        Dim placa As String = TxPlaca.Text.Trim().ToUpper() ' Convertir la placa a mayúsculas automáticamente
        TxPlaca.Text = placa ' Asignar el valor convertido al TextBox
        Dim isValid As Boolean = False

        If Not String.IsNullOrEmpty(placa) Then
            If tipoVehiculo = "1" Then ' Carro
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{3}$")
            ElseIf tipoVehiculo = "2" Then ' Moto
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{2}[A-Z]{1}$")
            End If
        Else
            isValid = True ' Permitir que el campo esté vacío
        End If

        If isValid Then
            lblPlacaError.Attributes("style") = "display:none; margin-top: 60px;"
            BtGuardar.Enabled = True
        Else
            lblPlacaError.Text = "Placa inválida. " & GetValidationMessage(tipoVehiculo)
            lblPlacaError.Attributes("style") = "display:block; margin-top: 60px;"
            BtGuardar.Enabled = False
        End If
    End Sub

    Private Function GetValidationMessage(tipoVehiculo As String) As String
        If tipoVehiculo = "1" Then ' Carro
            Return "Debe ser 3 letras y 3 números (ej: ABC123)"
        ElseIf tipoVehiculo = "2" Then ' Moto
            Return "Debe ser 3 letras, 2 números y 1 letra (ej: ABC12D)"
        Else
            Return String.Empty
        End If
    End Function

    Protected Sub CbSi_CheckedChanged(sender As Object, e As EventArgs) Handles CbSi.CheckedChanged
        If CbSi.Checked = True Then
            Serial.Visible = True
        Else
            Serial.Visible = False
        End If
    End Sub

    Protected Sub CbNo_CheckedChanged(sender As Object, e As EventArgs) Handles CbNo.CheckedChanged
        If CbNo.Checked = True Then
            Serial.Visible = False
        Else
            Serial.Visible = True
        End If
    End Sub

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
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Consultar_Id_Previsita()
        Dim Valor As Integer = ObtenerMaxId("Id_Preregistro_Visita", "Adm_Preregistro_Visita")
        Session("Id_Preregistro_Visita") = Valor + 1
    End Sub

    Private Function ObtenerMaxId(campo As String, tabla As String) As Integer
        Dim Valor As Integer = 0
        Dim query As String = $"SELECT MAX({campo}) FROM {tabla}"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Dim resultado = cmd.ExecuteScalar()
                If resultado IsNot Nothing AndAlso Not IsDBNull(resultado) Then
                    Valor = Convert.ToInt32(resultado)
                End If
            End Using
        End Using

        Return Valor
    End Function

    Private Sub Guardar_Persona()
        Try
            Dim query As String = "
        INSERT INTO dbo.Terceros 
        (Id_Tercero, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Cedula", TxCedula.Text.Trim())
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", 6)
                    cmd.Parameters.AddWithValue("@Correo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Celular", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Sede", If(String.IsNullOrWhiteSpace(TxSede.SelectedValue), DBNull.Value, TxSede.SelectedValue))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD en Guardar_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub Guardar_Control_Previsita()

        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Preregistro_Visita 
        (Id_Preregistro_Visita, Cedula, Id_Quien_Autoriza, Empresa, Id_Sede, Fecha_Ingreso, Equipo_Computo, Serial_Computo, Id_Tipo_Vehiculo, Placa, Id_ARL, Id_EPS, Privacidad, Estado, Anulada) 
        VALUES 
        (@Id_Preregistro_Visita, @Cedula, @Id_Quien_Autoriza, @Empresa, @Id_Sede, @Fecha_Ingreso, @Equipo_Computo, @Serial_Computo, @Id_Tipo_Vehiculo, @Placa, @Id_ARL, @Id_EPS, @Privacidad, @Estado, @Anulada)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_Preregistro_Visita", Session("Id_Preregistro_Visita"))
                    cmd.Parameters.AddWithValue("@Cedula", TxCedula.Text.Trim())
                    cmd.Parameters.AddWithValue("@Id_Quien_Autoriza", TxNombrePersona.SelectedValue)
                    cmd.Parameters.AddWithValue("@Empresa", TxEmpresa.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Sede", TxSede.SelectedValue)
                    Dim Fecha_Ing As Date = TxFechaIngreso.Text
                    cmd.Parameters.AddWithValue("@Fecha_Ingreso", Fecha_Ing.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@Equipo_Computo", If(CbSi.Checked, "1", "0"))
                    cmd.Parameters.AddWithValue("@Serial_Computo", If(String.IsNullOrWhiteSpace(TxSerialEquipo.Text), DBNull.Value, TxSerialEquipo.Text))
                    cmd.Parameters.AddWithValue("@Id_Tipo_Vehiculo", If(String.IsNullOrWhiteSpace(TxTipoVehiculo.SelectedValue), DBNull.Value, TxTipoVehiculo.SelectedValue))
                    cmd.Parameters.AddWithValue("@Placa", If(String.IsNullOrWhiteSpace(TxPlaca.Text.ToUpper()), DBNull.Value, TxPlaca.Text.ToUpper()))
                    cmd.Parameters.AddWithValue("@Id_ARL", If(String.IsNullOrWhiteSpace(TxARL.SelectedValue), DBNull.Value, TxARL.SelectedValue))
                    cmd.Parameters.AddWithValue("@Id_EPS", If(String.IsNullOrWhiteSpace(TxEPS.SelectedValue), DBNull.Value, TxEPS.SelectedValue))
                    cmd.Parameters.AddWithValue("@Privacidad", If(RbAcepto.Checked, "1", "0"))
                    cmd.Parameters.AddWithValue("@Estado", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Anulada", 0)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD Guardar_Control_Previsita():" & ex.Message)
        End Try
    End Sub

    Private Function ObtenerIdPrevisitaDeHoy(cedula As String, fechaTexto As String) As Integer
        Dim idPrevisita As Integer = 0
        Try
            Dim fecha As Date
            If Date.TryParse(fechaTexto, fecha) Then
                Dim query As String = "SELECT TOP 1 Id_Preregistro_Visita FROM Adm_Preregistro_Visita WHERE Cedula = @Cedula AND CONVERT(date, Fecha_Ingreso) = @Fecha ORDER BY Id_Preregistro_Visita DESC"

                Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Cedula", cedula)
                        cmd.Parameters.AddWithValue("@Fecha", fecha.Date)

                        conn.Open()
                        Dim result = cmd.ExecuteScalar()
                        If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                            idPrevisita = Convert.ToInt32(result)
                        End If
                    End Using
                End Using
            Else
                errores.Add("La fecha de ingreso no es válida.")
            End If
        Catch ex As Exception
            errores.Add("Error al verificar previsita de la fecha seleccionada: " & ex.Message)
        End Try

        Return idPrevisita
    End Function

    Private Sub Actualizar_Responsable(Id_Quien_Autoriza As String, Id_Preregistro_Visita As String)
        Dim sql As String = "UPDATE Adm_Preregistro_Visita SET Id_Quien_Autoriza = @Id_Quien_Autoriza WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Quien_Autoriza", Id_Quien_Autoriza)
                cmd.Parameters.AddWithValue("@Id_Preregistro_Visita", Id_Preregistro_Visita)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Try
            If Not RegistroExisteEnBD(TxCedula.Text.Trim()) Then
                Consultar_Id_Persona()
                Guardar_Persona()
                Consultar_Id_Previsita()
                Guardar_Control_Previsita()
                Consultar_Correo_Persona(TxNombrePersona.SelectedValue)
                If Not IsNothing(Session("Correo_Quien_Autoriza")) AndAlso Session("Correo_Quien_Autoriza").ToString().Trim() <> "" Then
                    Correo_Notificacion(Session("Correo_Quien_Autoriza").ToString())
                End If

                If errores.Count = 0 Then
                    Dim script As String = "swal('Excelente!', 'Privisita Guardada Correctamente.', 'success');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Response.AddHeader("Refresh", "3; URL=Gracias.aspx")
                Else
                    Dim mensajeError As String = String.Join("<br/>", errores)
                    Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                End If
            Else
                ' Validar si ya hay previsita hoy
                Dim idPrevisitaHoy As Integer = ObtenerIdPrevisitaDeHoy(TxCedula.Text.Trim(), TxFechaIngreso.Text)

                If idPrevisitaHoy > 0 Then
                    ' Ya tiene previsita hoy: actualizar responsable
                    Actualizar_Responsable(TxNombrePersona.SelectedValue, idPrevisitaHoy.ToString())
                Else
                    ' No tiene previsita hoy: registrar nueva previsita
                    Consultar_Id_Previsita()
                    Guardar_Control_Previsita()
                    Consultar_Correo_Persona(TxNombrePersona.SelectedValue)
                    If Not IsNothing(Session("Correo_Quien_Autoriza")) AndAlso Session("Correo_Quien_Autoriza").ToString().Trim() <> "" Then
                        Correo_Notificacion(Session("Correo_Quien_Autoriza").ToString())
                    End If
                End If

                If errores.Count = 0 Then
                    Dim script As String = "swal('Excelente!', 'Privisita Guardada Correctamente.', 'success');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Response.AddHeader("Refresh", "3; URL=Gracias.aspx")
                Else
                    Dim mensajeError As String = String.Join("<br/>", errores)
                    Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                End If
            End If
        Catch ex As Exception
            Dim detallesError As String = String.Join("<br>", errores)
            Dim script As String = $"swal('Error!', 'Verifica el formulario <br>Detalles del error:<br>{detallesError}', 'warning');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End Try
    End Sub

End Class