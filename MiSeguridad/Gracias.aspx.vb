Imports System.Data.SqlClient

Public Class Gracias
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            EstablecerLogo()
        End If
    End Sub

    Private Sub EstablecerLogo()
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener la ruta del logo
        Dim query As String = "SELECT TOP 1 Logo FROM Adm_Empresa"

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener la ruta de la imagen
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing Then
                    Logo.Src = "/../../../Adjunto/Logo/" + result.ToString()
                Else
                    Logo.Src = "/../../../Adjunto/Logo/miro.png"
                End If
            End Using
        End Using
    End Sub

End Class