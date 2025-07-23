Imports System.IO
Imports System.Net
Imports ImageMagick
Imports System.Drawing
Imports System.Net.Mail
Imports System.Net.Security
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Drawing.Imaging
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Control_Contratista_Masivo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session("Usuario") = User.Identity.Name

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
            End If
        End If
    End Sub

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
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub Guardar_Datos()
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        cmd.CommandType = CommandType.Text
        Try
            cmd.Connection = conn_Administracion1
            cmd.CommandText = Session("sql")
            conn_Administracion1.Open()
            cmd.ExecuteNonQuery()
            conn_Administracion1.Close()
            Session("Mensaje") = "Guardado Correctamente"
        Catch ex As Exception
            If ex.ToString.Contains("PRIMARY") Then
            Else
                Try
                    cmd.Connection = conn_Administracion2
                    cmd.CommandText = Session("sql")
                    conn_Administracion2.Open()
                    cmd.ExecuteNonQuery()
                    conn_Administracion2.Close()
                    Session("Mensaje") = "Guardado Correctamente"
                Catch ex1 As Exception
                    ErrorOp = "Private Sub Guardar_Datos:  " + Mid(ex1.ToString, 1, 300) + "<br /><br />" + "Sintaxis Sql: " + Session("sql")
                    conn_Administracion1.Close()
                    conn_Administracion2.Close()
                    EnviarCorreoError()
                    Session("Mensaje") = "!Error al Guardar, Verificar o llamar Administrador del sistema¡"
                End Try
            End If

        End Try

    End Sub

    Private Sub Ejecutar_Query()
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        cmd.CommandType = CommandType.Text
        Try
            cmd.Connection = conn_Administracion1
            cmd.CommandText = Session("sql")
            conn_Administracion1.Open()
            dr = cmd.ExecuteReader
        Catch ex As Exception
            Try
                cmd.Connection = conn_Administracion2
                cmd.CommandText = Session("sql")
                conn_Administracion2.Open()
                dr = cmd.ExecuteReader
            Catch ex1 As Exception
                ErrorOp = "Private Sub Ejecutar_Query: " + Mid(ex1.ToString, 1, 300) + "<br /><br />" + "Sintaxis Sql: " + Session("sql")
                conn_Administracion1.Close()
                conn_Administracion2.Close()
                EnviarCorreoError()
            End Try
        End Try

    End Sub

    Private ErrorOp As String

    Private Sub EnviarCorreoError()
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Error Miseguridad")
            msg.To.Add(New MailAddress("juan.aristizabal@miroseguridad.com"))
            msg.Subject = "Error Miseguridad"
            msg.Body = "Error al usuario: " + User.Identity.Name + "<br />" + "Nombre: " + Session("Nombres") + "<br />" + "Formulario: " + Request.Url.ToString + "<br /><br />" + "Error: " + ErrorOp
            msg.IsBodyHtml = True

            Dim smtp = New SmtpClient
            smtp.Host = SmtpHost
            smtp.Port = 587
            smtp.UseDefaultCredentials = False
            smtp.Credentials = New NetworkCredential(UsuarioCredentials, ContrasenaCredentials)
            smtp.EnableSsl = True

            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            smtp.Send(msg)
        Catch ex As Exception
        End Try
    End Sub

    Private Function ObtenerIdTercero(usuario As String) As Long
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

    Private Function ObtenerIdSede(Sede As String) As Long
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Sede FROM Adm_Sedes WHERE Nombre_Sede = @Sede"
        Dim idSede As Long = 0

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Sede", Sede)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el idSede
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idSede = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el idSede obtenido
        Return idSede
    End Function

    Private Function RegistroExisteEnBD(Cedula As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function Tiene_Facial(Cedula As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    Dim result As Object = command.ExecuteScalar()

                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Return True ' Id_HikCamara tiene datos
                    Else
                        Return False ' Id_HikCamara es NULL o no existe
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ObtenerId_HikCamara(Cedula As String) As Long
        Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
        Dim Id_HikCamara As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Cedula", Cedula)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Tercero
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    Id_HikCamara = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Tercero obtenido
        Return Id_HikCamara
    End Function

    Dim errores As New List(Of String)

    Private Function ParseFecha(fechaTexto As String) As Object
        Dim fechaConvertida As DateTime
        If DateTime.TryParse(fechaTexto, fechaConvertida) Then
            Return fechaConvertida
        Else
            Return DBNull.Value
        End If
    End Function

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Consultar_Id_Control()
        Dim Valor As Integer = ObtenerMaxId("Id_Control_Contratistas", "Adm_Control_Contratistas")
        Session("Id_Control_Contratistas") = Valor + 1
    End Sub

    Private Function ObtenerMaxId(campo As String, tabla As String) As Integer
        Dim Valor As Integer = 0
        Dim query As String = $"SELECT MAX({campo}) FROM {tabla}"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Dim resultado = cmd.ExecuteScalar()
                If resultado IsNot Nothing AndAlso Not IsDBNull(resultado) Then
                    Valor = Convert.ToInt32(resultado)
                End If
            End Using
        End Using

        Return Valor
    End Function

    Private Sub Guardar_Persona(ByVal Cedula As String, ByVal Nombres As String)
        Try
            Dim query As String = "
        INSERT INTO dbo.Terceros 
        (Id_Tercero, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Empresa, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Empresa, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Cedula", If(Cedula Is Nothing, DBNull.Value, Cedula))
                    cmd.Parameters.AddWithValue("@Nombres", Nombres.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", 5)
                    cmd.Parameters.AddWithValue("@Correo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Celular", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Empresa", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Sede", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Guardar_Control(ByVal Cedula As String, ByVal Nombres As String, ByVal Fecha_Inicio As String, ByVal Fecha_Fin As String, ByVal Observaciones As String, ByVal Sede As String, ByVal Fecha_ARL As String, ByVal Fecha_Seguridad_Social As String, ByVal Fecha_Trabajo_Alturas As String)
        Try
            Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(User.Identity.Name)
            Dim Id_Sede As Long = ObtenerIdSede(Sede)
            Dim query As String = "
        INSERT INTO dbo.Adm_Control_Contratistas 
        (Id_Control_Contratistas, Id_Quien_Registra, Cedula_Contratista, Fecha_Inicio, Fecha_Fin, Id_Sede, Observaciones, Fecha_ARL, Fecha_Seguridad_Social, Fecha_Trabajo_Alturas, Estado) 
        VALUES 
        (@Id_Control_Contratistas, @Id_Quien_Registra, @Cedula_Contratista, @Fecha_Inicio, @Fecha_Fin, @Id_Sede, @Observaciones, @Fecha_ARL, @Fecha_Seguridad_Social, @Fecha_Trabajo_Alturas, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Control_Contratistas", Session("Id_Control_Contratistas"))
                    cmd.Parameters.AddWithValue("@Id_Quien_Registra", idTercero_Quien_Registra)
                    cmd.Parameters.AddWithValue("@Cedula_Contratista", If(Cedula Is Nothing, DBNull.Value, Cedula))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", ParseFecha(Fecha_Inicio))
                    cmd.Parameters.AddWithValue("@Fecha_Fin", ParseFecha(Fecha_Fin))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(String.IsNullOrWhiteSpace(Id_Sede), DBNull.Value, Id_Sede))
                    cmd.Parameters.AddWithValue("@Observaciones", If(String.IsNullOrWhiteSpace(Observaciones), DBNull.Value, Observaciones))
                    cmd.Parameters.AddWithValue("@Fecha_ARL", ParseFecha(Fecha_ARL))
                    cmd.Parameters.AddWithValue("@Fecha_Seguridad_Social", ParseFecha(Fecha_Seguridad_Social))
                    cmd.Parameters.AddWithValue("@Fecha_Trabajo_Alturas", ParseFecha(Fecha_Trabajo_Alturas))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Control:" & ex.Message)
        End Try
    End Sub

    Private Sub LvControlContratista_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvControlContratista.ItemDataBound
        Dim Fecha_InicioLabel As Label = DirectCast(e.Item.FindControl("Fecha_InicioLabel"), Label)
        Fecha_InicioLabel.Text = Mid(Fecha_InicioLabel.Text.ToString, 1, 10)
        Dim Fecha_FinLabel As Label = DirectCast(e.Item.FindControl("Fecha_FinLabel"), Label)
        Fecha_FinLabel.Text = Mid(Fecha_FinLabel.Text.ToString, 1, 10)
    End Sub

    Private Sub LvControlContratista_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvControlContratista.ItemCommand
        Dim Id_Control_ContratistasLabel As Label = DirectCast(e.Item.FindControl("Id_Control_ContratistasLabel"), Label)
        Dim Cedula_ContratistaLabel As Label = DirectCast(e.Item.FindControl("Cedula_ContratistaLabel"), Label)
        Dim Id_HikCamaraLabel As Label = DirectCast(e.Item.FindControl("Id_HikCamaraLabel"), Label)
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If FlArchivo.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FlArchivo.FileName)
            FlArchivo.SaveAs(Server.MapPath("..\..\Adjunto\Temporal\Excel" & Ext))
            Dim filePath As String = Server.MapPath("..\..\Adjunto\Temporal\Excel" & Ext)

            ' Establecer el contexto de la licencia de EPPlus
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial

            ' Abrir el archivo Excel usando EPPlus dentro de un bloque Using
            Using package As New OfficeOpenXml.ExcelPackage(New FileInfo(filePath))
                Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets("Hoja1")

                Dim startRow As Integer = worksheet.Dimension.Start.Row
                Dim endRow As Integer = worksheet.Dimension.End.Row

                ' Recorre las filas, asumiendo que la primera es el encabezado
                For row As Integer = startRow + 1 To endRow

                    If Not RegistroExisteEnBD(worksheet.Cells(row, 1).Text.Trim()) Then
                        Consultar_Id_Persona()
                        Guardar_Persona(worksheet.Cells(row, 1).Text, worksheet.Cells(row, 2).Text)
                    End If
                    Consultar_Id_Control()
                    Guardar_Control(worksheet.Cells(row, 1).Text, worksheet.Cells(row, 2).Text, worksheet.Cells(row, 3).Text, worksheet.Cells(row, 4).Text, worksheet.Cells(row, 5).Text, worksheet.Cells(row, 6).Text, worksheet.Cells(row, 7).Text, worksheet.Cells(row, 8).Text, worksheet.Cells(row, 9).Text)

                Next
            End Using

            File.Delete(filePath)

            If errores.Count = 0 Then
                Dim script As String = String.Format("swal('Excelente!', 'Contratistas Registrados Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_Control_Contratista_Masivo.aspx")
            Else
                ' Une los errores en un texto con saltos de línea
                Dim erroresTexto As String = String.Join("<br>", errores)
                Dim script As String = $"swal('OJO!', 'Fallaron los siguientes usuarios :<br>{erroresTexto}', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            Dim script As String = String.Format("swal('OJO!', 'Debe agregar un archivo', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

End Class