Imports System.IO
Imports System.Data.SqlClient

Public Class RegistrarPlacasTEK
    Inherits System.Web.UI.Page

    ' Variable para almacenar el último evento recibido
    Private Shared ultimoEvento As String = "No se han recibido eventos aún."

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        GenerarCSVPlacas()
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

    Private Function ObtenerUltimaVisitaEmpleado(Placa As String, Ip_Camara As String) As Long
        Dim idVisita As Long = 0
        Dim Id_Inmueble As String = Identificar_Area_Facial(Ip_Camara)

        ' Crear conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()

            ' Consulta para buscar en el día actual
            Dim sqlHoy As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita 
                                WHERE Placa = @Placa 
                                AND CAST(Fecha_Inicio_Visita AS DATE) = CAST(GETDATE() AS DATE) 
                                AND Fecha_Fin_Visita IS NULL 
                                AND Id_Acceso_Entrada = (SELECT TOP 1 Id_Acceso FROM Adm_Accesos WHERE Tipo_Evento = 1 AND Id_Inmueble = @Id_Inmueble)
                                ORDER BY Id_Visita DESC"

            Using cmdHoy As New SqlCommand(sqlHoy, conn)
                cmdHoy.Parameters.AddWithValue("@Placa", Placa)
                cmdHoy.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)

                Using reader As SqlDataReader = cmdHoy.ExecuteReader()
                    If reader.Read() Then
                        idVisita = Convert.ToInt64(reader("Id_Visita"))
                    End If
                End Using
            End Using

            ' Si no se encuentra en el día actual, buscar en el día anterior
            If idVisita = 0 Then
                Dim sqlAyer As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita 
                                     WHERE Placa = @Placa 
                                     AND CAST(Fecha_Inicio_Visita AS DATE) = CAST(DATEADD(DAY, -1, GETDATE()) AS DATE) 
                                     AND Fecha_Fin_Visita IS NULL
                                     AND Id_Acceso_Entrada = (SELECT TOP 1 Id_Acceso FROM Adm_Accesos WHERE Tipo_Evento = 1 AND Id_Inmueble = @Id_Inmueble)
                                     ORDER BY Id_Visita DESC"

                Using cmdAyer As New SqlCommand(sqlAyer, conn)
                    cmdAyer.Parameters.AddWithValue("@Placa", Placa)
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

    Public Sub GenerarCSVPlacas()
        ' Ruta donde se generará el archivo CSV (debe tener permisos de escritura y estar compartida en el FTP)
        Dim rutaArchivo As String = "C:\Users\tecnico\OneDrive - MIRO SEGURIDAD\Documentos\MiSeguridad\MiSeguridad\bin\Release\Publish\Adjunto\Agregar_Placa/placas.csv"

        Try
            Using conexion As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conexion.Open()

                Dim query As String = "SELECT V.Placa, T.Tipo_Vehiculo, V.Modelo, V.Color FROM Adm_Vehiculo V LEFT JOIN Adm_Tipo_Vehiculo T ON T.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo"
                Dim comando As New SqlCommand(query, conexion)
                Dim reader As SqlDataReader = comando.ExecuteReader()

                Using writer As New StreamWriter(rutaArchivo, False, System.Text.Encoding.UTF8)
                    ' Cabeceras
                    writer.WriteLine("nllist-id;description;color;levenshteindist;;;")
                    writer.WriteLine("2;ALLOWLIST;#000000;0;;;")
                    writer.WriteLine("nlelemlist-id;numberplate;listid;timestamp;description;startvaliditydate;endvaliditydate")

                    ' Generar filas
                    Dim fechaHoraActual As String = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
                    Dim inicioValidez As String = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
                    Dim finValidez As String = "2030-12-31T23:59:59.999"

                    While reader.Read()
                        Dim placa As String = reader("Placa").ToString().Trim()
                        Dim tipo As String = If(IsDBNull(reader("Tipo_Vehiculo")), "", reader("Tipo_Vehiculo").ToString().Trim())
                        Dim modelo As String = If(IsDBNull(reader("Modelo")), "", reader("Modelo").ToString().Trim())
                        Dim color As String = If(IsDBNull(reader("Color")), "", reader("Color").ToString().Trim())

                        Dim descripcion As String = $"Tipo vehiculo: {tipo}, Modelo: {modelo}, Color: {color}"

                        writer.WriteLine($"2;{placa};2;{fechaHoraActual};{descripcion};{inicioValidez};{finValidez}")
                    End While
                End Using
            End Using
        Catch ex As Exception
            ' Manejo de errores
            ' Puedes registrar el error o mostrarlo
            Throw New Exception("Error al generar el archivo CSV: " & ex.Message)
        End Try
    End Sub

    Private Function ValidarSiEsDeConfianza(Placa As String) As Boolean
        Try
            ' Conexión a la base de datos
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT Placa FROM Adm_Vehiculo WHERE Placa = @Placa"
                Using command As New SqlCommand(query, connection)
                    ' Parámetro para la IP de la cámara
                    command.Parameters.AddWithValue("@Placa", Placa)

                    Dim result As Object = command.ExecuteScalar()

                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return True
                    Else
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try

        Return False
    End Function

    Private Function ExisteRegistroReciente(placa As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Consulta SQL para verificar si existe un registro en los últimos 60 segundos
                Dim query As String = "SELECT COUNT(*) FROM Adm_Visita WHERE Placa = @Placa AND CONVERT(DATETIME, CONVERT(VARCHAR(10), Fecha_Inicio_Visita, 120) + ' ' + CONVERT(VARCHAR(8), Hora_Inicio_Visita, 108)) >= DATEADD(SECOND, -60, GETDATE())"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Placa", placa)

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

    Private Sub Registrar_Entrada(Placa As String, Ip_Camara As String)
        'Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Placa))
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
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Ingresa", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Inmueble", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)

                        insertCmd.Parameters.AddWithValue("@Quien_Autoriza", "LPR")
                        insertCmd.Parameters.AddWithValue("@Fecha_Inicio_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        insertCmd.Parameters.AddWithValue("@Hora_Inicio_Visita", Date.Now.ToString("HH:mm:ss"))
                        insertCmd.Parameters.AddWithValue("@Placa", Placa)
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

    Private Sub Registrar_Log_Entrada(Placa As String, Ip_Camara As String)
        'Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Placa))
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

        Dim sql As String = "INSERT INTO dbo.Logs_Vehicular (" &
                         "Id_Log_Vehicular, Placa, Fecha_Hora_Evento, Tipo_Evento, Id_Acceso) VALUES (" &
                         "@Id_Log_Vehicular, @Placa, @Fecha_Hora_Evento, @Tipo_Evento, @Id_Acceso)"

        ' Conexión y comandos SQL
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Abrir la conexión
            conn.Open()

            ' Iniciar una transacción para asegurar la consistencia y el bloqueo exclusivo
            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Log_Vehicular), 0) + 1 FROM dbo.Logs_Vehicular WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    ' Ejecutar la consulta para obtener el nuevo Id_Visita
                    Dim newIdLog As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    ' Ahora que tenemos el nuevo Id_Visita, procedemos con la inserción
                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Log_Vehicular", newIdLog)
                        insertCmd.Parameters.AddWithValue("@Placa", Placa)
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

    Private Sub Registrar_Salida_Sin_Entrada(Placa As String, Ip_Camara As String)
        'Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Placa))
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
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Ingresa", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Inmueble", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)

                        insertCmd.Parameters.AddWithValue("@Quien_Autoriza", "LPR")
                        insertCmd.Parameters.AddWithValue("@Fecha_Fin_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        insertCmd.Parameters.AddWithValue("@Hora_fin_Visita", Date.Now.ToString("HH:mm:ss"))
                        insertCmd.Parameters.AddWithValue("@Placa", Placa)
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

    Private Sub Registrar_Salida(Placa As String, Ip_Camara As String)
        'Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Placa))
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

        Dim idVisita As Long = ObtenerUltimaVisitaEmpleado(Placa, Ip_Camara)

        If idVisita = 0 Then
            Registrar_Salida_Sin_Entrada(Placa, Ip_Camara)
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
                    cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", DBNull.Value)
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

    Private Sub Registrar_Log_Salida(Placa As String, Ip_Camara As String)
        'Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim idAcceso As Long = ObtenerAcceso(Ip_Camara)

        Dim sql As String = "INSERT INTO dbo.Logs_Vehicular (" &
                         "Id_Log_Vehicular, Placa, Fecha_Hora_Evento, Tipo_Evento, Id_Acceso) VALUES (" &
                         "@Id_Log_Vehicular, @Placa, @Fecha_Hora_Evento, @Tipo_Evento, @Id_Acceso)"

        ' Conexión y comandos SQL
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Abrir la conexión
            conn.Open()

            ' Iniciar una transacción para asegurar la consistencia y el bloqueo exclusivo
            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Log_Vehicular), 0) + 1 FROM dbo.Logs_Vehicular WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    ' Ejecutar la consulta para obtener el nuevo Id_Visita
                    Dim newIdLog As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    ' Ahora que tenemos el nuevo Id_Visita, procedemos con la inserción
                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Log_Vehicular", newIdLog)
                        insertCmd.Parameters.AddWithValue("@Placa", Placa)
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

    ' Función simple para extraer valores usando Regex
    Private Function ExtractValue(ByVal input As String, ByVal key As String) As String
        Dim pattern As String = $"{key}:\s*([\w\d\-]+)"
        Dim match As Match = Regex.Match(input, pattern)

        If match.Success Then
            Return match.Groups(1).Value
        End If

        Return String.Empty
    End Function

    ' Clase para deserializar el JSON del evento
    Public Class EventData
        Public Property Placa As String
        Public Property Id_Camara As Integer
    End Class

End Class