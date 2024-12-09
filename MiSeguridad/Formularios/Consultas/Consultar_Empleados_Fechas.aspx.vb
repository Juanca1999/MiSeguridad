Imports System.IO
Imports System.Data.SqlClient

Public Class Consultar_Empleados_Fechas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            ConsultarPermisos()
        End If
    End Sub

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
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC"

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
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC"

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

    Private Sub BtSalir_Click(sender As Object, e As EventArgs) Handles BtSalir.Click
        If TxFechaInicio.Text <> "" And TxFechaFin.Text <> "" Then
            Dim query As String = "UPDATE dbo.Adm_Visita SET " &
                                  "Fecha_Fin_Visita = @Fecha_Fin_Visita, " &
                                  "Hora_fin_Visita = @Hora_fin_Visita, " &
                                  "Masivo = 1, " &
                                  "Estado = 0 " &
                                  "WHERE Estado = 1 AND T.Id_Rol = 8 AND Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin"

            Try
                Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                    Using command As New SqlCommand(query, connection)
                        command.Parameters.AddWithValue("@Fecha_Fin_Visita", Date.Now.ToString("yyyy-MM-dd"))
                        command.Parameters.AddWithValue("@Hora_fin_Visita", Date.Now.ToString("HH:mm:ss"))
                        command.Parameters.AddWithValue("@Fecha_Inicio", Convert.ToDateTime(TxFechaInicio.Text).ToString("yyyy-MM-dd"))
                        command.Parameters.AddWithValue("@Fecha_Fin", Convert.ToDateTime(TxFechaFin.Text).ToString("yyyy-MM-dd"))

                        connection.Open()
                        command.ExecuteNonQuery()
                    End Using
                End Using

                Dim script As String = String.Format("swal('Excelente!', 'Salida Masiva Registrada Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                LvEmpleados.DataBind()
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
        Response.AddHeader("content-disposition", "attachment;filename=Empleados_Sin_Salida_Por_Fecha.xls")
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