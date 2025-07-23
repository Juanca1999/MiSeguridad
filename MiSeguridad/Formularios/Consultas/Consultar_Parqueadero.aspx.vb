Imports System.Data.SqlClient

Public Class Consultar_Parqueadero
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

    Private Sub BtBuscarParqueadero_Click(sender As Object, e As EventArgs) Handles BtBuscarParqueadero.Click
        If TxBuscarParqueadero.Text <> "" Then
            Dim parqueaderoBuscado As String = TxBuscarParqueadero.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT I.Id_inmueble, I.Id_Tipo_Inmueble, T.Tipo_Inmueble, I.Telefono, IP.Id_Parqueadero FROM Adm_Inmueble I LEFT JOIN Adm_Tipo_Inmueble T ON T.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble LEFT JOIN Adm_Inmueble_Parqueadero IP ON IP.Id_Inmueble = I.Id_inmueble WHERE I.Id_Sede = @Sede AND IP.Id_Parqueadero = @Parqueadero ORDER BY IP.Id_Parqueadero"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Parqueadero", parqueaderoBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Parqueadero no encontrada', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub TxBuscarParqueadero_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarParqueadero.TextChanged
        If TxBuscarParqueadero.Text <> "" Then
            Dim parqueaderoBuscado As String = TxBuscarParqueadero.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT I.Id_inmueble, I.Id_Tipo_Inmueble, T.Tipo_Inmueble, I.Telefono, IP.Id_Parqueadero FROM Adm_Inmueble I LEFT JOIN Adm_Tipo_Inmueble T ON T.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble LEFT JOIN Adm_Inmueble_Parqueadero IP ON IP.Id_Inmueble = I.Id_inmueble WHERE I.Id_Sede = @Sede AND IP.Id_Parqueadero = @Parqueadero ORDER BY IP.Id_Parqueadero"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Parqueadero", parqueaderoBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Parqueadero no encontrada', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

End Class