Imports System.IO
Imports System.Net
Imports ClosedXML.Excel
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class Consultar_Visitantes_Cedula
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

    Private Function Consultar_Control_Restaurante() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Control_Restaurante FROM Adm_Empresa"
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

    Protected Sub TxBuscarInmueble_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedula.TextChanged
        If TxBuscarCedula.Text <> "" Then
            ' Recuperar el valor ingresado en el TextBox
            Dim Cedula As String = TxBuscarCedula.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND T.Cedula = @Persona AND T.Id_Rol = 6 ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Persona", Cedula)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Traer_Datos(Cedula)
                            LvVisitantes.DataSourceID = "SqlVisitantes"
                            LvVisitantes.DataBind()
                            Tabla.Visible = True
                            If Consultar_Control_Restaurante() Then
                                ' Recorrer los elementos del ListView para mostrar las columnas ocultas
                                For Each item As ListViewDataItem In LvVisitantes.Items
                                    ' Encontrar el control Td_Restaurante en cada fila
                                    Dim tdRestaurante As HtmlTableCell = CType(item.FindControl("Td_Restaurante"), HtmlTableCell)
                                    If tdRestaurante IsNot Nothing Then
                                        tdRestaurante.Visible = True
                                    End If
                                Next

                                ' Encontrar el control Encabezado_Restaurante en el LayoutTemplate
                                Dim encabezadoRestaurante As HtmlTableCell = CType(LvVisitantes.FindControl("Encabezado_Restaurante"), HtmlTableCell)
                                If encabezadoRestaurante IsNot Nothing Then
                                    encabezadoRestaurante.Visible = True
                                End If
                            End If
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes en el numero de cedula', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub BtBuscarCedula_Click(sender As Object, e As EventArgs) Handles BtBuscarCedula.Click
        If TxBuscarCedula.Text <> "" Then
            Dim Cedula As String = TxBuscarCedula.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND T.Cedula = @Persona AND T.Id_Rol = 6 ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Persona", Cedula)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Traer_Datos(Cedula)
                            LvVisitantes.DataSourceID = "SqlVisitantes"
                            LvVisitantes.DataBind()
                            Tabla.Visible = True
                            If Consultar_Control_Restaurante() Then
                                ' Recorrer los elementos del ListView para mostrar las columnas ocultas
                                For Each item As ListViewDataItem In LvVisitantes.Items
                                    ' Encontrar el control Td_Restaurante en cada fila
                                    Dim tdRestaurante As HtmlTableCell = CType(item.FindControl("Td_Restaurante"), HtmlTableCell)
                                    If tdRestaurante IsNot Nothing Then
                                        tdRestaurante.Visible = True
                                    End If
                                Next

                                ' Encontrar el control Encabezado_Restaurante en el LayoutTemplate
                                Dim encabezadoRestaurante As HtmlTableCell = CType(LvVisitantes.FindControl("Encabezado_Restaurante"), HtmlTableCell)
                                If encabezadoRestaurante IsNot Nothing Then
                                    encabezadoRestaurante.Visible = True
                                End If
                            End If
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes en el numero de cedula', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub Traer_Datos(Cedula As Long)
        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Dim query As String = "SELECT Cedula, Nombres, Foto, Id_HikCamara FROM Terceros WHERE Cedula = @Cedula"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Cedula", Cedula)

                    ' Ejecuta el comando y utiliza un SqlDataReader para leer los datos
                    Using reader As SqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            ' Si encuentra datos, asigna a los controles los valores de las columnas
                            NombreLabel.Text = reader("Nombres").ToString()
                            CedulaLabel.Text = reader("Cedula").ToString()

                            ' Asegúrate de que Foto no sea DBNull antes de asignarla
                            If Not IsDBNull(reader("Foto")) Then
                                ' Asigna la URL o el contenido de la foto al control de la imagen
                                photo.Src = reader("Foto").ToString()
                            End If
                            If Not IsDBNull(reader("Id_HikCamara")) Then
                                Dim fotoUrl As String = Consultar_Foto(reader("Id_HikCamara").ToString())

                                ' Verificar si se obtuvo una URL válida
                                If Not String.IsNullOrEmpty(fotoUrl) AndAlso Not fotoUrl.StartsWith("Error:") Then
                                    photo.Src = fotoUrl
                                End If
                            End If
                        Else
                            ' Si no encuentra resultados, podrías manejarlo aquí, por ejemplo:
                            NombreLabel.Text = ""
                            CedulaLabel.Text = ""
                            photo.Src = "~/Diseno/Imagenes/Blanco.jpg"
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Aquí podrías registrar el error si lo necesitas
        End Try
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

    Private Sub LvVisitantes_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvVisitantes.ItemDataBound
        Dim Fecha_Inicio_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Inicio_VisitaLabel"), Label)
        Fecha_Inicio_VisitaLabel.Text = Mid(Fecha_Inicio_VisitaLabel.Text.ToString, 1, 10)
        Dim Fecha_Fin_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Fin_VisitaLabel"), Label)
        Fecha_Fin_VisitaLabel.Text = Mid(Fecha_Fin_VisitaLabel.Text.ToString, 1, 10)
    End Sub

    Protected Sub BtExportarExcel_Click(sender As Object, e As EventArgs) Handles BtExportarExcel.Click
        ' Obtener los datos de la base de datos
        Dim dv As DataView = CType(SqlVisitantes.Select(DataSourceSelectArguments.Empty), DataView)
        Dim dt As DataTable = dv.ToTable()

        ' Validar si hay datos
        If dt.Rows.Count = 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "NoData", "alert('No hay datos para exportar.');", True)
            Return
        End If

        If Not Consultar_Control_Restaurante() Then
            If dt.Columns.Contains("Tipo_Comida") Then
                dt.Columns.Remove("Tipo_Comida")
            End If
        End If

        ' Crear un nuevo libro de Excel
        Using workbook As New XLWorkbook()
            Dim worksheet = workbook.Worksheets.Add("Consulta Visitantes Cedula")

            ' Cargar los datos en la hoja
            worksheet.Cell(1, 1).InsertTable(dt, True)

            ' Formato de encabezados (negrita, fondo azul, texto blanco)
            Dim headerRow = worksheet.Range(1, 1, 1, dt.Columns.Count)
            headerRow.Style.Font.Bold = True
            headerRow.Style.Fill.BackgroundColor = XLColor.DarkBlue
            headerRow.Style.Font.FontColor = XLColor.White

            ' Ajustar ancho de columnas
            worksheet.Columns().AdjustToContents()

            ' Configurar la respuesta HTTP para la descarga
            Response.Clear()
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("content-disposition", "attachment; filename=Consulta_Visitantes_Cedula.xlsx")

            ' Guardar el archivo en memoria y enviarlo
            Using memoryStream As New MemoryStream()
                workbook.SaveAs(memoryStream)
                memoryStream.WriteTo(Response.OutputStream)
            End Using

            Response.Flush()
            Response.End()
        End Using
    End Sub

End Class