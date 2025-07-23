Imports System.Data.SqlClient

Public Class Consultar_Llegada
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
        End If
    End Sub

    Private Sub BtBuscarAsignado_Click(sender As Object, e As EventArgs) Handles BtBuscarAsignado.Click
        If TxNombreAsignado.Text <> "" Then
            Dim Asignado As String = TxNombreAsignado.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT Placa, Nombre_Asignado FROM Adm_Vehiculo_Asignado_A WHERE Nombre_Asignado LIKE '%' + @Nombre_Asignado + '%' ORDER BY Nombre_Asignado DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Nombre_Asignado", Asignado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Asignado no encontrado', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub TxNombreAsignado_TextChanged(sender As Object, e As EventArgs) Handles TxNombreAsignado.TextChanged
        If TxNombreAsignado.Text <> "" Then
            Dim Asignado As String = TxNombreAsignado.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT Placa, Nombre_Asignado FROM Adm_Vehiculo_Asignado_A WHERE Nombre_Asignado LIKE '%' + @Nombre_Asignado + '%' ORDER BY Nombre_Asignado DESC"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Nombre_Asignado", Asignado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Asignado no encontrado', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub Consultar_Detalle(ByVal Placa As String)
        Sql_Detalle_Placa.SelectCommand = "SELECT L.Id_Log_Vehicular, L.Placa, L.Fecha_Hora_Evento, CASE WHEN L.Tipo_Evento = 1 THEN 'ENTRADA' ELSE 'SALIDA' END AS Tipo_Evento, A.Nombre_Acceso FROM Logs_Vehicular L LEFT JOIN Adm_Vehiculo V ON V.Placa = L.Placa LEFT JOIN Adm_Accesos A ON A.Id_Acceso = L.Id_Acceso WHERE CAST(L.Fecha_Hora_Evento AS DATE) = CAST(GETDATE() AS DATE) AND L.Placa = '" + Placa + "' ORDER BY L.Id_Log_Vehicular DESC"
        LvDetallePlaca.DataBind()
    End Sub

    Private Sub LvAsignados_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvAsignados.ItemCommand
        Dim PlacaLabel As Label = DirectCast(e.Item.FindControl("PlacaLabel"), Label)
        Dim Nombre_AsignadoLabel As Label = DirectCast(e.Item.FindControl("Nombre_AsignadoLabel"), Label)

        If e.CommandName = "Detalle" Then
            NombreEstudianteLabel.Text = Nombre_AsignadoLabel.Text
            Consultar_Detalle(PlacaLabel.Text)
        End If
    End Sub

End Class