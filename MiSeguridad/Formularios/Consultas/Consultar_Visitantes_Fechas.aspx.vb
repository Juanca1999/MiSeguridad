Imports System.IO
Imports ClosedXML.Excel
Imports System.Data.SqlClient

Public Class Consultar_Visitantes_Fechas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then

            ConsultarPermisos()

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

    Private Function ObtenerIdTercero() As Integer
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Rol del usuario actual
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Usuario"
        Dim usuario As String = Convert.ToInt64(HttpContext.Current.User.Identity.Name)
        Dim idTercero As Integer = 0

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Usuario", usuario)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Rol
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idTercero = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Rol obtenido
        Return idTercero
    End Function

    Private Sub ConsultarPermisos()

        Dim idTercero As Integer = ObtenerIdTercero()

        Dim query As String = "SELECT Id_submenu FROM Sys_Permisos WHERE Id_Tercero = " & idTercero & " "

        Try
            ' Create and open a connection
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                ' Create a command with the query
                Using command As New SqlCommand(query, connection)

                    ' Execute the command and use a data reader to read the results
                    Using dr As SqlDataReader = command.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                If dr("Id_submenu").ToString() = "1" Then
                                    BtSalidaMasiva.Enabled = True
                                End If
                            End While
                        End If
                    End Using

                End Using

            End Using

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Protected Sub TxFechaFin_TextChanged(sender As Object, e As EventArgs) Handles TxFechaFin.TextChanged
        If TxFechaInicio.Text <> "" And TxFechaFin.Text <> "" Then

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND V.Estado = 1 AND T.Id_Rol = 6 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
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
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes en las fechas asignadas', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub BtBuscarFecha_Click(sender As Object, e As EventArgs) Handles BtBuscarFecha.Click
        If TxFechaInicio.Text <> "" And TxFechaFin.Text <> "" Then

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND V.Estado = 1 AND T.Id_Rol = 6 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
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
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes en las fechas asignadas', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub LvVisitantes_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvVisitantes.ItemDataBound
        Dim Fecha_Inicio_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Inicio_VisitaLabel"), Label)
        Fecha_Inicio_VisitaLabel.Text = Mid(Fecha_Inicio_VisitaLabel.Text.ToString, 1, 10)
        Dim Fecha_Fin_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Fin_VisitaLabel"), Label)
        Fecha_Fin_VisitaLabel.Text = Mid(Fecha_Fin_VisitaLabel.Text.ToString, 1, 10)
    End Sub

    Private Sub BtSalir_Click(sender As Object, e As EventArgs) Handles BtSalir.Click
        If TxFechaInicio.Text <> "" And TxFechaFin.Text <> "" Then
            Dim query As String = "UPDATE V SET " &
                                  "V.Fecha_Fin_Visita = @Fecha_Fin_Visita, " &
                                  "V.Hora_fin_Visita = @Hora_fin_Visita, " &
                                  "V.Masivo = 1, " &
                                  "V.Estado = 0 " &
                                  "FROM dbo.Adm_Visita V INNER JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa " &
                                  "WHERE V.Id_Sede = @Sede AND V.Estado = 1 AND T.Id_Rol = 6 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin"

            Try
                Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                    Using command As New SqlCommand(query, connection)
                        command.Parameters.AddWithValue("@Fecha_Fin_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        command.Parameters.AddWithValue("@Hora_fin_Visita", Date.Now.ToString("HH:mm:ss"))
                        command.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                        command.Parameters.AddWithValue("@Fecha_Inicio", Convert.ToDateTime(TxFechaInicio.Text).ToString("yyyy-MM-dd"))
                        command.Parameters.AddWithValue("@Fecha_Fin", Convert.ToDateTime(TxFechaFin.Text).ToString("yyyy-MM-dd"))

                        connection.Open()
                        command.ExecuteNonQuery()
                    End Using
                End Using

                Dim script As String = String.Format("swal('Excelente!', 'Salida Masiva Registrada Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                LvVisitantes.DataBind()
                TxFechaInicio.Text = ""
                TxFechaFin.Text = ""
                Tabla.Visible = False
            Catch ex As Exception
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End Try
        End If
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
            Dim worksheet = workbook.Worksheets.Add("Visitantes Por Fechas")

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
            Response.AddHeader("content-disposition", "attachment; filename=Visitantes_Fechas.xlsx")

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