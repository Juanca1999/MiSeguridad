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
    Public CorreoFrom As String = "barbershopdestiny37@gmail.com"
    Public SmtpHost As String = "smtp.gmail.com"
    Public UsuarioCredentials As String = "barbershopdestiny37@gmail.com"
    Public ContrasenaCredentials As String = "ybcr aopv jtlf zddm"
    Public Port As String = 587

End Module