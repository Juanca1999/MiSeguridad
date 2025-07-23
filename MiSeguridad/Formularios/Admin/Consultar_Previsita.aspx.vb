Imports System.Data.SqlClient

Public Class Consultar_Previsita
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
            ElseIf Not Consultar_Control_Previsita() Then
                Dim script As String = "swal({
                title: 'OJO!',
                text: 'Debe tener chuleado control visitantes para poder entrar al modulo',
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

    Private Function Consultar_Control_Previsita() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Control_Previsita FROM Adm_Empresa"
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

    Private Sub TxBuscarCedula_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedula.TextChanged
        If TxBuscarCedula.Text <> "" Then
            LvPrevisitas.DataSourceID = "SqlPrevisitasCedula"
        Else
            LvPrevisitas.DataSourceID = "SqlPrevisitas"
        End If
        LvPrevisitas.DataBind()
    End Sub

    Private Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            LvPrevisitas.DataSourceID = "SqlPrevisitasNombre"
        Else
            LvPrevisitas.DataSourceID = "SqlPrevisitas"
        End If
        LvPrevisitas.DataBind()
    End Sub

    Private Sub TxFechaIngreso_TextChanged(sender As Object, e As EventArgs) Handles TxFechaIngreso.TextChanged
        If TxFechaIngreso.Text <> "" Then
            LvPrevisitas.DataSourceID = "SqlPrevisitasFecha"
        Else
            LvPrevisitas.DataSourceID = "SqlPrevisitas"
        End If
        LvPrevisitas.DataBind()
    End Sub

    Private Sub BtContratistas_ServerClick(sender As Object, e As EventArgs) Handles BtContratistas.ServerClick
        Response.Redirect("Consultar_Contratistas.aspx")
    End Sub

    Private Sub BtVisitantes_ServerClick(sender As Object, e As EventArgs) Handles BtVisitantes.ServerClick
        Response.Redirect("Consultar_Previsita.aspx")
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

    Private Sub Consultar_Responsable(ByVal Id As Long)
        Dim sql As String = "SELECT P.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza FROM Adm_Preregistro_Visita P LEFT JOIN Terceros T ON T.Id_Tercero = P.Id_Quien_Autoriza WHERE P.Id_Preregistro_Visita = @Id_Preregistro_Visita"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Preregistro_Visita", Id)

                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim id_responsable As String = If(IsDBNull(reader("Id_Quien_Autoriza")), String.Empty, reader("Id_Quien_Autoriza").ToString())
                        Dim responsable As String = If(IsDBNull(reader("Quien_Autoriza")), String.Empty, reader("Quien_Autoriza").ToString())

                        ' Asignar los valores a los controles en la página si no están vacíos o nulos
                        TxReponsable.Items.Clear()
                        TxReponsable.Items.Add(New ListItem(If(String.IsNullOrEmpty(id_responsable), "", If(String.IsNullOrEmpty(responsable), "", responsable))))
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub Consultar_Fecha(ByVal Id As Long)
        Dim sql As String = "SELECT Fecha_Ingreso FROM Adm_Preregistro_Visita WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Preregistro_Visita", Id)

                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim Fecha_Ing As Date = reader("Fecha_Ingreso").ToString()
                        TxFechaIngresoEditar.Text = Fecha_Ing.ToString("yyyy-MM-dd")
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub LvPrevisitas_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvPrevisitas.ItemDataBound
        Dim Id_Preregistro_VisitaLabel As Label = DirectCast(e.Item.FindControl("Id_Preregistro_VisitaLabel"), Label)
        Dim Fecha_IngresoLabel As Label = DirectCast(e.Item.FindControl("Fecha_IngresoLabel"), Label)
        Dim EstadoLabel As Label = DirectCast(e.Item.FindControl("EstadoLabel"), Label)
        Dim AnuladaLabel As Label = DirectCast(e.Item.FindControl("AnuladaLabel"), Label)
        Dim BtConfirmar As ImageButton = DirectCast(e.Item.FindControl("BtConfirmar"), ImageButton)
        Dim BtAnulado As ImageButton = DirectCast(e.Item.FindControl("BtAnulado"), ImageButton)
        Dim LbFecha As ImageButton = DirectCast(e.Item.FindControl("LbFecha"), ImageButton)
        Dim LbResponsable2 As ImageButton = DirectCast(e.Item.FindControl("LbResponsable2"), ImageButton)
        Fecha_IngresoLabel.Text = Mid(Fecha_IngresoLabel.Text.ToString, 1, 10)

        If AnuladaLabel.Text = "True" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Anulado.png"
            BtConfirmar.Width = Unit.Pixel(19)
            BtConfirmar.Enabled = False
            BtAnulado.Enabled = False
            LbFecha.Enabled = False
            LbResponsable2.Enabled = False
        ElseIf AnuladaLabel.Text = "False" And EstadoLabel.Text = "True" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Verde.png"
            BtConfirmar.Enabled = False
            BtAnulado.Enabled = False
            LbFecha.Enabled = False
            LbResponsable2.Enabled = False
        ElseIf AnuladaLabel.Text = "False" And EstadoLabel.Text = "False" Then
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Rojo.png"
            BtConfirmar.Enabled = False
            BtAnulado.Enabled = False
            LbFecha.Enabled = False
            LbResponsable2.Enabled = False
        Else
            BtConfirmar.ImageUrl = "~/Diseno/Imagenes/Gris.png"
            BtConfirmar.Enabled = True
            BtAnulado.Enabled = True
            LbFecha.Enabled = True
            LbResponsable2.Enabled = True
        End If
    End Sub

    Private Sub LvPrevisitas_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvPrevisitas.ItemCommand
        Dim Id_Preregistro_VisitaLabel As Label = DirectCast(e.Item.FindControl("Id_Preregistro_VisitaLabel"), Label)
        Session("Id_Preregistro_Visita") = Id_Preregistro_VisitaLabel.Text

        If e.CommandName = "Responsable" Then
            Consultar_Responsable(Id_Preregistro_VisitaLabel.Text)
        ElseIf e.CommandName = "Fecha" Then
            Consultar_Fecha(Id_Preregistro_VisitaLabel.Text)
        End If
    End Sub

    Private Sub Aceptar_Previsita()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Preregistro_Visita SET Estado = 1 WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Preregistro_Visita", Session("Id_Preregistro_Visita"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub Rechazar_Previsita()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Preregistro_Visita SET Estado = 0 WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Preregistro_Visita", Session("Id_Preregistro_Visita"))

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
        Aceptar_Previsita()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Previsita Aceptada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub BtRechazar_Click(sender As Object, e As EventArgs) Handles BtRechazar.Click
        Rechazar_Previsita()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Previsita Rechazada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Actualizar_Responsable()
        Dim sql As String = "UPDATE dbo.Adm_Preregistro_Visita SET Id_Quien_Autoriza = @Id_Quien_Autoriza WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                command.Parameters.AddWithValue("@Id_Quien_Autoriza", TxReponsable.SelectedValue)
                command.Parameters.AddWithValue("@Id_Preregistro_Visita", Session("Id_Preregistro_Visita"))

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtEditarResponsable_Click(sender As Object, e As EventArgs) Handles BtEditarResponsable.Click
        Actualizar_Responsable()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Responsable Guardado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Actualizar_Fecha()
        Dim sql As String = "UPDATE dbo.Adm_Preregistro_Visita SET Fecha_Ingreso = @Fecha_Ingreso WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(sql, connection)
                Dim Fecha_Ing As Date = TxFechaIngresoEditar.Text
                command.Parameters.AddWithValue("@Fecha_Ingreso", Fecha_Ing.ToString("yyyy-MM-dd"))
                command.Parameters.AddWithValue("@Id_Preregistro_Visita", Session("Id_Preregistro_Visita"))

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtEditarFecha_Click(sender As Object, e As EventArgs) Handles BtEditarFecha.Click
        Actualizar_Fecha()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Fecha Editada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub Anular_Previsita()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Preregistro_Visita SET Anulada = 1 WHERE Id_Preregistro_Visita = @Id_Preregistro_Visita"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Id_Preregistro_Visita", Session("Id_Preregistro_Visita"))

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
        Anular_Previsita()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Previsita Anulada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            LvPrevisitas.DataBind()
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

End Class