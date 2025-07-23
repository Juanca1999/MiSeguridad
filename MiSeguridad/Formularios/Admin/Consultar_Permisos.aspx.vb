Imports System.Data.SqlClient

Public Class Consultar_Permisos
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

            Session("Id_Tercero_Usuario") = ObtenerIdTercero(User.Identity.Name)
        End If
    End Sub

    Private ErrorOp As String

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

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            LvPermisos.DataSourceID = "SqlPermisoNombre"
        Else
            LvPermisos.DataSourceID = "SqlPermisos"
        End If
        LvPermisos.DataBind()
    End Sub

    Private Sub TxFecha_TextChanged(sender As Object, e As EventArgs) Handles TxFecha.TextChanged
        If TxFecha.Text <> "" Then
            LvPermisos.DataSourceID = "SqlPermisoFecha"
        Else
            LvPermisos.DataSourceID = "SqlPermisos"
        End If
        LvPermisos.DataBind()
    End Sub

    Private Function ObtenerIdTercero(usuario As Long) As Long
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Usuario = @Usuario"
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

    Private Sub LvPermisos_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvPermisos.ItemDataBound
        Dim Id_PermisoLabel As Label = DirectCast(e.Item.FindControl("Id_PermisoLabel"), Label)
        Dim EstadoLabel As Label = DirectCast(e.Item.FindControl("EstadoLabel"), Label)
        Dim AnuladaLabel As Label = DirectCast(e.Item.FindControl("AnuladaLabel"), Label)
        Dim BtReenviar As ImageButton = DirectCast(e.Item.FindControl("BtReenviar"), ImageButton)
        Dim BtConfirmar As ImageButton = DirectCast(e.Item.FindControl("BtConfirmar"), ImageButton)
        Dim BtAnulado As ImageButton = DirectCast(e.Item.FindControl("BtAnulado"), ImageButton)

        If AnuladaLabel.Text = "True" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Anulado.png"
            BtConfirmar.Width = Unit.Pixel(19)
            BtConfirmar.Enabled = False
            BtAnulado.Enabled = False
            BtReenviar.Enabled = False
        ElseIf AnuladaLabel.Text = "False" And EstadoLabel.Text = "True" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Verde.png"
            BtConfirmar.Enabled = False
            BtAnulado.Enabled = False
            BtReenviar.Enabled = False
        ElseIf AnuladaLabel.Text = "False" And EstadoLabel.Text = "False" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Rojo.png"
            BtConfirmar.Enabled = False
            BtAnulado.Enabled = False
            BtReenviar.Enabled = False
        Else
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Gris.png"
            BtConfirmar.Enabled = True
            BtAnulado.Enabled = True
            BtReenviar.Enabled = True
        End If
    End Sub

    Private Sub LvPermisos_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvPermisos.ItemCommand
        Dim Id_PermisoLabel As Label = DirectCast(e.Item.FindControl("Id_PermisoLabel"), Label)
        Session("Id_Permiso") = Id_PermisoLabel.Text
        If e.CommandName = "Detalle" Then
            Consultar_Detalle(Id_PermisoLabel.Text)
        End If
    End Sub

    Private Sub Consultar_Detalle(ByVal Id As String)
        SqlDetalleSalida.SelectCommand = "SELECT PD.Id_Permiso, PD.Id_Tercero_Hijo, T.Nombres AS Nombre_Hijo FROM Adm_Permiso_Extemporaneo_Detalle PD LEFT JOIN Terceros T ON T.Id_Tercero = PD.Id_Tercero_Hijo WHERE PD.Id_Permiso = " + Id + " "
        LvDetalleEstudiante.DataBind()
    End Sub

    Private Sub Aceptar_Permiso()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Permiso_Extemporaneo SET Estado = 1 WHERE Id_Permiso = @Id_Permiso"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Permiso", Session("Id_Permiso"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub Rechazar_Permiso()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Permiso_Extemporaneo SET Estado = 0 WHERE Id_Permiso = @Id_Permiso"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Permiso", Session("Id_Permiso"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub BtAceptar_Click(sender As Object, e As EventArgs) Handles BtAceptar.Click
        Aceptar_Permiso()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Permiso Aceptado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPermisos.DataBind()
            TxBuscarPersona.Text = ""
            TxFecha.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub BtRechazar_Click(sender As Object, e As EventArgs) Handles BtRechazar.Click
        Aceptar_Permiso()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Permiso Rechazado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPermisos.DataBind()
            TxBuscarPersona.Text = ""
            TxFecha.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Anular_Permiso()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Permiso_Extemporaneo SET Anulada = 1 WHERE Id_Permiso = @Id_Permiso"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Permiso", Session("Id_Permiso"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub BtAnular_Click(sender As Object, e As EventArgs) Handles BtAnular.Click
        Anular_Permiso()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Permiso Anulado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPermisos.DataBind()
            TxBuscarPersona.Text = ""
            TxFecha.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

End Class