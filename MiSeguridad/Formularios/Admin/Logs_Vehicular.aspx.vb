Imports System.Data.SqlClient
Imports System.Web.Services

Public Class Logs_Vehicular
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then

        End If
    End Sub

    <WebMethod>
    Public Shared Function ObtenerUltimos14Logs() As List(Of Log_Vehicular)
        Dim logs As New List(Of Log_Vehicular)()

        ' Verifica si la sesión contiene la escuela
        If HttpContext.Current.Session("EscuelaSeleccionada") Is Nothing OrElse String.IsNullOrWhiteSpace(HttpContext.Current.Session("EscuelaSeleccionada").ToString()) Then
            Return logs ' Retorna lista vacía y evita el error
        End If

        Dim query As String = "
        SELECT TOP 14 L.Id_Log_Vehicular, L.Placa, T.Nombres AS Nombre_Asignado, L.Fecha_Hora_Evento, 
               CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso, T.Grado  
        FROM Logs_Vehicular L 
        LEFT JOIN Adm_Vehiculo V ON V.Placa = L.Placa 
        LEFT JOIN Adm_Vehiculo_Asignado_A VA ON VA.Placa = L.Placa
        LEFT JOIN Terceros T ON T.Id_Tercero = VA.Id_Tercero_Estudiante
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso 
        WHERE L.Tipo_Evento = 1 AND T.Id_Escuela = @IdEscuela 
        ORDER BY L.Id_Log_Vehicular DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IdEscuela", HttpContext.Current.Session("EscuelaSeleccionada"))
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim log As New Log_Vehicular()
                        log.Id_Log_Vehicular = reader("Id_Log_Vehicular").ToString()
                        log.Placa = reader("Placa").ToString()
                        log.Nombre_Asignado = reader("Nombre_Asignado").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.Grado = reader("Grado").ToString()
                        logs.Add(log)
                    End While
                End Using
            End Using
        End Using

        Return logs
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimosLogs() As List(Of Log_Vehicular)
        Dim logs As New List(Of Log_Vehicular)()

        ' Verifica si la sesión contiene la escuela
        If HttpContext.Current.Session("EscuelaSeleccionada") Is Nothing OrElse String.IsNullOrWhiteSpace(HttpContext.Current.Session("EscuelaSeleccionada").ToString()) Then
            Return logs ' Retorna lista vacía y evita el error
        End If

        Dim query As String = "
    SELECT TOP 14 L.Id_Log_Vehicular, L.Placa, T.Nombres AS Nombre_Asignado, L.Fecha_Hora_Evento, 
           CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
           A.Nombre_Acceso, T.Grado  
    FROM Logs_Vehicular L 
    LEFT JOIN Adm_Vehiculo V ON V.Placa = L.Placa 
    LEFT JOIN Adm_Vehiculo_Asignado_A VA ON VA.Placa = L.Placa 
    LEFT JOIN Terceros T ON T.Id_Tercero = VA.Id_Tercero_Estudiante
    LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso 
    WHERE L.Tipo_Evento = 1 AND T.Id_Escuela = @IdEscuela 
    ORDER BY L.Id_Log_Vehicular DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IdEscuela", HttpContext.Current.Session("EscuelaSeleccionada"))
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim log As New Log_Vehicular()
                        log.Id_Log_Vehicular = reader("Id_Log_Vehicular").ToString()
                        log.Placa = reader("Placa").ToString()
                        log.Nombre_Asignado = reader("Nombre_Asignado").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.Grado = reader("Grado").ToString()
                        logs.Add(log)
                    End While
                End Using
            End Using
        End Using

        Return logs
    End Function

    Protected Sub btnContinuar_Click(sender As Object, e As EventArgs) Handles btnContinuar.Click
        PanelContenido.Visible = True
        Session("EscuelaSeleccionada") = TxEscuela.SelectedValue
        PanelSeleccion.Visible = False
        EscuelaLabel.Text = TxEscuela.SelectedItem.Text.ToUpper()
    End Sub

End Class

Public Class Log_Vehicular
    Public Property Id_Log_Vehicular As String
    Public Property Placa As String
    Public Property Nombre_Asignado As String
    Public Property Fecha_Hora_Evento As String
    Public Property Tipo_Evento As String
    Public Property Nombre_Acceso As String
    Public Property Grado As String
End Class

