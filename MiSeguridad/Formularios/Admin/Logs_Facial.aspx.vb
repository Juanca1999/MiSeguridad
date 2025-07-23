Imports System.Data.SqlClient
Imports System.Web.Services

Public Class Logs_Facial
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then

        End If
    End Sub

    <WebMethod>
    Public Shared Function ObtenerUltimos10Logs() As List(Of Log)
        Dim logs As New List(Of Log)()
        Dim query As String = "
        SELECT TOP 10 L.Id_Log, L.Id_Tercero, T.Nombres, L.Fecha_Hora_Evento, 
               CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso 
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
               A.Nombre_Acceso 
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
                    End If
                End Using
            End Using
        End Using

        Return log
    End Function

End Class

Public Class Log
    Public Property Id_Log As String
    Public Property Id_Tercero As String
    Public Property Nombres As String
    Public Property Fecha_Hora_Evento As String
    Public Property Tipo_Evento As String
    Public Property Nombre_Acceso As String
    Public Property Id_HikCamara As String
    Public Property FotoUrl As String
    Public Property Registrado As Boolean
End Class

