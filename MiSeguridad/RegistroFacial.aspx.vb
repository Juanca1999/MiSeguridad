Imports System.IO
Imports System.Data.SqlClient
Imports Newtonsoft.Json

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

                        If EsEntrada = True Then
                            Registrar_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                        Else
                            Registrar_Salida(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
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

    Private Function ObtenerUltimaVisitaEmpleado(idTercero_Visitante As Long) As Long
        Dim idVisita As Long = 0

        ' Cadena SQL para obtener el último Id_Visita
        Dim sql As String = "SELECT TOP 1 Id_Visita FROM Adm_Visita WHERE Id_Quien_Ingresa = @Id_Quien_Ingresa AND Fecha_Fin_Visita IS NULL ORDER BY Id_Visita DESC"

        ' Conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Crear el comando SQL
            Using cmd As New SqlCommand(sql, conn)
                ' Agregar parámetro para Id_Quien_Ingresa
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        idVisita = Convert.ToInt64(reader("Id_Visita"))
                    End If
                End Using
            End Using
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
                        insertCmd.Parameters.AddWithValue("@Manual", 0)
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
