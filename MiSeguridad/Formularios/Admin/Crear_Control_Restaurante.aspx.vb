Imports QRCoder
Imports System.IO
Imports System.Net
Imports ImageMagick
Imports System.Drawing
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient
Imports System.Drawing.Imaging

Public Class Crear_Control_Restaurante
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

    Private ErrorOp As String

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

    Private Function RegistroExisteEnBD(Cedula As Long) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE Cedula = @Cedula AND Id_Rol = 6"
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

    Private Function Tiene_Facial(Cedula As Long) As Boolean
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT Id_HikCamara FROM Terceros WHERE Cedula = @Cedula AND Id_Rol = 6"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado es NULL o no
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

    Private Function ObtenerId_HikCamara(Cedula As Long) As Long
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

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Inmueble = 'COMEDOR'")

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
        Dim Valor As Integer = ObtenerMaxIdHikvisionQR("Id_HikCamara", "Adm_Control_Restaurante")
        Session("Id_HikCamara") = Valor + 1
    End Sub

    Private Sub Consultar_Id_Control()
        Dim Valor As Integer = ObtenerMaxId("Id_Control", "Adm_Control_Restaurante")
        Session("Id_Control") = Valor + 1
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

    Private Function ObtenerMaxIdHikvisionQR(campo As String, tabla As String) As Integer
        Dim Valor As Integer = 1000000
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
                    cmd.Parameters.AddWithValue("@Nombres", TxNombres.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", 6)
                    cmd.Parameters.AddWithValue("@Correo", If(String.IsNullOrWhiteSpace(TxCorreo.Text), DBNull.Value, TxCorreo.Text))
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Celular", If(String.IsNullOrWhiteSpace(TxCelular.Text), DBNull.Value, TxCelular.Text))
                    cmd.Parameters.AddWithValue("@Id_Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Guardar_Control_Restaurante()

        Dim cedula As String = TxCedula.Text.Trim()
        Dim fechaActivacion As Date
        Dim code As String = ""

        If Date.TryParse(TxFechaActivacion.Text, fechaActivacion) Then
            Dim fechaActivacionFormato As String = fechaActivacion.ToString("yyyyMMdd")
            code = $"{cedula}{fechaActivacionFormato}"
        End If

        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Control_Restaurante 
        (Id_Control, Cedula, Id_Quien_Autoriza, Fecha_Activacion, Cantidad_Accesos, Codigo_QR, Id_HikCamara, Numero_Factura) 
        VALUES 
        (@Id_Control, @Cedula, @Id_Quien_Autoriza, @Fecha_Activacion, @Cantidad_Accesos, @Codigo_QR, @Id_HikCamara, @Numero_Factura)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_Control", Session("Id_Control"))
                    cmd.Parameters.AddWithValue("@Cedula", TxCedula.Text.Trim())
                    cmd.Parameters.AddWithValue("@Id_Quien_Autoriza", TxNombrePersona.SelectedValue)
                    Dim Fecha_Act As Date = TxFechaActivacion.Text
                    cmd.Parameters.AddWithValue("@Fecha_Activacion", Fecha_Act.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@Cantidad_Accesos", TxCantidadAccesos.Text)
                    cmd.Parameters.AddWithValue("@Codigo_QR", code)
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                    cmd.Parameters.AddWithValue("@Numero_Factura", If(String.IsNullOrWhiteSpace(TxNumeroFactura.Text), DBNull.Value, TxNumeroFactura.Text))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD Guardar_Control_Restaurante():" & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Rol(Cedula As Long)
        Dim sql As String = "UPDATE Terceros SET Estado = 1, Id_Rol = 5 WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Cedula", Cedula)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Actualizar_Hikvision(Cedula As Long)
        Dim sql As String = "UPDATE Terceros SET Id_HikCamara = @Id_HikCamara WHERE Cedula = @Cedula AND Id_Rol = 6"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                cmd.Parameters.AddWithValue("@Cedula", Cedula)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub Crear_Usuario()

        ' Convertir las fechas de los controles a formato ISO 8601
        Dim fechaInicio As String = DateTime.Parse(TxFechaActivacion.Text).ToString("yyyy-MM-dd") & "T00:00:00"
        Dim fechaFin As String = DateTime.Parse(TxFechaActivacion.Text).ToString("yyyy-MM-dd") & "T23:59:59"

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
              ""userType"": ""normal"",
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": """ & fechaInicio & """,
                 ""endTime"": """ & fechaFin & """
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

    Private Sub Agregar_QR()

        Dim cedula As String = TxCedula.Text.Trim()
        Dim fechaActivacion As Date
        Dim code As String = ""

        If Date.TryParse(TxFechaActivacion.Text, fechaActivacion) Then
            Dim fechaActivacionFormato As String = fechaActivacion.ToString("yyyyMMdd")
            code = $"{cedula}{fechaActivacionFormato}"
        End If

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/CardInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""CardInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""cardNo"": """ & code & """,
              ""cardType"": ""normalCard""
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
                    Session("Status_Code_QR") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_QR") = 0
                errores.Add("Error al asignar qr al usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub GuardarImagen()
        Try
            ' Obtener la cadena Base64

            ' Definir la ruta de la carpeta
            Dim rutaCarpeta As String = Server.MapPath("../../Adjunto/Foto_Usuarios/")

            ' Nombre único para el archivo
            Dim nombreArchivo As String = TxCedula.Text.Trim() & ".png"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, nombreArchivo)

            ' Verificar si el archivo ya existe
            If File.Exists(rutaArchivo) Then
                ' Eliminar el archivo existente antes de guardar el nuevo
                File.Delete(rutaArchivo)
            End If

            ' Convertir Base64 a archivo y guardar temporalmente
            Dim rutaArchivoTemp As String = Path.Combine(rutaCarpeta, "temp_" & nombreArchivo)

            ' Reducir el peso de la imagen y guardar la versión optimizada
            ReducirPesoImagenTomada(rutaArchivoTemp, rutaArchivo)

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

    Public Sub ReducirPesoImagen(ByVal rutaEntrada As String, ByVal rutaSalida As String)
        Try
            ' Cargar la imagen con MagickImage
            Using imagen As New MagickImage(rutaEntrada)
                Dim maxAncho As UInteger = 800
                Dim maxAlto As UInteger = 800

                ' Redimensionar la imagen manteniendo la proporción (Fill, Fit, o ajusta según tu necesidad)
                imagen.Resize(New MagickGeometry(maxAncho, maxAlto) With {.IgnoreAspectRatio = False})

                ' Ajustar la calidad de la compresión
                imagen.Quality = 70 ' 70% de calidad

                ' Guardar la imagen comprimida
                imagen.Write(rutaSalida)
            End Using
        Catch ex As Exception
            errores.Add("Error en ReducirPesoImagen()" & ex.Message)
        End Try
    End Sub

    Private Sub ReducirPesoImagenTomada(ByVal rutaEntrada As String, ByVal rutaSalida As String)
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

    Private Sub Buscar_Persona(ByVal Usuario As String)
        Dim query As String = "SELECT Id_Tercero, Cedula, Nombres, Correo, Usuario, Password, Cargo, Celular, Id_Sede, Estado FROM Terceros WHERE Cedula = " & Usuario & ""

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.HasRows Then
                        dr.Read()

                        Session("Cedula") = Usuario.ToString()
                        TxNombres.Text = dr("Nombres").ToString()
                        TxCorreo.Text = If(dr("Correo").ToString() <> Nothing, dr("Correo").ToString(), String.Empty)
                        TxCelular.Text = If(dr("Celular").ToString() <> Nothing, dr("Celular").ToString(), String.Empty)
                    Else
                        Session("Id_Persona") = Nothing
                        TxNombres.Text = String.Empty
                        TxCorreo.Text = String.Empty
                        TxCelular.Text = String.Empty
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub Consultar_Tercero()
        Try
            sql = "SELECT Id_Tercero, Nombres FROM Terceros WHERE Id_Rol = 1 AND Nombres LIKE '%" + TxBuscarPersona.Text + "%' ORDER BY Nombres"
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
        End If
    End Sub

    Private Sub TxCedula_TextChanged(sender As Object, e As EventArgs) Handles TxCedula.TextChanged
        If TxCedula.Text <> Nothing Then
            Buscar_Persona(TxCedula.Text.Trim())
        End If
    End Sub

    Private Sub LvControlRestaurante_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvControlRestaurante.ItemDataBound
    End Sub

    Private Sub LvControlRestaurante_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvControlRestaurante.ItemCommand
    End Sub

    Private Sub Generar_QR()
        Dim cedula As String = TxCedula.Text.Trim()
        Dim fechaActivacion As Date
        Dim code As String = ""

        ' Intentar convertir la fecha ingresada al formato esperado
        If Date.TryParse(TxFechaActivacion.Text, fechaActivacion) Then
            ' Formatear la fecha como yyyyMMdd
            Dim fechaActivacionFormato As String = fechaActivacion.ToString("yyyyMMdd")

            ' Concatenar los valores
            code = $"{cedula}{fechaActivacionFormato}"

            ' Crear el generador de códigos QR
            Dim qrGenerator As New QRCodeGenerator()

            ' Generar los datos del código QR con nivel de corrección de errores Q
            Dim qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q)

            ' Crear el código QR a partir de los datos generados
            Dim qrCode As New QRCode(qrCodeData)

            ' Generar el código QR como un gráfico y convertirlo a Base64
            Using bitMap As Bitmap = qrCode.GetGraphic(20) ' 20 es el tamaño del píxel
                Using ms As New MemoryStream()
                    ' Guardar la imagen como PNG en el MemoryStream
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png)

                    ' Convertir el MemoryStream a un arreglo de bytes
                    Dim byteImage As Byte() = ms.ToArray()

                    ' Convertir los bytes a una cadena en formato Base64
                    Foto_QR.Src = "data:image/png;base64," + Convert.ToBase64String(byteImage)
                End Using
            End Using
        Else
            Foto_QR.Src = ""
        End If
    End Sub

    Private Sub BtAbrirQR_Click(sender As Object, e As EventArgs) Handles BtAbrirQR.Click

        Dim fechaActivacion As DateTime
        If Not DateTime.TryParse(TxFechaActivacion.Text, fechaActivacion) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('Por favor ingrese una fecha válida.');", True)
            Return
        End If

        ' Comparar solo las fechas (sin la hora)
        If fechaActivacion.Date < DateTime.Now.Date Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('La fecha de activación no puede ser menor a la fecha actual.');", True)
            Return
        End If

        ' Validar si el registro ya existe en la base de datos
        If ValidarRegistroExistente(fechaActivacion.Date, TxCedula.Text.Trim()) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('Ya existe un registro con esa fecha y cédula.');", True)
            Return
        End If

        Generar_QR()

        ' Abre el modal si el QR se generó correctamente
        If Not String.IsNullOrEmpty(Foto_QR.Src) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AbrirModal", "$('#Modal_QR').modal('show');", True)
        End If
    End Sub

    Private Function ValidarRegistroExistente(fechaActivacion As DateTime, cedula As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM Adm_Control_Restaurante WHERE Fecha_Activacion = @FechaActivacion AND Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@FechaActivacion", fechaActivacion) ' Solo fecha, sin hora
                cmd.Parameters.AddWithValue("@Cedula", cedula)

                conn.Open()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    Return True ' Registro encontrado
                End If
            End Using
        End Using

        Return False ' Registro no encontrado
    End Function

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Dim usuarioCreado As Boolean = False
        Dim QRAgregado As Boolean = False
        Dim mensaje As String = String.Empty
        Dim script As String = String.Empty

        If Not RegistroExisteEnBD(TxCedula.Text.Trim()) Then
            Consultar_Id_Persona()
            Consultar_Id_HikCamara()
            Guardar_Persona()
            Consultar_Id_Control()
            Guardar_Control_Restaurante()
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

            If usuarioCreado Then
                Agregar_QR()

                ' Verificar el estado del proceso de agregar qr
                If Session("Status_Code_QR") = "1" Then
                    QRAgregado = True
                    mensaje = "Usuario con QR creado correctamente."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                Else
                    mensaje &= " Sin embargo, ocurrió un error al agregar el QR"
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                End If
            End If

            ' Mostrar mensaje al usuario con SweetAlert
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Crear_Control_Restaurante.aspx")
        Else
            Consultar_Id_HikCamara()
            Consultar_Id_Control()
            Guardar_Control_Restaurante()
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

            If usuarioCreado Then
                Agregar_QR()

                ' Verificar el estado del proceso de agregar qr
                If Session("Status_Code_QR") = "1" Then
                    QRAgregado = True
                    mensaje = "Usuario con QR creado correctamente."
                    script = $"swal('Excelente!', '{mensaje}', 'success');"
                Else
                    mensaje &= " Sin embargo, ocurrió un error al agregar el QR"
                    Dim detallesError As String = String.Join("<br>", errores)
                    script = $"swal('Error!', '{mensaje}<br>Detalles del error:<br>{detallesError}', 'warning');"
                End If
            End If

            ' Mostrar mensaje al usuario con SweetAlert
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "3; URL=Crear_Control_Restaurante.aspx")
        End If

    End Sub

End Class