Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class RegistroFacial
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

                        Dim EsEntrada As Boolean = IdentificarCamara(eventObject.ipAddress)

                        Dim Id_Rol As Long = ObtenerRolTercero(eventObject.AccessControllerEvent.employeeNoString)

                        If Id_Rol = 5 Then
                            If EsEntrada = True Then
                                If Not ExisteRegistroReciente(eventObject.AccessControllerEvent.employeeNoString) Then
                                    VerificarYRegistrarVencimientos(eventObject.AccessControllerEvent.employeeNoString)
                                    Registrar_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                End If
                            Else
                                If Not ExisteRegistroReciente(eventObject.AccessControllerEvent.employeeNoString) Then
                                    VerificarYRegistrarVencimientos(eventObject.AccessControllerEvent.employeeNoString)
                                    Registrar_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                End If
                            End If
                        Else
                            If EsEntrada = True Then
                                If Not ExisteRegistroReciente(eventObject.AccessControllerEvent.employeeNoString) Then
                                    Registrar_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                    Registrar_Log_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                End If
                            Else
                                If Not ExisteRegistroReciente(eventObject.AccessControllerEvent.employeeNoString) Then
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
            Response.Write("<h1>Estado de Registro Facial</h1>")
            Response.Write("<p>" & ultimoEvento & "</p>")
            Response.Write("<p>Esperando eventos mediante solicitudes POST...</p>")
        Else
            Response.StatusCode = 405 ' Método no permitido
            Response.Write("Solo se permiten solicitudes POST.")
        End If
    End Sub

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

    Private Sub VerificarYRegistrarVencimientos(employeeNo As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim fechaActual As DateTime = DateTime.Now.Date

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "
                SELECT TOP 1 
                    C.Fecha_ARL, 
                    C.Fecha_Seguridad_Social, 
                    C.Fecha_Trabajo_Alturas 
                FROM Adm_Control_Contratistas C 
                LEFT JOIN Terceros TC ON TC.Cedula = C.Cedula_Contratista 
                WHERE TC.Id_Tercero = @Id_Tercero 
                ORDER BY C.Id_Control_Contratistas DESC"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Id_Tercero", idTercero_Visitante)

                    Using reader As SqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            If Not IsDBNull(reader("Fecha_ARL")) Then
                                Dim fechaARL As DateTime = Convert.ToDateTime(reader("Fecha_ARL")).Date
                                If fechaARL < fechaActual Then
                                    Registrar_Log_Contratista(idTercero_Visitante, "ARL VENCIDO")
                                End If
                            End If

                            If Not IsDBNull(reader("Fecha_Seguridad_Social")) Then
                                Dim fechaSS As DateTime = Convert.ToDateTime(reader("Fecha_Seguridad_Social")).Date
                                If fechaSS < fechaActual Then
                                    Registrar_Log_Contratista(idTercero_Visitante, "SEGURIDAD SOCIAL VENCIDO")
                                End If
                            End If

                            If Not IsDBNull(reader("Fecha_Trabajo_Alturas")) Then
                                Dim fechaAlturas As DateTime = Convert.ToDateTime(reader("Fecha_Trabajo_Alturas")).Date
                                If fechaAlturas < fechaActual Then
                                    Registrar_Log_Contratista(idTercero_Visitante, "TRABAJO EN ALTURAS VENCIDO")
                                End If
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Manejo de errores si es necesario
        End Try
    End Sub

    Private Sub Registrar_Log_Contratista(idTercero_Visitante As Long, tipoNovedad As String)
        Dim sql As String = "INSERT INTO dbo.Logs_Contratistas (" &
                         "Id_Novedad_Contratista, Id_Tercero, Fecha_Hora_Novedad, Tipo_Novedad) VALUES (" &
                         "@Id_Novedad_Contratista, @Id_Tercero, @Fecha_Hora_Novedad, @Tipo_Novedad)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()

            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Novedad_Contratista), 0) + 1 FROM dbo.Logs_Contratistas WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    Dim newIdLog As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Novedad_Contratista", newIdLog)
                        insertCmd.Parameters.AddWithValue("@Id_Tercero", idTercero_Visitante)
                        insertCmd.Parameters.AddWithValue("@Fecha_Hora_Novedad", Date.Now)
                        insertCmd.Parameters.AddWithValue("@Tipo_Novedad", tipoNovedad)

                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using
                trans.Commit()
            End Using
        End Using
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

    Private Function ExisteRegistroReciente(employeeNo As String) As Boolean
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Consulta SQL para verificar si existe un registro en los últimos 60 segundos
                Dim query As String = "SELECT COUNT(*) FROM Adm_Visita WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa AND CONVERT(DATETIME, CONVERT(VARCHAR(10), Fecha_Inicio_Visita, 120) + ' ' + CONVERT(VARCHAR(8), Hora_Inicio_Visita, 108)) >= DATEADD(SECOND, -(SELECT TOP 1 Segundos_Facial FROM Adm_Empresa),  GETDATE())"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    ' Si count > 0, significa que ya existe un registro reciente
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            ' En caso de error, asumimos que no hay registro reciente
            Return False
        End Try
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

    Private Function ObtenerUltimaVisitaEmpleado(idTercero_Visitante As Long) As Long
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

    Private Sub Registrar_Entrada(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))

        If idTercero_Visitante = 0 Then
            If Verificar_Si_Existe(Ip_Camara, employeeNo) Then
                Eliminar_Usuario(Ip_Camara, employeeNo)
            End If
        Else
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
        End If
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

        If idTercero_Visitante = 0 Then
            If Verificar_Si_Existe(Ip_Camara, employeeNo) Then
                Eliminar_Usuario(Ip_Camara, employeeNo)
            End If
        Else
            Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

            Dim idVisita As Long = ObtenerUltimaVisitaEmpleado(idTercero_Visitante)

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

    Private Function Verificar_Si_Existe(IP_EndPoint As String, Id_Empleado As String) As Boolean
        Try
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Search?format=json"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json"
            request.Credentials = New NetworkCredential("admin", "Miro-321")
            request.PreAuthenticate = False

            ' JSON que se enviará en el cuerpo
            Dim jsonBody As String = "
        {
            ""UserInfoSearchCond"": {
                ""searchID"": """ & Id_Empleado & """,
                ""searchResultPosition"": 0,
                ""maxResults"": 200,
                ""EmployeeNoList"": [
                    { ""employeeNo"": """ & Id_Empleado & """ }
                ]
            }
        }"

            ' Escribir el JSON en el cuerpo de la solicitud
            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(jsonBody)
                writer.Flush()
            End Using

            ' Obtener y procesar la respuesta
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Using reader As New StreamReader(response.GetResponseStream())
                Dim responseBody As String = reader.ReadToEnd()

                ' Deserializar la respuesta
                Dim result = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseBody)
                Dim userInfoSearch = result("UserInfoSearch")

                ' Verificar si hay usuarios
                If userInfoSearch("numOfMatches") = 0 Then
                    Return False ' Usuario no encontrado
                Else
                    Return True ' Usuario encontrado
                End If
            End Using
        Catch ex As Exception
            Return False ' Ocurrió un error al verificar la existencia
        End Try
    End Function

    Private Sub Eliminar_Usuario(IP_EndPoint As String, Id_Empleado As String)
        Try
            ' Usar la IP obtenida para construir la URL de la solicitud
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Delete?format=json"
            Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            requestDocument.Method = "PUT"
            requestDocument.ContentType = "application/json"
            Dim credentials As New NetworkCredential("admin", "Miro-321")
            requestDocument.Credentials = credentials
            requestDocument.PreAuthenticate = True

            ' JSON que quieres enviar en el cuerpo de la solicitud
            Dim jsonBody As String = "
        {
           ""UserInfoDelCond"": {
              ""EmployeeNoList"": [
                 {
                    ""employeeNo"": """ & Id_Empleado & """
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
            Dim responsee As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

            ' Leer la respuesta
            Using streamReader As New StreamReader(responsee.GetResponseStream())
                Dim responseJson As String = streamReader.ReadToEnd()

                ' Analizar la respuesta JSON si es necesario
                Dim jsonResponse As JObject = JObject.Parse(responseJson)
                Session("Status_Code_Eliminar") = jsonResponse("statusCode").ToString()
                Response.StatusCode = 200
            End Using
        Catch ex As Exception
        End Try
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
