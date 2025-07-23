Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class RegistroFacialUnificadoAccesos
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

                            Dim idAcceso As Long = ObtenerAcceso(eventObject.ipAddress)

                            ' Verificar si el registro fue muy reciente (entrada o salida)
                            If Not ExisteRegistroReciente(idTercero_Visitante) Then

                                Dim idVisita As Long = ObtenerUltimaVisitaEmpleado(idTercero_Visitante, eventObject.ipAddress)

                                If idVisita = 0 Then
                                    Registrar_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                Else
                                    Registrar_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
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
            Response.Write("<h1>Estado de Registro Facial Unificado Acceso</h1>")
            Response.Write("<p>" & ultimoEvento & "</p>")
            Response.Write("<p>Esperando eventos mediante solicitudes POST...</p>")
        Else
            Response.StatusCode = 405 ' Método no permitido
            Response.Write("Solo se permiten solicitudes POST.")
        End If
    End Sub

    Private Function ExisteRegistroReciente(idTercero As Long) As Boolean
        Dim toleranciaSegundos As Integer = 600
        Dim existeReciente As Boolean = False

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()

            Dim sql As String = "
            SELECT TOP 1 
                Fecha_Inicio_Visita AS FechaHoraEntrada,
                Fecha_Fin_Visita AS FechaHoraSalida,
                Hora_Inicio_Visita,
                Hora_Fin_Visita
            FROM Adm_Visita
            WHERE Id_Acceso_Entrada NOT IN (1,2) AND Id_Quien_Ingresa = @Id_Tercero
            ORDER BY Id_Visita DESC"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Tercero", idTercero)

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Dim ahora As DateTime = DateTime.Now
                        Dim fechaEntrada As DateTime
                        Dim fechaSalida As DateTime

                        ' Validar diferencia con la hora de entrada
                        If Not IsDBNull(reader("FechaHoraEntrada")) AndAlso Not IsDBNull(reader("Hora_Inicio_Visita")) Then
                            fechaEntrada = Convert.ToDateTime(reader("FechaHoraEntrada")).Date + TimeSpan.Parse(reader("Hora_Inicio_Visita").ToString())
                            Dim diferenciaEntrada = (ahora - fechaEntrada).TotalSeconds
                            If diferenciaEntrada >= 0 AndAlso diferenciaEntrada <= toleranciaSegundos Then
                                existeReciente = True
                            End If
                        End If

                        ' Validar diferencia con la hora de salida (si ya hay salida)
                        If Not IsDBNull(reader("FechaHoraSalida")) AndAlso Not IsDBNull(reader("Hora_Fin_Visita")) Then
                            fechaSalida = Convert.ToDateTime(reader("FechaHoraSalida")).Date + TimeSpan.Parse(reader("Hora_Fin_Visita").ToString())
                            Dim diferenciaSalida = (ahora - fechaSalida).TotalSeconds
                            If diferenciaSalida >= 0 AndAlso diferenciaSalida <= toleranciaSegundos Then
                                existeReciente = True
                            End If
                        End If
                    End If
                End Using
            End Using
        End Using

        Return existeReciente
    End Function

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

    Private Function ObtenerUltimaVisitaEmpleado(idTercero_Visitante As Long, Ip_Camara As String) As Long
        Dim idVisita As Long = 0
        Dim Id_Inmueble As String = Identificar_Area_Facial(Ip_Camara)

        ' Obtener la hora actual
        Dim horaActual As TimeSpan = DateTime.Now.TimeOfDay
        Dim horaLimite As TimeSpan = New TimeSpan(6, 0, 0) ' 6:00 AM

        ' Crear conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()

            ' Buscar primero en el día actual
            Dim sqlHoy As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita 
                            WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa 
                            AND CAST(Fecha_Inicio_Visita AS DATE) = CAST(GETDATE() AS DATE) 
                            AND Fecha_Fin_Visita IS NULL 
                            AND Id_Acceso_Entrada IN (SELECT Id_Acceso FROM Adm_Accesos WHERE Id_Inmueble = @Id_Inmueble)
                            ORDER BY Id_Visita DESC"

            Using cmdHoy As New SqlCommand(sqlHoy, conn)
                cmdHoy.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                cmdHoy.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)

                Using reader As SqlDataReader = cmdHoy.ExecuteReader()
                    If reader.Read() Then
                        idVisita = Convert.ToInt64(reader("Id_Visita"))
                    End If
                End Using
            End Using

            ' Si no hay visita hoy y es antes de las 6:00 AM, buscar en ayer
            If idVisita = 0 AndAlso horaActual < horaLimite Then
                Dim sqlAyer As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita 
                                 WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa 
                                 AND CAST(Fecha_Inicio_Visita AS DATE) = CAST(DATEADD(DAY, -1, GETDATE()) AS DATE) 
                                 AND Fecha_Fin_Visita IS NULL 
                                 AND Id_Acceso_Entrada IN (SELECT Id_Acceso FROM Adm_Accesos WHERE Id_Inmueble = @Id_Inmueble)
                                 ORDER BY Id_Visita DESC"

                Using cmdAyer As New SqlCommand(sqlAyer, conn)
                    cmdAyer.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                    cmdAyer.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)

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
                    cmd.Parameters.AddWithValue("@Fecha_Fin_Visita", DateTime.Now.Date)
                    cmd.Parameters.AddWithValue("@Hora_fin_Visita", DateTime.Now.TimeOfDay)
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
