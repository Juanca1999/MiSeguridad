Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Usuario_Equipo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            Session("datos1") = Nothing

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

    Private errores As New List(Of String)

    Private Sub EnviarCorreoError()
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Error Miseguridad")
            msg.To.Add(New MailAddress("juan.aristizabal@miroseguridad.com"))
            msg.Subject = "Error Miseguridad"
            msg.Body = "Error al usuario: " + User.Identity.Name + "<br />" + "Nombre: " + Session("Nombres") + "<br />" + "Formulario: " + Request.Url.ToString + "<br /><br />"
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

    Private Sub Buscar_Persona_Cedula()
        Try
            Dim sqlQuery As String = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Id_Sede = @Sede AND Estado = 1 AND Cedula = @Cedula"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Cedula", TxBuscarCedula.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxPersona.DataSource = datos
                            TxPersona.DataTextField = "Nombres"
                            TxPersona.DataValueField = "Id_Tercero"
                            TxPersona.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró la persona con la cedula ingresada, verificar y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub Buscar_Persona_Nombre()
        Try
            Dim sqlQuery As String = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Id_Sede = @Sede AND Estado = 1 AND Nombres LIKE '%' + @Nombre + '%' ORDER BY Nombres ASC"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Nombre", TxBuscarNombre.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxPersona.DataSource = datos
                            TxPersona.DataTextField = "Nombres"
                            TxPersona.DataValueField = "Id_Tercero"
                            TxPersona.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró la persona con el nombre ingresado, verificar y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarCedula_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedula.TextChanged
        If TxBuscarCedula.Text <> "" Then
            Buscar_Persona_Cedula()
        Else
            TxPersona.Items.Clear()
        End If
    End Sub

    Private Sub TxBuscarNombre_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarNombre.TextChanged
        If TxBuscarNombre.Text <> "" Then
            Buscar_Persona_Nombre()
        Else
            TxPersona.Items.Clear()
        End If
    End Sub

    Private Function Consultar_ID_Tercero(ByVal Cedula As String) As Integer
        Try
            ' Definir la cadena de conexión
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Definir la consulta
            Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Cedula"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Cedula", Cedula)

                    ' Abrir la conexión
                    conn.Open()

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = cmd.ExecuteScalar()

                    ' Verificar si el resultado es válido
                    If result IsNot Nothing Then
                        Return Convert.ToInt32(result)
                    Else
                        ' Devolver 0 o algún valor por defecto si no se encuentra el registro
                        Return 0
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Lanzar la excepción
            Throw ex
        End Try
    End Function

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Guardar_Equipo_Persona()

        If errores.Count = 0 Then
            Dim script As String = String.Format("swal('Excelente!', 'Equipo Asignado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Asignar_Usuario_Equipo.aspx")
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If

    End Sub

    Private Sub Guardar_Equipo_Persona()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Tercero_Equipo 
        (Id_Tercero, Marca, Serial, Detalle_Equipo) 
        VALUES 
        (@Id_Tercero, @Marca, @Serial, @Detalle_Equipo)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", TxPersona.SelectedValue)
                    cmd.Parameters.AddWithValue("@Marca", If(String.IsNullOrWhiteSpace(TxMarca.Text), DBNull.Value, TxMarca.Text.Trim.ToUpper()))
                    cmd.Parameters.AddWithValue("@Serial", If(String.IsNullOrWhiteSpace(TxSerial.Text), DBNull.Value, TxSerial.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Detalle_Equipo", TxDetalleEquipo.Text)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Equipo_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            LvEquiposAsignados.DataSourceID = "SqlEquiposAsignados"
        Else
            LvEquiposAsignados.DataSourceID = "Sql_Buscar_EquiposAsignados"
        End If
        LvEquiposAsignados.DataBind()
    End Sub

End Class