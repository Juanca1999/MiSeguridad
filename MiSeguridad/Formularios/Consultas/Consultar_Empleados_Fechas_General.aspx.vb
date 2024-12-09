Imports System.IO
Imports System.Data.SqlClient

Public Class Consultar_Empleados_Fechas_General
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
    End Sub

    Protected Sub TxFechaFin_TextChanged(sender As Object, e As EventArgs) Handles TxFechaFin.TextChanged
        If TxFechaInicio.Text <> "" And TxFechaFin.Text <> "" Then

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvEmpleados.DataSourceID = "SqlVisitantes"
                            LvEmpleados.DataBind()
                            Tabla.Visible = True
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
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvEmpleados.DataSourceID = "SqlVisitantes"
                            LvEmpleados.DataBind()
                            Tabla.Visible = True
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

    Private Sub LvEmpleados_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvEmpleados.ItemDataBound
        Dim Fecha_Inicio_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Inicio_VisitaLabel"), Label)
        Fecha_Inicio_VisitaLabel.Text = Mid(Fecha_Inicio_VisitaLabel.Text.ToString, 1, 10)
        Dim Fecha_Fin_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Fin_VisitaLabel"), Label)
        Fecha_Fin_VisitaLabel.Text = Mid(Fecha_Fin_VisitaLabel.Text.ToString, 1, 10)
    End Sub

    Protected Sub BtExportarExcel_Click(sender As Object, e As EventArgs) Handles BtExportarExcel.Click
        ' Encuentra el DataPager
        Dim pager As DataPager = TryCast(LvEmpleados.FindControl("DataPager1"), DataPager)

        ' Guarda el estado original de la paginación
        Dim originalPageSize = pager.PageSize
        Dim originalStartRowIndex = pager.StartRowIndex

        ' Desactiva la paginación
        pager.SetPageProperties(0, Integer.MaxValue, False)

        ' Actualiza el ListView
        LvEmpleados.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=Empleados_General_Por_Fecha.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            ' Ahora LvFacturacionGeneral incluirá todos los datos
            LvEmpleados.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End Using

        ' Restaura el estado original de la paginación
        pager.SetPageProperties(originalStartRowIndex, originalPageSize, False)

        ' Actualiza el ListView
        LvEmpleados.DataBind()
    End Sub

End Class