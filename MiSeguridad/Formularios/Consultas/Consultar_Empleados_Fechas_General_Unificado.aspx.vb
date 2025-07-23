Imports System.IO
Imports System.Data.SqlClient

Public Class Consultar_Empleados_Fechas_General_Unificado
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
            Dim query As String = "SELECT T.Cedula, T.Nombres, R.Rol, E.Empresa, V.Fecha_Inicio_Visita, MIN(V.Hora_Inicio_Visita) AS Hora_Inicio_Visita, V.Fecha_Inicio_Visita, MAX(V.Hora_Fin_Visita) AS Hora_Fin_Visita, CASE WHEN MAX(CAST(V.Estado AS INT)) = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END AS Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin AND V.Fecha_Inicio_Visita = V.Fecha_Fin_Visita GROUP BY T.Cedula, T.Nombres, R.Rol, E.Empresa, V.Fecha_Inicio_Visita ORDER BY T.Cedula, V.Fecha_Inicio_Visita;"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvEmpleados.DataSourceID = "SqlEmpleados"
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
            Dim query As String = "SELECT T.Cedula, T.Nombres, R.Rol, E.Empresa, V.Fecha_Inicio_Visita, MIN(V.Hora_Inicio_Visita) AS Hora_Inicio_Visita, V.Fecha_Inicio_Visita, MAX(V.Hora_Fin_Visita) AS Hora_Fin_Visita, CASE WHEN MAX(CAST(V.Estado AS INT)) = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END AS Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin AND V.Fecha_Inicio_Visita = V.Fecha_Fin_Visita GROUP BY T.Cedula, T.Nombres, R.Rol, E.Empresa, V.Fecha_Inicio_Visita ORDER BY T.Cedula, V.Fecha_Inicio_Visita;"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvEmpleados.DataSourceID = "SqlEmpleados"
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
        Response.AddHeader("content-disposition", "attachment;filename=Empleados_General_Por_Fecha_Unificado.xls")
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