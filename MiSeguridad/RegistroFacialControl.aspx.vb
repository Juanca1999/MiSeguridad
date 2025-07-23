Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class RegistroFacialControl
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
                            Dim cantidadEntradasAntes As Long = ObtenerCantidadVeces(eventObject.AccessControllerEvent.employeeNoString)

                            If Not ExisteRegistroRecienteEntrada(eventObject.AccessControllerEvent.employeeNoString) Then
                                Registrar_Entrada(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                            End If

                            Dim cantidadEntradasDespues As Long = ObtenerCantidadVeces(eventObject.AccessControllerEvent.employeeNoString)

                            If cantidadEntradasDespues >= 2 Then
                                Editar_Acceso_Puerta(eventObject.AccessControllerEvent.employeeNoString)
                            End If
                        End If

                    ElseIf eventObject.AccessControllerEvent IsNot Nothing AndAlso eventObject.AccessControllerEvent.majorEventType = 5 AndAlso eventObject.AccessControllerEvent.subEventType = 1 Then

                        Dim idTercero_QR As Long = ObtenerIdTerceroQR(Convert.ToInt64(eventObject.AccessControllerEvent.employeeNoString))

                        If idTercero_QR = 0 Then
                            RegistrarLogNoEncontrado(eventObject.AccessControllerEvent.employeeNoString)
                        Else
                            Dim cantidadEntradas As Long = ObtenerCantidadVecesQR(eventObject.AccessControllerEvent.employeeNoString) + 1
                            Dim cantidadEntradasControl As Long = ObtenerCantidadVecesControl(eventObject.AccessControllerEvent.employeeNoString)

                            If cantidadEntradasControl <> 0 Then
                                If Not ExisteRegistroRecienteEntradaQR(eventObject.AccessControllerEvent.employeeNoString) Then
                                    Registrar_Entrada_QR(eventObject.AccessControllerEvent.employeeNoString, eventObject.ipAddress)
                                End If

                                If cantidadEntradas >= cantidadEntradasControl Then
                                    Editar_Acceso_Puerta(eventObject.AccessControllerEvent.employeeNoString)
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
            Response.Write("<h1>Estado de Registro Facial Control Restaurante</h1>")
            Response.Write("<p>" & ultimoEvento & "</p>")
            Response.Write("<p>Esperando eventos mediante solicitudes POST...</p>")
        Else
            Response.StatusCode = 405 ' Método no permitido
            Response.Write("Solo se permiten solicitudes POST.")
        End If
    End Sub

    Private Function ExisteRegistroRecienteEntrada(employeeNo As String) As Boolean
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Nueva consulta más segura y robusta
                Dim query As String = "
                SELECT COUNT(*) 
                FROM Adm_Visita 
                WHERE Id_Acceso_Entrada IN (1,2) AND Id_Quien_Ingresa = @Id_Quien_Ingresa 
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Inicio_Visita, ' ', Hora_Inicio_Visita)) IS NOT NULL
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Inicio_Visita, ' ', Hora_Inicio_Visita)) >= 
                    DATEADD(SECOND, -ISNULL((SELECT TOP 1 Segundos_Facial FROM Adm_Empresa), 1800), GETDATE())"

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

    Private Function ExisteRegistroRecienteEntradaQR(employeeNo As String) As Boolean
        Dim idTercero_Visitante As Long = ObtenerIdTerceroQR(Convert.ToInt64(employeeNo))

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Nueva consulta más segura y robusta
                Dim query As String = "
                SELECT COUNT(*) 
                FROM Adm_Visita 
                WHERE Id_Acceso_Entrada IN (1,2) AND Quien_Autoriza = 'QR' AND Id_Quien_Ingresa = @Id_Quien_Ingresa 
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Inicio_Visita, ' ', Hora_Inicio_Visita)) IS NOT NULL
                AND TRY_CONVERT(DATETIME, CONCAT(Fecha_Inicio_Visita, ' ', Hora_Inicio_Visita)) >= 
                    DATEADD(SECOND, -120, GETDATE())"

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

    Private Function ObtenerIdTerceroQR(usuario As String) As Long
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT TOP 1 T.Id_Tercero FROM Adm_Control_Restaurante CR INNER JOIN Terceros T ON T.Cedula = CR.Cedula WHERE CR.Id_HikCamara = @Usuario"
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

    Private Function ObtenerCantidadVeces(employeeNo As String) As Long
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(employeeNo))
        Dim Cantidad As Long = 0

        ' Cadena SQL para obtener el último Id_Visita
        Dim sql As String = "SELECT COUNT(Id_Visita) AS Cantidad FROM Adm_Visita WHERE Id_Acceso_Entrada IN (1,2) AND Id_Quien_Ingresa = @Id_Quien_Ingresa AND Fecha_Inicio_Visita = CAST(GETDATE() AS DATE)"

        ' Conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Crear el comando SQL
            Using cmd As New SqlCommand(sql, conn)
                ' Agregar parámetro para Id_Quien_Ingresa
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Cantidad = Convert.ToInt64(reader("Cantidad"))
                    End If
                End Using
            End Using
        End Using

        Return Cantidad
    End Function

    Private Function ObtenerCantidadVecesQR(employeeNo As String) As Long
        Dim idTercero_Visitante As Long = ObtenerIdTerceroQR(Convert.ToInt64(employeeNo))
        Dim Cantidad As Long = 0

        ' Cadena SQL para obtener el último Id_Visita
        Dim sql As String = "SELECT COUNT(Id_Visita) AS Cantidad FROM Adm_Visita WHERE Id_Acceso_Entrada IN (1,2) AND Id_Quien_Ingresa = @Id_Quien_Ingresa AND Fecha_Inicio_Visita = CAST(GETDATE() AS DATE) AND Quien_Autoriza = 'QR'"

        ' Conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Crear el comando SQL
            Using cmd As New SqlCommand(sql, conn)
                ' Agregar parámetro para Id_Quien_Ingresa
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Cantidad = Convert.ToInt64(reader("Cantidad"))
                    End If
                End Using
            End Using
        End Using

        Return Cantidad
    End Function

    Private Function ObtenerCantidadVecesControl(employeeNo As String) As Long
        Dim CantidadControl As Long = 0

        ' Cadena SQL para obtener el último Id_Visita
        Dim sql As String = "SELECT Cantidad_Accesos FROM Adm_Control_Restaurante WHERE Id_HikCamara = @Id_HikCamara AND Fecha_Activacion = CAST(GETDATE() AS DATE)"

        ' Conexión a la base de datos
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Crear el comando SQL
            Using cmd As New SqlCommand(sql, conn)
                ' Agregar parámetro para Id_Quien_Ingresa
                cmd.Parameters.AddWithValue("@Id_HikCamara", employeeNo)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        CantidadControl = Convert.ToInt64(reader("Cantidad_Accesos"))
                    End If
                End Using
            End Using
        End Using

        Return CantidadControl
    End Function

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Inmueble = 'COMEDOR'")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
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
                    End Try
                    Return dt
                End Using
            End Using
        End Using
    End Function

    Private Sub Editar_Acceso_Puerta(ByVal Id As String)
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try

                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Id & """,
              ""doorRight"": """",
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
                    Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                End Using
            Catch ex As Exception
                Session("Status_Code_Editar") = 0
            End Try
        Next
    End Sub

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

    Private Sub Registrar_Entrada_QR(employeeNo As String, Ip_Camara As String)
        Dim idTercero_Visitante As Long = ObtenerIdTerceroQR(Convert.ToInt64(employeeNo))
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

                        insertCmd.Parameters.AddWithValue("@Quien_Autoriza", "QR")
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
