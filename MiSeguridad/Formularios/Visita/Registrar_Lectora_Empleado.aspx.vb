Imports System.Data.SqlClient

Public Class Registrar_Lectora_Empleado
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

    Private Function ObtenerIdTercero(usuario As Long) As Long
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
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                            Session("Acceso_Usuario") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Protected Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then

            Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(TxBuscarPersona.Text))

            If idTercero_Visitante <> 0 Then
                Dim idVisita As Long = ObtenerUltimaVisitaEmpleado(idTercero_Visitante)

                If idVisita = 0 Then
                    Try
                        Registrar_Entrada(TxBuscarPersona.Text)
                        Registrar_Log_Entrada(TxBuscarPersona.Text)
                        Dim script As String = String.Format("swal('Excelente!', 'Entrada Registrada Correctamente', 'success');")
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        TxBuscarPersona.Text = ""
                    Catch ex As Exception
                        Dim script As String = String.Format("swal('Error!', 'Error al registrar entrada, verificar datos', 'warning');")
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    End Try
                Else
                    Try
                        Registrar_Salida(TxBuscarPersona.Text)
                        Registrar_Log_Salida(TxBuscarPersona.Text)
                        Dim script As String = String.Format("swal('Excelente!', 'Salida Registrada Correctamente', 'success');")
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        TxBuscarPersona.Text = ""
                    Catch ex As Exception
                        Dim script As String = String.Format("swal('Error!', 'Error al registrar salida, verificar datos', 'warning');")
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    End Try
                End If
            Else
                Dim script As String = String.Format("swal('OJO!', 'Persona no encontrada, verificar cedula', 'warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If

        End If
    End Sub

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
                                AND Id_Acceso_Entrada = 2
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
                                     AND Id_Acceso_Entrada = 2
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

    Private Sub Registrar_Entrada(Cedula As Long)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Cedula))
        Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(Convert.ToInt64(User.Identity.Name))

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
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Registra", idTercero_Quien_Registra)
                        insertCmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                        insertCmd.Parameters.AddWithValue("@Id_Inmueble", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Quien_Autoriza", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Fecha_Inicio_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        insertCmd.Parameters.AddWithValue("@Hora_Inicio_Visita", Date.Now.ToString("HH:mm:ss"))
                        insertCmd.Parameters.AddWithValue("@Placa", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Observacion", DBNull.Value)
                        insertCmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))
                        insertCmd.Parameters.AddWithValue("@Id_Acceso_Entrada", 2)
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

    Private Sub Registrar_Log_Entrada(Cedula As Long)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Cedula))

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
                        insertCmd.Parameters.AddWithValue("@Id_Acceso", 1)

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Private Sub Registrar_Salida(Cedula As Long)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Cedula))
        Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(Convert.ToInt64(User.Identity.Name))

        Dim idVisita As Long = ObtenerUltimaVisitaEmpleado(idTercero_Visitante)

        If idVisita <> 0 Then
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
                    cmd.Parameters.AddWithValue("@Id_Acceso_Salida", 2)
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

    Private Sub Registrar_Log_Salida(Cedula As Long)
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(Cedula))

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
                        insertCmd.Parameters.AddWithValue("@Id_Acceso", 1)

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

End Class