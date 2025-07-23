Imports System.IO
Imports System.Net
Imports ImageMagick
Imports System.Drawing
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Drawing.Imaging
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Control_Contratista_Empresa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session("Usuario") = ObtenerIdTercero(User.Identity.Name)
            BtGuardar.Enabled = False
        End If
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
                    errores.Add("Error al guardar en BD:" & ErrorOp)
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

    Private Sub Correo_Notificacion(ByVal Correo_Notificacion As String)
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Solicitud Ingreso contratista")
            msg.To.Add(New MailAddress(Correo_Notificacion))
            msg.Subject = "Solicitud Ingreso contratista"
            msg.Body = "<table style='width: 100%; padding: 10px; padding-top: 30px; padding-bottom: 30px; background-color: #1E2833; border-radius: 12px;'>
          <tr>
            <td style='border-radius: 12px; box-shadow: 0 10px 20px 0px rgba(0, 0, 0, 0.56), 0 7px 20px -5px rgba(26, 30, 36, 0.40); padding: 12px; width: 70%; background-color: white'>
                <p style='font-size: 11pt; font-family: Trebuchet MS; color: #1F4E79;'>
                   <br />
                   <b>CEDULA:</b> " & TxCedulaContratista.Text & " 
                   <br />
                   <b>NOMBRE CONTRATISTA:</b> " & TxNombreContratista.Text.ToUpper() & " 
                   <br />
                   <b>EMPRESA:</b> " & TxEmpresa.SelectedItem.Text & " 
                   <br />
                   <b>SEDE:</b> " & TxSede.SelectedItem.Text & " 
                   <br />
                   <br />
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
            If FLARL.HasFile = True Then
                Dim Ext As String = System.IO.Path.GetExtension(FLARL.FileName)

                If Request.Url.ToString.Contains("biometrico.auteco") Then
                    msg.Attachments.Add(New Attachment("C:\inetpub\wwwroot\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & "ARL_" + TxCedulaContratista.Text + Ext.ToString & ""))
                Else
                    msg.Attachments.Add(New Attachment("C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & "ARL_" + TxCedulaContratista.Text + Ext.ToString & ""))
                End If
            End If

            If FLSeguridadSocial.HasFile = True Then
                Dim Ext As String = System.IO.Path.GetExtension(FLSeguridadSocial.FileName)

                If Request.Url.ToString.Contains("biometrico.auteco") Then
                    msg.Attachments.Add(New Attachment("C:\inetpub\wwwroot\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" + "Seguridad_Social_" + TxCedulaContratista.Text + Ext.ToString & ""))
                Else
                    msg.Attachments.Add(New Attachment("C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" + "Seguridad_Social_" + TxCedulaContratista.Text + Ext.ToString & ""))
                End If
            End If

            If FLTrabajoAlturas.HasFile = True Then
                Dim Ext As String = System.IO.Path.GetExtension(FLTrabajoAlturas.FileName)

                If Request.Url.ToString.Contains("biometrico.auteco") Then
                    msg.Attachments.Add(New Attachment("C:\inetpub\wwwroot\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" + "Trabajo_Alturas_" + TxCedulaContratista.Text + Ext.ToString & ""))
                Else
                    msg.Attachments.Add(New Attachment("C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" + "Trabajo_Alturas_" + TxCedulaContratista.Text + Ext.ToString & ""))
                End If
            End If

            Dim smtp = New SmtpClient
            smtp.Host = SmtpHost
            smtp.Credentials = New NetworkCredential(UsuarioCredentials, ContrasenaCredentials)
            smtp.Port = Port
            smtp.EnableSsl = True

            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            smtp.Send(msg)
        Catch ex As Exception
            ErrorOp = "Private Sub Correo_Notificacion: " + Mid(ex.ToString, 1, 900)
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

    Private Function Tiene_Facial(Cedula As Long) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado es NULL o no
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Return True ' Id_HikCamara tiene datos
                    Else
                        Return False ' Id_HikCamara es NULL o no existe
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ObtenerId_HikCamara(Cedula As Long) As Long
        Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
        Dim Id_HikCamara As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Cedula", Cedula)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Tercero
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    Id_HikCamara = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Tercero obtenido
        Return Id_HikCamara
    End Function

    Dim errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL")

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
    Private Function EjecutarConsultaSQL(query As String) As DataTable
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

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Consultar_Id_HikCamara()
        Dim Valor As Integer = ObtenerMaxId("Id_HikCamara", "Terceros")
        Session("Id_HikCamara") = Valor + 1
    End Sub

    Private Sub Consultar_Id_Empresa()
        Dim Valor As Integer = ObtenerMaxId("Id_Empresa", "Empresas")
        Session("Id_Empresa") = Valor + 1
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
        (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Empresa, Id_Sede,  Estado) 
        VALUES 
        (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Empresa, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Id_HikCamara", If(String.IsNullOrWhiteSpace(Session("Id_HikCamara")), DBNull.Value, Session("Id_HikCamara")))
                    cmd.Parameters.AddWithValue("@Cedula", If(String.IsNullOrWhiteSpace(TxCedulaContratista.Text.Trim()), DBNull.Value, TxCedulaContratista.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Nombres", TxNombreContratista.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", 5)
                    cmd.Parameters.AddWithValue("@Correo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(TxCargo.Text), DBNull.Value, TxCargo.Text.ToUpper()))
                    cmd.Parameters.AddWithValue("@Celular", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Empresa", If(IsNumeric(TxEmpresa.SelectedValue), Convert.ToInt32(TxEmpresa.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(IsNumeric(TxSede.SelectedValue), Convert.ToInt32(TxSede.SelectedValue), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Foto_Persona_Archivo()

        ' Validar extensión permitida
        Dim extensionesPermitidas As String() = {".jpg", ".jpeg", ".png"}
        If Not extensionesPermitidas.Contains(Path.GetExtension(FlFoto.FileName).ToLower()) Then
            errores.Add("Formato de archivo no permitido. Solo se aceptan imágenes en formato JPG o PNG.")
            Exit Sub
        End If

        ' Rutas temporales
        Dim rutaEntrada As String = Path.Combine(Server.MapPath("../../Adjunto/Temporal/"), "Primera_" & TxCedulaContratista.Text.Trim() & Path.GetExtension(FlFoto.FileName))
        Dim rutaReducida As String = Path.Combine(Server.MapPath("../../Adjunto/Temporal/"), TxCedulaContratista.Text.Trim() & Path.GetExtension(FlFoto.FileName))

        ' Guardar el archivo original en la ruta temporal
        Try
            FlFoto.SaveAs(rutaEntrada)

            ' Reducir el peso del archivo
            ReducirPesoImagen(rutaEntrada, rutaReducida)
        Catch ex As Exception
            errores.Add("Error al guardar o reducir la imagen: " & ex.Message)
            Exit Sub
        End Try

        ' Eliminar los archivos temporales
        Try
            If File.Exists(rutaEntrada) Then File.Delete(rutaEntrada)
        Catch ex As Exception
            errores.Add("Error al eliminar los archivos temporales: " & ex.Message)
        End Try
    End Sub

    Private Sub GuardarImagen()
        Try
            ' Obtener la cadena Base64
            Dim base64Foto As String = base64image.Value

            ' Validar si la imagen corresponde al valor por defecto
            If String.IsNullOrEmpty(base64Foto) Then
                Throw New Exception("No se puede guardar la imagen predeterminada. Seleccione una imagen válida.")
            End If

            ' Definir la ruta de la carpeta
            Dim rutaCarpeta As String = Server.MapPath("../../Adjunto/Temporal/")

            ' Nombre único para el archivo
            Dim nombreArchivo As String = TxCedulaContratista.Text.Trim() & ".png"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, nombreArchivo)

            ' Verificar si el archivo ya existe
            If File.Exists(rutaArchivo) Then
                ' Eliminar el archivo existente antes de guardar el nuevo
                File.Delete(rutaArchivo)
            End If

            ' Convertir Base64 a archivo y guardar temporalmente
            Dim imageBytes As Byte() = Convert.FromBase64String(base64Foto.Replace("data:image/png;base64,", ""))
            Dim rutaArchivoTemp As String = Path.Combine(rutaCarpeta, "temp_" & nombreArchivo)
            File.WriteAllBytes(rutaArchivoTemp, imageBytes)

            ' Reducir el peso de la imagen y guardar la versión optimizada
            ReducirPesoImagenTomada(rutaArchivoTemp, rutaArchivo)

            ' Eliminar la imagen temporal
            If File.Exists(rutaArchivoTemp) Then
                File.Delete(rutaArchivoTemp)
            End If

        Catch ex As Exception
            errores.Add("Error en GuardarImagen()" & ex.Message)
        End Try
    End Sub

    Public Sub ReducirPesoImagen(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            ' Cargar la imagen con MagickImage
            Using imagen As New MagickImage(rutaEntrada)
                Dim maxAncho As UInteger = 450
                Dim maxAlto As UInteger = 450

                ' Redimensionar la imagen manteniendo la proporción (Fill, Fit, o ajusta según tu necesidad)
                imagen.Resize(New MagickGeometry(maxAncho, maxAlto) With {.IgnoreAspectRatio = False})

                ' Ajustar la calidad de la compresión
                imagen.Quality = 70 ' 70% de calidad

                ' Guardar la imagen comprimida
                imagen.Write(rutaSalida)
            End Using
        Catch ex As Exception
            errores.Add("Error en ReducirPesoImagen()" & ex.Message)
        End Try
    End Sub

    Private Sub ReducirPesoImagenTomada(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            Using imgOriginal As Bitmap = New Bitmap(rutaEntrada)
                ' Definir la calidad de compresión
                Dim codecInfo As ImageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg)
                Dim qualityParam As New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L) ' 80% de calidad
                Dim encoderParams As New EncoderParameters(1)
                encoderParams.Param(0) = qualityParam

                ' Guardar la imagen con el nivel de compresión especificado
                imgOriginal.Save(rutaSalida, codecInfo, encoderParams)
            End Using
        Catch ex As Exception
            errores.Add("Error en ReducirPesoImagenTomada()" & ex.Message)
        End Try
    End Sub

    Private Function GetEncoderInfo(ByVal format As ImageFormat) As ImageCodecInfo
        For Each codec As ImageCodecInfo In ImageCodecInfo.GetImageEncoders()
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

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
            Dim script As String = String.Format("swal('Error!', 'No se encontró la persona', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            Consultar_Tercero()
        Else
            TxNombrePersona.Items.Clear()
        End If
    End Sub

    Private Sub Buscar_Empresa()
        Try
            sql = "SELECT Id_Empresa, Empresa FROM Empresas WHERE Tipo_Empresa = 0 AND Empresa LIKE '%" + TxBuscarEmpresa.Text + "%' ORDER BY Empresa"
            Using cmd As New SqlCommand(sql, conn)
                cmd.CommandType = CommandType.Text
                Using mda As New SqlDataAdapter(cmd)
                    Using datos As New DataTable
                        mda.Fill(datos)
                        TxEmpresa.DataSource = datos
                        TxEmpresa.DataTextField = "Empresa"
                        TxEmpresa.DataValueField = "Id_Empresa"
                        TxEmpresa.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('Error!', 'No se encontró la empresa', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarEmpresa_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarEmpresa.TextChanged
        If TxBuscarEmpresa.Text <> "" Then
            Buscar_Empresa()
        Else
            TxEmpresa.Items.Clear()
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
        Session("sql") += "Cedula_Contratista, "
        Session("sql") += "Fecha_Inicio, "
        Session("sql") += "Fecha_Fin, "
        Session("sql") += "Tratamiento_Datos, "
        If TxObservacion.Text <> "" Then
            Session("sql") += "Observaciones, "
        End If
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
            Session("sql") += "Trabajo_Alturas, "
        End If
        If TxFechaTrabajoAlturas.Text <> "" Then
            Session("sql") += "Fecha_Trabajo_Alturas, "
        End If
        Session("sql") += "Lunes, "
        Session("sql") += "Martes, "
        Session("sql") += "Miercoles, "
        Session("sql") += "Jueves, "
        Session("sql") += "Viernes, "
        Session("sql") += "Sabado, "
        Session("sql") += "Domingo, "
        Session("sql") += "Festivo, "
        Session("sql") += "Estado) VALUES ("

        Session("sql") += "" & Session("Id_Control_Contratistas") & ", "
        Session("sql") += "" & idTercero_Quien_Registra & ", "
        Session("sql") += "" & TxNombrePersona.SelectedValue & ", "
        Session("sql") += "" & TxCedulaContratista.Text & ", "
        Dim Fecha_Ini As Date = TxFechaInicio.Text
        Session("sql") += "'" & Fecha_Ini.ToString("yyyy-MM-dd") & "', "
        Dim Fecha_Fin As Date = TxFechaFin.Text
        Session("sql") += "'" & Fecha_Fin.ToString("yyyy-MM-dd") & "', "
        Session("sql") += "1, "
        If TxObservacion.Text <> "" Then
            Session("sql") += "'" & TxObservacion.Text & "', "
        End If
        If FLARL.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLARL.FileName)

            FLARL.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "ARL_" + TxCedulaContratista.Text + Ext.ToString))
            Session("sql") += "'" & "ARL_" + TxCedulaContratista.Text + Ext.ToString & "', "
        End If
        If TxFechaARL.Text <> "" Then
            Dim Fecha_ARL As Date = TxFechaARL.Text
            Session("sql") += "'" & Fecha_ARL.ToString("yyyy-MM-dd") & "', "
        End If
        If FLSeguridadSocial.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLSeguridadSocial.FileName)

            FLSeguridadSocial.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Seguridad_Social_" + TxCedulaContratista.Text + Ext.ToString))
            Session("sql") += "'" & "Seguridad_Social_" + TxCedulaContratista.Text + Ext.ToString & "', "
        End If
        If TxFechaSeguridadSocial.Text <> "" Then
            Dim Fecha_Seg As Date = TxFechaSeguridadSocial.Text
            Session("sql") += "'" & Fecha_Seg.ToString("yyyy-MM-dd") & "', "
        End If
        If FLTrabajoAlturas.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLTrabajoAlturas.FileName)

            FLTrabajoAlturas.SaveAs(Server.MapPath("../../Adjunto/Documentos_Contratistas/" + "Trabajo_Alturas_" + TxCedulaContratista.Text + Ext.ToString))
            Session("sql") += "'" & "Trabajo_Alturas_" + TxCedulaContratista.Text + Ext.ToString & "', "
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
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        Session("sql") += " NULL) "
        Guardar_Datos()
    End Sub

    Private Sub TxCedulaContratista_TextChanged(sender As Object, e As EventArgs) Handles TxCedulaContratista.TextChanged
        If TxCedulaContratista.Text <> "" Then
            Consultar_Usuario(TxCedulaContratista.Text.Trim())
        End If
    End Sub

    Private Sub Consultar_Usuario(ByVal Cedula As Long)
        Dim query As String = "SELECT TOP 1 CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Id_Empresa, E.Empresa, T.Id_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, CC.Fecha_Inicio, CC.Fecha_Fin, CC.Observaciones, CC.Fecha_ARL, CC.Fecha_Seguridad_Social, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo FROM Adm_Control_Contratistas CC " &
                              "LEFT JOIN Terceros T on T.Cedula = CC.Cedula_Contratista " &
                              "LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza " &
                              "LEFT JOIN Empresas E on E.Id_Empresa = T.Id_Empresa " &
                              "WHERE T.Cedula = @Cedula ORDER BY CC.Id_Control_Contratistas DESC"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Cedula", Cedula)

                conn.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        TxBuscarPersona.Text = dr("Quien_Autoriza").ToString()
                        TxNombrePersona.Items.Clear()
                        TxNombrePersona.Items.Add(New ListItem(dr("Quien_Autoriza").ToString, dr("Id_Quien_Autoriza").ToString))
                        TxCedulaContratista.Text = dr("Cedula_Contratista").ToString()
                        TxNombreContratista.Text = dr("Contratista").ToString()
                        TxBuscarEmpresa.Text = dr("Quien_Autoriza").ToString()
                        TxEmpresa.Items.Clear()
                        TxEmpresa.Items.Add(New ListItem(dr("Empresa").ToString, dr("Id_Empresa").ToString))
                        If dr("Id_Sede").ToString() <> "" Then
                            TxSede.SelectedValue = dr("Id_Sede").ToString()
                        End If
                        Dim Fecha_Ini As Date = dr("Fecha_Inicio").ToString()
                        TxFechaInicio.Text = Fecha_Ini.ToString("yyyy-MM-dd")
                        Dim Fecha_Fin As Date = dr("Fecha_Fin").ToString()
                        TxFechaFin.Text = Fecha_Fin.ToString("yyyy-MM-dd")
                        If dr("Observaciones").ToString() <> "" Then
                            TxObservacion.Text = dr("Observaciones").ToString()
                        End If
                        If dr("Fecha_ARL").ToString() <> "" Then
                            Dim Fecha_ARL As Date = dr("Fecha_ARL").ToString()
                            TxFechaARL.Text = Fecha_ARL.ToString("yyyy-MM-dd")
                        End If
                        If dr("Fecha_Seguridad_Social").ToString() <> "" Then
                            Dim Fecha_Seguridad As Date = dr("Fecha_Seguridad_Social").ToString()
                            TxFechaSeguridadSocial.Text = Fecha_Seguridad.ToString("yyyy-MM-dd")
                        End If
                        If dr("Fecha_Trabajo_Alturas").ToString() <> "" Then
                            Dim Fecha_Trab As Date = dr("Fecha_Trabajo_Alturas").ToString()
                            TxFechaTrabajoAlturas.Text = Fecha_Trab.ToString("yyyy-MM-dd")
                        End If
                        TxLunes.Checked = If(dr("Lunes").ToString() = "True", True, False)
                        TxMartes.Checked = If(dr("Martes").ToString() = "True", True, False)
                        TxMiercoles.Checked = If(dr("Miercoles").ToString() = "True", True, False)
                        TxJueves.Checked = If(dr("Jueves").ToString() = "True", True, False)
                        TxViernes.Checked = If(dr("Viernes").ToString() = "True", True, False)
                        TxSabado.Checked = If(dr("Sabado").ToString() = "True", True, False)
                        TxDomingo.Checked = If(dr("Domingo").ToString() = "True", True, False)
                    Else
                        TxBuscarPersona.Text = String.Empty
                        TxNombrePersona.Items.Clear()
                        TxNombreContratista.Text = String.Empty
                        TxBuscarEmpresa.Text = String.Empty
                        TxEmpresa.Items.Clear()
                        TxFechaInicio.Text = String.Empty
                        TxFechaFin.Text = String.Empty
                        TxObservacion.Text = String.Empty
                        TxLunes.Checked = False
                        TxMartes.Checked = False
                        TxMiercoles.Checked = False
                        TxJueves.Checked = False
                        TxViernes.Checked = False
                        TxSabado.Checked = False
                        TxDomingo.Checked = False
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Function Consultar_Correo(idTercero As Integer) As String
        Dim correo As String = Nothing
        Dim query As String = "SELECT Correo FROM Terceros WHERE Id_Tercero = @IdTercero"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IdTercero", idTercero)
                connection.Open()
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not Convert.IsDBNull(result) Then
                    correo = result.ToString()
                End If
            End Using
        End Using

        Return correo
    End Function

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Try
            ' Validación: debe haber al menos una imagen
            If Not FlFoto.HasFile AndAlso String.IsNullOrWhiteSpace(base64image.Value) Then
                Dim script As String = "swal('Advertencia!', 'Debe seleccionar una imagen o tomar una foto.', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "imagenRequerida", script, True)
                Exit Sub
            End If

            If Not RegistroExisteEnBD(TxCedulaContratista.Text.Trim()) Then
                Consultar_Id_Persona()
                Consultar_Id_HikCamara()
                Guardar_Persona()
                Consultar_Id_Control()
                Guardar_Control()
            Else
                Consultar_Id_Control()
                Guardar_Control()
            End If

            Dim correoDestino As String = Consultar_Correo(Convert.ToInt32(TxNombrePersona.SelectedValue))

            If Not String.IsNullOrEmpty(correoDestino) Then
                Correo_Notificacion(correoDestino)
            End If

            ' Guardar imagen
            If FlFoto.HasFile Then
                Actualizar_Foto_Persona_Archivo()
            ElseIf Not String.IsNullOrWhiteSpace(base64image.Value) Then
                GuardarImagen()
            End If

            ' Mostrar resultado
            If errores.Count = 0 Then
                Dim script As String = "swal('Excelente!', 'Contratista Guardado Correctamente.', 'success');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista_Empresa.aspx")
            Else
                Dim mensajeError As String = String.Join("<br/>", errores)
                Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If

        Catch ex As Exception
            Dim detallesError As String = String.Join("<br>", errores)
            Dim script As String = $"swal('Error!', 'Verifica el formulario <br>Detalles del error:<br>{detallesError}', 'warning');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End Try
    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = "" Then
            LvControlContratista.DataSourceID = "SqlControlContratista"
        Else
            LvControlContratista.DataSourceID = "SqlBuscarControlContratista"
        End If
        LvControlContratista.DataBind()
    End Sub

End Class