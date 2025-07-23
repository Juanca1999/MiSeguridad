Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Registrar_Visita_Contratista
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

            If Consultar_Visita_Proveedor() Then
                BtVisitaProveedor.Visible = True
            Else
                BtVisitaProveedor.Visible = False
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

    Private Function Consultar_Visita_Proveedor() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Visita_Proveedores FROM Adm_Empresa"
                Using command As New SqlCommand(query, connection)

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado no es NULL y devolver el valor como Boolean
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return Convert.ToBoolean(result)
                    Else
                        ' Si el resultado es NULL, devolver False
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

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

    Private Sub BtVisitaProveedor_ServerClick(sender As Object, e As EventArgs) Handles BtVisitaProveedor.ServerClick
        Response.Redirect("Registrar_Visita_Proveedor.aspx")
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

    Private Sub BtBuscarContratista_Click(sender As Object, e As EventArgs) Handles BtBuscarContratista.Click
        If TxBuscarCedulaContratista.Text <> "" Then
            Dim cedulaContratista As String = TxBuscarCedulaContratista.Text.Trim()
            Dim fechaActual As DateTime = DateTime.Now.Date ' Obtener solo la fecha actual, sin la hora

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT TOP 1 C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.Fecha_ARL, C.Fecha_Seguridad_Social, C.Fecha_Trabajo_Alturas FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = C.Cedula_Contratista WHERE C.Estado = 1 AND C.Cedula_Contratista = @Contratista ORDER BY C.Id_Control_Contratistas DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Contratista", cedulaContratista)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            ' Obtener solo la parte de la fecha (sin la hora)
                            Dim fechaInicio As DateTime = If(IsDBNull(reader("Fecha_Inicio")), DateTime.MinValue, Convert.ToDateTime(reader("Fecha_Inicio")).Date)
                            Dim fechaFin As DateTime = If(IsDBNull(reader("Fecha_Fin")), DateTime.MaxValue, Convert.ToDateTime(reader("Fecha_Fin")).Date)
                            Session("Id_Inmueble") = reader("Id_Inmueble")
                            Session("Nombre_Autoriza") = reader("Nombres")

                            ' Verificar si la fecha actual está dentro del rango de vigencia general
                            If fechaActual >= fechaInicio AndAlso fechaActual <= fechaFin Then
                                ' Validar fechas adicionales de vencimiento
                                Dim mensajesVencimiento As New List(Of String)

                                ' Validar Fecha_ARL
                                If Not IsDBNull(reader("Fecha_ARL")) Then
                                    Dim fechaARL As DateTime = Convert.ToDateTime(reader("Fecha_ARL")).Date
                                    If fechaARL < fechaActual Then
                                        mensajesVencimiento.Add("Documento ARL vencido")
                                    End If
                                End If

                                ' Validar Fecha_Seguridad_Social
                                If Not IsDBNull(reader("Fecha_Seguridad_Social")) Then
                                    Dim fechaSS As DateTime = Convert.ToDateTime(reader("Fecha_Seguridad_Social")).Date
                                    If fechaSS < fechaActual Then
                                        mensajesVencimiento.Add("Documento Seguridad Social vencido")
                                    End If
                                End If

                                ' Validar Fecha_Trabajo_Alturas
                                If Not IsDBNull(reader("Fecha_Trabajo_Alturas")) Then
                                    Dim fechaAlturas As DateTime = Convert.ToDateTime(reader("Fecha_Trabajo_Alturas")).Date
                                    If fechaAlturas < fechaActual Then
                                        mensajesVencimiento.Add("Documento Trabajo Alturas vencido")
                                    End If
                                End If

                                ' Mostrar alertas si hay alguna fecha vencida
                                ' Mostrar alertas si hay alguna fecha vencida
                                If mensajesVencimiento.Count > 0 Then
                                    Dim mensajeFinal As String = String.Join("<br>", mensajesVencimiento) ' Con salto de línea HTML para mayor visibilidad
                                    Dim script As String = "
                                    swal({
                                        title: '¡ATENCIÓN!',
                                        text: '" & mensajeFinal & "',
                                        type: 'warning',
                                        confirmButtonText: 'OK',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false,
                                        customClass: 'alerta-vencimiento'
                                    });
                                "
                                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "vencimiento", script, True)

                                    ' Opcional: ocultar tabla y botón si deseas bloquear el acceso
                                    Tabla.Visible = False
                                    BtGuardar.Visible = False
                                Else
                                    Tabla.Visible = True
                                    BtGuardar.Visible = True
                                End If
                            Else
                                ' Si no está dentro del rango de fechas de inicio/fin
                                Tabla.Visible = False
                                BtGuardar.Visible = False
                                Session("Id_Inmueble") = Nothing
                                Session("Nombre_Autoriza") = Nothing
                                Dim script As String = "swal('OJO!', 'El contratista no está autorizado para la fecha actual.', 'warning');"
                                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                            End If
                        Else
                            Tabla.Visible = False
                            BtGuardar.Visible = False
                            Session("Id_Inmueble") = Nothing
                            Session("Nombre_Autoriza") = Nothing
                            Dim script As String = String.Format("swal('OJO!', 'Contratista no autorizado, consulte con el administrador', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
            BtGuardar.Visible = False
            Session("Id_Inmueble") = Nothing
            Session("Nombre_Autoriza") = Nothing
        End If
    End Sub

    Private Sub TxBuscarCedulaContratista_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedulaContratista.TextChanged
        If TxBuscarCedulaContratista.Text <> "" Then
            Dim cedulaContratista As String = TxBuscarCedulaContratista.Text.Trim()
            Dim fechaActual As DateTime = DateTime.Now.Date ' Obtener solo la fecha actual, sin la hora

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT TOP 1 C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.Fecha_ARL, C.Fecha_Seguridad_Social, C.Fecha_Trabajo_Alturas FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = C.Cedula_Contratista WHERE C.Estado = 1 AND C.Cedula_Contratista = @Contratista ORDER BY C.Id_Control_Contratistas DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Contratista", cedulaContratista)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            ' Obtener solo la parte de la fecha (sin la hora)
                            Dim fechaInicio As DateTime = If(IsDBNull(reader("Fecha_Inicio")), DateTime.MinValue, Convert.ToDateTime(reader("Fecha_Inicio")).Date)
                            Dim fechaFin As DateTime = If(IsDBNull(reader("Fecha_Fin")), DateTime.MaxValue, Convert.ToDateTime(reader("Fecha_Fin")).Date)
                            Session("Id_Inmueble") = reader("Id_Inmueble")
                            Session("Nombre_Autoriza") = reader("Nombres")

                            ' Verificar si la fecha actual está dentro del rango de vigencia general
                            If fechaActual >= fechaInicio AndAlso fechaActual <= fechaFin Then
                                ' Validar fechas adicionales de vencimiento
                                Dim mensajesVencimiento As New List(Of String)

                                ' Validar Fecha_ARL
                                If Not IsDBNull(reader("Fecha_ARL")) Then
                                    Dim fechaARL As DateTime = Convert.ToDateTime(reader("Fecha_ARL")).Date
                                    If fechaARL < fechaActual Then
                                        mensajesVencimiento.Add("Documento ARL vencido")
                                    End If
                                End If

                                ' Validar Fecha_Seguridad_Social
                                If Not IsDBNull(reader("Fecha_Seguridad_Social")) Then
                                    Dim fechaSS As DateTime = Convert.ToDateTime(reader("Fecha_Seguridad_Social")).Date
                                    If fechaSS < fechaActual Then
                                        mensajesVencimiento.Add("Documento Seguridad Social vencido")
                                    End If
                                End If

                                ' Validar Fecha_Trabajo_Alturas
                                If Not IsDBNull(reader("Fecha_Trabajo_Alturas")) Then
                                    Dim fechaAlturas As DateTime = Convert.ToDateTime(reader("Fecha_Trabajo_Alturas")).Date
                                    If fechaAlturas < fechaActual Then
                                        mensajesVencimiento.Add("Documento Trabajo Alturas vencido")
                                    End If
                                End If

                                ' Mostrar alertas si hay alguna fecha vencida
                                If mensajesVencimiento.Count > 0 Then
                                    Dim mensajeFinal As String = String.Join("<br>", mensajesVencimiento) ' Con salto de línea HTML para mayor visibilidad
                                    Dim script As String = "
                                    swal({
                                        title: '¡ATENCIÓN!',
                                        text: '" & mensajeFinal & "',
                                        type: 'warning',
                                        confirmButtonText: 'OK',
                                        allowOutsideClick: false,
                                        allowEscapeKey: false,
                                        customClass: 'alerta-vencimiento'
                                    });
                                "
                                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "vencimiento", script, True)

                                    ' Opcional: ocultar tabla y botón si deseas bloquear el acceso
                                    Tabla.Visible = False
                                    BtGuardar.Visible = False
                                Else
                                    Tabla.Visible = True
                                    BtGuardar.Visible = True
                                End If
                            Else
                                ' Si no está dentro del rango de fechas de inicio/fin
                                Tabla.Visible = False
                                BtGuardar.Visible = False
                                Session("Id_Inmueble") = Nothing
                                Session("Nombre_Autoriza") = Nothing
                                Dim script As String = "swal('OJO!', 'El contratista no está autorizado para la fecha actual.', 'warning');"
                                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                            End If
                        Else
                            Tabla.Visible = False
                            BtGuardar.Visible = False
                            Session("Id_Inmueble") = Nothing
                            Session("Nombre_Autoriza") = Nothing
                            Dim script As String = String.Format("swal('OJO!', 'Contratista no autorizado, consulte con el administrador', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
            BtGuardar.Visible = False
            Session("Id_Inmueble") = Nothing
            Session("Nombre_Autoriza") = Nothing
        End If
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
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(TxBuscarCedulaContratista.Text))
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
                ' Recuperar el valor de Id_Visita de la sesión
                cmd.Parameters.AddWithValue("@Id_Visita", Session("Id_Visita"))
                cmd.Parameters.AddWithValue("@Id_Quien_Registra", idTercero_Quien_Registra)
                cmd.Parameters.AddWithValue("@Id_Quien_Ingresa", idTercero_Visitante)
                Dim idInmueble = Session("Id_Inmueble")
                If idInmueble Is Nothing OrElse String.IsNullOrWhiteSpace(idInmueble.ToString()) Then
                    cmd.Parameters.AddWithValue("@Id_Inmueble", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@Id_Inmueble", idInmueble)
                End If
                Dim autoriza = Session("Nombre_Autoriza")
                If autoriza Is Nothing OrElse String.IsNullOrWhiteSpace(autoriza.ToString()) Then
                    cmd.Parameters.AddWithValue("@Persona_Visitada", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Quien_Autoriza", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@Persona_Visitada", autoriza)
                    cmd.Parameters.AddWithValue("@Quien_Autoriza", autoriza)
                End If
                cmd.Parameters.AddWithValue("@Fecha_Inicio_Visita", Date.Now.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@Hora_Inicio_Visita", Date.Now.ToString("HH:mm:ss"))

                cmd.Parameters.AddWithValue("@Placa", DBNull.Value)
                cmd.Parameters.AddWithValue("@Observacion", DBNull.Value)
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

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Consultar_Id_Visita()
        Registrar_Entrada()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Visita Registrada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Registrar_Visita_Contratista.aspx")
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If
    End Sub

    Private Sub LvContratistas_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvContratistas.ItemDataBound
        Dim Fecha_InicioLabel As Label = DirectCast(e.Item.FindControl("Fecha_InicioLabel"), Label)
        Dim Fecha_FinLabel As Label = DirectCast(e.Item.FindControl("Fecha_FinLabel"), Label)
        Dim ARL As HtmlAnchor = DirectCast(e.Item.FindControl("ARL"), HtmlAnchor)
        Dim Seguridad_Social As HtmlAnchor = DirectCast(e.Item.FindControl("Seguridad_Social"), HtmlAnchor)
        Dim Trabajo_Alturas As HtmlAnchor = DirectCast(e.Item.FindControl("Trabajo_Alturas"), HtmlAnchor)

        If String.IsNullOrEmpty(ARL.Attributes("title")) Then
            ARL.Visible = False
        End If

        If String.IsNullOrEmpty(Seguridad_Social.Attributes("title")) Then
            Seguridad_Social.Visible = False
        End If

        If String.IsNullOrEmpty(Trabajo_Alturas.Attributes("title")) Then
            Trabajo_Alturas.Visible = False
        End If
    End Sub

End Class