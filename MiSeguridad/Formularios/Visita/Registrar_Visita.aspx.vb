Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates
Imports System.Drawing

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

            If Consultar_Control_Previsita() Then
                DeshabilitarControles()
                TxCedulaVisitante.AutoPostBack = True
            Else
                TxCedulaVisitante.AutoPostBack = False
            End If

            If Consultar_Visita_Proveedor() Then
                BtVisitaProveedor.Visible = True
            Else
                BtVisitaProveedor.Visible = False
            End If

        End If
    End Sub

    Private Sub DeshabilitarControles()
        BtGuardar.Enabled = False
        TxTipoVehiculo.Enabled = False
        TxPlaca.Enabled = False
        TxPersonaVisitada.Enabled = False
        TxQuienAutoriza.Enabled = False
        TxNombreVisitante.Enabled = False
        TxObservacion.ReadOnly = True
    End Sub

    Protected Sub TxCedulaVisitante_TextChanged(sender As Object, e As EventArgs) Handles TxCedulaVisitante.TextChanged
        If Consultar_Control_Previsita() Then
            If TxCedulaVisitante.Text <> "" Then
                Dim queryinfoPre As String = "SELECT P.Cedula, T2.Nombres, P.Id_Tipo_Vehiculo, P.Placa, P.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza, 
                                      P.Equipo_Computo, P.Serial_Computo, IR.Id_Inmueble, P.Estado, P.Anulada 
                                      FROM Adm_Preregistro_Visita P 
                                      LEFT JOIN Terceros T ON T.Id_Tercero = P.Id_Quien_Autoriza
                                      LEFT JOIN Terceros T2 ON T2.Cedula = P.Cedula
                                      LEFT JOIN Adm_Inmueble_Residentes IR ON IR.Id_Tercero = T.Id_Tercero
                                      WHERE P.Anulada = 0 AND Fecha_Ingreso = CAST(GETDATE() AS DATE) 
                                      AND P.Cedula = @Cedula"

                Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                    conn.Open()
                    Using cmdinfoPre As New SqlCommand(queryinfoPre, conn)
                        cmdinfoPre.Parameters.AddWithValue("@Cedula", TxCedulaVisitante.Text)
                        Using reader As SqlDataReader = cmdinfoPre.ExecuteReader()
                            If reader.Read() Then
                                If reader("Estado") Is DBNull.Value Then
                                    TxCedulaVisitante.Text = ""
                                    TxNombreVisitante.Text = ""
                                    TxPersonaVisitada.Text = ""
                                    TxQuienAutoriza.Text = ""
                                    TxBuscarInmueble.Text = ""
                                    TxTipoVehiculo.Text = ""
                                    TxPlaca.Text = ""
                                    TxPlaca.Enabled = False
                                    BtGuardar.Enabled = False
                                    TxObservacion.Text = ""
                                    Tabla.Visible = False
                                    Dim script As String = String.Format("swal('OJO!', 'El registro se ha realizado, pero sin aprobación', 'warning');")
                                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                                    Exit Sub
                                ElseIf Convert.ToInt32(reader("Estado")) = 0 Then
                                    Dim script As String = String.Format("swal('OJO!', 'El registro se ha realizado, pero se ha rechazado.', 'warning');")
                                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                                    TxCedulaVisitante.Text = ""
                                    TxNombreVisitante.Text = ""
                                    Exit Sub
                                End If
                                TxBuscarInmueble.SelectedValue = reader("Id_Inmueble").ToString()
                                TxPersonaVisitada.Text = reader("Quien_Autoriza").ToString()
                                TxQuienAutoriza.Text = reader("Quien_Autoriza").ToString()
                                TxNombreVisitante.Text = reader("Nombres").ToString()
                                TxTipoVehiculo.SelectedValue = reader("Id_Tipo_Vehiculo").ToString()
                                TxPlaca.Text = reader("Placa").ToString()
                                If Not IsDBNull(reader("Equipo_Computo")) AndAlso Convert.ToBoolean(reader("Equipo_Computo")) Then
                                    If Not IsDBNull(reader("Serial_Computo")) Then
                                        TxObservacion.Text = "Equipo de computo: " & Convert.ToString(reader("Serial_Computo"))
                                    End If
                                Else
                                    TxObservacion.Text = ""
                                End If
                                BtGuardar.Enabled = True
                            Else
                                Dim script As String = String.Format("swal('OJO!', 'No se encuentran registros con la información suministrada.', 'warning');")
                                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                                TxCedulaVisitante.Text = ""
                                TxNombreVisitante.Text = ""
                            End If
                        End Using
                    End Using
                End Using
            Else
                TxCedulaVisitante.Text = ""
                TxNombreVisitante.Text = ""
                TxPersonaVisitada.Text = ""
                TxQuienAutoriza.Text = ""
                TxBuscarInmueble.Text = ""
                TxTipoVehiculo.Text = ""
                TxPlaca.Text = ""
                TxPlaca.Enabled = False
                BtGuardar.Enabled = False
                TxObservacion.Text = ""
                Tabla.Visible = False
            End If
        End If
    End Sub

    Private Function Consultar_Control_Previsita() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Control_Previsita FROM Adm_Empresa"
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

    Private Function Consultar_Control_Visitantes() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Visitantes_Facial FROM Adm_Empresa"
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

    Private Sub BtVisitaProveedor_ServerClick(sender As Object, e As EventArgs) Handles BtVisitaProveedor.ServerClick
        Response.Redirect("Registrar_Visita_Proveedor.aspx")
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
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Public Shared Function ObtenerEndPoint() As String
        Dim IP_EndPoint As String = String.Empty

        Dim sql As String = "SELECT TOP 1 IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Sede = @Id_Sede ORDER BY Id_Acceso ASC"

        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Sede", HttpContext.Current.Session("Sucursal_Usuario"))
                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            IP_EndPoint = reader("IP_Camara").ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try

        Return IP_EndPoint
    End Function

    Public Shared Function Consultar_Foto(ByVal Id As String) As String
        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/Intelligent/FDLib/FDSearch?format=json"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json"
            request.Credentials = New NetworkCredential("admin", "Miro-321")
            request.PreAuthenticate = False

            ' JSON que se enviará en el cuerpo
            Dim jsonBody As String = "
        {
            ""searchResultPosition"": 0,
            ""maxResults"": 100,
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Id & """
        }"

            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(jsonBody)
            End Using

            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Dim responseText As String = String.Empty
            Using reader As New StreamReader(response.GetResponseStream())
                responseText = reader.ReadToEnd()
            End Using

            Dim jsonResponse As JObject = JObject.Parse(responseText)

            If jsonResponse("statusCode").ToString() = "1" AndAlso jsonResponse("numOfMatches").ToString() = "1" Then
                ' Obtener la URL de la imagen desde el JSON
                Dim faceUrl As String = jsonResponse("MatchList")(0)("faceURL").ToString()

                ' Definir el nombre y la ruta del archivo
                Dim fileName As String = "face_" & Id & ".jpg"
                Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)

                ' Verificar si el archivo ya existe
                If System.IO.File.Exists(serverPath) Then
                    ' Eliminar el archivo existente antes de descargar el nuevo
                    System.IO.File.Delete(serverPath)
                End If

                ' Descargar la nueva imagen
                Dim webClient As New WebClient()
                webClient.Credentials = New NetworkCredential("admin", "Miro-321")

                ' Descargar y guardar la imagen en el servidor
                webClient.DownloadFile(faceUrl, serverPath)

                ' Retornar la URL local del servidor para el frontend
                Return "../../Adjunto/Foto_Usuarios/" & fileName
            Else
                ' Retornar mensaje de error
                Return "Error: Foto no encontrada."
            End If
        Catch ex As Exception
            ' Retornar mensaje de error
            Return "Error: " & ex.Message
        End Try
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimos4Logs() As List(Of Log)
        Dim logs As New List(Of Log)()
        Dim query As String = "
        SELECT TOP 4 L.Id_Log, L.Id_Tercero, T.Nombres, L.Fecha_Hora_Evento, 
               CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso, T.Foto, T.Id_HikCamara
        FROM Logs L
        LEFT JOIN Terceros T ON T.Id_Tercero = L.Id_Tercero
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso
        WHERE T.Id_Sede = @IdSede
        ORDER BY L.Id_Log DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IdSede", HttpContext.Current.Session("Sucursal_Usuario"))
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
                        log.Id_HikCamara = reader("Id_HikCamara").ToString()
                        Dim fileName As String = "face_" & log.Id_HikCamara & ".jpg"
                        Dim fileName2 As String = "visitor_" & log.Id_Tercero & ".jpg"
                        Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)
                        Dim serverPath2 As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName2)
                        If System.IO.File.Exists(serverPath) Then
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName
                        ElseIf Not IsDBNull(reader("Foto")) Then
                            Dim base64String As String = reader("Foto").ToString()
                            If base64String.StartsWith("data:image/") Then
                                base64String = base64String.Split(","c)(1) ' Eliminar el prefijo
                            End If
                            Dim fotoBytes As Byte() = Convert.FromBase64String(base64String)
                            System.IO.File.WriteAllBytes(serverPath2, fotoBytes)
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName2
                        Else
                            If log.Id_HikCamara <> Nothing Then
                                log.FotoUrl = Consultar_Foto(log.Id_HikCamara)
                            Else
                                log.FotoUrl = "../../../Diseno/Imagenes/Blanco.jpg"
                            End If
                        End If
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
               A.Nombre_Acceso, T.Foto, T.Id_HikCamara
        FROM Logs L
        LEFT JOIN Terceros T ON T.Id_Tercero = L.Id_Tercero
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso
        WHERE T.Id_Sede = @IdSede
        ORDER BY L.Id_Log DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IdSede", HttpContext.Current.Session("Sucursal_Usuario"))
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        log.Id_Log = reader("Id_Log").ToString()
                        log.Id_Tercero = reader("Id_Tercero").ToString()
                        log.Nombres = reader("Nombres").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.Id_HikCamara = reader("Id_HikCamara").ToString()
                        Dim fileName As String = "face_" & log.Id_HikCamara & ".jpg"
                        Dim fileName2 As String = "visitor_" & log.Id_Tercero & ".jpg"
                        Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)
                        Dim serverPath2 As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName2)
                        If System.IO.File.Exists(serverPath) Then
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName
                        ElseIf Not IsDBNull(reader("Foto")) Then
                            Dim base64String As String = reader("Foto").ToString()
                            If base64String.StartsWith("data:image/") Then
                                base64String = base64String.Split(","c)(1) ' Eliminar el prefijo
                            End If
                            Dim fotoBytes As Byte() = Convert.FromBase64String(base64String)
                            System.IO.File.WriteAllBytes(serverPath2, fotoBytes)
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName2
                        Else
                            If log.Id_HikCamara <> Nothing Then
                                log.FotoUrl = Consultar_Foto(log.Id_HikCamara)
                            Else
                                log.FotoUrl = "../../../Diseno/Imagenes/Blanco.jpg"
                            End If
                        End If
                    End If
                End Using
            End Using
        End Using

        Return log
    End Function

    <WebMethod()>
    Public Shared Function ObtenerFoto(cedula As String) As Object
        Dim base64Foto As String = String.Empty
        Dim nombres As String = String.Empty
        Dim autoriza As String = String.Empty
        Dim persona_visitada As String = String.Empty
        Dim inmueble As String = String.Empty
        Dim placa As String = String.Empty
        Dim tipo_vehiculo As String = String.Empty
        Dim observacion As String = String.Empty
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
                        ElseIf Not IsDBNull(reader("Id_Tercero")) Then
                            Dim queryinfo As String = "SELECT TOP 1 Id_Visita, Persona_Visitada, Quien_Autoriza, Id_Inmueble, Placa, Observacion FROM Adm_Visita WHERE Id_Quien_Ingresa = @Id_Tercero ORDER BY Id_Visita DESC"
                            Using cmdinfo As New SqlCommand(queryinfo, conn)
                                cmdinfo.Parameters.AddWithValue("@Id_Tercero", (reader("Id_Tercero")))
                                reader.Close()

                                Using readerInfo As SqlDataReader = cmdinfo.ExecuteReader()
                                    If readerInfo.Read() Then
                                        If Not IsDBNull(readerInfo("Quien_Autoriza")) Then
                                            autoriza = Convert.ToString(readerInfo("Quien_Autoriza"))
                                        End If
                                        If Not IsDBNull(readerInfo("Persona_Visitada")) Then
                                            persona_visitada = Convert.ToString(readerInfo("Persona_Visitada"))
                                        End If
                                        If Not IsDBNull(readerInfo("Id_Inmueble")) Then
                                            inmueble = Convert.ToString(readerInfo("Id_Inmueble"))
                                        End If
                                        If Not IsDBNull(readerInfo("Placa")) Then
                                            placa = Convert.ToString(readerInfo("Placa"))

                                            ' Verificar si la placa es de un carro (formato ABC123)
                                            If System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{3}$") Then
                                                tipo_vehiculo = "1" ' Carro
                                                ' Verificar si la placa es de una moto (formato ABC12A)
                                            ElseIf System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{2}[A-Z]{1}$") Then
                                                tipo_vehiculo = "2" ' Moto
                                            Else
                                                tipo_vehiculo = ""
                                            End If
                                        End If
                                        If Not IsDBNull(readerInfo("Observacion")) Then
                                            observacion = Convert.ToString(readerInfo("Observacion"))
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
        .Autoriza = If(String.IsNullOrEmpty(autoriza), String.Empty, autoriza),
        .Persona_Visitada = If(String.IsNullOrEmpty(persona_visitada), String.Empty, persona_visitada),
        .Id_Inmueble = If(String.IsNullOrEmpty(inmueble), String.Empty, inmueble),
        .Placa = If(String.IsNullOrEmpty(placa), String.Empty, placa),
        .Tipo_Vehiculo = If(String.IsNullOrEmpty(tipo_vehiculo), String.Empty, tipo_vehiculo),
        .Observacion = If(String.IsNullOrEmpty(observacion), String.Empty, observacion)
    }
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimos4LogsContratista() As List(Of Log)
        Dim logs As New List(Of Log)()
        Dim query As String = "
        SELECT TOP 4 L.Id_Novedad_Contratista, L.Id_Tercero, T.Nombres, L.Fecha_Hora_Novedad, Tipo_Novedad, T.Foto, T.Id_HikCamara 
        FROM Logs_Contratistas L
        LEFT JOIN Terceros T ON T.Id_Tercero = L.Id_Tercero
        ORDER BY L.Id_Novedad_Contratista DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim log As New Log()
                        log.Id_Log = reader("Id_Novedad_Contratista").ToString()
                        log.Id_Tercero = reader("Id_Tercero").ToString()
                        log.Nombres = reader("Nombres").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Novedad").ToString()
                        log.Tipo_Evento = reader("Tipo_Novedad").ToString()
                        log.Id_HikCamara = reader("Id_HikCamara").ToString()
                        Dim fileName As String = "face_" & log.Id_HikCamara & ".jpg"
                        Dim fileName2 As String = "visitor_" & log.Id_Tercero & ".jpg"
                        Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)
                        Dim serverPath2 As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName2)
                        If System.IO.File.Exists(serverPath) Then
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName
                        ElseIf Not IsDBNull(reader("Foto")) Then
                            Dim base64String As String = reader("Foto").ToString()
                            If base64String.StartsWith("data:image/") Then
                                base64String = base64String.Split(","c)(1) ' Eliminar el prefijo
                            End If
                            Dim fotoBytes As Byte() = Convert.FromBase64String(base64String)
                            System.IO.File.WriteAllBytes(serverPath2, fotoBytes)
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName2
                        Else
                            If log.Id_HikCamara <> Nothing Then
                                log.FotoUrl = Consultar_Foto(log.Id_HikCamara)
                            Else
                                log.FotoUrl = "../../../Diseno/Imagenes/Blanco.jpg"
                            End If
                        End If
                        logs.Add(log)
                    End While
                End Using
            End Using
        End Using

        Return logs
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimoLogContratista() As Log
        Dim log As New Log()
        Dim query As String = "
        SELECT TOP 1 L.Id_Novedad_Contratista, L.Id_Tercero, T.Nombres, L.Fecha_Hora_Novedad, Tipo_Novedad, T.Foto, T.Id_HikCamara 
        FROM Logs_Contratistas L
        LEFT JOIN Terceros T ON T.Id_Tercero = L.Id_Tercero
        ORDER BY L.Id_Novedad_Contratista DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        log.Id_Log = reader("Id_Novedad_Contratista").ToString()
                        log.Id_Tercero = reader("Id_Tercero").ToString()
                        log.Nombres = reader("Nombres").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Novedad").ToString()
                        log.Tipo_Evento = reader("Tipo_Novedad").ToString()
                        log.Id_HikCamara = reader("Id_HikCamara").ToString()
                        Dim fileName As String = "face_" & log.Id_HikCamara & ".jpg"
                        Dim fileName2 As String = "visitor_" & log.Id_Tercero & ".jpg"
                        Dim serverPath As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName)
                        Dim serverPath2 As String = HttpContext.Current.Server.MapPath("../../Adjunto/Foto_Usuarios/" & fileName2)
                        If System.IO.File.Exists(serverPath) Then
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName
                        ElseIf Not IsDBNull(reader("Foto")) Then
                            Dim base64String As String = reader("Foto").ToString()
                            If base64String.StartsWith("data:image/") Then
                                base64String = base64String.Split(","c)(1) ' Eliminar el prefijo
                            End If
                            Dim fotoBytes As Byte() = Convert.FromBase64String(base64String)
                            System.IO.File.WriteAllBytes(serverPath2, fotoBytes)
                            log.FotoUrl = "../../Adjunto/Foto_Usuarios/" & fileName2
                        Else
                            If log.Id_HikCamara <> Nothing Then
                                log.FotoUrl = Consultar_Foto(log.Id_HikCamara)
                            Else
                                log.FotoUrl = "../../../Diseno/Imagenes/Blanco.jpg"
                            End If
                        End If
                    End If
                End Using
            End Using
        End Using

        Return log
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimos4LogsVehiculos() As List(Of Log)
        Dim logs As New List(Of Log)()
        Dim query As String = "
        SELECT TOP 4 V.Id_Log_Vehicular, V.Placa, V.Fecha_Hora_Evento, 
               CASE WHEN V.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso, 
               CASE WHEN VE.Placa IS NOT NULL THEN 1 ELSE 0 END AS Registrado 
        FROM Logs_Vehicular V 
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = V.Id_Acceso 
        LEFT JOIN Adm_Vehiculo VE ON VE.Placa = V.Placa 
        ORDER BY V.Id_Log_Vehicular DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim log As New Log()
                        log.Id_Log = reader("Id_Log_Vehicular").ToString()
                        log.Nombres = reader("Placa").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.FotoUrl = "/Diseno/Imagenes/Carro.jpg"
                        log.Registrado = Convert.ToBoolean(reader("Registrado"))
                        logs.Add(log)
                    End While
                End Using
            End Using
        End Using

        Return logs
    End Function

    <WebMethod>
    Public Shared Function ObtenerUltimoLogVehiculo() As Log
        Dim log As New Log()
        Dim query As String = "
        SELECT TOP 1 V.Id_Log_Vehicular, V.Placa, V.Fecha_Hora_Evento, 
               CASE WHEN V.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, 
               A.Nombre_Acceso, 
               CASE WHEN VE.Placa IS NOT NULL THEN 1 ELSE 0 END AS Registrado 
        FROM Logs_Vehicular V 
        LEFT JOIN Adm_Accesos A ON A.Id_Acceso = V.Id_Acceso 
        LEFT JOIN Adm_Vehiculo VE ON VE.Placa = V.Placa 
        ORDER BY V.Id_Log_Vehicular DESC"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        log.Id_Log = reader("Id_Log_Vehicular").ToString()
                        log.Nombres = reader("Placa").ToString()
                        log.Fecha_Hora_Evento = reader("Fecha_Hora_Evento").ToString()
                        log.Tipo_Evento = reader("Tipo_Evento").ToString()
                        log.Nombre_Acceso = reader("Nombre_Acceso").ToString()
                        log.FotoUrl = "/Diseno/Imagenes/Carro.jpg"
                        log.Registrado = Convert.ToBoolean(reader("Registrado"))
                    End If
                End Using
            End Using
        End Using

        Return log
    End Function

    Protected Sub TxTipoVehiculo_TextChanged(sender As Object, e As EventArgs) Handles TxTipoVehiculo.TextChanged
        If TxTipoVehiculo.SelectedValue = "1" Or TxTipoVehiculo.SelectedValue = "2" Or TxTipoVehiculo.SelectedValue = "3" Then
            TxPlaca.Enabled = True
            TxPlaca.Attributes("required") = "1"
            lblPlacaError.Style("display") = "none"
        Else
            TxPlaca.Enabled = False
            TxPlaca.Text = ""
            TxPlaca.Attributes.Remove("required")
            lblPlacaError.Style("display") = "none"
            BtGuardar.Enabled = True
        End If
    End Sub

    Protected Sub TxPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxPlaca.TextChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue
        Dim placa As String = TxPlaca.Text.Trim()
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
        Else
            lblPlacaError.Text = "Placa inválida. " & GetValidationMessage(tipoVehiculo)
            lblPlacaError.Attributes("style") = "display:block; margin-top: 60px;"
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
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Guardar_Persona()
        Dim sql As String = "INSERT INTO dbo.Terceros (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Usuario, Password, Foto, Id_Sede, Estado) " &
                        "VALUES (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Usuario, @Password, @Foto, @Id_Sede, @Estado)"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                Dim idHikCamara As Object = Session("Id_HikCamara")
                If idHikCamara IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(idHikCamara.ToString()) Then
                    cmd.Parameters.AddWithValue("@Id_HikCamara", idHikCamara)
                Else
                    cmd.Parameters.AddWithValue("@Id_HikCamara", DBNull.Value)
                End If

                If Not String.IsNullOrEmpty(TxCedulaVisitante.Text) Then
                    cmd.Parameters.AddWithValue("@Cedula", TxCedulaVisitante.Text)
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
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
        Dim Valor As Integer = ObtenerMaxId("Id_Visita", "Adm_Visita")
        Session("Id_Visita") = Valor + 1
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

    Private Sub Registrar_Log_Entrada()
        Dim idTercero_Visitante As Long = ObtenerIdTercero(Convert.ToInt64(TxCedulaVisitante.Text))

        Dim sql As String = "INSERT INTO dbo.Logs (" &
                         "Id_Log, Id_Tercero, Fecha_Hora_Evento, Tipo_Evento, Id_Acceso) VALUES (" &
                         "@Id_Log, @Id_Tercero, @Fecha_Hora_Evento, @Tipo_Evento, @Id_Acceso)"

        ' Conexión y comandos SQL
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            ' Abrir la conexión
            conn.Open()

            ' Iniciar una transacción para asegurar la consistencia y el bloqueo exclusivo
            Using trans As SqlTransaction = conn.BeginTransaction()
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id_Log), 0) + 1 FROM dbo.Logs WITH (UPDLOCK, HOLDLOCK)", conn, trans)
                    ' Ejecutar la consulta para obtener el nuevo Id_Visita
                    Dim newIdLog As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    ' Ahora que tenemos el nuevo Id_Visita, procedemos con la inserción
                    Using insertCmd As New SqlCommand(sql, conn, trans)
                        insertCmd.Parameters.AddWithValue("@Id_Log", newIdLog)
                        insertCmd.Parameters.AddWithValue("@Id_Tercero", idTercero_Visitante)
                        insertCmd.Parameters.AddWithValue("@Fecha_Hora_Evento", Date.Now)
                        insertCmd.Parameters.AddWithValue("@Tipo_Evento", 1)
                        insertCmd.Parameters.AddWithValue("@Id_Acceso", Session("Acceso_Usuario"))

                        ' Ejecutar el comando de inserción
                        insertCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Confirmar la transacción si todo se ejecutó correctamente
                trans.Commit()
            End Using
        End Using
    End Sub

    Dim errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatosVisitantes() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 1 AND Visitante = 1")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras:" & ex.Message)
        End Try
        Return ipCamaras
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

    Private Sub Consultar_Id_HikCamara()
        Dim Valor As Integer = ObtenerMaxId("Id_HikCamara", "Terceros")
        Session("Id_HikCamara") = Valor + 1
    End Sub

    Private Sub Consultar_Id_Empresa()
        Dim Valor As Integer = ObtenerMaxId("Id_Empresa", "Empresas")
        Session("Id_Empresa") = Valor + 1
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

    Private Sub Crear_Usuario_Faciales_Visitante()

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatosVisitantes()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""name"": """ & TxNombreVisitante.Text.ToUpper() & """,
              ""userType"": ""normal"",
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": ""1"",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
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
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al crear usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Agregar_Foto_Persona_Base64()
        ' Validar base64
        If String.IsNullOrWhiteSpace(base64image.Value) Then
            errores.Add("No hay imagen disponible en base64 para enviar a la cámara.")
            Exit Sub
        End If

        ' Extraer solo los datos de la imagen si hay encabezado
        Dim base64Data As String = base64image.Value.Split(","c).Last()
        Dim imageBytes As Byte()

        Try
            imageBytes = Convert.FromBase64String(base64Data)
        Catch ex As Exception
            errores.Add("Error al convertir la imagen base64: " & ex.Message)
            Exit Sub
        End Try

        ' Intentar reducir tamaño si es necesario
        Dim imagenReducida As Byte() = ReducirImagenAJpegMenos200KB(imageBytes)
        If imagenReducida Is Nothing OrElse imagenReducida.Length > 200000 Then
            errores.Add("La imagen no pudo ser reducida a menos de 200 KB. Verifica que sea una imagen nítida de rostro.")
            Exit Sub
        End If

        ' Obtener IPs
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatosVisitantes()

        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Parte 1: JSON
                Dim formData As New StringBuilder()
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
                ""faceLibType"": ""blackFD"",
                ""FDID"": ""1"",
                ""FPID"": """ & Session("Id_HikCamara").ToString() & """
            }")

                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: Imagen
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                     "Content-Disposition: form-data; name=""FaceImage""; filename=""imagen.jpg""" & vbCrLf &
                                                     "Content-Type: image/jpeg" & vbCrLf & vbCrLf,
                                                     boundary)

                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imagenReducida.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imagenReducida, 0, imagenReducida.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Leer respuesta
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Base64") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Base64") = 0
                errores.Add("Se creó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Function ReducirImagenAJpegMenos200KB(originalBytes As Byte()) As Byte()
        Try
            Using msOriginal As New MemoryStream(originalBytes)
                Using imagen As System.Drawing.Image = System.Drawing.Image.FromStream(msOriginal)

                    ' 🔎 Revisar si es demasiado pequeña
                    Dim anchoOriginal As Integer = imagen.Width
                    Dim altoOriginal As Integer = imagen.Height

                    ' Mínimo recomendado por Hikvision
                    Dim anchoDeseado As Integer = 480
                    Dim altoDeseado As Integer = 640

                    Dim nuevoAncho As Integer = anchoOriginal
                    Dim nuevoAlto As Integer = altoOriginal

                    ' Si es más pequeño que el mínimo, escalar hacia arriba
                    If anchoOriginal < anchoDeseado Or altoOriginal < altoDeseado Then
                        Dim escalaAncho As Double = anchoDeseado / anchoOriginal
                        Dim escalaAlto As Double = altoDeseado / altoOriginal
                        Dim escalaFinal As Double = Math.Max(escalaAncho, escalaAlto) ' Escalar al más grande

                        nuevoAncho = CInt(anchoOriginal * escalaFinal)
                        nuevoAlto = CInt(altoOriginal * escalaFinal)
                    Else
                        ' Si ya es grande, escalar hacia abajo si supera 640x800
                        Dim maxAncho As Integer = 640
                        Dim maxAlto As Integer = 800
                        Dim escala As Double = Math.Min(maxAncho / anchoOriginal, maxAlto / altoOriginal)
                        If escala < 1 Then
                            nuevoAncho = CInt(anchoOriginal * escala)
                            nuevoAlto = CInt(altoOriginal * escala)
                        End If
                    End If

                    ' 📐 Crear bitmap reescalado
                    Using bmpRedimensionado As New Bitmap(imagen, nuevoAncho, nuevoAlto)
                        Dim codec As Imaging.ImageCodecInfo = Imaging.ImageCodecInfo.GetImageEncoders().First(Function(c) c.MimeType = "image/jpeg")
                        Dim parametros As New Imaging.EncoderParameters(1)

                        ' Intentar diferentes calidades hasta lograr < 200 KB
                        For calidad As Long = 90 To 30 Step -10
                            Using msReducida As New MemoryStream()
                                parametros.Param(0) = New Imaging.EncoderParameter(Imaging.Encoder.Quality, calidad)
                                bmpRedimensionado.Save(msReducida, codec, parametros)

                                If msReducida.Length < 200000 Then
                                    Return msReducida.ToArray()
                                End If
                            End Using
                        Next
                    End Using
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al redimensionar/comprimir la imagen: " & ex.Message)
        End Try

        Return Nothing
    End Function

    Private Sub Actualizar_Id_Hikvision(Id_HikCamara As String, Id_Tercero As String)
        Dim sql As String = "UPDATE Terceros SET Estado = 1, Id_HikCamara = @Id WHERE Id_Tercero = @Id_Tercero"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id", Id_HikCamara)
                cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

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
        ElseIf Consultar_Control_Visitantes() AndAlso photo.Src = "/Diseno/Imagenes/Blanco.jpg" AndAlso String.IsNullOrEmpty(base64image.Value) Then
            Dim script As String = String.Format("swal('OJO!', 'Debe tomar foto a la persona', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        Else
            If TxCedulaVisitante.Text <> "" Then
                If Not RegistroExisteEnBD(TxCedulaVisitante.Text) Then
                    Consultar_Id_Persona()
                    If Consultar_Control_Visitantes() Then
                        Consultar_Id_HikCamara()
                    End If
                    Guardar_Persona()
                    If Consultar_Control_Visitantes() Then
                        Crear_Usuario_Faciales_Visitante()
                        Agregar_Foto_Persona_Base64()
                    End If
                Else
                    If Consultar_Control_Visitantes() Then
                        Dim IdTercero As Long = ObtenerIdTercero(TxCedulaVisitante.Text.Trim())
                        Consultar_Id_HikCamara()
                        Actualizar_Id_Hikvision(Session("Id_HikCamara").ToString, IdTercero)
                        Crear_Usuario_Faciales_Visitante()
                        Agregar_Foto_Persona_Base64()
                    End If
                    If Not String.IsNullOrEmpty(base64image.Value) Then
                        Actualizar_Foto()
                    End If
                End If
            End If
            If Not Consultar_Control_Visitantes() Then
                Consultar_Id_Visita()
                Registrar_Entrada()
                Registrar_Log_Entrada()
            End If

            If ErrorOp = Nothing Then
                Dim script As String = "swal('Excelente!', 'Visita Registrada Correctamente', 'success'); setTimeout(function(){ window.location.href = 'Registrar_Visita.aspx'; }, 3000);"
                ScriptManager.RegisterStartupScript(Page, GetType(Page), "redirect", script, True)
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                EnviarCorreoError()
            End If
        End If
    End Sub

    Private Sub TxBuscarResidente_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarResidente.TextChanged
        If TxBuscarResidente.Text = "" Then
            LvResidentes.DataSourceID = "SqlResidentes"
        Else
            LvResidentes.DataSourceID = "SqlBuscarResidentes"
        End If
        LvResidentes.DataBind()
    End Sub

End Class