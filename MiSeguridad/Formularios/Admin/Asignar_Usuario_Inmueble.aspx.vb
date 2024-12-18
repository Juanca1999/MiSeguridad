Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Usuario_Inmueble
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            Session("datos1") = Nothing
        End If
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

    Private Sub Buscar_Persona_Cedula()
        Try
            Dim sqlQuery As String = "SELECT Id_Tercero, Cedula, Nombres FROM Terceros WHERE Estado = 1 AND Cedula = @Cedula"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Cedula", TxBuscarCedula.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxPersona.DataSource = datos
                            TxPersona.DataTextField = "Nombres"
                            TxPersona.DataValueField = "Cedula"
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
            Dim sqlQuery As String = "SELECT Id_Tercero, Cedula, Nombres FROM Terceros WHERE Estado = 1 AND Nombres LIKE '%' + @Nombre + '%' ORDER BY Nombres ASC"
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@Nombre", TxBuscarNombre.Text)
                    cmd.CommandType = CommandType.Text
                    Using mda As New SqlDataAdapter(cmd)
                        Using datos As New DataTable
                            mda.Fill(datos)
                            TxPersona.DataSource = datos
                            TxPersona.DataTextField = "Nombres"
                            TxPersona.DataValueField = "Cedula"
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


    Private Sub BtnAgregarPersona_Click(sender As Object, e As EventArgs) Handles BtnAgregarPersona.Click

        Try

            If TxPersona.SelectedValue <> "" Then

                Dim dt1 As New DataTable

                If Session("datos1") Is Nothing Then

                    'columnas
                    dt1.Columns.Add("CEDULA")
                    dt1.Columns.Add("NOMBRES")

                    'Agregar Datos    
                    Dim row As DataRow = dt1.NewRow()
                    row("CEDULA") = TxPersona.SelectedValue
                    row("NOMBRES") = TxPersona.SelectedItem.Text.ToUpper()

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                Else
                    dt1 = TryCast(Session("datos1"), DataTable)
                    'Agregar Datos  

                    Dim row As DataRow = dt1.NewRow()
                    row("CEDULA") = TxPersona.SelectedValue
                    row("NOMBRES") = TxPersona.SelectedItem.Text.ToUpper()

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                End If
                TxBuscarCedula.Text = ""
                TxBuscarNombre.Text = ""
                TxPersona.Items.Clear()
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
                "T.Id_Tercero, " &
                "T.Cedula, " &
                "T.Nombres, " &
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

                        While dr.Read()
                            ' Aquí accedo a los datos de cada fila según necesites
                            Dim Cedula As String = dr("Cedula").ToString()
                            Dim Nombre_Persona As String = dr("Nombres").ToString()

                            ' Agregar los datos a la tabla
                            dt1.Rows.Add(Cedula, Nombre_Persona)
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
            ErrorOp = ex.Message
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

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Elimina la fila correspondiente de tu fuente de datos  
            dt.Rows(index).Delete()

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

    Private Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click

        Try

            Eliminar_Detalle(Session("Index"))
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
        Response.Redirect("Asignar_Usuario_Inmueble.aspx")
    End Sub

End Class