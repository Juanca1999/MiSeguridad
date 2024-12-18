Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Residentes
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


    Private Sub BtnAgregarPersona_Click(sender As Object, e As EventArgs) Handles BtnAgregarPersona.Click

        Try

            If TxCedula.Text <> "" And TxNombres.Text <> "" And TxCorreo.Text <> "" And TxCelular.Text <> "" Then

                Dim dt1 As New DataTable

                If Session("datos1") Is Nothing Then

                    'columnas
                    dt1.Columns.Add("CEDULA")
                    dt1.Columns.Add("NOMBRES")
                    dt1.Columns.Add("CORREO")
                    dt1.Columns.Add("CELULAR")

                    'Agregar Datos    
                    Dim row As DataRow = dt1.NewRow()
                    row("CEDULA") = TxCedula.Text
                    row("NOMBRES") = TxNombres.Text.ToUpper()
                    row("CORREO") = TxCorreo.Text
                    row("CELULAR") = TxCelular.Text

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                Else
                    dt1 = TryCast(Session("datos1"), DataTable)
                    'Agregar Datos  

                    Dim row As DataRow = dt1.NewRow()
                    row("CEDULA") = TxCedula.Text
                    row("NOMBRES") = TxNombres.Text.ToUpper()
                    row("CORREO") = TxCorreo.Text
                    row("CELULAR") = TxCelular.Text

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                End If
                TxCedula.Text = ""
                TxNombres.Text = ""
                TxCorreo.Text = ""
                TxCelular.Text = ""
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
                "IR.Id_Inmueble, " &
                "T.Cedula, " &
                "T.Nombres, " &
                "T.Correo, " &
                "T.Celular, " &
                "IR.Estado " &
                "FROM Adm_Inmueble_Residentes IR LEFT JOIN Terceros T ON T.Id_Tercero = IR.Id_Tercero " &
                "WHERE IR.Id_Inmueble = @Id_Inmueble"

                Using cmd As New SqlCommand(query, conn)
                    'Se define el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Inmueble", inmueble)

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim dt1 As New DataTable()
                        dt1.Columns.Add("CEDULA")
                        dt1.Columns.Add("NOMBRES")
                        dt1.Columns.Add("CORREO")
                        dt1.Columns.Add("CELULAR")

                        While dr.Read()
                            ' Aquí accedo a los datos de cada fila según necesites
                            Dim Cedula As String = dr("Cedula").ToString()
                            Dim Nombre_Persona As String = dr("Nombres").ToString()
                            Dim Correo As String = dr("Correo").ToString()
                            Dim Celular As String = dr("Celular").ToString()

                            ' Agregar los datos a la tabla
                            dt1.Rows.Add(Cedula, Nombre_Persona, Correo, Celular)
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

    Private Function Validar_Detalle_Inmueble(ByVal Id_Inmueble As String, ByVal Id_Tercero As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Adm_Inmueble_Residentes WHERE Id_Inmueble = @Id_Inmueble AND Id_Tercero = @Id_Tercero"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

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

    Private Function Validar_Persona(ByVal Id_Tercero As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Terceros WHERE Id_Tercero = @Id_Tercero"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

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

    Private Sub Eliminar_Detalle_Inmueble(ByVal Id_Inmueble As String, ByVal Id_Tercero As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Adm_Inmueble_Residentes WHERE Id_Inmueble = @Id_Inmueble AND Id_Tercero = @Id_Tercero"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

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

    Private Sub Eliminar_Persona(ByVal Id_Tercero As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Terceros WHERE Id_Tercero = @Id_Tercero"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)

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

                Dim Id_Tercero As Integer = Consultar_ID_Tercero(Convert.ToString(GridView1.SelectedRow.Cells(1).Text))

                If Not Validar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Id_Tercero) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        Dim queryInsert As String = "INSERT INTO dbo.Adm_Inmueble_Residentes (" &
                                                "Id_Inmueble, " &
                                                "Id_Tercero, " &
                                                "Estado) " &
                                                "VALUES (" &
                                                "@Id_Inmueble, " &
                                                "@Id_Tercero, " &
                                                "@Estado)"

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Id_Inmueble", Convert.ToString(Session("Id_inmueble")))
                            cmdInsert.Parameters.AddWithValue("@Id_Tercero", Id_Tercero)
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

    Private Sub Guardar_Persona()

        Try

            Dim cont As Integer = 0

            For Each r As GridViewRow In GridView1.Rows

                GridView1.SelectRow(cont)

                Dim Id_Tercero As Integer = Consultar_ID_Tercero(Convert.ToString(GridView1.SelectedRow.Cells(1).Text))

                If Not Validar_Persona(Id_Tercero) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        conn.Open()

                        ' Obtener el valor máximo de Id_Tercero y sumarle 1
                        Dim queryMaxId As String = "SELECT ISNULL(MAX(Id_Tercero), 0) + 1 FROM Terceros"
                        Dim newIdTercero As Integer

                        Using cmdMaxId As New SqlCommand(queryMaxId, conn)
                            newIdTercero = Convert.ToInt32(cmdMaxId.ExecuteScalar())
                        End Using

                        Dim queryInsert As String = "INSERT INTO dbo.Terceros (" &
                                                "Id_Tercero, " &
                                                "Cedula, " &
                                                "Nombres, " &
                                                "Correo, " &
                                                "Celular, " &
                                                "Id_Rol, " &
                                                "Id_Sede, " &
                                                "Estado) " &
                                                "VALUES (" &
                                                "@Id_Tercero, " &
                                                "@Cedula, " &
                                                "@Nombres, " &
                                                "@Correo, " &
                                                "@Celular," &
                                                "@Id_Rol," &
                                                "@Id_Sede, " &
                                                "@Estado) "

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Id_Tercero", newIdTercero)
                            cmdInsert.Parameters.AddWithValue("@Cedula", Convert.ToString(GridView1.SelectedRow.Cells(1).Text))
                            cmdInsert.Parameters.AddWithValue("@Nombres", Convert.ToString(GridView1.SelectedRow.Cells(2).Text))
                            cmdInsert.Parameters.AddWithValue("@Correo", Convert.ToString(GridView1.SelectedRow.Cells(3).Text))
                            cmdInsert.Parameters.AddWithValue("@Celular", Convert.ToString(GridView1.SelectedRow.Cells(4).Text))
                            cmdInsert.Parameters.AddWithValue("@Id_Rol", 3)
                            cmdInsert.Parameters.AddWithValue("@Id_Sede", TxIdInmueble.ToolTip)
                            cmdInsert.Parameters.AddWithValue("@Estado", 1)

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
        Guardar_Persona()
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
                Session("Index") = Convert.ToInt32(e.CommandArgument)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MostrarModal", "$('#Confirmacion').modal('show');", True)
            End If

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Sub Eliminar_Detalle(ByRef index As Integer)

        Try
            Dim Id_Tercero As Integer

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            ' Agregar las columnas al DataTable 
            dt.Columns.Add("CEDULA")
            dt.Columns.Add("NOMBRES")
            dt.Columns.Add("CORREO")
            dt.Columns.Add("CELULAR")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Id_Tercero = Consultar_ID_Tercero(Convert.ToString(row.Cells(1).Text))
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("CEDULA") = row.Cells(1).Text
                dr("NOMBRES") = row.Cells(2).Text
                dr("CORREO") = row.Cells(3).Text
                dr("CELULAR") = row.Cells(4).Text

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Id_Tercero) Then

                Eliminar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Id_Tercero)

            End If

        Catch ex As Exception
            Throw ex

        End Try

    End Sub

    Private Sub Eliminar(ByRef index As Integer)

        Try
            Dim Id_Tercero As Integer

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            dt.Columns.Add("CEDULA")
            dt.Columns.Add("NOMBRES")
            dt.Columns.Add("CORREO")
            dt.Columns.Add("CELULAR")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Id_Tercero = Consultar_ID_Tercero(Convert.ToString(row.Cells(1).Text))
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("CEDULA") = row.Cells(1).Text
                dr("NOMBRES") = row.Cells(2).Text
                dr("CORREO") = row.Cells(3).Text
                dr("CELULAR") = row.Cells(4).Text

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Elimina la fila correspondiente de tu fuente de datos  
            dt.Rows(index).Delete()

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Persona(Id_Tercero) Then

                Eliminar_Persona(Id_Tercero)

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
        Response.Redirect("Asignar_Residentes.aspx")
    End Sub

End Class