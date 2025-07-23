Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Consultar_Contratistas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then

            Consultar_Info_Usuario()

            If Session("Sucursal_Usuario") Is Nothing Then
                Dim script As String = "swal({
                title: 'OJO!',
                text: 'Debe estar registrado en alguna sucursal',
                type: 'warning',
                allowOutsideClick: false
                }).then(function() {
                    window.location.href = '../../Inicio.aspx';
                });"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            ElseIf Not Consultar_Control_Previsita() Then
                Dim script As String = "swal({
                title: 'OJO!',
                text: 'Debe tener chuleado control visitantes para poder entrar al modulo',
                type: 'warning',
                allowOutsideClick: false
                }).then(function() {
                    window.location.href = '../../Inicio.aspx';
                });"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If

            Session("Id_Tercero_Usuario") = ObtenerIdTercero(User.Identity.Name)
        End If
    End Sub

    Private ErrorOp As String

    Private Sub Correo_Notificacion(ByVal Correo_Notificacion As String)
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Autorizacion ingreso contratista")
            msg.To.Add(New MailAddress(Correo_Notificacion))
            msg.Subject = "Autorizacion ingreso contratista"
            msg.Body = "<table style='width: 100%; padding: 10px; padding-top: 30px; padding-bottom: 30px; background-color: #1E2833; border-radius: 12px;'>
          <tr>
            <td style='border-radius: 12px; box-shadow: 0 10px 20px 0px rgba(0, 0, 0, 0.56), 0 7px 20px -5px rgba(26, 30, 36, 0.40); padding: 12px; width: 70%; background-color: white'>
                <p style='font-size: 11pt; font-family: Trebuchet MS; color: #1F4E79;'>
                   <br />
                   <b>CEDULA:</b> " & Session("Cedula_Contratista").ToString & " 
                   <br />
                   <b>NOMBRE CONTRATISTA:</b> " & Session("Contratista").ToString & " 
                   <br />
                   <b>EMPRESA:</b> " & Session("Empresa").ToString & " 
                   <br />
                   <b>SEDE:</b> " & Session("Sede").ToString & " 
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
            If Session("ARL") <> Nothing Then
                If Request.Url.ToString.Contains("biometrico.auteco") Then
                    msg.Attachments.Add(New Attachment("C:\inetpub\wwwroot\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & Session("ARL").ToString & ""))
                Else
                    msg.Attachments.Add(New Attachment("C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & Session("ARL").ToString & ""))
                End If
            End If

            If Session("Seguridad_Social") <> Nothing Then
                If Request.Url.ToString.Contains("biometrico.auteco") Then
                    msg.Attachments.Add(New Attachment("C:\inetpub\wwwroot\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & Session("Seguridad_Social").ToString & ""))
                Else
                    msg.Attachments.Add(New Attachment("C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & Session("Seguridad_Social").ToString & ""))
                End If
            End If

            If Session("Trabajo_Alturas") <> Nothing Then
                If Request.Url.ToString.Contains("biometrico.auteco") Then
                    msg.Attachments.Add(New Attachment("C:\inetpub\wwwroot\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & Session("Trabajo_Alturas").ToString & ""))
                Else
                    msg.Attachments.Add(New Attachment("C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\Adjunto\Documentos_Contratistas\" & Session("Trabajo_Alturas").ToString & ""))
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

    Private Sub Consultar_Info_Usuario()
        Try
            Dim query As String = "SELECT Id_Tercero, Nombres, Id_Sede, Id_Acceso FROM Terceros WHERE Usuario = @Usuario"

            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Usuario", User.Identity.Name)

                    Using dr As SqlDataReader = command.ExecuteReader()
                        If dr.HasRows Then
                            dr.Read()

                            ' Asignación de Id_Sede verificando si es DBNull
                            If IsDBNull(dr("Id_Sede")) Then
                                Session("Sucursal_Usuario") = Nothing
                            Else
                                Session("Sucursal_Usuario") = dr("Id_Sede").ToString()
                            End If
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function Consultar_Control_Previsita() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Control_Previsita FROM Adm_Empresa"
                Using command As New SqlCommand(query, connection)

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado no es NULL y devolver el valor como Boolean
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return Convert.ToBoolean(result)
                    Else
                        ' Si el resultado es NULL, devolver False
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub TxBuscarCedula_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedula.TextChanged
        If TxBuscarCedula.Text <> "" Then
            LvPrevisitas.DataSourceID = "SqlPrevisitasCedula"
        Else
            LvPrevisitas.DataSourceID = "SqlPrevisitas"
        End If
        LvPrevisitas.DataBind()
    End Sub

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            LvPrevisitas.DataSourceID = "SqlPrevisitasNombre"
        Else
            LvPrevisitas.DataSourceID = "SqlPrevisitas"
        End If
        LvPrevisitas.DataBind()
    End Sub

    Private Sub TxFechaIngreso_TextChanged(sender As Object, e As EventArgs) Handles TxFechaIngreso.TextChanged
        If TxFechaIngreso.Text <> "" Then
            LvPrevisitas.DataSourceID = "SqlPrevisitasFecha"
        Else
            LvPrevisitas.DataSourceID = "SqlPrevisitas"
        End If
        LvPrevisitas.DataBind()
    End Sub

    Private Sub BtContratistas_ServerClick(sender As Object, e As EventArgs) Handles BtContratistas.ServerClick
        Response.Redirect("Consultar_Contratistas.aspx")
    End Sub

    Private Sub BtVisitantes_ServerClick(sender As Object, e As EventArgs) Handles BtVisitantes.ServerClick
        Response.Redirect("Consultar_Previsita.aspx")
    End Sub

    Private Function ObtenerIdTercero(usuario As Long) As Long
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Usuario = @Usuario"
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

    Private Sub Consultar_Responsable(ByVal Id As Long)
        Dim sql As String = "SELECT CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza WHERE CC.Id_Control_Contratistas = @Id_Control_Contratistas"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Control_Contratistas", Id)

                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim id_responsable As String = If(IsDBNull(reader("Id_Quien_Autoriza")), String.Empty, reader("Id_Quien_Autoriza").ToString())
                        Dim responsable As String = If(IsDBNull(reader("Quien_Autoriza")), String.Empty, reader("Quien_Autoriza").ToString())

                        ' Asignar los valores a los controles en la página si no están vacíos o nulos
                        TxReponsable.Items.Clear()
                        TxReponsable.Items.Add(New ListItem(If(String.IsNullOrEmpty(id_responsable), "", If(String.IsNullOrEmpty(responsable), "", responsable))))
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub Consultar_Fecha(ByVal Id As Long)
        Dim sql As String = "SELECT Fecha_Inicio, Fecha_Fin FROM Adm_Control_Contratistas WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Control_Contratistas", Id)

                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim Fecha_Ing As Date = reader("Fecha_Inicio").ToString()
                        TxFechaIngresoEditar.Text = Fecha_Ing.ToString("yyyy-MM-dd")
                        Dim Fecha_Sal As Date = reader("Fecha_Fin").ToString()
                        TxFechaSalidaEditar.Text = Fecha_Sal.ToString("yyyy-MM-dd")
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub Consultar_Dias(ByVal Id As Long)
        Dim sql As String = "SELECT Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo, Festivo FROM Adm_Control_Contratistas WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Control_Contratistas", Id)

                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        TxLunes.Checked = If(reader("Lunes").ToString() = "True", True, False)
                        TxMartes.Checked = If(reader("Martes").ToString() = "True", True, False)
                        TxMiercoles.Checked = If(reader("Miercoles").ToString() = "True", True, False)
                        TxJueves.Checked = If(reader("Jueves").ToString() = "True", True, False)
                        TxViernes.Checked = If(reader("Viernes").ToString() = "True", True, False)
                        TxSabado.Checked = If(reader("Sabado").ToString() = "True", True, False)
                        TxDomingo.Checked = If(reader("Domingo").ToString() = "True", True, False)
                        TxFestivo.Checked = If(reader("Festivo").ToString() = "True", True, False)
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub LvPrevisitas_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvPrevisitas.ItemDataBound
        Dim Id_Control_ContratistasLabel As Label = DirectCast(e.Item.FindControl("Id_Control_ContratistasLabel"), Label)
        Dim Fecha_InicioLabel As Label = DirectCast(e.Item.FindControl("Fecha_InicioLabel"), Label)
        Dim Fecha_FinLabel As Label = DirectCast(e.Item.FindControl("Fecha_FinLabel"), Label)
        Dim EstadoLabel As Label = DirectCast(e.Item.FindControl("EstadoLabel"), Label)
        Dim BtConfirmar As ImageButton = DirectCast(e.Item.FindControl("BtConfirmar"), ImageButton)
        Dim ARL As HtmlAnchor = DirectCast(e.Item.FindControl("ARL"), HtmlAnchor)
        Dim Seguridad_Social As HtmlAnchor = DirectCast(e.Item.FindControl("Seguridad_Social"), HtmlAnchor)
        Dim Trabajo_Alturas As HtmlAnchor = DirectCast(e.Item.FindControl("Trabajo_Alturas"), HtmlAnchor)
        Fecha_InicioLabel.Text = Mid(Fecha_InicioLabel.Text.ToString, 1, 10)
        Fecha_FinLabel.Text = Mid(Fecha_FinLabel.Text.ToString, 1, 10)
        ARL.Attributes("title") = Mid(ARL.Attributes("title"), 1, 9)
        Seguridad_Social.Attributes("title") = Mid(Seguridad_Social.Attributes("title"), 1, 9)
        Trabajo_Alturas.Attributes("title") = Mid(Trabajo_Alturas.Attributes("title"), 1, 9)

        If String.IsNullOrEmpty(ARL.Attributes("title")) Then
            ARL.Visible = False
        End If

        If String.IsNullOrEmpty(Seguridad_Social.Attributes("title")) Then
            Seguridad_Social.Visible = False
        End If

        If String.IsNullOrEmpty(Trabajo_Alturas.Attributes("title")) Then
            Trabajo_Alturas.Visible = False
        End If

        If EstadoLabel.Text = "True" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Verde.png"
            BtConfirmar.Enabled = False
        ElseIf EstadoLabel.Text = "False" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Rojo.png"
            BtConfirmar.Enabled = False
        Else
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Gris.png"
            BtConfirmar.Enabled = True
        End If
    End Sub

    Private Sub LvPrevisitas_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvPrevisitas.ItemCommand
        Dim Id_Control_ContratistasLabel As Label = DirectCast(e.Item.FindControl("Id_Control_ContratistasLabel"), Label)
        Dim Fecha_InicioLabel As Label = DirectCast(e.Item.FindControl("Fecha_InicioLabel"), Label)
        Dim Fecha_FinLabel As Label = DirectCast(e.Item.FindControl("Fecha_FinLabel"), Label)
        Dim Id_HikCamaraLabel As Label = DirectCast(e.Item.FindControl("Id_HikCamaraLabel"), Label)
        Dim Cedula_ContratistaLabel As Label = DirectCast(e.Item.FindControl("Cedula_ContratistaLabel"), Label)
        Dim ContratistaLabel As Label = DirectCast(e.Item.FindControl("ContratistaLabel"), Label)
        Dim EmpresaLabel As Label = DirectCast(e.Item.FindControl("EmpresaLabel"), Label)
        Dim Nombre_SedeLabel As Label = DirectCast(e.Item.FindControl("Nombre_SedeLabel"), Label)
        Dim ARLLabel As Label = DirectCast(e.Item.FindControl("ARLLabel"), Label)
        Dim Seguridad_SocialLabel As Label = DirectCast(e.Item.FindControl("Seguridad_SocialLabel"), Label)
        Dim Trabajo_AlturasLabel As Label = DirectCast(e.Item.FindControl("Trabajo_AlturasLabel"), Label)
        Session("Id_Control_Contratistas") = Id_Control_ContratistasLabel.Text
        Session("Fecha_Inicio") = Fecha_InicioLabel.Text
        Session("Fecha_Fin") = Fecha_FinLabel.Text
        Session("Id_HikCamara") = Id_HikCamaraLabel.Text
        Session("Contratista") = ContratistaLabel.Text
        Session("Cedula_Contratista") = Cedula_ContratistaLabel.Text
        Session("Empresa") = EmpresaLabel.Text
        Session("Sede") = Nombre_SedeLabel.Text
        Session("ARL") = ARLLabel.Text
        Session("Seguridad_Social") = Seguridad_SocialLabel.Text
        Session("Trabajo_Alturas") = Trabajo_AlturasLabel.Text
        Session("RutaImagen") = Server.MapPath("../../Adjunto/Temporal/" + Cedula_ContratistaLabel.Text + "")

        If e.CommandName = "Responsable" Then
            Consultar_Responsable(Id_Control_ContratistasLabel.Text)
        ElseIf e.CommandName = "Fecha" Then
            Consultar_Fecha(Id_Control_ContratistasLabel.Text)
        ElseIf e.CommandName = "Dias" Then
            Consultar_Dias(Id_Control_ContratistasLabel.Text)
        End If
    End Sub

    Private Sub Aceptar_Previsita()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Control_Contratistas SET Estado = 1 WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub Rechazar_Previsita()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Control_Contratistas SET Estado = 0 WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Dim errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso <> 2")

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

    Private Sub Crear_Usuario()

        ' Convertir las fechas de los controles a formato ISO 8601
        Dim fechaInicio As String = DateTime.Parse(Session("Fecha_Inicio").ToString).ToString("yyyy-MM-dd") & "T00:00:00"
        Dim fechaFin As String = DateTime.Parse(Session("Fecha_Fin").ToString).ToString("yyyy-MM-dd") & "T23:59:59"

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""name"": """ & Session("Contratista").ToString.ToUpper() & """,
              ""userType"": ""normal"",
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": """ & fechaInicio & """,
                 ""endTime"": """ & fechaFin & """
              },
              ""doorRight"": ""1"",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al crear usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Agregar_Foto_Persona()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Detectar la imagen válida usando la cédula (del path en Session)
        Dim rutaBaseSinExtension As String = Session("RutaImagen").ToString()
        Dim cedulaArchivo As String = Path.GetFileNameWithoutExtension(rutaBaseSinExtension)
        Dim carpeta As String = Path.GetDirectoryName(rutaBaseSinExtension)
        Dim extensionesValidas As String() = {".jpg", ".jpeg", ".png"}

        Dim rutaFinalImagen As String = Nothing
        For Each ext In extensionesValidas
            Dim posibleRuta As String = Path.Combine(carpeta, cedulaArchivo & ext)
            If File.Exists(posibleRuta) Then
                rutaFinalImagen = posibleRuta
                Exit For
            End If
        Next

        ' Si no se encuentra imagen válida, salir
        If String.IsNullOrEmpty(rutaFinalImagen) Then
            Session("Status_Code_Foto") = 0
            errores.Add("No se encontró imagen válida para la cédula " & cedulaArchivo)
            Exit Sub
        End If

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Parte 1: JSON con datos de la persona
                Dim formData As New StringBuilder()
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
                ""faceLibType"": ""blackFD"",
                ""FDID"": ""1"",
                ""FPID"": """ & Session("Id_HikCamara").ToString() & """
            }")

                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: La imagen
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                      "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                      "Content-Type: image/png" & vbCrLf & vbCrLf,
                                                      boundary,
                                                      Path.GetFileName(rutaFinalImagen))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaFinalImagen)
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Foto") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Foto") = 0
                errores.Add("Se creó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next

        ' Eliminar los archivos temporales
        Try
            If File.Exists(rutaFinalImagen) Then File.Delete(rutaFinalImagen)
        Catch ex As Exception
            errores.Add("Error al eliminar los archivos temporales: " & ex.Message)
        End Try
    End Sub

    Private Sub BtAceptar_Click(sender As Object, e As EventArgs) Handles BtAceptar.Click

        Dim usuarioCreado As Boolean = False
        Dim fotoAgregada As Boolean = False
        Dim mensaje As String = String.Empty
        Dim script As String = String.Empty

        Crear_Usuario()

        ' Verificar si el usuario fue creado
        If Session("Status_Code_Usuario") = "1" Then
            usuarioCreado = True
            mensaje = "Usuario creado correctamente."
        Else
            mensaje = "Error al crear el usuario."
            Dim detallesError As String = String.Join("<br>", errores)
            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Exit Sub
        End If

        ' Si el usuario fue creado, proceder con la foto
        If usuarioCreado Then
            Agregar_Foto_Persona()
            Aceptar_Previsita()
            Correo_Notificacion("contratistasauteco@auteco.com")
            'Correo_Notificacion("juan.aristizabal@miroseguridad.com")

            ' Verificar el estado del proceso de agregar foto desde archivo
            If Session("Status_Code_Foto") = "1" Then

                fotoAgregada = True
                mensaje = "Usuario con foto creado correctamente."
                script = $"swal('Excelente!', '{mensaje}', 'success');"
            Else
                mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                Dim detallesError As String = String.Join("<br>", errores)
                script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
            End If
        End If

        ' Mostrar mensaje al usuario con SweetAlert
        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        Response.AddHeader("Refresh", "3; URL=Consultar_Contratistas.aspx")
    End Sub

    Private Sub BtRechazar_Click(sender As Object, e As EventArgs) Handles BtRechazar.Click
        Rechazar_Previsita()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Contratista Rechazado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Actualizar_Responsable()
        Dim sql As String = "UPDATE dbo.Adm_Control_Contratistas SET Id_Quien_Autoriza = @Id_Quien_Autoriza WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Quien_Autoriza", TxReponsable.SelectedValue)
                command.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtEditarResponsable_Click(sender As Object, e As EventArgs) Handles BtEditarResponsable.Click
        Actualizar_Responsable()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Responsable Reasignado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Actualizar_Fecha()
        Dim sql As String = "UPDATE dbo.Adm_Control_Contratistas SET Fecha_Inicio = @Fecha_Inicio, Fecha_Fin = @Fecha_Fin WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                Dim Fecha_Ing As Date = TxFechaIngresoEditar.Text
                Dim Fecha_Sal As Date = TxFechaSalidaEditar.Text
                command.Parameters.AddWithValue("@Fecha_Inicio", Fecha_Ing.ToString("yyyy-MM-dd"))
                command.Parameters.AddWithValue("@Fecha_Fin", Fecha_Sal.ToString("yyyy-MM-dd"))
                command.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtEditarFecha_Click(sender As Object, e As EventArgs) Handles BtEditarFecha.Click
        Actualizar_Fecha()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Fecha Reagendada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Actualizar_Dias()
        Dim sql As String = "UPDATE dbo.Adm_Control_Contratistas SET Lunes = @Lunes, Martes = @Martes, Miercoles = @Miercoles, Jueves = @Jueves, Viernes = @Viernes, Sabado = @Sabado, Domingo = @Domingo, Festivo = @Festivo WHERE Id_Control_Contratistas = @Id_Control_Contratistas"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                If TxLunes.Checked = True Then
                    command.Parameters.AddWithValue("@Lunes", 1)
                Else
                    command.Parameters.AddWithValue("@Lunes", 0)
                End If
                If TxMartes.Checked = True Then
                    command.Parameters.AddWithValue("@Martes", 1)
                Else
                    command.Parameters.AddWithValue("@Martes", 0)
                End If
                If TxMiercoles.Checked = True Then
                    command.Parameters.AddWithValue("@Miercoles", 1)
                Else
                    command.Parameters.AddWithValue("@Miercoles", 0)
                End If
                If TxJueves.Checked = True Then
                    command.Parameters.AddWithValue("@Jueves", 1)
                Else
                    command.Parameters.AddWithValue("@Jueves", 0)
                End If
                If TxViernes.Checked = True Then
                    command.Parameters.AddWithValue("@Viernes", 1)
                Else
                    command.Parameters.AddWithValue("@Viernes", 0)
                End If
                If TxSabado.Checked = True Then
                    command.Parameters.AddWithValue("@Sabado", 1)
                Else
                    command.Parameters.AddWithValue("@Sabado", 0)
                End If
                If TxDomingo.Checked = True Then
                    command.Parameters.AddWithValue("@Domingo", 1)
                Else
                    command.Parameters.AddWithValue("@Domingo", 0)
                End If
                If TxFestivo.Checked = True Then
                    command.Parameters.AddWithValue("@Festivo", 1)
                Else
                    command.Parameters.AddWithValue("@Festivo", 0)
                End If
                command.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtEditarDias_Click(sender As Object, e As EventArgs) Handles BtEditarDias.Click
        Actualizar_Dias()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Dias Reagendados Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

End Class