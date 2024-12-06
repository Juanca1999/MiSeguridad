Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Asignar_Vehiculo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
        If Not IsPostBack Then
            Session("datos1") = Nothing
        End If
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

    Protected Sub TxTipoVehiculo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TxTipoVehiculo.SelectedIndexChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue

        If tipoVehiculo = "1" Then ' Carro
            TxPlaca.Enabled = True
        ElseIf tipoVehiculo = "2" Then ' Moto
            TxPlaca.Enabled = True
        Else
            TxPlaca.Enabled = False
        End If
    End Sub

    Protected Sub TxPlaca_TextChanged(sender As Object, e As EventArgs) Handles TxPlaca.TextChanged
        Dim tipoVehiculo As String = TxTipoVehiculo.SelectedValue
        Dim placa As String = TxPlaca.Text.Trim()
        Dim isValid As Boolean = False

        If Not String.IsNullOrEmpty(placa) Then
            If tipoVehiculo = "1" Then ' Carro
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{3}$")
            ElseIf tipoVehiculo = "2" Then ' Moto
                isValid = System.Text.RegularExpressions.Regex.IsMatch(placa, "^[A-Z]{3}[0-9]{2}[A-Z]{1}$")
            End If
        Else
            isValid = True ' Permitir que el campo esté vacío
        End If

        If isValid Then
            lblPlacaError.Visible = False
            hfPlacaValida.Value = True
            BtnAgregarVehiculo.Enabled = True
        Else
            lblPlacaError.Text = "Placa inválida. " & GetValidationMessage(tipoVehiculo)
            lblPlacaError.Visible = True
            hfPlacaValida.Value = False
            BtnAgregarVehiculo.Enabled = False
        End If
    End Sub

    Private Function GetValidationMessage(tipoVehiculo As String) As String
        If tipoVehiculo = "1" Then ' Carro
            Return "Debe ser 3 letras y 3 números (ej: ABC123)"
        ElseIf tipoVehiculo = "2" Then ' Moto
            Return "Debe ser 3 letras, 2 números y 1 letra (ej: ABC12D)"
        Else
            Return String.Empty
        End If
    End Function

    Private Sub BtnAgregarVehiculo_Click(sender As Object, e As EventArgs) Handles BtnAgregarVehiculo.Click

        Try

            If TxPlaca.Text <> "" And TxTipoVehiculo.SelectedValue <> "" And TxMarca.Text <> "" And TxModelo.Text <> "" And TxColor.Text <> "" Then

                Dim dt1 As New DataTable

                If Session("datos1") Is Nothing Then

                    'columnas
                    dt1.Columns.Add("TIPO VEHICULO")
                    dt1.Columns.Add("PLACA")
                    dt1.Columns.Add("MARCA")
                    dt1.Columns.Add("MODELO")
                    dt1.Columns.Add("COLOR")

                    'Agregar Datos    
                    Dim row As DataRow = dt1.NewRow()
                    row("TIPO VEHICULO") = TxTipoVehiculo.SelectedItem.Text
                    row("PLACA") = TxPlaca.Text
                    row("MARCA") = TxMarca.Text.ToUpper()
                    row("MODELO") = TxModelo.Text.ToUpper()
                    row("COLOR") = TxColor.Text.ToUpper()

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                Else
                    dt1 = TryCast(Session("datos1"), DataTable)
                    'Agregar Datos  

                    Dim row As DataRow = dt1.NewRow()
                    row("TIPO VEHICULO") = TxTipoVehiculo.SelectedItem.Text
                    row("PLACA") = TxPlaca.Text
                    row("MARCA") = TxMarca.Text.ToUpper()
                    row("MODELO") = TxModelo.Text.ToUpper()
                    row("COLOR") = TxColor.Text.ToUpper()

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                End If
                TxTipoVehiculo.SelectedIndex = 0
                TxPlaca.Text = ""
                TxMarca.Text = ""
                TxModelo.Text = ""
                TxColor.Text = ""
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
                "IV.Id_Inmueble, " &
                "V.Placa, " &
                "V.Id_Tipo_Vehiculo, " &
                "T.Tipo_Vehiculo, " &
                "V.Marca, " &
                "V.Modelo, " &
                "V.Color " &
                "FROM Adm_Inmueble_Vehiculo IV LEFT JOIN Adm_Vehiculo V ON V.Placa = IV.Placa " &
                "LEFT JOIN Adm_Tipo_Vehiculo T ON T.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo " &
                "WHERE IV.Id_Inmueble = @Id_Inmueble"

                Using cmd As New SqlCommand(query, conn)
                    'Se define el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Inmueble", inmueble)

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim dt1 As New DataTable()
                        dt1.Columns.Add("TIPO VEHICULO")
                        dt1.Columns.Add("PLACA")
                        dt1.Columns.Add("MARCA")
                        dt1.Columns.Add("MODELO")
                        dt1.Columns.Add("COLOR")

                        While dr.Read()
                            ' Aquí accedo a los datos de cada fila según necesites
                            Dim Tipo_Vehiculo As String = dr("Tipo_Vehiculo").ToString()
                            Dim Placa As String = dr("Placa").ToString()
                            Dim Marca As String = dr("Marca").ToString()
                            Dim Modelo As String = dr("Modelo").ToString()
                            Dim Color As String = dr("Color").ToString()

                            ' Agregar los datos a la tabla
                            dt1.Rows.Add(Tipo_Vehiculo, Placa, Marca, Modelo, Color)
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

    Private Function Validar_Detalle_Inmueble(ByVal Id_Inmueble As String, ByVal Placa As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Adm_Inmueble_Vehiculo WHERE Id_Inmueble = @Id_Inmueble AND Placa = @Placa"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)
                    cmd.Parameters.AddWithValue("@Placa", Placa)

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

    Private Function Validar_Vehiculo(ByVal Placa As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Adm_Vehiculo WHERE Placa = @Placa"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Placa", Placa)

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

    Private Sub Eliminar_Detalle_Inmueble(ByVal Id_Inmueble As String, ByVal Placa As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Adm_Inmueble_Vehiculo WHERE Id_Inmueble = @Id_Inmueble AND Placa = @Placa"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Id_Inmueble", Id_Inmueble)
                    cmd.Parameters.AddWithValue("@Placa", Placa)

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

    Private Sub Eliminar_Vehiculo(ByVal Placa As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Adm_Vehiculo WHERE Placa = @Placa"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Placa", Placa)

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

                If Not Validar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Convert.ToString(GridView1.SelectedRow.Cells(2).Text)) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        Dim queryInsert As String = "INSERT INTO dbo.Adm_Inmueble_Vehiculo (" &
                                                "Id_Inmueble, " &
                                                "Placa) " &
                                                "VALUES (" &
                                                "@Id_Inmueble, " &
                                                "@Placa)"

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Id_Inmueble", Convert.ToString(Session("Id_inmueble")))
                            cmdInsert.Parameters.AddWithValue("@Placa", Convert.ToString(GridView1.SelectedRow.Cells(2).Text))

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

    Private Sub Guardar_Vehiculo()

        Try

            Dim cont As Integer = 0

            For Each r As GridViewRow In GridView1.Rows

                GridView1.SelectRow(cont)

                If Not Validar_Vehiculo(Convert.ToString(GridView1.SelectedRow.Cells(2).Text)) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        Dim queryInsert As String = "INSERT INTO dbo.Adm_Vehiculo (" &
                                                "Placa, " &
                                                "Id_Tipo_Vehiculo, " &
                                                "Marca, " &
                                                "Modelo, " &
                                                "Color) " &
                                                "VALUES (" &
                                                "@Placa, " &
                                                "@Id_Tipo_Vehiculo, " &
                                                "@Marca, " &
                                                "@Modelo, " &
                                                "@Color)"

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Placa", Convert.ToString(GridView1.SelectedRow.Cells(2).Text))
                            If Convert.ToString(GridView1.SelectedRow.Cells(1).Text) = "CARRO" Then
                                cmdInsert.Parameters.AddWithValue("@Id_Tipo_Vehiculo", 1)
                            Else
                                cmdInsert.Parameters.AddWithValue("@Id_Tipo_Vehiculo", 2)
                            End If
                            cmdInsert.Parameters.AddWithValue("@Marca", Convert.ToString(GridView1.SelectedRow.Cells(3).Text))
                            cmdInsert.Parameters.AddWithValue("@Modelo", Convert.ToString(GridView1.SelectedRow.Cells(4).Text))
                            cmdInsert.Parameters.AddWithValue("@Color", Convert.ToString(GridView1.SelectedRow.Cells(5).Text))

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
        Session("Id_inmueble") = Id_inmuebleLabel.Text

        If e.CommandName = "Editar" Then
            PUsuario.Visible = True
            TxIdInmueble.Text = Id_inmuebleLabel.Text
            Buscar_Detalle_Inmueble(Id_inmuebleLabel.Text)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Guardar_Vehiculo()
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
            Dim Placa As String

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            dt.Columns.Add("TIPO VEHICULO")
            dt.Columns.Add("PLACA")
            dt.Columns.Add("MARCA")
            dt.Columns.Add("MODELO")
            dt.Columns.Add("COLOR")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Placa = Convert.ToString(row.Cells(2).Text)
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("TIPO VEHICULO") = row.Cells(1).Text
                dr("PLACA") = row.Cells(2).Text
                dr("MARCA") = row.Cells(3).Text
                dr("MODELO") = row.Cells(4).Text
                dr("COLOR") = row.Cells(5).Text

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Placa) Then

                Eliminar_Detalle_Inmueble(Convert.ToString(Session("Id_inmueble")), Placa)

            End If

        Catch ex As Exception
            Throw ex

        End Try

    End Sub

    Private Sub Eliminar(ByRef index As Integer)

        Try
            Dim Placa As String

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            dt.Columns.Add("TIPO VEHICULO")
            dt.Columns.Add("PLACA")
            dt.Columns.Add("MARCA")
            dt.Columns.Add("MODELO")
            dt.Columns.Add("COLOR")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Placa = Convert.ToString(row.Cells(2).Text)
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("TIPO VEHICULO") = row.Cells(1).Text
                dr("PLACA") = row.Cells(2).Text
                dr("MARCA") = row.Cells(3).Text
                dr("MODELO") = row.Cells(4).Text
                dr("COLOR") = row.Cells(5).Text

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Elimina la fila correspondiente de tu fuente de datos  
            dt.Rows(index).Delete()

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Vehiculo(Placa) Then

                Eliminar_Vehiculo(Placa)

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
        Response.Redirect("Asignar_Vehiculo.aspx")
    End Sub

End Class