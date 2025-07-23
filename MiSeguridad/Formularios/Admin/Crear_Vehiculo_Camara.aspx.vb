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

Public Class Crear_Vehiculo_Camara
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
        Response.Redirect("Crear_Vehiculo_Camara.aspx")
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

    Protected Sub TxTipoVehiculo_TextChanged(sender As Object, e As EventArgs) Handles TxTipoVehiculo.TextChanged
        If TxTipoVehiculo.SelectedValue = "1" Or TxTipoVehiculo.SelectedValue = "2" Or TxTipoVehiculo.SelectedValue = "3" Then
            TxPlaca.Enabled = True
            TxPlaca.Attributes("required") = "1"
            lblPlacaError.Style("display") = "none"

            ' Si ya hay una placa ingresada, validar automáticamente
            If Not String.IsNullOrEmpty(TxPlaca.Text.Trim()) Then
                TxPlaca_TextChanged(TxPlaca, EventArgs.Empty)
            End If
        Else
            TxPlaca.Text = ""
            lblPlacaError.Style("display") = "none"
            BtGuardar.Enabled = True
        End If
    End Sub

    Protected Sub TxPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxPlaca.TextChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue
        Dim placa As String = TxPlaca.Text.Trim().ToUpper() ' Convertir la placa a mayúsculas automáticamente
        TxPlaca.Text = placa ' Asignar el valor convertido al TextBox
        Dim isValid As Boolean = False

        If Not String.IsNullOrEmpty(placa) Then
            If tipoVehiculo = "1" Or tipoVehiculo = "3" Then ' Carro
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{3}$")
            ElseIf tipoVehiculo = "2" Then ' Moto
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{2}[A-Z]{1}$")
            End If
        Else
            isValid = True ' Permitir que el campo esté vacío
        End If

        If isValid Then
            lblPlacaError.Attributes("style") = "display:none; margin-top: 60px;"
            BtGuardar.Enabled = True
            Consultar_Vehiculo(TxPlaca.Text.Trim())
        Else
            lblPlacaError.Text = "Placa inválida. " & GetValidationMessage(tipoVehiculo)
            lblPlacaError.Attributes("style") = "display:block; margin-top: 60px;"
            BtGuardar.Enabled = False
        End If
    End Sub

    Private Function GetValidationMessage(tipoVehiculo As String) As String
        If tipoVehiculo = "1" Or tipoVehiculo = "3" Then ' Carro
            Return "Debe ser 3 letras y 3 números (ej: ABC123)"
        ElseIf tipoVehiculo = "2" Then ' Moto
            Return "Debe ser 3 letras, 2 números y 1 letra (ej: ABC12D)"
        Else
            Return String.Empty
        End If
    End Function

    Private Sub Consultar_Tercero()
        Try
            sql = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Estado = 1 AND Nombres LIKE '%" + TxBuscarPersona.Text + "%' ORDER BY Nombres"
            Using cmd As New SqlCommand(sql, conn)
                cmd.CommandType = CommandType.Text
                Using mda As New SqlDataAdapter(cmd)
                    Using datos As New DataTable
                        mda.Fill(datos)
                        TxNombrePersona.DataSource = datos
                        TxNombrePersona.DataTextField = "Nombres"
                        TxNombrePersona.DataValueField = "Id_Tercero"
                        TxNombrePersona.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = "Private Sub Consultar_Tercero: " + Mid(ex.ToString, 1, 500) + "<br /><br />" + "Sintaxis Sql: " + sql
            Dim script As String = String.Format("swal('Error!', 'No se encontró la persona', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            Consultar_Tercero()
        Else
            TxNombrePersona.Items.Clear()
        End If
    End Sub

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
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

    Private Sub Guardar_Persona()
        Try
            Dim query As String = "
        INSERT INTO dbo.Terceros 
        (Id_Tercero, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Cedula", TxCedula.Text.Trim())
                    cmd.Parameters.AddWithValue("@Nombres", TxNombre.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", 8)
                    cmd.Parameters.AddWithValue("@Correo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Celular", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Sede", If(String.IsNullOrWhiteSpace(Session("Sucursal_Usuario")), DBNull.Value, Session("Sucursal_Usuario")))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD en Guardar_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub BtCrearPersona_Click(sender As Object, e As EventArgs) Handles BtCrearPersona.Click
        If TxCedula.Text <> "" And TxNombre.Text <> "" Then
            If Not RegistroExisteEnBD(TxCedula.Text) Then
                Consultar_Id_Persona()
                Guardar_Persona()

                If errores.Count = 0 Then
                    Dim script As String = "swal('Excelente!', 'Persona Guardada Correctamente.', 'success');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    TxNombrePersona.Items.Clear()
                Else
                    Dim mensajeError As String = String.Join("<br/>", errores)
                    Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                End If
            Else
                Dim script As String = "swal('OJO!', 'Ya existe un registro con esa cedula.', 'warning');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        End If
    End Sub

    Private Sub Guardar_Vehiculo()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Vehiculo 
        (Placa, Id_Tipo_Vehiculo, Marca, Modelo, Color, Estado) 
        VALUES 
        (@Placa, @Id_Tipo_Vehiculo, @Marca, @Modelo, @Color, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Tipo_Vehiculo", TxTipoVehiculo.SelectedValue)
                    cmd.Parameters.AddWithValue("@Marca", TxMarca.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Modelo", TxModelo.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Color", TxColor.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Vehiculo():" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Vehiculo()
        Try
            Dim query As New System.Text.StringBuilder("UPDATE dbo.Adm_Vehiculo SET ")

            query.Append("Id_Tipo_Vehiculo = @Id_Tipo_Vehiculo, ")
            query.Append("Marca = @Marca, ")
            query.Append("Modelo = @Modelo, ")
            query.Append("Color = @Color, ")
            query.Append("Estado = @Estado ")
            query.Append("WHERE Placa = @Placa")

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query.ToString(), conn)
                    cmd.Parameters.AddWithValue("@Id_Tipo_Vehiculo", TxTipoVehiculo.SelectedValue)
                    cmd.Parameters.AddWithValue("@Marca", TxMarca.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Modelo", TxModelo.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Color", TxColor.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Estado", 1)
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error en Actualizar_Vehiculo()" & ex.Message)
        End Try
    End Sub

    Private Sub Guardar_Vehiculo_Inmueble()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Inmueble_Vehiculo 
        (Id_Inmueble, Placa) 
        VALUES 
        (@Id_Inmueble, @Placa)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Inmueble", TxInmueble.SelectedValue)
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Vehiculo_Inmueble():" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Vehiculo_Inmueble()
        Try
            Dim idInmuebleAnterior = Session("Id_Inmueble_Anterior")
            Dim placaAnterior = Session("Placa_Anterior")
            Dim idInmuebleNuevo = TxInmueble.SelectedValue
            Dim placaNueva = TxPlaca.Text.Trim.ToUpper()

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()

                Dim query As String
                If String.IsNullOrEmpty(idInmuebleAnterior) Then
                    ' Hacer INSERT
                    query = "INSERT INTO dbo.Adm_Inmueble_Vehiculo (Id_Inmueble, Placa) VALUES (@Id_Inmueble, @Placa)"
                Else
                    ' Hacer UPDATE
                    query = "UPDATE dbo.Adm_Inmueble_Vehiculo SET Id_Inmueble = @Id_Inmueble, Placa = @Placa WHERE Id_Inmueble = @Id_Inmueble_Anterior AND Placa = @Placa_Anterior"
                End If

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_Inmueble", idInmuebleNuevo)
                    cmd.Parameters.AddWithValue("@Placa", placaNueva)

                    If Not String.IsNullOrEmpty(idInmuebleAnterior) Then
                        cmd.Parameters.AddWithValue("@Id_Inmueble_Anterior", idInmuebleAnterior)
                        cmd.Parameters.AddWithValue("@Placa_Anterior", placaAnterior)
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Actualizar_Vehiculo_Inmueble(): " & ex.Message)
        End Try
    End Sub


    Private Sub Guardar_Vehiculo_Persona()
        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Vehiculo_Persona 
        (Id_Tercero, Placa, Id_Sede) 
        VALUES 
        (@Id_Tercero, @Placa, @Id_Sede)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", TxNombrePersona.SelectedValue)
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Sede", TxSede.SelectedValue)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Guardar_Vehiculo_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Vehiculo_Persona()
        Try
            Dim query As String = "UPDATE dbo.Adm_Vehiculo_Persona SET Id_Tercero = @Id_Tercero, Placa = @Placa, Id_Sede = @Id_Sede WHERE Id_Tercero = @Id_Tercero_Anterior AND Placa = @Placa_Anterior AND Id_Sede = @Sede_Anterior"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", TxNombrePersona.SelectedValue)
                    cmd.Parameters.AddWithValue("@Placa", TxPlaca.Text.Trim.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Sede", TxSede.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_Tercero_Anterior", Session("Id_Tercero_Anterior"))
                    cmd.Parameters.AddWithValue("@Placa_Anterior", Session("Placa_Anterior"))
                    cmd.Parameters.AddWithValue("@Sede_Anterior", Session("Id_Sede_Anterior"))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en Actualizar_Vehiculo_Persona():" & ex.Message)
        End Try
    End Sub

    Private Sub Consultar_Vehiculo(ByVal Placa As String)
        Dim query As String = "SELECT V.Placa, V.Id_Tipo_Vehiculo, T.Tipo_Vehiculo, V.Marca, V.Modelo, V.Color, IV.Id_Inmueble, IP.Id_Tercero, TE.Nombres, IP.Id_Sede FROM Adm_Vehiculo V LEFT JOIN Adm_Tipo_Vehiculo T ON T.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo LEFT JOIN Adm_Inmueble_Vehiculo IV ON IV.Placa = V.Placa LEFT JOIN Adm_Vehiculo_Persona IP ON IP.Placa = V.Placa LEFT JOIN Terceros TE ON TE.Id_Tercero = IP.Id_Tercero " &
                          "WHERE V.Placa = @Placa"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Placa", Placa)

                conn.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        TxPlaca.Text = dr("Placa").ToString()
                        Session("Placa_Anterior") = If(dr("Placa") IsNot DBNull.Value, dr("Placa").ToString(), String.Empty)
                        TxTipoVehiculo.SelectedValue = dr("Id_Tipo_Vehiculo").ToString()
                        TxMarca.Text = dr("Marca").ToString()
                        TxModelo.Text = dr("Modelo").ToString()
                        TxColor.Text = dr("Color").ToString()
                        Session("Id_Inmueble_Anterior") = If(dr("Id_Inmueble") IsNot DBNull.Value, dr("Id_Inmueble").ToString(), String.Empty)
                        TxInmueble.SelectedValue = If(dr("Id_Inmueble") IsNot DBNull.Value, dr("Id_Inmueble").ToString(), String.Empty)
                        TxNombrePersona.Items.Clear()
                        Dim idTercero As String = If(dr("Id_Tercero") IsNot DBNull.Value, dr("Id_Tercero").ToString(), "")
                        Dim nombres As String = If(dr("Nombres") IsNot DBNull.Value, dr("Nombres").ToString(), "")
                        Session("Id_Tercero_Anterior") = If(dr("Id_Tercero") IsNot DBNull.Value, dr("Id_Tercero").ToString(), String.Empty)
                        If Not String.IsNullOrWhiteSpace(idTercero) AndAlso Not String.IsNullOrWhiteSpace(nombres) Then
                            TxNombrePersona.Items.Add(New ListItem(nombres, idTercero))
                        End If
                        TxNombrePersona.DataBind()
                        Session("Id_Sede_Anterior") = If(dr("Id_Sede") IsNot DBNull.Value, dr("Id_Sede").ToString(), String.Empty)
                        TxSede.SelectedValue = If(dr("Id_Sede") IsNot DBNull.Value, dr("Id_Sede").ToString(), String.Empty)
                        BtGuardar.Text = "ACTUALIZAR"
                        Dim script As String = "document.getElementById('" & BtGuardar.ClientID & "').value = 'ACTUALIZAR';"
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "updateButtonText", script, True)
                    Else
                        TxMarca.Text = String.Empty
                        TxModelo.Text = String.Empty
                        TxColor.Text = String.Empty
                        TxInmueble.SelectedIndex = 0
                        TxBuscarPersona.Text = String.Empty
                        TxNombrePersona.Items.Clear()
                        TxSede.SelectedIndex = 0
                        BtGuardar.Text = "GUARDAR"
                        Dim script As String = "document.getElementById('" & BtGuardar.ClientID & "').value = 'GUARDAR';"
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "updateButtonText", script, True)
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub Crear_Placa()

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        ServicePointManager.ServerCertificateValidationCallback = Function(sender, cert, chain, errors) True

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "https://" & ipCamara & "/ISAPI/Traffic/channels/1/licensePlateAuditData/record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
                {
                    ""LicensePlateInfoList"": [
                        {
                            ""id"": """",
                            ""listType"": ""allowList"",
                            ""LicensePlate"": """ & TxPlaca.Text.Trim.ToUpper() & """
                        }
                    ]
                }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Placa") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Placa") = 0
                errores.Add("Error al crear placa en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub LvVehiculos_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvVehiculos.ItemDataBound
    End Sub

    Private Sub LvVehiculos_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvVehiculos.ItemCommand
        Dim PlacaLabel As Label = DirectCast(e.Item.FindControl("PlacaLabel"), Label)
        Dim Id_InmuebleLabel As Label = DirectCast(e.Item.FindControl("Id_InmuebleLabel"), Label)
        Dim Id_TerceroLabel As Label = DirectCast(e.Item.FindControl("Id_TerceroLabel"), Label)
        Dim Id_SedeLabel As Label = DirectCast(e.Item.FindControl("Id_SedeLabel"), Label)
        Session("Placa_Anterior") = PlacaLabel.Text
        Session("Id_Inmueble_Anterior") = Id_InmuebleLabel.Text
        Session("Id_Tercero_Anterior") = Id_TerceroLabel.Text
        Session("Id_Sede_Anterior") = Id_SedeLabel.Text

        If e.CommandName = "Editar" Then
            TxPlaca.Enabled = False
            Consultar_Vehiculo(PlacaLabel.Text)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then
            Dim vehiculoCreado As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            If RegistroExisteEnBD(TxPlaca.Text.Trim.ToUpper()) Then

                Actualizar_Vehiculo()
                Actualizar_Vehiculo_Inmueble()

                ' Verificar si el usuario fue creado
                If errores.Count = 0 Then
                    vehiculoCreado = True
                    mensaje = "vehiculo creado correctamente."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                Else
                    mensaje = String.Join("<br/>", errores)
                    script = String.Format("swal('Error!', '{0}', 'warning');", mensaje)
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Mostrar mensaje al usuario con SweetAlert
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_vehiculo_Camara.aspx")

            Else

                Guardar_Vehiculo()
                Guardar_Vehiculo_Inmueble()
                Guardar_Vehiculo_Persona()
                Crear_Placa()

                ' Verificar si el vehiculo fue creado
                If errores.Count = 0 Then
                    vehiculoCreado = True
                    mensaje = "vehiculo creado correctamente."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                Else
                    mensaje = String.Join("<br/>", errores)
                    script = String.Format("swal('Error!', '{0}', 'warning');", mensaje)
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Mostrar mensaje al usuario con SweetAlert
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "3; URL=Crear_vehiculo_Camara.aspx")
            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            Dim vehiculoCreado As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            Actualizar_Vehiculo()
            Actualizar_Vehiculo_Inmueble()
            Actualizar_Vehiculo_Persona()
            Crear_Placa()

            ' Verificar si el usuario fue creado
            If errores.Count = 0 Then
                vehiculoCreado = True
                mensaje = "Vehiculo actualizado correctamente."
                script = $"swal('Excelente!', '{mensaje}', 'success');"
            Else
                mensaje = String.Join("<br/>", errores)
                script = String.Format("swal('Error!', '{0}', 'warning');", mensaje)
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Exit Sub
            End If

            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Crear_vehiculo_Camara.aspx")
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