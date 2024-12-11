Imports System.IO
Imports System.Net
Imports System.Drawing
Imports Newtonsoft.Json
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Drawing.Imaging

Public Class Crear_Usuario_Camara
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        LlenarListView()
    End Sub

    Private Function ObtenerIdTercero(Cedula As Long) As Long
        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Cedula"
        Dim idTercero As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Cedula", Cedula)

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

    ' Clase para mapear los usuarios
    Public Class Usuarios
        Public Property employeeNo As String
        Public Property name As String
        Public Property userType As String
        Public Property gender As String
        Public Property numOfFace As Integer
    End Class

    Private Sub LlenarListView()
        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Search?format=json"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json"
            request.Credentials = New NetworkCredential("admin", "Miro-321")
            request.PreAuthenticate = False

            ' JSON que se enviará en el cuerpo
            Dim jsonBody As String = "
        {
            ""UserInfoSearchCond"": {
                ""searchID"": ""1"",
                ""searchResultPosition"": 0,
                ""maxResults"": 200,
                ""EmployeeNoList"": []
            }
        }"

            ' Escribir el JSON en el cuerpo de la solicitud
            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(jsonBody)
                writer.Flush()
            End Using

            ' Obtener y procesar la respuesta
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Using reader As New StreamReader(response.GetResponseStream())
                Dim responseBody As String = reader.ReadToEnd()

                ' Deserializar la respuesta
                Dim result = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseBody)
                Dim userInfoSearch = result("UserInfoSearch")

                ' Verificar si hay usuarios
                If userInfoSearch("numOfMatches") = 0 Then
                    ' No hay usuarios, limpiar y mostrar mensaje
                    LvUsuarios.DataSource = Nothing
                    LvUsuarios.DataBind()
                Else
                    ' Hay usuarios, deserializar y vincular
                    Dim userInfoList = JsonConvert.DeserializeObject(Of List(Of Usuarios))(userInfoSearch("UserInfo").ToString())
                    LvUsuarios.DataSource = userInfoList
                    LvUsuarios.DataBind()
                End If
            End Using
        Catch ex As UriFormatException
            ' Error específico de URI no válido
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String = "URI no válido: No se puede analizar el nombre de host."
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)

        Catch ex As WebException
            ' Error específico de solicitud web
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String
            If ex.Response IsNot Nothing Then
                Using reader As New StreamReader(ex.Response.GetResponseStream())
                    mensajeError = reader.ReadToEnd()
                End Using
            Else
                mensajeError = ex.Message
            End If
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)

        Catch ex As Exception
            ' Cualquier otro error general
            LvUsuarios.DataSource = Nothing
            LvUsuarios.DataBind()
            Dim mensajeError As String = ex.Message
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Dim errores As New List(Of String)

    Private Sub Consultar_Usuario(ByVal Id As Long)
        Dim query As String = "SELECT Id_Tercero, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Foto, Estado, Id_Sede, Id_Acceso, Id_HikCamara FROM Terceros " &
                          "WHERE Id_HikCamara = @Id"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Id", Id)

                conn.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        TxCedula.Text = dr("Cedula").ToString()
                        TxNombres.Text = dr("Nombres").ToString()
                        TxCorreo.Text = If(dr("Correo") IsNot DBNull.Value, dr("Correo").ToString(), String.Empty)
                        TxUsuario.Text = If(dr("Usuario") IsNot DBNull.Value, dr("Usuario").ToString(), String.Empty)
                        TxPassword.Text = If(dr("Password") IsNot DBNull.Value, dr("Password").ToString(), String.Empty)
                        TxCargo.Text = If(dr("Cargo") IsNot DBNull.Value, dr("Cargo").ToString(), String.Empty)
                        TxIdRol.SelectedValue = If(dr("Id_Rol") IsNot DBNull.Value, dr("Id_Rol").ToString(), String.Empty)
                        TxCelular.Text = If(dr("Celular") IsNot DBNull.Value, dr("Celular").ToString(), String.Empty)

                        If dr("Estado").ToString() = "True" Then
                            TxEstado.SelectedValue = "1"
                        ElseIf dr("Estado").ToString() = "False" Then
                            TxEstado.SelectedValue = "0"
                        Else
                            TxEstado.SelectedValue = String.Empty
                        End If
                        TxSede.SelectedValue = If(dr("Id_Sede") IsNot DBNull.Value, dr("Id_Sede").ToString(), String.Empty)
                        BtGuardar.Text = "ACTUALIZAR"
                    Else
                        TxCedula.Text = String.Empty
                        TxNombres.Text = String.Empty
                        TxCorreo.Text = String.Empty
                        TxPassword.Text = String.Empty
                        TxCargo.Text = String.Empty
                        TxIdRol.SelectedValue = String.Empty
                        TxCelular.Text = String.Empty
                        TxEstado.SelectedValue = String.Empty
                        TxSede.SelectedIndex = String.Empty
                    End If
                End Using
            End Using
        End Using
    End Sub

    Public Shared Function ObtenerEndPoint() As String
        Dim IP_EndPoint As String = String.Empty

        Dim sql As String = "SELECT TOP 1 IP_Camara FROM Adm_Accesos ORDER BY Id_Acceso ASC"

        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
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

    <WebMethod>
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
                Return "Foto no encontrada."
            End If
        Catch ex As Exception
            ' Retornar mensaje de error
            Return "Error: " & ex.Message
        End Try
    End Function

    Private Sub LvUsuarios_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvUsuarios.ItemDataBound
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim genderLabel As Label = DirectCast(e.Item.FindControl("genderLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Dim numOfFaceLabel As Label = DirectCast(e.Item.FindControl("numOfFaceLabel"), Label)
        Dim Foto As LinkButton = DirectCast(e.Item.FindControl("Foto"), LinkButton)

        If Convert.ToInt32(numOfFaceLabel.Text) > 0 Then
            Foto.Visible = True
        Else
            Foto.Visible = False
        End If
    End Sub

    Private Sub LvUsuarios_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvUsuarios.ItemCommand
        Dim employeeNoLabel As Label = DirectCast(e.Item.FindControl("employeeNoLabel"), Label)
        Dim nameLabel As Label = DirectCast(e.Item.FindControl("nameLabel"), Label)
        Dim genderLabel As Label = DirectCast(e.Item.FindControl("genderLabel"), Label)
        Dim userTypelabel As Label = DirectCast(e.Item.FindControl("userTypelabel"), Label)
        Dim numOfFaceLabel As Label = DirectCast(e.Item.FindControl("numOfFaceLabel"), Label)
        Session("Id_Empleado") = employeeNoLabel.Text
        If e.CommandName = "Editar" Then
            TxTipoUsuario.SelectedValue = userTypelabel.Text
            TxGenero.SelectedValue = genderLabel.Text
            Consultar_Usuario(employeeNoLabel.Text)
        End If
    End Sub

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos")

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

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Consultar_Id_HikCamara()
        Dim Valor As Integer = ObtenerMaxId("Id_HikCamara", "Terceros")
        Session("Id_HikCamara") = Valor + 1
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
        (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                    cmd.Parameters.AddWithValue("@Cedula", If(String.IsNullOrWhiteSpace(TxCedula.Text), DBNull.Value, TxCedula.Text))
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", If(TxIdRol.SelectedValue Is Nothing, DBNull.Value, TxIdRol.SelectedValue))
                    cmd.Parameters.AddWithValue("@Correo", If(String.IsNullOrWhiteSpace(TxCorreo.Text), DBNull.Value, TxCorreo.Text))
                    cmd.Parameters.AddWithValue("@Usuario", If(String.IsNullOrWhiteSpace(TxUsuario.Text), DBNull.Value, TxUsuario.Text))
                    cmd.Parameters.AddWithValue("@Password", If(String.IsNullOrWhiteSpace(TxPassword.Text), DBNull.Value, TxPassword.Text))
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(TxCargo.Text), DBNull.Value, TxCargo.Text))
                    cmd.Parameters.AddWithValue("@Celular", If(String.IsNullOrWhiteSpace(TxCelular.Text), DBNull.Value, TxCelular.Text))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(TxSede.SelectedValue Is Nothing, DBNull.Value, TxSede.SelectedValue))
                    cmd.Parameters.AddWithValue("@Estado", TxEstado.SelectedValue)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

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

    Private Sub Crear_Usuario()

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

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
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & TxTipoUsuario.SelectedValue & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2025-12-31T23:59:59""
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

    Private Sub Agregar_Foto_Persona_Archivo()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_HikCamara").ToString & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: {2}" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  FlFoto.FileName,
                                                  FlFoto.PostedFile.ContentType)

                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo cargado en FileUpload en un byte array
                Dim imageBytes As Byte() = FlFoto.FileBytes

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Archivo") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Archivo") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Sub Agregar_Foto_Persona_Tomada()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_HikCamara").ToString() & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: image/png" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  Path.GetFileName(Session("RutaImagen").ToString()))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo desde la ruta guardada
                Dim rutaArchivo As String = Session("RutaImagen").ToString()
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaArchivo)

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Tomada") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Tomada") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Sub Actualizar_Foto_Persona_Archivo()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FDSetUp?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_Empleado").ToString & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: {2}" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  FlFoto.FileName,
                                                  FlFoto.PostedFile.ContentType)

                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo cargado en FileUpload en un byte array
                Dim imageBytes As Byte() = FlFoto.FileBytes

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Actualizar_Archivo") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Actualizar_Archivo") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Sub Actualizar_Foto_Persona_Tomada()
        ' Obtener las IPs de las cámaras desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        ' Iterar a través de las IPs obtenidas
        For Each ipCamara As String In ipCamaras
            Try
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/Intelligent/FDLib/FDSetUp?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' Generar el boundary para separar las partes del form-data
                Dim boundary As String = "------------------------" & DateTime.Now.Ticks.ToString("x")
                requestDocument.ContentType = "multipart/form-data; boundary=" & boundary

                ' Construir el contenido form-data
                Dim formData As New StringBuilder()

                ' Parte 1: FaceDataRecord (tipo texto)
                formData.AppendLine("--" & boundary)
                formData.AppendLine("Content-Disposition: form-data; name=""FaceDataRecord""")
                formData.AppendLine("Content-Type: application/json")
                formData.AppendLine()
                formData.AppendLine("{
            ""faceLibType"": ""blackFD"",
            ""FDID"": ""1"",
            ""FPID"": """ & Session("Id_Empleado").ToString() & """
        }")

                ' Convertir la parte inicial a bytes
                Dim formDataBytes As Byte() = Encoding.UTF8.GetBytes(formData.ToString())

                ' Parte 2: FaceImage (archivo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf &
                                                  "Content-Disposition: form-data; name=""FaceImage""; filename=""{1}""" & vbCrLf &
                                                  "Content-Type: image/png" & vbCrLf & vbCrLf,
                                                  boundary,
                                                  Path.GetFileName(Session("RutaImagen").ToString()))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                ' Leer el archivo desde la ruta guardada
                Dim rutaArchivo As String = Session("RutaImagen").ToString()
                Dim imageBytes As Byte() = File.ReadAllBytes(rutaArchivo)

                ' Parte final del formulario
                Dim boundaryBytes As Byte() = Encoding.UTF8.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Combinar todo el contenido en un solo flujo
                requestDocument.ContentLength = formDataBytes.Length + fileHeaderBytes.Length + imageBytes.Length + boundaryBytes.Length

                Using requestStream As Stream = requestDocument.GetRequestStream()
                    ' Escribir el contenido del formulario
                    requestStream.Write(formDataBytes, 0, formDataBytes.Length)
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                    requestStream.Write(imageBytes, 0, imageBytes.Length)
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                End Using

                ' Obtener la respuesta del servidor
                Using response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseText As String = streamReader.ReadToEnd()
                        ' Analizar la respuesta JSON si es necesario
                        Dim jsonResponse As JObject = JObject.Parse(responseText)
                        Session("Status_Code_Actualizar_Tomada") = jsonResponse("statusCode").ToString()
                    End Using
                End Using

            Catch ex As Exception
                Session("Status_Code_Actualizar_Tomada") = 0
                errores.Add("Se créó el usuario pero no se pudo agregar la foto en la cámara " & ipCamara & ".")
            End Try
        Next
    End Sub

    Private Sub GuardarImagen()
        Try
            ' Obtener la cadena Base64
            Dim base64Foto As String = base64image.Value

            ' Validar si la imagen corresponde al valor por defecto
            If String.IsNullOrEmpty(base64Foto) Then
                Throw New Exception("No se puede guardar la imagen predeterminada. Seleccione una imagen válida.")
            End If

            ' Definir la ruta de la carpeta
            Dim rutaCarpeta As String = Server.MapPath("../../Adjunto/Foto_Usuarios/")

            ' Nombre único para el archivo
            Dim nombreArchivo As String = TxCedula.Text & ".png"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, nombreArchivo)

            ' Verificar si el archivo ya existe
            If File.Exists(rutaArchivo) Then
                ' Eliminar el archivo existente antes de guardar el nuevo
                File.Delete(rutaArchivo)
            End If

            ' Convertir Base64 a archivo y guardar temporalmente
            Dim imageBytes As Byte() = Convert.FromBase64String(base64Foto.Replace("data:image/png;base64,", ""))
            Dim rutaArchivoTemp As String = Path.Combine(rutaCarpeta, "temp_" & nombreArchivo)
            File.WriteAllBytes(rutaArchivoTemp, imageBytes)

            ' Reducir el peso de la imagen y guardar la versión optimizada
            ReducirPesoImagen(rutaArchivoTemp, rutaArchivo)

            ' Eliminar la imagen temporal
            If File.Exists(rutaArchivoTemp) Then
                File.Delete(rutaArchivoTemp)
            End If

            ' Guardar la ruta del archivo en la sesión
            Session("RutaImagen") = rutaArchivo
        Catch ex As Exception
            errores.Add("Error en GuardarImagen()" & ex.Message)
        End Try
    End Sub

    Private Sub Editar_Usuario()
        Try
            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

            For Each ipCamara As String In ipCamaras
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_Empleado").ToString & """,
              ""name"": """ & TxNombres.Text.ToUpper() & """,
              ""gender"": """ & TxGenero.SelectedValue & """,
              ""userType"": """ & TxTipoUsuario.SelectedValue & """,
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2025-12-31T23:59:59""
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
                    Session("Status_Code_Editar") = jsonResponse("statusCode").ToString()
                End Using
            Next

        Catch ex As Exception
            Session("Status_Code_Editar") = 0
            errores.Add("Error en Editar_Usuario()" & ex.Message)
        End Try
    End Sub

    Private Sub Eliminar_Usuario(ByVal Id As String)
        Try
            ' Obtener las IPs de las cámaras desde la base de datos
            Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

            For Each ipCamara As String In ipCamaras
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Delete?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfoDelCond"": {
              ""EmployeeNoList"": [
                 {
                    ""employeeNo"": """ & Id & """
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
                    Session("Status_Code_Eliminar") = jsonResponse("statusCode").ToString()
                End Using
            Next
        Catch ex As Exception
            errores.Add("Error en Eliminar_Usuario()" & ex.Message)
        End Try
    End Sub

    Private Sub Inactivar_Persona(ByVal Id As Long)
        Try
            Dim query As String = "UPDATE Terceros SET Estado = 0, Id_HikCamara = NULL WHERE Id_HikCamara = @Id"

            ' Configuración de la conexión a la base de datos
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id", Id)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error en Inactivar_Persona()" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Persona()
        Try
            Dim query As New System.Text.StringBuilder("UPDATE dbo.Terceros SET ")

            query.Append("Nombres = @Nombres, ")

            If Not String.IsNullOrEmpty(TxCorreo.Text) Then
                query.Append("Correo = @Correo, ")
            Else
                query.Append("Correo = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxIdRol.SelectedValue) Then
                query.Append("Id_Rol = @IdRol, ")
            Else
                query.Append("Id_Rol = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxUsuario.Text) Then
                query.Append("Usuario = @Usuario, ")
            Else
                query.Append("Usuario = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxPassword.Text) Then
                query.Append("Password = @Password, ")
            Else
                query.Append("Password = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxCargo.Text) Then
                query.Append("Cargo = @Cargo, ")
            Else
                query.Append("Cargo = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxCelular.Text) Then
                query.Append("Celular = @Celular, ")
            Else
                query.Append("Celular = NULL, ")
            End If

            If Not String.IsNullOrEmpty(TxSede.SelectedValue) Then
                query.Append("Id_Sede = @IdSede, ")
            Else
                query.Append("Id_Sede = NULL, ")
            End If

            query.Append("Estado = @Estado ")
            query.Append("WHERE Id_HikCamara = @Id_HikCamara")

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query.ToString(), conn)
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())

                    If Not String.IsNullOrEmpty(TxCorreo.Text) Then
                        cmd.Parameters.AddWithValue("@Correo", TxCorreo.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxIdRol.SelectedValue) Then
                        cmd.Parameters.AddWithValue("@IdRol", TxIdRol.SelectedValue)
                    End If

                    If Not String.IsNullOrEmpty(TxUsuario.Text) Then
                        cmd.Parameters.AddWithValue("@Usuario", TxUsuario.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxPassword.Text) Then
                        cmd.Parameters.AddWithValue("@Password", TxPassword.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxCargo.Text) Then
                        cmd.Parameters.AddWithValue("@Cargo", TxCargo.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxCelular.Text) Then
                        cmd.Parameters.AddWithValue("@Celular", TxCelular.Text)
                    End If

                    If Not String.IsNullOrEmpty(TxSede.SelectedValue) Then
                        cmd.Parameters.AddWithValue("@IdSede", TxSede.SelectedValue)
                    End If

                    cmd.Parameters.AddWithValue("@Estado", TxEstado.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_Empleado"))

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error en Actualizar_Persona()" & ex.Message)
        End Try
    End Sub

    Private Sub ReducirPesoImagen(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            Using imgOriginal As Bitmap = New Bitmap(rutaEntrada)
                ' Definir la calidad de compresión
                Dim codecInfo As ImageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg)
                Dim qualityParam As New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L) ' 80% de calidad
                Dim encoderParams As New EncoderParameters(1)
                encoderParams.Param(0) = qualityParam

                ' Guardar la imagen con el nivel de compresión especificado
                imgOriginal.Save(rutaSalida, codecInfo, encoderParams)
            End Using
        Catch ex As Exception
            errores.Add("Error en ReducirPesoImagen()" & ex.Message)
        End Try
    End Sub

    Private Function GetEncoderInfo(ByVal format As ImageFormat) As ImageCodecInfo
        For Each codec As ImageCodecInfo In ImageCodecInfo.GetImageEncoders()
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    Protected Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click
        ' Obtener el ID del empleado desde el HiddenField
        Dim idEmpleado As String = HiddenEmployeeNo.Value

        ' Validar que el ID no esté vacío
        If Not String.IsNullOrEmpty(idEmpleado) Then
            Eliminar_Usuario(idEmpleado)
            Inactivar_Persona(idEmpleado)

            If errores.Count = 0 And Session("Status_Code_Eliminar") = "1" Then
                Dim script As String = "swal('Excelente!', 'Usuario eliminado correctamente.', 'success');"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 3000
            Else
                Dim mensajeError As String = String.Join("<br/>", errores)
                Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            ' Manejo en caso de que no haya un valor
            Dim script As String = "swal('Error!', 'No se pudo identificar el empleado.', 'error');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then
            Dim usuarioCreado As Boolean = False
            Dim fotoAgregada As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            Dim IdTercero As Long = ObtenerIdTercero(TxCedula.Text)

            If IdTercero = 0 Then

                Consultar_Id_Persona()
                Consultar_Id_HikCamara()
                Guardar_Persona()
                Crear_Usuario()

                ' Verificar si el usuario fue creado
                If Session("Status_Code_Usuario") = "1" Then
                    usuarioCreado = True
                    mensaje = "Usuario creado correctamente."
                Else
                    mensaje = "Error al crear el usuario."
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Si el usuario fue creado, proceder con la foto
                If usuarioCreado Then
                    ' Verificar si se subió una foto desde un archivo
                    If FlFoto.HasFile Then
                        Agregar_Foto_Persona_Archivo()

                        ' Verificar el estado del proceso de agregar foto desde archivo
                        If Session("Status_Code_Archivo") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (archivo) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If

                        ' Verificar si se subió una foto desde imagen tomada (base64)
                    ElseIf base64image.Value <> "" Then
                        GuardarImagen()
                        Agregar_Foto_Persona_Tomada()

                        ' Verificar el estado del proceso de agregar foto tomada
                        If Session("Status_Code_Tomada") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (tomada) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto tomada."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If
                    End If

                    ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                    If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                        mensaje &= " OJO sin foto."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    End If
                End If

                ' Mostrar mensaje al usuario con SweetAlert
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 3000

            Else

                Consultar_Id_HikCamara()
                Actualizar_Id_Hikvision(Session("Id_HikCamara").ToString, IdTercero)
                Crear_Usuario()

                ' Verificar si el usuario fue creado
                If Session("Status_Code_Usuario") = "1" Then
                    usuarioCreado = True
                    mensaje = "Usuario creado correctamente."
                Else
                    mensaje = "Error al crear el usuario."
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                    Exit Sub
                End If

                ' Si el usuario fue creado, proceder con la foto
                If usuarioCreado Then
                    ' Verificar si se subió una foto desde un archivo
                    If FlFoto.HasFile Then
                        Agregar_Foto_Persona_Archivo()

                        ' Verificar el estado del proceso de agregar foto desde archivo
                        If Session("Status_Code_Archivo") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (archivo) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If

                        ' Verificar si se subió una foto desde imagen tomada (base64)
                    ElseIf base64image.Value <> "" Then
                        GuardarImagen()
                        Agregar_Foto_Persona_Tomada()

                        ' Verificar el estado del proceso de agregar foto tomada
                        If Session("Status_Code_Tomada") = "1" Then
                            fotoAgregada = True
                            mensaje = "Usuario con foto (tomada) creado correctamente."
                            script = $"swal('Excelente!', '{mensaje}', 'success');"
                        Else
                            mensaje &= " Sin embargo, ocurrió un error al agregar la foto tomada."
                            Dim detallesError As String = String.Join("<br>", errores)
                            script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                        End If
                    End If

                    ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                    If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                        mensaje &= " OJO sin foto."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    End If
                End If

                ' Mostrar mensaje al usuario con SweetAlert
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer1.Interval = 3000
            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            Dim usuarioActualizado As Boolean = False
            Dim fotoAgregada As Boolean = False
            Dim mensaje As String = String.Empty
            Dim script As String = String.Empty

            Actualizar_Persona()
            Editar_Usuario()

            If Session("Status_Code_Editar") = "1" Then
                usuarioActualizado = True
                mensaje = "Usuario actualizado correctamente."
                script = $"swal('Excelente!', '{mensaje}', 'success');"
            Else
                mensaje = " Ocurrió un error al actualizar el usuario."
                Dim detallesError As String = String.Join("<br>", errores)
                script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                mensaje = "Error al crear el usuario."
                Exit Sub
            End If

            ' Si el usuario fue actualizado, proceder con la foto
            If usuarioActualizado Then
                ' Verificar si se subió una foto desde un archivo
                If FlFoto.HasFile Then
                    Actualizar_Foto_Persona_Archivo()

                    ' Verificar el estado del proceso de agregar foto desde archivo
                    If Session("Status_Code_Actualizar_Archivo") = "1" Then
                        fotoAgregada = True
                        mensaje = "Usuario con foto (archivo) actualizado correctamente."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    Else
                        mensaje &= " Sin embargo, ocurrió un error al agregar la foto desde archivo."
                        Dim detallesError As String = String.Join("<br>", errores)
                        script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    End If

                    ' Verificar si se subió una foto desde imagen tomada (base64)
                ElseIf base64image.Value <> "" Then
                    GuardarImagen()
                    Actualizar_Foto_Persona_Tomada()

                    ' Verificar el estado del proceso de agregar foto tomada
                    If Session("Status_Code_Actualizar_Tomada") = "1" Then
                        fotoAgregada = True
                        mensaje = "Usuario con foto (tomada) actualizada correctamente."
                        script = $"swal('Excelente!', '{mensaje}', 'success');"
                    Else
                        mensaje &= " Sin embargo, ocurrió un error al actualizar la foto tomada."
                        Dim detallesError As String = String.Join("<br>", errores)
                        script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                    End If
                End If

                ' Si no se subió ninguna foto, solo mostrar mensaje de usuario creado
                If Not fotoAgregada AndAlso (base64image.Value = "" AndAlso Not FlFoto.HasFile) Then
                    mensaje &= " OJO sin foto."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                End If
            End If

            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Timer1.Interval = 3000
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Usuario_Camara.aspx")
    End Sub

End Class