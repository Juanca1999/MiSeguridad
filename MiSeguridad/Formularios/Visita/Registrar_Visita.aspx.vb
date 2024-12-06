Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Web.Services
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Registrar_Visita
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

    Private Sub BtEntrada_ServerClick(sender As Object, e As EventArgs) Handles BtEntrada.ServerClick
        Response.Redirect("Registrar_Visita.aspx")
    End Sub

    Private Sub BtSalida_ServerClick(sender As Object, e As EventArgs) Handles BtSalida.ServerClick
        Response.Redirect("Registrar_Salida.aspx")
    End Sub

    Private Sub BtEntradaContratista_ServerClick(sender As Object, e As EventArgs) Handles BtEntradaContratista.ServerClick
        Response.Redirect("Registrar_Visita_Contratista.aspx")
    End Sub

    Private Sub BtSalidaContratista_ServerClick(sender As Object, e As EventArgs) Handles BtSalidaContratista.ServerClick
        Response.Redirect("Registrar_Salida_Contratista.aspx")
    End Sub

    Private Sub CargarTitulo()
        Dim query As String = "SELECT Titulo_Personas FROM Adm_Empresa"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                Try
                    connection.Open()
                    Dim titulo As String = Convert.ToString(command.ExecuteScalar())
                    TituloLabel.Text = titulo
                Catch ex As Exception
                    TituloLabel.Text = ""
                Finally
                    connection.Close()
                End Try
            End Using
        End Using
    End Sub

    Protected Sub TxBuscarInmueble_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarInmueble.TextChanged
        If TxBuscarInmueble.Text <> "" Then
            ' Recuperar el valor ingresado en el TextBox
            Dim inmuebleBuscado As String = TxBuscarInmueble.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT IR.Id_Inmueble, TI.Tipo_Inmueble, I.Telefono, IR.Id_Tercero, T.Cedula, T.Nombres, T.Celular FROM Adm_Inmueble_Residentes IR LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IR.Id_Inmueble LEFT JOIN Terceros T ON T.Id_Tercero = IR.Id_Tercero LEFT JOIN Adm_Tipo_Inmueble TI ON TI.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble WHERE IR.Id_Inmueble = @Inmueble ORDER BY T.Nombres"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Inmueble", inmuebleBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            CargarTitulo()
                            Tabla.Visible = True
                            If Fotico.Value <> "" Then
                                photo.Src = Fotico.Value
                            End If
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Residentes no encontrados', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        End If
    End Sub

    <WebMethod()>
    Public Shared Function ObtenerFoto(cedula As String) As Object
        Dim base64Foto As String = String.Empty
        Dim nombres As String = String.Empty
        Dim autoriza As String = String.Empty
        Dim query As String = "SELECT Id_Tercero, Foto, Nombres, Id_Rol FROM Terceros WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Cedula", cedula)
                conn.Open()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        If Not IsDBNull(reader("Foto")) Then
                            base64Foto = Convert.ToString(reader("Foto"))
                        End If

                        If Not IsDBNull(reader("Nombres")) Then
                            nombres = Convert.ToString(reader("Nombres"))
                        End If

                        ' Verificar si el Id_Rol es igual a 4
                        If Not IsDBNull(reader("Id_Rol")) AndAlso Convert.ToInt32(reader("Id_Rol")) = 4 AndAlso Not IsDBNull(reader("Id_Tercero")) Then
                            Dim queryAutoriza As String = "SELECT TOP 1 PC.Id_Tercero, T1.Nombres, PC.Id_inmueble, IR.Id_Tercero AS Residente_Id_Tercero, T2.Nombres AS Residente_Nombres FROM Adm_Inmueble_Per_Confianza PC LEFT JOIN Terceros T1 ON T1.Id_Tercero = PC.Id_Tercero LEFT JOIN Adm_Inmueble_Residentes IR ON IR.Id_Inmueble = PC.Id_inmueble LEFT JOIN Terceros T2 ON T2.Id_Tercero = IR.Id_Tercero WHERE PC.Id_Tercero = @Id_Tercero"
                            Using cmdAutoriza As New SqlCommand(queryAutoriza, conn)
                                ' Asegúrate de proporcionar el valor correcto para TxInmueble.SelectValue
                                cmdAutoriza.Parameters.AddWithValue("@Id_Tercero", (reader("Id_Tercero")))
                                reader.Close() ' Cerrar el primer DataReader

                                Using readerAutoriza As SqlDataReader = cmdAutoriza.ExecuteReader()
                                    If readerAutoriza.Read() Then
                                        If Not IsDBNull(readerAutoriza("Residente_Nombres")) Then
                                            autoriza = Convert.ToString(readerAutoriza("Residente_Nombres"))
                                        End If
                                    End If
                                End Using
                            End Using
                        End If
                    End If
                End Using
            End Using
        End Using

        ' Devolver un objeto JSON con la foto, nombres y autoriza si aplica
        Return New With {
        .Foto = base64Foto,
        .Nombress = nombres,
        .Autoriza = If(String.IsNullOrEmpty(autoriza), String.Empty, autoriza)
    }
    End Function


    Protected Sub TxTipoVehiculo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TxTipoVehiculo.SelectedIndexChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue

        If tipoVehiculo = "1" Then ' Carro
            TxPlaca.Enabled = True
        ElseIf tipoVehiculo = "2" Then ' Moto
            TxPlaca.Enabled = True
        Else
            TxPlaca.Enabled = False
        End If
    End Sub

    Protected Sub TxPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxPlaca.TextChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue
        Dim placa As String = TxPlaca.Text.Trim()
        Dim isValid As Boolean = False

        If Not String.IsNullOrEmpty(placa) Then
            If tipoVehiculo = "1" Then ' Carro
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{3}$")
            ElseIf tipoVehiculo = "2" Then ' Moto
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{2}[A-Z]{1}$")
            End If
        Else
            isValid = True ' Permitir que el campo esté vacío
        End If

        If isValid Then
            lblPlacaError.Visible = False
            hfPlacaValida.Value = True
        Else
            lblPlacaError.Text = "Placa inválida. " & GetValidationMessage(tipoVehiculo)
            lblPlacaError.Visible = True
            hfPlacaValida.Value = False

        End If
    End Sub

    Private Function GetValidationMessage(tipoVehiculo As String) As String
        If tipoVehiculo = "1" Then ' Carro
            Return "Debe ser 3 letras y 3 números (ej: ABC123)"
        ElseIf tipoVehiculo = "2" Then ' Moto
            Return "Debe ser 3 letras, 2 números y 1 letra (ej: ABC12D)"
        Else
            Return String.Empty
        End If
    End Function

    Private Function RegistroExisteEnBD(Cedula As Long) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

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

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer
        Session("sql") = "SELECT MAX(Id_Tercero) FROM Terceros"
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Try
                Valor = dr(0)
            Catch ex As Exception
                Valor = 0
            End Try
            dr.Close()

        End If
        Session("Id_Tercero") = Valor + 1
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub Guardar_Persona()
        Dim sql As String = "INSERT INTO dbo.Terceros (Id_Tercero, Cedula, Nombres, Id_Rol, Usuario, Password, Foto, Id_Sede, Estado) " &
                        "VALUES (@Id_Tercero, @Cedula, @Nombres, @Id_Rol, @Usuario, @Password, @Foto, @Id_Sede, @Estado)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))

                If Not String.IsNullOrEmpty(TxCedulaVisitante.Text) Then
                    cmd.Parameters.AddWithValue("@Cedula", TxCedulaVisitante.Text)
                    cmd.Parameters.AddWithValue("@Usuario", Convert.ToInt64(TxCedulaVisitante.Text))
                    cmd.Parameters.AddWithValue("@Password", Convert.ToInt64(TxCedulaVisitante.Text))
                Else
                    cmd.Parameters.AddWithValue("@Cedula", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Nombres", TxNombreVisitante.Text.ToUpper())
                If CbDomiciliario.Checked = True Then
                    cmd.Parameters.AddWithValue("@Id_Rol", 7)
                Else
                    cmd.Parameters.AddWithValue("@Id_Rol", 6)
                End If


                If Not String.IsNullOrEmpty(base64image.Value) Then
                    cmd.Parameters.AddWithValue("@Foto", base64image.Value)
                Else
                    cmd.Parameters.AddWithValue("@Foto", DBNull.Value)
                End If

                If Not String.IsNullOrEmpty(Session("Sucursal_Usuario")) Then
                    cmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))
                Else
                    cmd.Parameters.AddWithValue("@Id_Sede", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Estado", 1)

                ' Abrir la conexión y ejecutar el comando
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Consultar_Id_Visita()
        Dim Valor As Integer
        Session("sql") = "SELECT MAX(Id_Visita) FROM Adm_Visita"
        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            Try
                Valor = dr(0)
            Catch ex As Exception
                Valor = 0
            End Try
            dr.Close()

        End If
        Session("Id_Visita") = Valor + 1
        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub Registrar_Entrada()
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(TxCedulaVisitante.Text))
        Dim idTercero_Quien_Registra As Long = ObtenerIdTercero(Convert.ToInt64(User.Identity.Name))
        Dim sql As String = "INSERT INTO dbo.Adm_Visita (" &
                         "Id_Visita, Id_Quien_Registra, Id_Quien_Ingresa, Id_Inmueble, " &
                         "Persona_Visitada, Quien_Autoriza, Fecha_Inicio_Visita, Hora_Inicio_Visita, " &
                         "Placa, Observacion, Id_Sede, Id_Acceso_Entrada, Manual, Masivo, Estado) VALUES (" &
                         "@Id_Visita, @Id_Quien_Registra, @Id_Quien_Ingresa, @Id_Inmueble, " &
                         "@Persona_Visitada, @Quien_Autoriza, @Fecha_Inicio_Visita, @Hora_Inicio_Visita, " &
                         "@Placa, @Observacion, @Id_Sede, @Id_Acceso_Entrada, @Manual, @Masivo, @Estado)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Visita", Session("Id_Visita"))
                cmd.Parameters.AddWithValue("@Id_Quien_Registra", idTercero_Quien_Registra)
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                cmd.Parameters.AddWithValue("@Id_Inmueble", TxBuscarInmueble.Text)

                If TxPersonaVisitada.Text <> String.Empty Then
                    cmd.Parameters.AddWithValue("@Persona_Visitada", TxPersonaVisitada.Text.ToUpper())
                Else
                    cmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Quien_Autoriza", TxQuienAutoriza.Text.ToUpper())
                cmd.Parameters.AddWithValue("@Fecha_Inicio_Visita", Date.Now.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@Hora_Inicio_Visita", Date.Now.ToString("HH:mm:ss"))

                If TxPlaca.Text <> String.Empty Then
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.ToUpper())
                Else
                    cmd.Parameters.AddWithValue("@Placa", DBNull.Value)
                End If

                If TxObservacion.Text <> String.Empty Then
                    cmd.Parameters.AddWithValue("@Observacion", TxObservacion.Text())
                Else
                    cmd.Parameters.AddWithValue("@Observacion", DBNull.Value)
                End If

                If Not String.IsNullOrEmpty(Session("Sucursal_Usuario")) Then
                    cmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))
                Else
                    cmd.Parameters.AddWithValue("@Id_Sede", DBNull.Value)
                End If

                If Not String.IsNullOrEmpty(Session("Acceso_Usuario")) Then
                    cmd.Parameters.AddWithValue("@Id_Acceso_Entrada", Session("Acceso_Usuario"))
                Else
                    cmd.Parameters.AddWithValue("@Id_Acceso_Entrada", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@Manual", 0)
                cmd.Parameters.AddWithValue("@Masivo", 0)
                cmd.Parameters.AddWithValue("@Estado", 1)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Actualizar_Foto()
        Dim sql As String = "UPDATE Terceros SET Foto = @Foto WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Foto", base64image.Value)
                cmd.Parameters.AddWithValue("@Cedula", TxCedulaVisitante.Text)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If String.IsNullOrWhiteSpace(TxCedulaVisitante.Text) Then
            Dim script As String = String.Format("swal('OJO!', 'El campo cedula visitante no puede estar vacio', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        ElseIf String.IsNullOrWhiteSpace(TxNombreVisitante.Text) Then
            Dim script As String = String.Format("swal('OJO!', 'El campo nombre visitante no puede estar vacio', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        ElseIf String.IsNullOrWhiteSpace(TxBuscarInmueble.SelectedValue) Then
            Dim script As String = String.Format("swal('OJO!', 'Debe seleccionar un inmueble', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        ElseIf String.IsNullOrWhiteSpace(Session("Acceso_Usuario")) Then
            Dim script As String = String.Format("swal('OJO!', 'Debe seleccionar el acceso', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        Else
            If TxCedulaVisitante.Text <> "" Then
                If Not RegistroExisteEnBD(TxCedulaVisitante.Text) Then
                    Consultar_Id_Persona()
                    Guardar_Persona()
                Else
                    If Not String.IsNullOrEmpty(base64image.Value) Then
                        Actualizar_Foto()
                    End If
                End If
            End If
            Consultar_Id_Visita()
            Registrar_Entrada()

            If ErrorOp = Nothing Then
                Dim script As String = String.Format("swal('Excelente!', 'Visita Registrada Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 3000
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                EnviarCorreoError()
            End If
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Registrar_Visita.aspx")
    End Sub

End Class