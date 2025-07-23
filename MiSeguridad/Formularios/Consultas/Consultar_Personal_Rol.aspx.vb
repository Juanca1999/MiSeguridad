Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports ClosedXML.Excel
Imports System.Data.SqlClient

Public Class Consultar_Personal_Rol
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            LlenarListView()
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

    ' Clase para mapear los usuarios
    Public Class Usuarios
        Public Property employeeNo As String
        Public Property name As String
        Public Property userType As String
        Public Property gender As String
        Public Property numOfFace As Integer
    End Class

    Private Function GetUserDataTable() As DataTable
        Dim dtUsuarios As New DataTable()
        dtUsuarios.Columns.Add("employeeNo", GetType(String))
        dtUsuarios.Columns.Add("name", GetType(String))
        dtUsuarios.Columns.Add("userType", GetType(String))
        dtUsuarios.Columns.Add("gender", GetType(String))
        dtUsuarios.Columns.Add("numOfFace", GetType(Integer))

        Try
            Dim IP_EndPoint = ObtenerEndPoint()
            Dim HikvisionURL As String = "http://" & IP_EndPoint & "/ISAPI/AccessControl/UserInfo/Search?format=json"
            Dim maxResultsPerPage As Integer = 30
            Dim totalMatches As Integer = 0
            Dim position As Integer = 0

            Do
                Dim request As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                request.Method = "POST"
                request.ContentType = "application/json"
                request.Accept = "application/json"
                request.Credentials = New NetworkCredential("admin", "Miro-321")
                request.PreAuthenticate = False

                Dim jsonBody As String = "
            {
                ""UserInfoSearchCond"": {
                    ""searchID"": ""1"",
                    ""searchResultPosition"": " & position & ",
                    ""maxResults"": " & maxResultsPerPage & ",
                    ""EmployeeNoList"": []
                }
            }"

                Using writer As New StreamWriter(request.GetRequestStream())
                    writer.Write(jsonBody)
                    writer.Flush()
                End Using

                Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseBody As String = reader.ReadToEnd()
                    Dim result = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseBody)
                    Dim userInfoSearch = result("UserInfoSearch")

                    If totalMatches = 0 Then
                        totalMatches = CInt(userInfoSearch("totalMatches"))
                    End If

                    Dim userInfoList = JsonConvert.DeserializeObject(Of List(Of Usuarios))(userInfoSearch("UserInfo").ToString())

                    ' Llenar DataTable con los usuarios de la API
                    For Each usercito In userInfoList
                        dtUsuarios.Rows.Add(usercito.employeeNo, usercito.name, usercito.userType, usercito.gender, usercito.numOfFace)
                    Next

                    position += maxResultsPerPage
                End Using
            Loop While position < totalMatches

        Catch ex As Exception
            dtUsuarios.Clear()
        End Try

        Return dtUsuarios
    End Function

    ' Método para obtener los datos de SQL en DataTable
    Private Function GetDatabaseUsers() As DataTable
        Dim dtBD As New DataTable()

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Dim query As String = "
            SELECT 
                T.Id_HikCamara, T.Id_Tercero, T.Cedula, T.Nombres, T.Cargo, 
                R.Rol, S.Nombre_Sede, E.Empresa, 
                CASE WHEN T.Estado = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado 
            FROM Terceros T 
            LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol 
            LEFT JOIN Adm_sedes S ON S.Id_Sede = T.Id_Sede 
            LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa 
            ORDER BY R.Rol DESC"

            Using cmd As New SqlCommand(query, conn)
                Dim adapter As New SqlDataAdapter(cmd)
                adapter.Fill(dtBD)
            End Using
        End Using

        Return dtBD
    End Function

    Private Sub LlenarListView()
        Try
            Dim dtAPI As DataTable = GetUserDataTable()
            Dim dtBD As DataTable = GetDatabaseUsers()

            If dtBD Is Nothing OrElse dtBD.Rows.Count = 0 Then
                LvPersonalRol.DataSource = Nothing
                LvPersonalRol.DataBind()
                Exit Sub
            End If

            ' Agregar la columna numOfFace si no existe
            If Not dtBD.Columns.Contains("numOfFace") Then
                dtBD.Columns.Add("numOfFace", GetType(String))
            End If

            ' Diccionario para emparejar employeeNo con numOfFace
            Dim dictAPI As New Dictionary(Of String, Integer)
            For Each row As DataRow In dtAPI.Rows
                Dim employeeNo As String = row("employeeNo").ToString()
                Dim numOfFace As Integer = If(IsDBNull(row("numOfFace")), 0, Convert.ToInt32(row("numOfFace")))
                dictAPI(employeeNo) = numOfFace
            Next

            ' Asignar valores de numOfFace a los registros coincidentes con "SI" o "NO"
            For Each row As DataRow In dtBD.Rows
                Dim idHikCamara As String = row("Id_HikCamara").ToString()
                Dim numOfFaceValue As Integer = If(dictAPI.ContainsKey(idHikCamara), dictAPI(idHikCamara), 0)
                row("numOfFace") = If(numOfFaceValue > 0, "SI", "NO")
            Next

            ' Guardar en sesión
            Session("UsuariosDataTable") = dtBD

            ' Cargar la primera página
            Dim pager As DataPager = CType(LvPersonalRol.FindControl("DataPager1"), DataPager)
            If pager IsNot Nothing Then
                pager.SetPageProperties(0, pager.PageSize, True)
            End If

            ' Mostrar datos
            If dtBD.Rows.Count > 0 Then
                LvPersonalRol.DataSource = dtBD
                LvPersonalRol.DataBind()
            Else
                LvPersonalRol.DataSource = Nothing
                LvPersonalRol.DataBind()
            End If

        Catch ex As Exception
            LvPersonalRol.DataSource = Nothing
            LvPersonalRol.DataBind()
            Dim mensajeError As String = ex.Message.Replace("'", "")
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "ErrorScript", script, True)
        End Try
    End Sub

    Protected Sub LvPersonalRol_PagePropertiesChanging(sender As Object, e As PagePropertiesChangingEventArgs) Handles LvPersonalRol.PagePropertiesChanging
        Try
            ' Obtener el DataPager
            Dim pager As DataPager = CType(LvPersonalRol.FindControl("DataPager1"), DataPager)
            If pager IsNot Nothing Then
                pager.SetPageProperties(e.StartRowIndex, e.MaximumRows, False)
            End If

            ' Obtener los datos almacenados en la sesión
            Dim dtUsuarios As DataTable = TryCast(Session("UsuariosDataTable"), DataTable)

            ' Verificar si hay datos en sesión
            If dtUsuarios IsNot Nothing Then
                LvPersonalRol.DataSource = dtUsuarios
                LvPersonalRol.DataBind()
            Else
                LvPersonalRol.DataSource = Nothing
                LvPersonalRol.DataBind()
            End If

        Catch ex As Exception
            LvPersonalRol.DataSource = Nothing
            LvPersonalRol.DataBind()
            Dim mensajeError As String = ex.Message.Replace("'", "")
            Dim script As String = String.Format("swal('Error!', '{0}', 'warning');", mensajeError)
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "ErrorScript", script, True)
        End Try
    End Sub

    Protected Sub BtExportarExcel_Click(sender As Object, e As EventArgs) Handles BtExportarExcel.Click
        Try
            ' Recuperar los datos desde la sesión
            Dim dt As DataTable = TryCast(Session("UsuariosDataTable"), DataTable)

            ' Validar si hay datos
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "NoData", "alert('No hay datos para exportar.');", True)
                Return
            End If

            ' Crear una copia para no modificar la original
            Dim dtExport As DataTable = dt.Copy()

            ' Cambiar el nombre de la columna numOfFace a Tiene_Foto si existe
            If dtExport.Columns.Contains("numOfFace") Then
                dtExport.Columns("numOfFace").ColumnName = "Tiene_Foto"
            End If

            ' Eliminar la columna Id_HikCamara si existe
            If dtExport.Columns.Contains("Id_HikCamara") Then
                dtExport.Columns.Remove("Id_HikCamara")
            End If

            ' Crear un nuevo libro de Excel
            Using workbook As New XLWorkbook()
                Dim worksheet = workbook.Worksheets.Add("Personal Rol")

                ' Cargar los datos en la hoja
                worksheet.Cell(1, 1).InsertTable(dtExport, True)

                ' Formato de encabezados (negrita, fondo azul, texto blanco)
                Dim headerRow = worksheet.Range(1, 1, 1, dtExport.Columns.Count)
                headerRow.Style.Font.Bold = True
                headerRow.Style.Fill.BackgroundColor = XLColor.DarkBlue
                headerRow.Style.Font.FontColor = XLColor.White

                ' Ajustar ancho de columnas
                worksheet.Columns().AdjustToContents()

                ' Configurar la respuesta HTTP para la descarga
                Response.Clear()
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                Response.AddHeader("content-disposition", "attachment; filename=Personal_Rol.xlsx")

                ' Guardar el archivo en memoria y enviarlo
                Using memoryStream As New MemoryStream()
                    workbook.SaveAs(memoryStream)
                    memoryStream.WriteTo(Response.OutputStream)
                End Using

                Response.Flush()
                Response.End()
            End Using
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ExportError", String.Format("swal('Error!', '{0}', 'error');", ex.Message.Replace("'", "")), True)
        End Try
    End Sub

End Class