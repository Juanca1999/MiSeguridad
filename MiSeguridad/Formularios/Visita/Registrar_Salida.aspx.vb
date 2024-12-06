Imports System.Data.SqlClient

Public Class Registrar_Salida
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
            ElseIf Session("Acceso_Usuario") Is Nothing Then
                Dim script As String = "swal({
                title: 'OJO!',
                text: 'Debe estar registrado en algún acceso',
                type: 'warning',
                allowOutsideClick: false
                }).then(function() {
                    window.location.href = '../../Inicio.aspx';
                });"
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
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

                            ' Asignación de Id_Acceso verificando si es DBNull
                            If IsDBNull(dr("Id_Acceso")) Then
                                Session("Acceso_Usuario") = Nothing
                            Else
                                Session("Acceso_Usuario") = dr("Id_Acceso").ToString()
                            End If
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                            Session("Acceso_Usuario") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BtEntrada_ServerClick(sender As Object, e As EventArgs) Handles BtEntrada.ServerClick
        Response.Redirect("Registrar_Visita.aspx")
    End Sub

    Private Sub BtSalida_ServerClick(sender As Object, e As EventArgs) Handles BtSalida.ServerClick
        Response.Redirect("Registrar_Salida.aspx")
    End Sub

    Private Sub BtEntradaContratista_ServerClick(sender As Object, e As EventArgs) Handles BtEntradaContratista.ServerClick
        Response.Redirect("Registrar_Visita_Contratista.aspx")
    End Sub

    Private Sub BtSalidaContratista_ServerClick(sender As Object, e As EventArgs) Handles BtSalidaContratista.ServerClick
        Response.Redirect("Registrar_Salida_Contratista.aspx")
    End Sub

    Protected Sub TxBuscarInmueble_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarInmueble.TextChanged
        If TxBuscarInmueble.Text <> "" Then
            ' Recuperar el valor ingresado en el TextBox
            Dim inmuebleBuscado As String = TxBuscarInmueble.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND V.Id_Inmueble = @Inmueble ORDER BY T.Nombres"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Inmueble", inmuebleBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvVisitantes.DataSourceID = "SqlVisitantes"
                            LvVisitantes.DataBind()
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes en el inmueble', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Protected Sub TxBuscarCedula_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedula.TextChanged
        If TxBuscarCedula.Text <> "" Then
            ' Recuperar el valor ingresado en el TextBox
            Dim cedulaBuscado As String = TxBuscarCedula.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND T.Cedula = @Cedula ORDER BY T.Nombres"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Cedula", cedulaBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                            LvVisitantes.DataSourceID = "SqlVisitantesCedula"
                            LvVisitantes.DataBind()
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes con la cedula', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Protected Sub TxBuscarPersona_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarPersona.TextChanged
        If TxBuscarPersona.Text <> "" Then
            ' Recuperar el valor ingresado en el TextBox
            Dim nombreBuscado As String = TxBuscarPersona.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND T.Nombres LIKE '%' + @Nombre + '%' ORDER BY T.Nombres"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Nombre", nombreBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            LvVisitantes.DataSourceID = "SqlVisitantesNombre"
                            LvVisitantes.DataBind()
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'No se encontraron visitantes con el nombre', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub Registrar_Salida()
        Try
            Dim sql As String = "UPDATE dbo.Adm_Visita SET Fecha_Fin_Visita = @FechaFinVisita, " &
                            "Hora_fin_Visita = @HoraFinVisita, " &
                            "Id_Acceso_Salida = @IdAccesoSalida, " &
                            "Manual = 1, " &
                            "Estado = 0 " &
                            "WHERE Id_Visita = @IdVisita"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(sql, conn)
                    ' Agregar los parámetros a la consulta SQL
                    cmd.Parameters.AddWithValue("@FechaFinVisita", Date.Now.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@HoraFinVisita", Date.Now.ToString("HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@IdAccesoSalida", Session("Acceso_Usuario"))
                    cmd.Parameters.AddWithValue("@IdVisita", Session("Id_Visita"))

                    ' Abrir la conexión y ejecutar el comando
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub LvVisitantes_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvVisitantes.ItemDataBound
        Dim Id_VisitaLabel As Label = DirectCast(e.Item.FindControl("Id_VisitaLabel"), Label)
        Dim Fecha_Inicio_VisitaLabel As Label = DirectCast(e.Item.FindControl("Fecha_Inicio_VisitaLabel"), Label)
        Fecha_Inicio_VisitaLabel.Text = Mid(Fecha_Inicio_VisitaLabel.Text.ToString, 1, 10)
    End Sub

    Private Sub LvVisitantes_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvVisitantes.ItemCommand
        Dim Id_VisitaLabel As Label = DirectCast(e.Item.FindControl("Id_VisitaLabel"), Label)
        Session("Id_Visita") = Id_VisitaLabel.Text
    End Sub

    Private Sub BtSalir_Click(sender As Object, e As EventArgs) Handles BtSalir.Click
        Registrar_Salida()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Salida Guardada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Tabla.Visible = False
            LvVisitantes.DataBind()
            TxBuscarInmueble.SelectedIndex = 0
            TxBuscarCedula.Text = ""
            TxBuscarPersona.Text = ""
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

End Class