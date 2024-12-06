Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Cuarto_Util
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
    End Sub

    Private Sub Guardar_Datos()
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        cmd.CommandType = CommandType.Text
        Try
            cmd.Connection = conn_Administracion1
            cmd.CommandText = Session("sql")
            conn_Administracion1.Open()
            cmd.ExecuteNonQuery()
            conn_Administracion1.Close()
            Session("Mensaje") = "Guardado Correctamente"
        Catch ex As Exception
            If ex.ToString.Contains("PRIMARY") Then
            Else
                Try
                    cmd.Connection = conn_Administracion2
                    cmd.CommandText = Session("sql")
                    conn_Administracion2.Open()
                    cmd.ExecuteNonQuery()
                    conn_Administracion2.Close()
                    Session("Mensaje") = "Guardado Correctamente"
                Catch ex1 As Exception
                    ErrorOp = "Private Sub Guardar_Datos:  " + Mid(ex1.ToString, 1, 300) + "<br /><br />" + "Sintaxis Sql: " + Session("sql")
                    conn_Administracion1.Close()
                    conn_Administracion2.Close()
                    EnviarCorreoError()
                    Session("Mensaje") = "!Error al Guardar, Verificar o llamar Administrador del sistema¡"
                End Try
            End If

        End Try

    End Sub

    Private Sub Ejecutar_Query()
        conn_Administracion1.Close()
        conn_Administracion2.Close()
        cmd.CommandType = CommandType.Text
        Try
            cmd.Connection = conn_Administracion1
            cmd.CommandText = Session("sql")
            conn_Administracion1.Open()
            dr = cmd.ExecuteReader
        Catch ex As Exception
            Try
                cmd.Connection = conn_Administracion2
                cmd.CommandText = Session("sql")
                conn_Administracion2.Open()
                dr = cmd.ExecuteReader
            Catch ex1 As Exception
                ErrorOp = "Private Sub Ejecutar_Query: " + Mid(ex1.ToString, 1, 300) + "<br /><br />" + "Sintaxis Sql: " + Session("sql")
                conn_Administracion1.Close()
                conn_Administracion2.Close()
                EnviarCorreoError()
            End Try
        End Try

    End Sub

    Private ErrorOp As String

    Private Sub EnviarCorreoError()
        Try
            Dim msg = New MailMessage
            msg.From = New MailAddress(CorreoFrom, "Error Miseguridad")
            msg.To.Add(New MailAddress("juan.aristizabal@miroseguridad.com"))
            msg.Subject = "Error Miseguridad"
            msg.Body = "Error al usuario: " + User.Identity.Name + "<br />" + "Nombre: " + Session("Nombres") + "<br />" + "Formulario: " + Request.Url.ToString + "<br /><br />" + "Error: " + ErrorOp
            msg.IsBodyHtml = True

            Dim smtp = New SmtpClient
            smtp.Host = SmtpHost
            smtp.Port = 587
            smtp.UseDefaultCredentials = False
            smtp.Credentials = New NetworkCredential(UsuarioCredentials, ContrasenaCredentials)
            smtp.EnableSsl = True

            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            smtp.Send(msg)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BtnAgregarCuartoUtil_Click(sender As Object, e As EventArgs) Handles BtnAgregarCuartoUtil.Click

        Try

            If TxIdCuartoUtil.Text <> "" Then

                Dim dt1 As New DataTable

                If Session("datos1") Is Nothing Then

                    'columnas
                    dt1.Columns.Add("NUMERO CUARTO UTIL")

                    'Agregar Datos    
                    Dim row As DataRow = dt1.NewRow()
                    row("NUMERO CUARTO UTIL") = TxIdCuartoUtil.Text

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                Else
                    dt1 = TryCast(Session("datos1"), DataTable)
                    'Agregar Datos  

                    Dim row As DataRow = dt1.NewRow()
                    row("NUMERO CUARTO UTIL") = TxIdCuartoUtil.Text

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                End If
                TxIdCuartoUtil.Text = ""
            End If

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Sub Buscar_Detalle_Inmueble(ByVal inmueble)

        Try

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())

                Dim query As String =
                "SELECT " &
                "IC.Id_Inmueble, " &
                "C.Id_Cuarto_Util, " &
                "C.Estado " &
                "FROM Adm_Inmueble_Cuarto_Util IC LEFT JOIN Adm_Cuarto_Util C ON C.Id_Cuarto_Util = IC.Id_Cuarto_Util " &
                "WHERE IC.Id_Inmueble = @Id_Inmueble"

                Using cmd As New SqlCommand(query, conn)
                    'Se define el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Inmueble", inmueble)

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim dt1 As New DataTable()
                        dt1.Columns.Add("NUMERO CUARTO UTIL")

                        While dr.Read()
                            ' Aquí accedo a los datos de cada fila según necesites
                            Dim CuartoUtil As String = dr("Id_Cuarto_Util").ToString()

                            ' Agregar los datos a la tabla
                            dt1.Rows.Add(CuartoUtil)
                        End While

                        ' Asignar la tabla como origen de datos del GridView

                        Session("datos1") = dt1
                        GridView1.DataSource = dt1
                        GridView1.DataBind()
                    End Using
                End Using
            End Using

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Function Validar_Detalle_Inmueble(ByVal Id_Inmueble As String, ByVal Id_Cuarto_Util As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Adm_Inmueble_Cuarto_Util WHERE Id_Inmueble = @Id_Inmueble AND Id_Cuarto_Util = @Id_Cuarto_Util"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)
                    cmd.Parameters.AddWithValue("@Id_Cuarto_Util", Id_Cuarto_Util)

                    ' Abrir la conexión
                    conn.Open()

                    ' Ejecutar la consulta y obtener el resultado
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                    ' Si el conteo es mayor que cero, significa que el dato existe
                    If count > 0 Then
                        existe = True
                    End If
                End Using
            End Using

            ' Devolver el resultado
            Return existe

        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Private Function Validar_Cuarto_Util(ByVal Id_Cuarto_Util As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Adm_Cuarto_Util WHERE Id_Cuarto_Util = @Id_Cuarto_Util"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Id_Cuarto_Util", Id_Cuarto_Util)

                    ' Abrir la conexión
                    conn.Open()

                    ' Ejecutar la consulta y obtener el resultado
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                    ' Si el conteo es mayor que cero, significa que el dato existe
                    If count > 0 Then
                        existe = True
                    End If
                End Using
            End Using

            ' Devolver el resultado
            Return existe

        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Private Sub Eliminar_Detalle_Inmueble(ByVal Id_Inmueble As String, ByVal Id_Cuarto_Util As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Adm_Inmueble_Cuarto_Util WHERE Id_Inmueble = @Id_Inmueble AND Id_Cuarto_Util = @Id_Cuarto_Util"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)
                    cmd.Parameters.AddWithValue("@Id_Cuarto_Util", Id_Cuarto_Util)

                    ' Abrir la conexión
                    conn.Open()

                    ' Ejecutar el comando SQL
                    cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub Eliminar_Cuaro_Util(ByVal Id_Cuarto_Util As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Adm_Cuarto_Util WHERE Id_Cuarto_Util = @Id_Cuarto_Util"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Cuarto_Util", Id_Cuarto_Util)

                    ' Abrir la conexión
                    conn.Open()

                    ' Ejecutar el comando SQL
                    cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub Guardar_Detalle_Inmueble()

        Try

            Dim cont As Integer = 0

            For Each r As GridViewRow In GridView1.Rows

                GridView1.SelectRow(cont)

                If Not Validar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Convert.ToString(GridView1.SelectedRow.Cells(1).Text)) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        Dim queryInsert As String = "INSERT INTO dbo.Adm_Inmueble_Cuarto_Util (" &
                                                "Id_Inmueble, " &
                                                "Id_Cuarto_Util) " &
                                                "VALUES (" &
                                                "@Id_Inmueble, " &
                                                "@Id_Cuarto_Util)"

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Id_Inmueble", Convert.ToString(Session("Id_inmueble")))
                            cmdInsert.Parameters.AddWithValue("@Id_Cuarto_Util", Convert.ToString(GridView1.SelectedRow.Cells(1).Text))

                            conn.Open()
                            cmdInsert.ExecuteNonQuery()
                        End Using
                    End Using
                End If
                cont = cont + 1
            Next

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub Guardar_Cuarto_Util()

        Try

            Dim cont As Integer = 0

            For Each r As GridViewRow In GridView1.Rows

                GridView1.SelectRow(cont)

                If Not Validar_Cuarto_Util(Convert.ToString(GridView1.SelectedRow.Cells(1).Text)) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        Dim queryInsert As String = "INSERT INTO dbo.Adm_Cuarto_Util (" &
                                                "Id_Cuarto_Util, " &
                                                "Id_Sede, " &
                                                "Estado) " &
                                                "VALUES (" &
                                                "@Id_Cuarto_Util, " &
                                                "@Id_Sede, " &
                                                "@Estado)"

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Id_Cuarto_Util", Convert.ToString(GridView1.SelectedRow.Cells(1).Text))
                            cmdInsert.Parameters.AddWithValue("@Id_Sede", TxIdInmueble.ToolTip)
                            cmdInsert.Parameters.AddWithValue("@Estado", 1)

                            conn.Open()
                            cmdInsert.ExecuteNonQuery()
                        End Using
                    End Using
                End If
                cont = cont + 1
            Next

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub Lvinmuebles_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles Lvinmuebles.ItemCommand
        Dim Id_inmuebleLabel As Label = DirectCast(e.Item.FindControl("Id_inmuebleLabel"), Label)
        Dim Id_SedeLabel As Label = DirectCast(e.Item.FindControl("Id_SedeLabel"), Label)
        Session("Id_inmueble") = Id_inmuebleLabel.Text

        If e.CommandName = "Editar" Then
            PUsuario.Visible = True
            TxIdInmueble.Text = Id_inmuebleLabel.Text
            TxIdInmueble.ToolTip = Id_SedeLabel.Text
            Buscar_Detalle_Inmueble(Id_inmuebleLabel.Text)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Guardar_Cuarto_Util()
        Guardar_Detalle_Inmueble()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Persona A Inmueble Guardado Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Timer1.Interval = 3000
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If

    End Sub

    Private Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand

        Try

            If e.CommandName = "Actualizar" Then

                ' Habilitar el modal
                Session("Index") = Convert.ToString(e.CommandArgument)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MostrarModal", "$('#Confirmacion').modal('show');", True)
            End If

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Sub Eliminar_Detalle(ByRef index As String)

        Try
            Dim Id_Cuarto_Util As String

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            dt.Columns.Add("NUMERO CUARTO UTIL")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Id_Cuarto_Util = Convert.ToString(row.Cells(1).Text)
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("NUMERO CUARTO UTIL") = row.Cells(1).Text ' La primera columna del GridView (índice 0)

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Id_Cuarto_Util) Then

                Eliminar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Id_Cuarto_Util)

            End If

        Catch ex As Exception
            Throw ex

        End Try

    End Sub

    Private Sub Eliminar(ByRef index As Integer)

        Try
            Dim Id_Cuarto_Util As String

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            dt.Columns.Add("NUMERO CUARTO UTIL")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Id_Cuarto_Util = Convert.ToString(row.Cells(1).Text)
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("NUMERO CUARTO UTIL") = row.Cells(1).Text ' La primera columna del GridView (índice 0)

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Elimina la fila correspondiente de tu fuente de datos  
            dt.Rows(index).Delete()

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Cuarto_Util(Id_Cuarto_Util) Then

                Eliminar_Cuaro_Util(Id_Cuarto_Util)

            End If

        Catch ex As Exception
            Throw ex

        End Try

    End Sub

    Private Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click

        Try
            Eliminar_Detalle(Session("Index"))
            Eliminar(Session("Index"))
            Session("Index") = Nothing

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            Lvinmuebles.DataSourceID = "SqlInmuebles"
        Else
            Lvinmuebles.DataSourceID = "Sql_Buscar_Inmueble"
        End If
        Lvinmuebles.DataBind()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Asignar_Cuarto_Util.aspx")
    End Sub

End Class