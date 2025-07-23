Imports System.IO
Imports ClosedXML.Excel
Imports System.Data.SqlClient

Public Class Consultar_Contratistas_Control
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
            Dim query As String = "SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Cargo, T.Id_Empresa, E.Empresa, R.Rol, S.Nombre_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, T.Id_HikCamara, CC.Fecha_Inicio, CC.Fecha_Fin, CC.Observaciones, CC.ARL, CC.Fecha_ARL, CC.Seguridad_Social, CC.Fecha_Seguridad_Social, CC.Trabajo_Alturas, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo, S.Nombre_Sede, CC.Estado FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T ON T.Cedula = CONVERT(NVARCHAR, CC.Cedula_Contratista) LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Terceros T2 ON T2.Id_Tercero = CC.Id_Quien_Autoriza LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Sedes S ON S.Id_Sede = CC.Id_Sede WHERE CC.Fecha_Inicio BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY CC.Id_Control_Contratistas DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvControlContratista.DataSourceID = "SqlControlContratista"
                            LvControlContratista.DataBind()
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
            Dim query As String = "SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Cargo, T.Id_Empresa, E.Empresa, R.Rol, S.Nombre_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, T.Id_HikCamara, CC.Fecha_Inicio, CC.Fecha_Fin, CC.Observaciones, CC.ARL, CC.Fecha_ARL, CC.Seguridad_Social, CC.Fecha_Seguridad_Social, CC.Trabajo_Alturas, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo, S.Nombre_Sede, CC.Estado FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T ON T.Cedula = CONVERT(NVARCHAR, CC.Cedula_Contratista) LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Terceros T2 ON T2.Id_Tercero = CC.Id_Quien_Autoriza LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Sedes S ON S.Id_Sede = CC.Id_Sede WHERE CC.Fecha_Inicio BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY CC.Id_Control_Contratistas DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Fecha_Inicio", TxFechaInicio.Text)
                    cmd.Parameters.AddWithValue("@Fecha_Fin", TxFechaFin.Text)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvControlContratista.DataSourceID = "SqlControlContratista"
                            LvControlContratista.DataBind()
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

    Protected Sub BtExportarExcel_Click(sender As Object, e As EventArgs) Handles BtExportarExcel.Click
        ' Obtener los datos de la base de datos
        Dim dv As DataView = CType(SqlControlContratista.Select(DataSourceSelectArguments.Empty), DataView)
        Dim dt As DataTable = dv.ToTable()

        ' Validar si hay datos
        If dt.Rows.Count = 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "NoData", "alert('No hay datos para exportar.');", True)
            Return
        End If

        ' Crear un nuevo libro de Excel
        Using workbook As New XLWorkbook()
            Dim worksheet = workbook.Worksheets.Add("Consulta Control Contratista")

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
            Response.AddHeader("content-disposition", "attachment; filename=Consulta_Control_Contratista.xlsx")

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