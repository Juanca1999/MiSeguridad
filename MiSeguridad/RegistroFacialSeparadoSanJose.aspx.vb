Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports Newtonsoft.Json
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Mime

Public Class RegistroFacialSeparadoSanJose
    Inherits System.Web.UI.Page

    ' Variable para almacenar el último evento recibido (opcional)
    Private Shared ultimoEvento As String = "No se han recibido eventos aún."

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.HttpMethod = "POST" Then
            Try
                ' Leer el cuerpo de la solicitud (datos enviados en el POST)
                Dim requestBody As String
                Using reader As New StreamReader(Request.InputStream)
                    requestBody = reader.ReadToEnd()
                End Using

                ' Guardar el contenido recibido para mostrarlo en el navegador
                ultimoEvento = "¡Evento recibido! Datos:<br/>" & Server.HtmlEncode(requestBody)

                ' Extraer el JSON del evento
                Dim eventData As String = ExtractJsonFromRequest(requestBody)

                If Not String.IsNullOrEmpty(eventData) Then
                    ' Deserializar el JSON recibido
                    Dim eventObject = JsonConvert.DeserializeObject(Of EventData)(eventData)

                    ' Filtrar los eventos deseados
                    If eventObject.AccessControllerEvent IsNot Nothing AndAlso eventObject.AccessControllerEvent.majorEventType = 5 AndAlso eventObject.AccessControllerEvent.subEventType = 75 Then

                        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(eventObject.AccessControllerEvent.employeeNoString))

                        If idTercero_Visitante = 0 Then
                            RegistrarLogNoEncontrado(eventObject.AccessControllerEvent.employeeNoString)
                        Else
                            Dim EsEntrada As Boolean = IdentificarCamara(eventObject.ipAddress)

                            Dim Id_Rol As Long = ObtenerRolTercero(eventObject.AccessControllerEvent.employeeNoString)

                            If EsEntrada = True Then
                                If Not ExisteRegistroRecienteEntrada(eventObject.AccessControllerEvent.employeeNoString) Then
                                    Registrar_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    'If Id_Rol = 11 Then
                                    'Dim NombreAcceso As String = ObtenerNombreAcceso(eventObject.ipAddress)
                                    'Consultar_Padre(eventObject.AccessControllerEvent.employeeNoString, True, NombreAcceso)
                                    'End If
                                End If
                            Else
                                If Not ExisteRegistroRecienteSalida(eventObject.AccessControllerEvent.employeeNoString) Then
                                    Registrar_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    'If Id_Rol = 11 Then
                                    'Dim NombreAcceso As String = ObtenerNombreAcceso(eventObject.ipAddress)
                                    'Consultar_Padre(eventObject.AccessControllerEvent.employeeNoString, False, NombreAcceso)
                                    'End If
                                End If
                            End If

                        End If
                    End If
                End If

                ' Responder al cliente con un mensaje simple
                Response.StatusCode = 200
                Response.Write("¡Evento recibido correctamente!")
            Catch ex As Exception
                ' Manejo de errores
                Response.StatusCode = 500
                Response.Write("Error al procesar el evento: " & ex.Message)
            End Try
        ElseIf Request.HttpMethod = "GET" Then
            ' Mostrar el último evento recibido en la página
            Response.Write("<h1>Estado de Registro Facial Separado San Jose De Las Vegas</h1>")
            Response.Write("<p>" & ultimoEvento & "</p>")
            Response.Write("<p>Esperando eventos mediante solicitudes POST...</p>")
        Else
            Response.StatusCode = 405 ' Método no permitido
            Response.Write("Solo se permiten solicitudes POST.")
        End If
    End Sub

    Private Sub Consultar_Padre(Id_HikCamara As String, EsEntrada As Boolean, NombreAcceso As String)
        Dim query As String = "
        SELECT PH.Id_Tercero_Padres, TP.Nombres AS Padre, TP.Correo, 
               PH.Id_Tercero_Hijos, TH.Nombres AS Hijo, S.Nombre_Sede 
        FROM Adm_Padre_Hijos PH
        LEFT JOIN Terceros TP ON TP.Id_Tercero = PH.Id_Tercero_Padres
        LEFT JOIN Terceros TH ON TH.Id_Tercero = PH.Id_Tercero_Hijos
        LEFT JOIN Adm_Sedes S ON S.Id_Sede = TH.Id_Sede
        WHERE TH.Id_HikCamara = @Id_HikCamara"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Id_HikCamara", Id_HikCamara)
                conn.Open()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Try
                            Dim correoPadre As String = If(reader("Correo") IsNot DBNull.Value, reader("Correo").ToString().Trim(), "")
                            Dim nombreHijo As String = If(reader("Hijo") IsNot DBNull.Value, reader("Hijo").ToString().Trim(), "")
                            Dim sede As String = If(reader("Nombre_Sede") IsNot DBNull.Value, reader("Nombre_Sede").ToString().Trim(), "")

                            ' Validar que haya correo y nombre del hijo
                            If Not String.IsNullOrWhiteSpace(correoPadre) AndAlso Not String.IsNullOrWhiteSpace(nombreHijo) Then
                                If EsEntrada Then
                                    Correo_Notificacion_Entrada(correoPadre, nombreHijo, sede, NombreAcceso)
                                Else
                                    Correo_Notificacion_Salida(correoPadre, nombreHijo, sede, NombreAcceso)
                                End If
                            End If

                        Catch ex As Exception
                        End Try
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub Correo_Notificacion_Entrada(ByVal Correo_Notificacion As String, ByVal NombreHijo As String, ByVal Sede As String, ByVal NombreAcceso As String)
        Try
            Dim msg = New MailMessage()
            msg.From = New MailAddress(CorreoFrom, "Notificación entrada estudiante")
            msg.To.Add(New MailAddress(Correo_Notificacion))
            msg.Subject = "Notificación entrada estudiante"
            msg.IsBodyHtml = True

            Dim rutaImagen As String = HttpContext.Current.Server.MapPath("~/Adjunto/Logo/800084408.png")

            Dim recursoImagen As New LinkedResource(rutaImagen, "image/png")
            recursoImagen.ContentId = "Logo800084408"

            Dim htmlBody As String = "
        <table style='width: 100%; padding: 10px; padding-top: 30px; padding-bottom: 30px; background-color: #1E2833; border-radius: 12px;'>
            <tr>
                <td style='border-radius: 12px; box-shadow: 0 10px 20px 0px rgba(0, 0, 0, 0.56), 0 7px 20px -5px rgba(26, 30, 36, 0.40); padding: 12px; width: 70%; background-color: white'>
                    <p style='font-size: 11pt; font-family: Trebuchet MS; color: #1F4E79;'>
                        <b>Estudiante:</b> " & NombreHijo & "<br />
                        <b>Sede:</b> " & Sede & "<br />
                        <b>Puerta:</b> " & NombreAcceso & "<br />
                        <b>Fecha y hora:</b> " & Date.Now.ToString("dd/MM/yyyy HH:mm") & "<br />
                    </p>
                </td>
                <td style='width: 30%; text-align: center; vertical-align: middle'>
                    <img alt='Logo' src='cid:Logo800084408' width='150' />
                </td>
            </tr>
        </table>"

            Dim vistaHtml As AlternateView = AlternateView.CreateAlternateViewFromString(htmlBody, Nothing, MediaTypeNames.Text.Html)
            vistaHtml.LinkedResources.Add(recursoImagen)
            msg.AlternateViews.Add(vistaHtml)

            Dim smtp = New SmtpClient()
            smtp.Host = SmtpHost
            smtp.Port = Port
            smtp.Credentials = New NetworkCredential(UsuarioCredentials, ContrasenaCredentials)
            smtp.EnableSsl = True

            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, cert As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            smtp.Send(msg)

        Catch ex As Exception
        End Try
    End Sub

    Private Sub Correo_Notificacion_Salida(ByVal Correo_Notificacion As String, ByVal NombreHijo As String, ByVal Sede As String, ByVal NombreAcceso As String)
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Notificacion salida estudiante")
            msg.To.Add(New MailAddress(Correo_Notificacion))
            msg.Subject = "Notificación salida estudiante"
            msg.IsBodyHtml = True

            Dim rutaImagen As String = HttpContext.Current.Server.MapPath("~/Adjunto/Logo/800084408.png")

            Dim recursoImagen As New LinkedResource(rutaImagen, "image/png")
            recursoImagen.ContentId = "Logo800084408"

            Dim htmlBody As String = "
        <table style='width: 100%; padding: 10px; padding-top: 30px; padding-bottom: 30px; background-color: #1E2833; border-radius: 12px;'>
            <tr>
                <td style='border-radius: 12px; box-shadow: 0 10px 20px 0px rgba(0, 0, 0, 0.56), 0 7px 20px -5px rgba(26, 30, 36, 0.40); padding: 12px; width: 70%; background-color: white'>
                    <p style='font-size: 11pt; font-family: Trebuchet MS; color: #1F4E79;'>
                        <b>Estudiante:</b> " & NombreHijo & "<br />
                        <b>Sede:</b> " & Sede & "<br />
                        <b>Puerta:</b> " & NombreAcceso & "<br />
                        <b>Fecha y hora:</b> " & Date.Now.ToString("dd/MM/yyyy HH:mm") & "<br />
                    </p>
                </td>
                <td style='width: 30%; text-align: center; vertical-align: middle'>
                    <img alt='Logo' src='cid:Logo800084408' width='150' />
                </td>
            </tr>
        </table>"

            Dim vistaHtml As AlternateView = AlternateView.CreateAlternateViewFromString(htmlBody, Nothing, MediaTypeNames.Text.Html)
            vistaHtml.LinkedResources.Add(recursoImagen)
            msg.AlternateViews.Add(vistaHtml)

            Dim smtp = New SmtpClient()
            smtp.Host = SmtpHost
            smtp.Port = Port
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
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Id_HikCamara = @Usuario"
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

    Private Function ObtenerRolTercero(usuario As String) As Long
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Rol FROM Terceros WHERE Id_HikCamara = @Usuario"
        Dim Id_Rol As Long = 0

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Usuario", usuario)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Tercero
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    Id_Rol = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Tercero obtenido
        Return Id_Rol
    End Function

    Private Function ObtenerSede(Ip_Camara As String) As Long
        Dim query As String = "SELECT Id_Sede FROM Adm_Accesos WHERE IP_Camara = @IP_Camara"
        Dim idSede As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IP_Camara", Ip_Camara)
                connection.Open()

                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idSede = Convert.ToInt64(result)
                End If
            End Using
        End Using

        Return idSede
    End Function

    Private Function ObtenerAcceso(Ip_Camara As String) As Long
        Dim query As String = "SELECT Id_Acceso FROM Adm_Accesos WHERE IP_Camara = @IP_Camara"
        Dim idAcceso As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IP_Camara", Ip_Camara)
                connection.Open()

                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idAcceso = Convert.ToInt64(result)
                End If
            End Using
        End Using

        Return idAcceso
    End Function

    Private Function ObtenerNombreAcceso(Ip_Camara As String) As String
        Dim query As String = "SELECT Nombre_Acceso FROM Adm_Accesos WHERE IP_Camara = @IP_Camara"
        Dim NombreAcceso As String = ""

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IP_Camara", Ip_Camara)
                connection.Open()

                Dim result = command.ExecuteScalar()
                If result IsNot Nothing Then
                    NombreAcceso = result.ToString().Trim()
                End If
            End Using
        End Using

        Return NombreAcceso
    End Function

    Private Function ExisteRegistroRecienteEntrada(employeeNo As String) As Boolean
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Nueva consulta más segura y robusta
                Dim query As String = "
                SELECT COUNT(*) 
                FROM Adm_Visita 
                WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa 
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Inicio_Visita, ' ', Hora_Inicio_Visita)) IS NOT NULL
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Inicio_Visita, ' ', Hora_Inicio_Visita)) >= 
                    DATEADD(SECOND, -ISNULL((SELECT TOP 1 Segundos_Facial FROM Adm_Empresa), 10), GETDATE())"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ExisteRegistroRecienteSalida(employeeNo As String) As Boolean
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "
                SELECT COUNT(*) 
                FROM Adm_Visita 
                WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa 
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Fin_Visita, ' ', Hora_Fin_Visita)) IS NOT NULL
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Fin_Visita, ' ', Hora_Fin_Visita)) >= 
                    DATEADD(SECOND, -ISNULL((SELECT TOP 1 Segundos_Facial FROM Adm_Empresa), 10), GETDATE())"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ObtenerUltimaVisitaEmpleado(idTercero_Visitante As Long, Ip_Camara As String) As Long
        Dim idVisita As Long = 0

        ' Crear conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()

            ' Consulta para buscar en el día actual
            Dim sqlHoy As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita 
                                WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa 
                                AND CAST(Fecha_Inicio_Visita AS DATE) = CAST(GETDATE() AS DATE) 
                                AND Fecha_Fin_Visita IS NULL 
                                ORDER BY Id_Visita DESC"

            Using cmdHoy As New SqlCommand(sqlHoy, conn)
                cmdHoy.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)

                Using reader As SqlDataReader = cmdHoy.ExecuteReader()
                    If reader.Read() Then
                        idVisita = Convert.ToInt64(reader("Id_Visita"))
                    End If
                End Using
            End Using

            ' Si no se encuentra en el día actual, buscar en el día anterior
            If idVisita = 0 Then
                Dim sqlAyer As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita 
                                     WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa 
                                     AND CAST(Fecha_Inicio_Visita AS DATE) = CAST(DATEADD(DAY, -1, GETDATE()) AS DATE) 
                                     AND Fecha_Fin_Visita IS NULL
                                     ORDER BY Id_Visita DESC"

                Using cmdAyer As New SqlCommand(sqlAyer, conn)
                    cmdAyer.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)

                    Using reader As SqlDataReader = cmdAyer.ExecuteReader()
                        If reader.Read() Then
                            idVisita = Convert.ToInt64(reader("Id_Visita"))
                        End If
                    End Using
                End Using
            End If
        End Using

        Return idVisita
    End Function

    Private Function IdentificarCamara(Ip_Camara As String) As Boolean
        Try
            ' Conexión a la base de datos
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT Tipo_Evento FROM Adm_Accesos WHERE IP_Camara = @IP_Camara"
                Using command As New SqlCommand(query, connection)
                    ' Parámetro para la IP de la cámara
                    command.Parameters.AddWithValue("@IP_Camara", Ip_Camara)

                    ' Ejecutar el comando y obtener el resultado
                    Dim result = command.ExecuteScalar()

                    ' Verificar si el resultado no es nulo y devolver como booleano
                    If result IsNot Nothing Then
                        Return Convert.ToBoolean(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try

        Return False
    End Function

    Private Function Identificar_Area_Facial(Ip_Camara As String) As String
        Try
            ' Conexión a la base de datos
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Id_Inmueble FROM Adm_Accesos WHERE IP_Camara = @IP_Camara"
                Using command As New SqlCommand(query, connection)
                    ' Parámetro para la IP de la cámara
                    command.Parameters.AddWithValue("@IP_Camara", Ip_Camara)

                    ' Ejecutar el comando y obtener el resultado
                    Dim result = command.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                        ' Devuelve el Id_Inmueble como String
                        Return result.ToString()
                    Else
                        ' Devuelve una cadena vacía si no hay datos
                        Return ""
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' En caso de error, devuelve una cadena vacía
            Return ""
        End Try
    End Function

    Private Sub Registrar_Entrada(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim idSede As Long = ObtenerSede(Ip_Camara)
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

        Dim sql As String = "INSERT INTO dbo.Adm_Visita (" &
                         "Id_Visita, Id_Quien_Registra, Id_Quien_Ingresa, Id_Inmueble, " &
                         "Persona_Visitada, Quien_Autoriza, Fecha_Inicio_Visita, Hora_Inicio_Visita, " &
                         "Placa, Observacion, Id_Sede, Id_Acceso_Entrada, Manual, Masivo, Estado) VALUES (" &
                         "@Id_Visita, @Id_Quien_Registra, @Id_Quien_Ingresa, @Id_Inmueble, " &
                         "@Persona_Visitada, @Quien_Autoriza, @Fecha_Inicio_Visita, @Hora_Inicio_Visita, " &
                         "@Placa, @Observacion, @Id_Sede, @Id_Acceso_Entrada, @Manual, @Masivo, @Estado)"

        ' Conexión y comandos SQL
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Abrir la conexión
            conn.Open()

            ' Iniciar una transacción para asegurar la consistencia y el bloqueo exclusivo
            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Visita), 0) + 1 FROM dbo.Adm_Visita WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    ' Ejecutar la consulta para obtener el nuevo Id_Visita
                    Dim newIdVisita As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    ' Ahora que tenemos el nuevo Id_Visita, procedemos con la inserción
                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Visita", newIdVisita)
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Registra", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                        insertCmd.Parameters.AddWithValue("@Id_Inmueble", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)

                        insertCmd.Parameters.AddWithValue("@Quien_Autoriza", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Fecha_Inicio_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        insertCmd.Parameters.AddWithValue("@Hora_Inicio_Visita", Date.Now.ToString("HH:mm:ss"))
                        insertCmd.Parameters.AddWithValue("@Placa", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Observacion", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Sede", idSede)
                        insertCmd.Parameters.AddWithValue("@Id_Acceso_Entrada", idAcceso)
                        insertCmd.Parameters.AddWithValue("@Manual", 1)
                        insertCmd.Parameters.AddWithValue("@Masivo", 0)
                        insertCmd.Parameters.AddWithValue("@Estado", 1)

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Private Sub Registrar_Log_Entrada(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

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
                        insertCmd.Parameters.AddWithValue("@Id_Acceso", idAcceso)

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Private Sub Registrar_Salida_Sin_Entrada(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim idSede As Long = ObtenerSede(Ip_Camara)
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

        Dim sql As String = "INSERT INTO dbo.Adm_Visita (" &
                         "Id_Visita, Id_Quien_Registra, Id_Quien_Ingresa, Id_Inmueble, " &
                         "Persona_Visitada, Quien_Autoriza, Fecha_Fin_Visita, Hora_fin_Visita, " &
                         "Placa, Observacion, Id_Sede, Id_Acceso_Salida, Manual, Masivo, Estado) VALUES (" &
                         "@Id_Visita, @Id_Quien_Registra, @Id_Quien_Ingresa, @Id_Inmueble, " &
                         "@Persona_Visitada, @Quien_Autoriza, @Fecha_Fin_Visita, @Hora_fin_Visita, " &
                         "@Placa, @Observacion, @Id_Sede, @Id_Acceso_Salida, @Manual, @Masivo, @Estado)"

        ' Conexión y comandos SQL
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Abrir la conexión
            conn.Open()

            ' Iniciar una transacción para asegurar la consistencia y el bloqueo exclusivo
            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Visita), 0) + 1 FROM dbo.Adm_Visita WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    ' Ejecutar la consulta para obtener el nuevo Id_Visita
                    Dim newIdVisita As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    ' Ahora que tenemos el nuevo Id_Visita, procedemos con la inserción
                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Visita", newIdVisita)
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Registra", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                        insertCmd.Parameters.AddWithValue("@Id_Inmueble", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)

                        insertCmd.Parameters.AddWithValue("@Quien_Autoriza", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Fecha_Fin_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        insertCmd.Parameters.AddWithValue("@Hora_fin_Visita", Date.Now.ToString("HH:mm:ss"))
                        insertCmd.Parameters.AddWithValue("@Placa", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Observacion", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Sede", idSede)
                        insertCmd.Parameters.AddWithValue("@Id_Acceso_Salida", idAcceso)
                        insertCmd.Parameters.AddWithValue("@Manual", 1)
                        insertCmd.Parameters.AddWithValue("@Masivo", 0)
                        insertCmd.Parameters.AddWithValue("@Estado", 0)

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Private Sub Registrar_Salida(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

        Dim idVisita As Long = ObtenerUltimaVisitaEmpleado(idTercero_Visitante, Ip_Camara)

        If idVisita = 0 Then
            Registrar_Salida_Sin_Entrada(employeeNo, Ip_Camara)
        Else
            Dim sql As String = "UPDATE Adm_Visita SET Fecha_Fin_Visita = @Fecha_Fin_Visita, Hora_fin_Visita = @Hora_fin_Visita, Id_Acceso_Salida = @Id_Acceso_Salida, Manual = 1, Estado = 0 WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa AND Id_Visita = @Id_Visita"

            ' Conexión y comandos SQL
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                ' Abrir la conexión
                conn.Open()

                ' Crear el comando SQL
                Using cmd As New SqlCommand(sql, conn)
                    ' Agregar parámetros para prevenir inyección de SQL
                    cmd.Parameters.AddWithValue("@Fecha_Fin_Visita", Date.Now.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@Hora_fin_Visita", Date.Now.ToString("HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@Id_Acceso_Salida", idAcceso)
                    cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                    cmd.Parameters.AddWithValue("@Id_Visita", idVisita)

                    ' Ejecutar el comando
                    Try
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                        If rowsAffected > 0 Then
                            ' Registro actualizado correctamente
                        Else
                            ' No se encontró el registro a actualizar
                        End If
                    Catch ex As Exception
                    End Try
                End Using
            End Using
        End If
    End Sub

    Private Sub Registrar_Log_Salida(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

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
                        insertCmd.Parameters.AddWithValue("@Tipo_Evento", 0)
                        insertCmd.Parameters.AddWithValue("@Id_Acceso", idAcceso)

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Private Sub RegistrarLogNoEncontrado(employeeNo As String)
        Dim rutaLog As String = HttpContext.Current.Server.MapPath("~/Diseno/log_errores.txt")
        Dim mensaje As String = $"{Date.Now:yyyy-MM-dd HH:mm:ss} - Usuario no encontrado en access point: {employeeNo}"
        File.AppendAllText(rutaLog, mensaje & Environment.NewLine)
    End Sub

    ' Método para extraer JSON de la solicitud
    Private Function ExtractJsonFromRequest(requestBody As String) As String
        Dim jsonStartIndex As Integer = requestBody.IndexOf("{")
        Dim jsonEndIndex As Integer = requestBody.LastIndexOf("}") + 1

        If jsonStartIndex >= 0 AndAlso jsonEndIndex > jsonStartIndex Then
            Return requestBody.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex)
        Else
            Return String.Empty
        End If
    End Function

    ' Clase para deserializar el JSON del evento
    Public Class EventData
        Public Property AccessControllerEvent As AccessControllerEventData
        Public Property ipAddress As String
        Public Property dateTime As String
    End Class

    Public Class AccessControllerEventData
        Public Property majorEventType As Integer
        Public Property subEventType As Integer
        Public Property employeeNoString As String
        Public Property name As String
        Public Property userType As String
    End Class

End Class
