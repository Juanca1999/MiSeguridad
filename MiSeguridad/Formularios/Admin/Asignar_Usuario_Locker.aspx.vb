Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Usuario_Locker
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            Session("datos1") = Nothing

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

    Private ErrorOp As String

    Private Sub Buscar_Locker()
        Try
            If String.IsNullOrEmpty(TxVestier.SelectedValue) Then
                TxLocker.Items.Clear()
                TxLocker.Items.Add(New ListItem("Locker", ""))
                Return
            End If

            Dim sqlQuery As String = "SELECT L.Id_Locker FROM Adm_Lockers L LEFT JOIN Adm_Tercero_Locker TL ON TL.Id_Locker = L.Id_Locker WHERE L.Id_Vestier = @Id_Vestier AND TL.Id_Tercero IS NULL AND L.Id_Locker LIKE @Locker ORDER BY CAST(L.Id_Locker AS INT) ASC"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Id_Vestier", TxVestier.SelectedValue)
                    cmd.Parameters.AddWithValue("@Locker", "%" & TxBuscarLocker.Text.Trim() & "%")
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)

                            TxLocker.Items.Clear()
                            TxLocker.Items.Add(New ListItem("Locker", ""))

                            TxLocker.DataSource = datos
                            TxLocker.DataTextField = "Id_Locker"
                            TxLocker.DataValueField = "Id_Locker"
                            TxLocker.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró el locker ingresado, verificar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub Buscar_Lockers_Vestier()
        If String.IsNullOrEmpty(TxVestier.SelectedValue) Then
            TxLocker.Items.Clear()
            TxLocker.Items.Add(New ListItem("Locker", ""))
            Return
        End If

        Dim sqlQuery As String = "SELECT L.Id_Locker FROM Adm_Lockers L LEFT JOIN Adm_Tercero_Locker TL ON TL.Id_Locker = L.Id_Locker WHERE L.Id_Vestier = @Id_Vestier AND TL.Id_Tercero IS NULL ORDER BY CAST(L.Id_Locker AS INT) ASC"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(sqlQuery, conn)
                cmd.Parameters.AddWithValue("@Id_Vestier", TxVestier.SelectedValue)

                Using mda As New SqlDataAdapter(cmd)
                    Using datos As New DataTable
                        mda.Fill(datos)

                        TxLocker.Items.Clear()
                        TxLocker.Items.Add(New ListItem("Locker", ""))

                        TxLocker.DataSource = datos
                        TxLocker.DataTextField = "Id_Locker"
                        TxLocker.DataValueField = "Id_Locker"
                        TxLocker.DataBind()
                    End Using
                End Using
            End Using
        End Using
    End Sub

    Private Sub Buscar_Persona_Cedula()
        Try
            Dim sqlQuery As String = "SELECT T.Id_Tercero, T.Nombres FROM Terceros T WHERE T.Estado = 1 AND T.Cedula = @Cedula"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Cedula", TxBuscarCedula.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxPersona.DataSource = datos
                            TxPersona.DataTextField = "Nombres"
                            TxPersona.DataValueField = "Id_Tercero"
                            TxPersona.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró la persona con la cedula ingresada, verificar y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub Buscar_Persona_Nombre()
        Try
            Dim sqlQuery As String = "SELECT T.Id_Tercero, T.Nombres FROM Terceros T WHERE T.Estado = 1 AND T.Nombres LIKE '%' + @Nombre + '%' ORDER BY T.Nombres ASC"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Sede", Session("Sucursal_Usuario"))
                    cmd.Parameters.AddWithValue("@Nombre", TxBuscarNombre.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxPersona.DataSource = datos
                            TxPersona.DataTextField = "Nombres"
                            TxPersona.DataValueField = "Id_Tercero"
                            TxPersona.DataBind()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Dim script As String = String.Format("swal('OJO!', 'No se encontró la persona con el nombre ingresado, verificar y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End Try
    End Sub

    Private Sub TxVestierSelectedIndexChanged(sender As Object, e As EventArgs) Handles TxVestier.SelectedIndexChanged
        Buscar_Lockers_Vestier()
    End Sub

    Private Sub TxBuscarLocker_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarLocker.TextChanged
        Buscar_Locker()
    End Sub

    Private Sub TxBuscarCedula_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarCedula.TextChanged
        If TxBuscarCedula.Text <> "" Then
            Buscar_Persona_Cedula()
        Else
            TxPersona.Items.Clear()
        End If
    End Sub

    Private Sub TxBuscarNombre_TextChanged(sender As Object, e As EventArgs) Handles TxBuscarNombre.TextChanged
        If TxBuscarNombre.Text <> "" Then
            Buscar_Persona_Nombre()
        Else
            TxPersona.Items.Clear()
        End If
    End Sub

    Private Function Consultar_ID_Tercero(ByVal Cedula As String) As Integer
        Try
            ' Definir la cadena de conexión
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Definir la consulta
            Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Cedula"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Cedula", Cedula)

                    ' Abrir la conexión
                    conn.Open()

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = cmd.ExecuteScalar()

                    ' Verificar si el resultado es válido
                    If result IsNot Nothing Then
                        Return Convert.ToInt32(result)
                    Else
                        ' Devolver 0 o algún valor por defecto si no se encuentra el registro
                        Return 0
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Lanzar la excepción
            Throw ex
        End Try
    End Function

    Private Sub Asignar_Persona_Locker()

        Try
            Dim query As String = "
        INSERT INTO dbo.Adm_Tercero_Locker 
        (Id_Tercero, Id_Locker, Id_Vestier) 
        VALUES 
        (@Id_Tercero, @Id_Locker, @Id_Vestier)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_Tercero", TxPersona.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_Locker", TxLocker.SelectedValue)
                    cmd.Parameters.AddWithValue("@Id_Vestier", TxVestier.SelectedValue)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            ErrorOp = ex.Message
        End Try
    End Sub

    Private Sub Eliminar_Asignacion()
        Dim sql As String = "DELETE FROM Adm_Tercero_Locker WHERE Id_Tercero = @Id_Tercero"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_Tercero", HiddenIdTercero.Value)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Asignar_Persona_Locker()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Locker Asignado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Response.AddHeader("Refresh", "2; URL=Asignar_Usuario_Locker.aspx")
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If

    End Sub

    Private Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click
        If HiddenIdTercero.Value <> "" Then
            Eliminar_Asignacion()

            If ErrorOp = Nothing Then
                Dim script As String = String.Format("swal('Excelente!', 'Locker Desasignado Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Response.AddHeader("Refresh", "2; URL=Asignar_Usuario_Locker.aspx")
            Else
                Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            End If
        Else
            Dim script As String = String.Format("swal('OJO!', 'Error al identificar persona', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
        End If
    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            LvPersonas.DataSourceID = "SqlPersonas"
        Else
            LvPersonas.DataSourceID = "Sql_Buscar_Persona"
        End If
        LvPersonas.DataBind()
    End Sub

End Class