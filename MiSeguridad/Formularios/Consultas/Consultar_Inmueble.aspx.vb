Imports System.Data.SqlClient

Public Class Consultar_Inmueble
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
    End Sub

    Private Sub Buscar_Residentes()
        If TxBuscarInmueble.Text <> "" Then
            Dim inmuebleBuscado As String = TxBuscarInmueble.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT IR.Id_Inmueble, TI.Tipo_Inmueble, I.Telefono, IR.Id_Tercero, T.Cedula, T.Nombres, T.Celular FROM Adm_Inmueble_Residentes IR LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IR.Id_Inmueble LEFT JOIN Terceros T ON T.Id_Tercero = IR.Id_Tercero LEFT JOIN Adm_Tipo_Inmueble TI ON TI.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble WHERE IR.Id_Inmueble = @Inmueble ORDER BY T.Nombres"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Inmueble", inmuebleBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla.Visible = True
                        Else
                            Tabla.Visible = False
                            Dim script As String = String.Format("swal('OJO!', 'Inmueble no encontrada', 'warning');")
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla.Visible = False
        End If
    End Sub

    Private Sub Buscar_Vehiculos()
        If TxBuscarInmueble.Text <> "" Then
            Dim inmuebleBuscado As String = TxBuscarInmueble.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT IV.Id_Inmueble, V.Placa, V.Id_Tipo_Vehiculo, T.Tipo_Vehiculo, V.Marca, V.Modelo, V.Color FROM Adm_Inmueble_Vehiculo IV LEFT JOIN Adm_Vehiculo V ON V.Placa = IV.Placa LEFT JOIN Adm_Tipo_Vehiculo T ON T.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo WHERE IV.Id_Inmueble = @Inmueble ORDER BY V.Placa"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Inmueble", inmuebleBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla2.Visible = True
                        Else
                            Tabla2.Visible = False
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla2.Visible = False
        End If
    End Sub

    Private Sub Buscar_Parqueaderos()
        If TxBuscarInmueble.Text <> "" Then
            Dim inmuebleBuscado As String = TxBuscarInmueble.Text.Trim()

            ' Configurar la conexión y el comando SQL
            Dim query As String = "SELECT I.Id_inmueble, I.Id_Tipo_Inmueble, T.Tipo_Inmueble, I.Telefono, IP.Id_Parqueadero FROM Adm_Inmueble_Parqueadero IP LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IP.Id_Inmueble LEFT JOIN Adm_Tipo_Inmueble T ON T.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble WHERE I.Id_inmueble = @Inmueble ORDER BY IP.Id_Parqueadero"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Inmueble", inmuebleBuscado)
                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Tabla3.Visible = True
                        Else
                            Tabla3.Visible = False
                        End If
                    End Using
                End Using
            End Using
        Else
            Tabla3.Visible = False
        End If
    End Sub

    Private Sub BtBuscarInmueble_Click(sender As Object, e As EventArgs) Handles BtBuscarInmueble.Click
        Buscar_Residentes()
        Buscar_Vehiculos()
        Buscar_Parqueaderos()
    End Sub

    Private Sub TxBuscarInmueble_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarInmueble.TextChanged
        Buscar_Residentes()
        Buscar_Vehiculos()
        Buscar_Parqueaderos()
    End Sub

End Class