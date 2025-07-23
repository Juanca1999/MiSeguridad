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

Public Class Asignar_Tag
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name

        If Not IsPostBack Then

            Consultar_Info_Usuario()
            ' Cargar el código de tarjeta si ya existe
            Dim ruta As String = Server.MapPath("../../Adjunto/ultimo_codigo.txt")
            If File.Exists(ruta) Then
                TxCodigo.Text = File.ReadAllText(ruta).Trim()
            End If

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

    Private Sub BtCrearVehiculo_ServerClick(sender As Object, e As EventArgs) Handles BtCrearVehiculo.ServerClick
        Response.Redirect("crear_Vehiculo_Camara.aspx")
    End Sub

    Private Sub BtAsignarTag_ServerClick(sender As Object, e As EventArgs) Handles BtAsignarTag.ServerClick
        Response.Redirect("Asignar_Tag.aspx")
    End Sub

    Dim errores As New List(Of String)

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

    Private Function RegistroExisteEnBD(Placa As String) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Adm_Vehiculo WHERE Placa = @Placa"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Placa", Placa)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    If count > 0 Then
                        Return True ' El Registro existe en la base de datos
                    Else
                        Return False ' La Registro no existe en la base de datos
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False ' Ocurrió un error al verificar la existencia de la carpeta
        End Try
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
                        errores.Add("Error al ejecutar la consulta SQL:" & ex.Message)
                    End Try
                    Return dt
                End Using
            End Using
        End Using
    End Function

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 2")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras:" & ex.Message)
        End Try
        Return ipCamaras
    End Function

    Private Sub LvVehiculos_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvVehiculos.ItemDataBound
    End Sub

    Private Sub LvVehiculos_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvVehiculos.ItemCommand
        Dim PlacaLabel As Label = DirectCast(e.Item.FindControl("PlacaLabel"), Label)
        Dim Id_InmuebleLabel As Label = DirectCast(e.Item.FindControl("Id_InmuebleLabel"), Label)
        Dim Id_TerceroLabel As Label = DirectCast(e.Item.FindControl("Id_TerceroLabel"), Label)
        Session("Placa_Anterior") = PlacaLabel.Text
        Session("Id_Inmueble_Anterior") = Id_InmuebleLabel.Text
        Session("Id_Tercero_Anterior") = Id_TerceroLabel.Text

        If e.CommandName = "Asignar" Then
            PAsignar.Visible = True
            TxPlaca.Text = PlacaLabel.Text
            Dim ruta As String = "C:\Soyal\ultimo_codigo.txt"
            If File.Exists(ruta) Then
                Dim lineas() As String = File.ReadAllLines(ruta)
                If lineas.Length >= 4 Then
                    ' Opcional: mostrar todos los códigos si tienes controles extra
                    ' txtHex1.Text = lineas(0)
                    ' txtAscii1.Text = lineas(1)
                    ' txtHex2.Text = lineas(2)
                    ' txtAscii2.Text = lineas(3)
                    TxCodigo.Text = lineas(3) ' Este es el segundo ASCII, el más reciente
                End If
            End If
        End If
    End Sub

    Private Sub Guardar_Tag_Placa()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Placa_Tag 
        (Placa, Codigo_Tag) 
        VALUES 
        (@Placa, @Codigo_Tag)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())
                    cmd.Parameters.AddWithValue("@Codigo_Tag", TxCodigo.Text.Trim())

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Tag_Placa():" & ex.Message)
        End Try
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Guardar_Tag_Placa()

        If errores.Count = 0 Then
            Dim script As String = String.Format("swal('Excelente!', Ttag A Placa Guardado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Asignar_Tag.aspx")
        Else
            Dim mensaje As String = String.Join("<br/>", errores)
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensaje)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", Script, True)
            Exit Sub
        End If
    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            LvVehiculos.DataSourceID = "SqlVehiculos"
        Else
            LvVehiculos.DataSourceID = "Sql_Buscar_Placa"
        End If
        LvVehiculos.DataBind()
    End Sub

End Class