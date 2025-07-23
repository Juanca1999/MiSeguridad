Imports System.IO
Imports ClosedXML.Excel
Imports System.Data.SqlClient

Public Class Consultar_General
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

    Protected Sub TxFechaFin_TextChanged(sender As Object, e As EventArgs) Handles TxFechaFin.TextChanged
        If TxFechaInicio.Text <> "" And TxFechaFin.Text <> "" Then

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Quien_Autoriza = 'QR' THEN 'QR' ELSE 'NORMAL' END Tipo_Entrada, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida, C.Numero_Factura FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Adm_Accesos AE ON AE.Id_Acceso = V.Id_Acceso_Entrada LEFT JOIN Adm_Accesos ASA ON ASA.Id_Acceso = V.Id_Acceso_Salida LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Control_Restaurante C ON C.Cedula = T.Cedula AND C.Fecha_Activacion = V.Fecha_Inicio_Visita AND V.Quien_Autoriza = 'QR' WHERE V.Id_Sede = @Sede AND AE.Id_Inmueble = 'COMEDOR' AND T.Id_Rol <> 5 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvGeneral.DataSourceID = "SqlGeneral"
                            LvGeneral.DataBind()
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron personas en las fechas asignadas', 'warning');")
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
            Dim query As String = "SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Quien_Autoriza = 'QR' THEN 'QR' ELSE 'NORMAL' END Tipo_Entrada, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida, C.Numero_Factura FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Adm_Accesos AE ON AE.Id_Acceso = V.Id_Acceso_Entrada LEFT JOIN Adm_Accesos ASA ON ASA.Id_Acceso = V.Id_Acceso_Salida LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Control_Restaurante C ON C.Cedula = T.Cedula AND C.Fecha_Activacion = V.Fecha_Inicio_Visita AND V.Quien_Autoriza = 'QR' WHERE V.Id_Sede = @Sede AND AE.Id_Inmueble = 'COMEDOR' AND T.Id_Rol <> 5 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvGeneral.DataSourceID = "SqlGeneral"
                            LvGeneral.DataBind()
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron personas en las fechas asignadas', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub LvGeneral_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvGeneral.ItemDataBound
        Dim Fecha_Inicio_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Inicio_VisitaLabel"), Label)
        Fecha_Inicio_VisitaLabel.Text = Mid(Fecha_Inicio_VisitaLabel.Text.ToString, 1, 10)
        Dim Fecha_Fin_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Fin_VisitaLabel"), Label)
        Fecha_Fin_VisitaLabel.Text = Mid(Fecha_Fin_VisitaLabel.Text.ToString, 1, 10)
    End Sub

    Protected Sub BtExportarExcel_Click(sender As Object, e As EventArgs) Handles BtExportarExcel.Click
        ' Obtener los datos de la base de datos
        Dim dv As DataView = CType(SqlGeneral.Select(DataSourceSelectArguments.Empty), DataView)
        Dim dt As DataTable = dv.ToTable()

        ' Validar si hay datos
        If dt.Rows.Count = 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "NoData", "alert('No hay datos para exportar.');", True)
            Return
        End If

        ' Crear un nuevo libro de Excel
        Using workbook As New XLWorkbook()
            Dim worksheet = workbook.Worksheets.Add("Consulta General Por Fechas")

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
            Response.AddHeader("content-disposition", "attachment; filename=Consulta_General_Fechas.xlsx")

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