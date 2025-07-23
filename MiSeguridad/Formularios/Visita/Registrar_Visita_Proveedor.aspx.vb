Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Registrar_Visita_Proveedor
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
            ElseIf Session("Acceso_Usuario") Is Nothing Then
                Dim script As String = "swal({
                title: 'OJO!',
                text: 'Debe estar registrado en algún acceso',
                type: 'warning',
                allowOutsideClick: false
                }).then(function() {
                    window.location.href = '../../Inicio.aspx';
                });"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If

        End If
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
            msg.From = New MailAddress(CorreoFrom, "Crear Empresa Auteco")
            msg.To.Add(New MailAddress(Correo_Notificacion))
            msg.Subject = "Crear Empresa Auteco"
            msg.Body = "<table style='width: 100%; padding: 10px; padding-top: 30px; padding-bottom: 30px; background-color: #1E2833; border-radius: 12px;'>
          <tr>
            <td style='border-radius: 12px; box-shadow: 0 10px 20px 0px rgba(0, 0, 0, 0.56), 0 7px 20px -5px rgba(26, 30, 36, 0.40); padding: 12px; width: 70%; background-color: white'>
                <p style='font-size: 11pt; font-family: Trebuchet MS; color: #1F4E79;'>
                   <br />
                   <b>OBSERVACION:</b> " & TxObservacion.Text & " 
                   <br />
                   <br />
                   <br />
                   <b>QUIEN GENERÓ LA VISITA:</b> " & Session("Nombres").ToString & " 
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

                            ' Asignación de Id_Acceso verificando si es DBNull
                            If IsDBNull(dr("Id_Acceso")) Then
                                Session("Acceso_Usuario") = Nothing
                            Else
                                Session("Acceso_Usuario") = dr("Id_Acceso").ToString()
                            End If

                            If IsDBNull(dr("Nombres")) Then
                                Session("Nombres") = Nothing
                            Else
                                Session("Nombres") = dr("Nombres").ToString()
                            End If
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                            Session("Acceso_Usuario") = Nothing
                            Session("Nombres") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BtEntrada_ServerClick(sender As Object, e As EventArgs) Handles BtEntrada.ServerClick
        Response.Redirect("Registrar_Visita.aspx")
    End Sub

    Private Sub BtSalida_ServerClick(sender As Object, e As EventArgs) Handles BtSalida.ServerClick
        Response.Redirect("Registrar_Salida.aspx")
    End Sub

    Private Sub BtEntradaContratista_ServerClick(sender As Object, e As EventArgs) Handles BtEntradaContratista.ServerClick
        Response.Redirect("Registrar_Visita_Contratista.aspx")
    End Sub

    Private Sub BtSalidaContratista_ServerClick(sender As Object, e As EventArgs) Handles BtSalidaContratista.ServerClick
        Response.Redirect("Registrar_Salida_Contratista.aspx")
    End Sub

    Private Sub BtVisitaProveedor_ServerClick(sender As Object, e As EventArgs) Handles BtVisitaProveedor.ServerClick
        Response.Redirect("Registrar_Visita_Proveedor.aspx")
    End Sub

    Public Shared Function ObtenerEndPoint() As String
        Dim IP_EndPoint As String = String.Empty

        Dim sql As String = "SELECT TOP 1 IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Sede = @Id_Sede ORDER BY Id_Acceso ASC"

        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Sede", HttpContext.Current.Session("Sucursal_Usuario"))
                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            IP_EndPoint = reader("IP_Camara").ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try

        Return IP_EndPoint
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
        End If
    End Sub

    Public Shared Function Consultar_Foto(ByVal Id As String) As String
        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/Intelligent/FDLib/FDSearch?format=json"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json"
            request.Credentials = New NetworkCredential("admin", "Miro-321")
            request.PreAuthenticate = False

            ' JSON que se enviará en el cuerpo
            Dim jsonBody As String = "
        {
            ""searchResultPosition"": 0,
            ""maxResults"": 100,
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Id & """
        }"

            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(jsonBody)
            End Using

            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Dim responseText As String = String.Empty
            Using reader As New StreamReader(response.GetResponseStream())
                responseText = reader.ReadToEnd()
            End Using

            Dim jsonResponse As JObject = JObject.Parse(responseText)

            If jsonResponse("statusCode").ToString() = "1" AndAlso jsonResponse("numOfMatches").ToString() = "1" Then
                ' Obtener la URL de la imagen desde el JSON
                Dim faceUrl As String = jsonResponse("MatchList")(0)("faceURL").ToString()

                ' Definir el nombre y la ruta del archivo
                Dim fileName As String = "face_" & Id & ".jpg"
                Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)

                ' Verificar si el archivo ya existe
                If System.IO.File.Exists(serverPath) Then
                    ' Eliminar el archivo existente antes de descargar el nuevo
                    System.IO.File.Delete(serverPath)
                End If

                ' Descargar la nueva imagen
                Dim webClient As New WebClient()
                webClient.Credentials = New NetworkCredential("admin", "Miro-321")

                ' Descargar y guardar la imagen en el servidor
                webClient.DownloadFile(faceUrl, serverPath)

                ' Retornar la URL local del servidor para el frontend
                Return "../../Adjunto/Foto_Usuarios/" & fileName
            Else
                ' Retornar mensaje de error
                Return "Error: Foto no encontrada."
            End If
        Catch ex As Exception
            ' Retornar mensaje de error
            Return "Error: " & ex.Message
        End Try
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimos10Logs() As List(Of Log)
        Dim logs As New List(Of Log)()
        Dim query As String = "
        SELECT TOP 10 L.Id_Log, L.Id_Tercero, T.Nombres, L.Fecha_Hora_Evento, 
               CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso, T.Foto, T.Id_HikCamara
        FROM Logs L
        LEFT JOIN Terceros T ON T.Id_Tercero = L.Id_Tercero
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso
        ORDER BY L.Id_Log DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim log As New Log()
                        log.Id_Log = reader("Id_Log").ToString()
                        log.Id_Tercero = reader("Id_Tercero").ToString()
                        log.Nombres = reader("Nombres").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.Id_HikCamara = reader("Id_HikCamara").ToString()
                        Dim fileName As String = "face_" & log.Id_HikCamara & ".jpg"
                        Dim fileName2 As String = "visitor_" & log.Id_Tercero & ".jpg"
                        Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)
                        Dim serverPath2 As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName2)
                        If System.IO.File.Exists(serverPath) Then
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName
                        ElseIf Not IsDBNull(reader("Foto")) Then
                            Dim base64String As String = reader("Foto").ToString()
                            If base64String.StartsWith("data:image/") Then
                                base64String = base64String.Split(","c)(1) ' Eliminar el prefijo
                            End If
                            Dim fotoBytes As Byte() = Convert.FromBase64String(base64String)
                            System.IO.File.WriteAllBytes(serverPath2, fotoBytes)
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName2
                        Else
                            If log.Id_HikCamara <> Nothing Then
                                log.FotoUrl = Consultar_Foto(log.Id_HikCamara)
                            Else
                                log.FotoUrl = "../../../Diseno/Imagenes/Blanco.jpg"
                            End If
                        End If
                        logs.Add(log)
                    End While
                End Using
            End Using
        End Using

        Return logs
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimoLog() As Log
        Dim log As New Log()
        Dim query As String = "
        SELECT TOP 1 L.Id_Log, L.Id_Tercero, T.Nombres, L.Fecha_Hora_Evento, 
               CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso, T.Foto, T.Id_HikCamara
        FROM Logs L
        LEFT JOIN Terceros T ON T.Id_Tercero = L.Id_Tercero
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso
        ORDER BY L.Id_Log DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        log.Id_Log = reader("Id_Log").ToString()
                        log.Id_Tercero = reader("Id_Tercero").ToString()
                        log.Nombres = reader("Nombres").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.Id_HikCamara = reader("Id_HikCamara").ToString()
                        Dim fileName As String = "face_" & log.Id_HikCamara & ".jpg"
                        Dim fileName2 As String = "visitor_" & log.Id_Tercero & ".jpg"
                        Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)
                        Dim serverPath2 As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName2)
                        If System.IO.File.Exists(serverPath) Then
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName
                        ElseIf Not IsDBNull(reader("Foto")) Then
                            Dim base64String As String = reader("Foto").ToString()
                            If base64String.StartsWith("data:image/") Then
                                base64String = base64String.Split(","c)(1) ' Eliminar el prefijo
                            End If
                            Dim fotoBytes As Byte() = Convert.FromBase64String(base64String)
                            System.IO.File.WriteAllBytes(serverPath2, fotoBytes)
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName2
                        Else
                            If log.Id_HikCamara <> Nothing Then
                                log.FotoUrl = Consultar_Foto(log.Id_HikCamara)
                            Else
                                log.FotoUrl = "../../../Diseno/Imagenes/Blanco.jpg"
                            End If
                        End If
                    End If
                End Using
            End Using
        End Using

        Return log
    End Function

    <WebMethod()>
    Public Shared Function ObtenerFoto(cedula As String) As Object
        Dim base64Foto As String = String.Empty
        Dim nombres As String = String.Empty
        Dim autoriza As String = String.Empty
        Dim persona_visitada As String = String.Empty
        Dim inmueble As String = String.Empty
        Dim placa As String = String.Empty
        Dim tipo_vehiculo As String = String.Empty
        Dim observacion As String = String.Empty
        Dim query As String = "SELECT Id_Tercero, Foto, Nombres, Id_Rol FROM Terceros WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Cedula", cedula)
                conn.Open()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        If Not IsDBNull(reader("Foto")) Then
                            base64Foto = Convert.ToString(reader("Foto"))
                        End If

                        If Not IsDBNull(reader("Nombres")) Then
                            nombres = Convert.ToString(reader("Nombres"))
                        End If

                        ' Verificar si el Id_Rol es igual a 4
                        If Not IsDBNull(reader("Id_Rol")) AndAlso Convert.ToInt32(reader("Id_Rol")) = 4 AndAlso Not IsDBNull(reader("Id_Tercero")) Then
                            Dim queryAutoriza As String = "SELECT TOP 1 PC.Id_Tercero, T1.Nombres, PC.Id_inmueble, IR.Id_Tercero AS Residente_Id_Tercero, T2.Nombres AS Residente_Nombres FROM Adm_Inmueble_Per_Confianza PC LEFT JOIN Terceros T1 ON T1.Id_Tercero = PC.Id_Tercero LEFT JOIN Adm_Inmueble_Residentes IR ON IR.Id_Inmueble = PC.Id_inmueble LEFT JOIN Terceros T2 ON T2.Id_Tercero = IR.Id_Tercero WHERE PC.Id_Tercero = @Id_Tercero"
                            Using cmdAutoriza As New SqlCommand(queryAutoriza, conn)
                                ' Asegúrate de proporcionar el valor correcto para TxInmueble.SelectValue
                                cmdAutoriza.Parameters.AddWithValue("@Id_Tercero", (reader("Id_Tercero")))
                                reader.Close() ' Cerrar el primer DataReader

                                Using readerAutoriza As SqlDataReader = cmdAutoriza.ExecuteReader()
                                    If readerAutoriza.Read() Then
                                        If Not IsDBNull(readerAutoriza("Residente_Nombres")) Then
                                            autoriza = Convert.ToString(readerAutoriza("Residente_Nombres"))
                                        End If
                                    End If
                                End Using
                            End Using
                        ElseIf Not IsDBNull(reader("Id_Tercero")) Then
                            Dim queryinfo As String = "SELECT TOP 1 Id_Visita, Persona_Visitada, Quien_Autoriza, Id_Inmueble, Placa, Observacion FROM Adm_Visita WHERE Id_Quien_Ingresa = @Id_Tercero ORDER BY Id_Visita DESC"
                            Using cmdinfo As New SqlCommand(queryinfo, conn)
                                cmdinfo.Parameters.AddWithValue("@Id_Tercero", (reader("Id_Tercero")))
                                reader.Close()

                                Using readerInfo As SqlDataReader = cmdinfo.ExecuteReader()
                                    If readerInfo.Read() Then
                                        If Not IsDBNull(readerInfo("Quien_Autoriza")) Then
                                            autoriza = Convert.ToString(readerInfo("Quien_Autoriza"))
                                        End If
                                        If Not IsDBNull(readerInfo("Persona_Visitada")) Then
                                            persona_visitada = Convert.ToString(readerInfo("Persona_Visitada"))
                                        End If
                                        If Not IsDBNull(readerInfo("Id_Inmueble")) Then
                                            inmueble = Convert.ToString(readerInfo("Id_Inmueble"))
                                        End If
                                        If Not IsDBNull(readerInfo("Placa")) Then
                                            placa = Convert.ToString(readerInfo("Placa"))

                                            ' Verificar si la placa es de un carro (formato ABC123)
                                            If System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{3}$") Then
                                                tipo_vehiculo = "1" ' Carro
                                                ' Verificar si la placa es de una moto (formato ABC12A)
                                            ElseIf System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{2}[A-Z]{1}$") Then
                                                tipo_vehiculo = "2" ' Moto
                                            Else
                                                tipo_vehiculo = ""
                                            End If
                                        End If
                                        If Not IsDBNull(readerInfo("Observacion")) Then
                                            observacion = Convert.ToString(readerInfo("Observacion"))
                                        End If
                                    End If
                                End Using
                            End Using
                        End If
                    End If
                End Using
            End Using
        End Using

        ' Devolver un objeto JSON con la foto, nombres y autoriza si aplica
        Return New With {
        .Foto = base64Foto,
        .Nombress = nombres,
        .Autoriza = If(String.IsNullOrEmpty(autoriza), String.Empty, autoriza),
        .Persona_Visitada = If(String.IsNullOrEmpty(persona_visitada), String.Empty, persona_visitada),
        .Id_Inmueble = If(String.IsNullOrEmpty(inmueble), String.Empty, inmueble),
        .Placa = If(String.IsNullOrEmpty(placa), String.Empty, placa),
        .Tipo_Vehiculo = If(String.IsNullOrEmpty(tipo_vehiculo), String.Empty, tipo_vehiculo),
        .Observacion = If(String.IsNullOrEmpty(observacion), String.Empty, observacion)
    }
    End Function

    Protected Sub TxTipoVehiculo_TextChanged(sender As Object, e As EventArgs) Handles TxTipoVehiculo.TextChanged
        If TxTipoVehiculo.SelectedValue = "1" Or TxTipoVehiculo.SelectedValue = "2" Then
            TxPlaca.Enabled = True
            TxPlaca.Attributes("required") = "1"
            lblPlacaError.Style("display") = "none"
        Else
            TxPlaca.Enabled = False
            TxPlaca.Text = ""
            TxPlaca.Attributes.Remove("required")
            lblPlacaError.Style("display") = "none"
            BtGuardar.Enabled = True
        End If
    End Sub

    Protected Sub TxPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxPlaca.TextChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue
        Dim placa As String = TxPlaca.Text.Trim()
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
        Else
            lblPlacaError.Text = "Placa inválida. " & GetValidationMessage(tipoVehiculo)
            lblPlacaError.Attributes("style") = "display:block; margin-top: 60px;"
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

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Guardar_Persona()
        Dim sql As String = "INSERT INTO dbo.Terceros (Id_Tercero, Cedula, Nombres, Id_Rol, Usuario, Password, Foto, Id_Empresa, Id_Sede, Estado) " &
                        "VALUES (@Id_Tercero, @Cedula, @Nombres, @Id_Rol, @Usuario, @Password, @Foto, @Id_Empresa, @Id_Sede, @Estado)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))

                If Not String.IsNullOrEmpty(TxCedulaVisitante.Text) Then
                    cmd.Parameters.AddWithValue("@Cedula", TxCedulaVisitante.Text)
                    cmd.Parameters.AddWithValue("@Usuario", Convert.ToInt64(TxCedulaVisitante.Text))
                    cmd.Parameters.AddWithValue("@Password", Convert.ToInt64(TxCedulaVisitante.Text))
                Else
                    cmd.Parameters.AddWithValue("@Cedula", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Nombres", TxNombreVisitante.Text.ToUpper())
                cmd.Parameters.AddWithValue("@Id_Rol", 9)

                If Not String.IsNullOrEmpty(base64image.Value) Then
                    cmd.Parameters.AddWithValue("@Foto", base64image.Value)
                Else
                    cmd.Parameters.AddWithValue("@Foto", DBNull.Value)
                End If
                cmd.Parameters.AddWithValue("@Id_Empresa", If(IsNumeric(TxEmpresa.SelectedValue), Convert.ToInt32(TxEmpresa.SelectedValue), DBNull.Value))
                If Not String.IsNullOrEmpty(Session("Sucursal_Usuario")) Then
                    cmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))
                Else
                    cmd.Parameters.AddWithValue("@Id_Sede", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Estado", 1)

                ' Abrir la conexión y ejecutar el comando
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Consultar_Id_Visita()
        Dim Valor As Integer = ObtenerMaxId("Id_Visita", "Adm_Visita")
        Session("Id_Visita") = Valor + 1
    End Sub

    Private Sub Registrar_Entrada()
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(TxCedulaVisitante.Text))
        Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(Convert.ToInt64(User.Identity.Name))
        Dim sql As String = "INSERT INTO dbo.Adm_Visita (" &
                         "Id_Visita, Id_Quien_Registra, Id_Quien_Ingresa, Id_Inmueble, " &
                         "Persona_Visitada, Quien_Autoriza, Fecha_Inicio_Visita, Hora_Inicio_Visita, " &
                         "Placa, Observacion, Id_Sede, Id_Acceso_Entrada, Manual, Masivo, Estado) VALUES (" &
                         "@Id_Visita, @Id_Quien_Registra, @Id_Quien_Ingresa, @Id_Inmueble, " &
                         "@Persona_Visitada, @Quien_Autoriza, @Fecha_Inicio_Visita, @Hora_Inicio_Visita, " &
                         "@Placa, @Observacion, @Id_Sede, @Id_Acceso_Entrada, @Manual, @Masivo, @Estado)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Visita", Session("Id_Visita"))
                cmd.Parameters.AddWithValue("@Id_Quien_Registra", idTercero_Quien_Registra)
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                cmd.Parameters.AddWithValue("@Id_Inmueble", If(Session("Id_Inmueble") Is Nothing, DBNull.Value, Session("Id_Inmueble").ToString))

                If TxNombrePersona.SelectedValue <> String.Empty Then
                    cmd.Parameters.AddWithValue("@Persona_Visitada", TxNombrePersona.SelectedItem.Text.ToUpper())
                Else
                    cmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Quien_Autoriza", TxNombrePersona.SelectedItem.Text.ToUpper())
                cmd.Parameters.AddWithValue("@Fecha_Inicio_Visita", Date.Now.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@Hora_Inicio_Visita", Date.Now.ToString("HH:mm:ss"))

                If TxPlaca.Text <> String.Empty Then
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.ToUpper())
                Else
                    cmd.Parameters.AddWithValue("@Placa", DBNull.Value)
                End If

                If TxObservacion.Text <> String.Empty Then
                    cmd.Parameters.AddWithValue("@Observacion", TxObservacion.Text())
                Else
                    cmd.Parameters.AddWithValue("@Observacion", DBNull.Value)
                End If

                If Not String.IsNullOrEmpty(Session("Sucursal_Usuario")) Then
                    cmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))
                Else
                    cmd.Parameters.AddWithValue("@Id_Sede", DBNull.Value)
                End If

                If Not String.IsNullOrEmpty(Session("Acceso_Usuario")) Then
                    cmd.Parameters.AddWithValue("@Id_Acceso_Entrada", Session("Acceso_Usuario"))
                Else
                    cmd.Parameters.AddWithValue("@Id_Acceso_Entrada", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Manual", 0)
                cmd.Parameters.AddWithValue("@Masivo", 0)
                cmd.Parameters.AddWithValue("@Estado", 1)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Actualizar_Foto()
        Dim sql As String = "UPDATE Terceros SET Foto = @Foto WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Foto", base64image.Value)
                cmd.Parameters.AddWithValue("@Cedula", TxCedulaVisitante.Text)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Registrar_Log_Entrada()
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(TxCedulaVisitante.Text))

        Dim sql As String = "INSERT INTO dbo.Logs (" &
                         "Id_Log, Id_Tercero, Fecha_Hora_Evento, Tipo_Evento, Id_Acceso) VALUES (" &
                         "@Id_Log, @Id_Tercero, @Fecha_Hora_Evento, @Tipo_Evento, @Id_Acceso)"

        ' Conexión y comandos SQL
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Abrir la conexión
            conn.Open()

            ' Iniciar una transacción para asegurar la consistencia y el bloqueo exclusivo
            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Log), 0) + 1 FROM dbo.Logs WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    ' Ejecutar la consulta para obtener el nuevo Id_Visita
                    Dim newIdLog As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    ' Ahora que tenemos el nuevo Id_Visita, procedemos con la inserción
                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Log", newIdLog)
                        insertCmd.Parameters.AddWithValue("@Id_Tercero", idTercero_Visitante)
                        insertCmd.Parameters.AddWithValue("@Fecha_Hora_Evento", Date.Now)
                        insertCmd.Parameters.AddWithValue("@Tipo_Evento", 1)
                        insertCmd.Parameters.AddWithValue("@Id_Acceso", Session("Acceso_Usuario"))

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Private Sub Consultar_Inmueble_Persona(Id As Integer)

        Using conexion As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using comando As New SqlCommand("
            SELECT TOP 1 IE.Id_Inmueble 
            FROM Adm_Inmueble_Residentes IE 
            LEFT JOIN Terceros T ON T.Id_Tercero = IE.Id_Tercero 
            WHERE IE.Id_Tercero = @Id_Tercero", conexion)

                comando.Parameters.AddWithValue("@Id_Tercero", Id)

                Try
                    conexion.Open()
                    Dim resultado As Object = comando.ExecuteScalar()

                    If resultado IsNot Nothing AndAlso Not IsDBNull(resultado) Then
                        Session("Id_Inmueble") = Convert.ToString(resultado)
                    Else
                        Session("Id_Inmueble") = Nothing
                    End If
                Catch ex As Exception
                End Try

            End Using
        End Using
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If String.IsNullOrWhiteSpace(TxCedulaVisitante.Text) Then
            Dim script As String = String.Format("swal('OJO!', 'El campo cedula visitante no puede estar vacio', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        ElseIf String.IsNullOrWhiteSpace(TxNombreVisitante.Text) Then
            Dim script As String = String.Format("swal('OJO!', 'El campo nombre visitante no puede estar vacio', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        ElseIf String.IsNullOrWhiteSpace(Session("Acceso_Usuario")) Then
            Dim script As String = String.Format("swal('OJO!', 'Debe seleccionar el acceso', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        Else
            If TxCedulaVisitante.Text <> "" Then
                If Not RegistroExisteEnBD(TxCedulaVisitante.Text) Then
                    Consultar_Id_Persona()
                    Guardar_Persona()
                Else
                    If Not String.IsNullOrEmpty(base64image.Value) Then
                        Actualizar_Foto()
                    End If
                End If
            End If
            Consultar_Id_Visita()
            Consultar_Inmueble_Persona(TxNombrePersona.SelectedValue)
            Registrar_Entrada()
            Registrar_Log_Entrada()
            If TxEmpresa.SelectedItem.Text = "OTRO" Then
                Correo_Notificacion("monitoreo@auteco.com.co")
            End If

            If ErrorOp = Nothing Then
                Dim script As String = "swal('Excelente!', 'Visita Registrada Correctamente', 'success'); setTimeout(function(){ window.location.href = 'Registrar_Visita_Proveedor.aspx'; }, 3000);"
                ScriptManager.RegisterStartupScript(Page, GetType(Page), "redirect", script, True)
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                EnviarCorreoError()
            End If
        End If
    End Sub

End Class