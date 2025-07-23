Imports System.IO
Imports System.Data.SqlClient

Module Funciones

    Public conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
    Public ConSqlMiSeguridad As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())

    Public conn_Administracion1 As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
    Public conn_Administracion2 As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())

    Public cmd As New SqlCommand
    Public cmdPuntos As New SqlCommand
    Public cmdCorrespondencia As New SqlCommand
    Public Adaptador As New SqlDataAdapter
    Public dr As SqlDataReader
    Public dr2 As SqlDataReader
    Public sql As String = ""
    Public dt As New DataSet

    'CONFIGURACION CORREO SMTP
    Public CorreoFrom As String = LeerCorreoDesdeArchivo("CorreoFrom")
    Public SmtpHost As String = LeerCorreoDesdeArchivo("SmtpHost")
    Public UsuarioCredentials As String = LeerCorreoDesdeArchivo("UsuarioCredentials")
    Public ContrasenaCredentials As String = LeerCorreoDesdeArchivo("ContrasenaCredentials")
    Public Port As String = LeerCorreoDesdeArchivo("Port")

    Private Function LeerCorreoDesdeArchivo(clave As String) As String
        Dim ruta As String = HttpContext.Current.Server.MapPath("~/Diseno/Correo_SMTP.txt")

        If File.Exists(ruta) Then
            Dim lineas = File.ReadAllLines(ruta)
            For Each linea In lineas
                If linea.Trim().StartsWith(clave & "=", StringComparison.OrdinalIgnoreCase) Then
                    Dim idx As Integer = linea.IndexOf("=")
                    If idx > -1 AndAlso idx < linea.Length - 1 Then
                        Return linea.Substring(idx + 1).Trim()
                    End If
                End If
            Next
        End If

        Return ""
    End Function

End Module