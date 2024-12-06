Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Accesos
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

    Private Sub BtCrearEmpresa_ServerClick(sender As Object, e As EventArgs) Handles BtCrearEmpresa.ServerClick
        Response.Redirect("Crear_Empresa.aspx")
    End Sub

    Private Sub BtCrearSedes_ServerClick(sender As Object, e As EventArgs) Handles BtCrearSedes.ServerClick
        Response.Redirect("Crear_Sedes.aspx")
    End Sub

    Private Sub BtCrearAccesos_ServerClick(sender As Object, e As EventArgs) Handles BtCrearAccesos.ServerClick
        Response.Redirect("Crear_Accesos.aspx")
    End Sub

    Private Function Consultar_ID_Acceso(ByVal Acceso As String) As Integer
        Try
            ' Definir la cadena de conexión
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Definir la consulta
            Dim query As String = "SELECT Id_Acceso FROM Adm_Accesos WHERE Nombre_Acceso = @Acceso"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@Acceso", Acceso)

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

    Private Sub BtnAgregarSede_Click(sender As Object, e As EventArgs) Handles BtnAgregarSede.Click

        Try

            If TxNombreAcceso.Text <> "" Then

                Dim dt1 As New DataTable

                If Session("datos1") Is Nothing Then

                    'columnas
                    dt1.Columns.Add("NOMBRE ACCESO")
                    dt1.Columns.Add("IP CAMARA")
                    dt1.Columns.Add("ID TIPO EVENTO")
                    dt1.Columns.Add("TIPO EVENTO")

                    'Agregar Datos    
                    Dim row As DataRow = dt1.NewRow()
                    row("NOMBRE ACCESO") = TxNombreAcceso.Text
                    row("IP CAMARA") = TxIpCamara.Text
                    row("ID TIPO EVENTO") = TxTipoEvento.SelectedValue
                    If TxTipoEvento.SelectedValue = "" Then
                        row("TIPO EVENTO") = ""
                    Else
                        row("TIPO EVENTO") = TxTipoEvento.SelectedItem.Text
                    End If

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                Else
                    dt1 = TryCast(Session("datos1"), DataTable)
                    'Agregar Datos  

                    Dim row As DataRow = dt1.NewRow()
                    row("NOMBRE ACCESO") = TxNombreAcceso.Text
                    row("IP CAMARA") = TxIpCamara.Text
                    row("ID TIPO EVENTO") = TxTipoEvento.SelectedValue
                    If TxTipoEvento.SelectedValue = "" Then
                        row("TIPO EVENTO") = ""
                    Else
                        row("TIPO EVENTO") = TxTipoEvento.SelectedItem.Text
                    End If

                    dt1.Rows.Add(row)
                    'enlazas datatable a griedview
                    GridView1.DataSource = dt1
                    GridView1.DataBind()
                    Session("datos1") = dt1
                End If
                TxNombreAcceso.Text = ""
                TxIpCamara.Text = ""
                TxTipoEvento.SelectedIndex = 0
            End If

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Sub Buscar_Detalle_Sede(ByVal Sede)

        Try

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())

                Dim query As String =
                "SELECT " &
                "Id_Acceso, " &
                "Nombre_Acceso, " &
                "IP_Camara, " &
                "Tipo_Evento, " &
                "Id_Sede " &
                "FROM Adm_Accesos " &
                "WHERE Id_Sede = @Sede"

                Using cmd As New SqlCommand(query, conn)
                    'Se define el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@Sede", Sede)

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim dt1 As New DataTable()
                        dt1.Columns.Add("NOMBRE ACCESO")
                        dt1.Columns.Add("IP CAMARA")
                        dt1.Columns.Add("ID TIPO EVENTO")
                        dt1.Columns.Add("TIPO EVENTO")

                        While dr.Read()
                            ' Aquí accedo a los datos de cada fila según necesites
                            Dim Acceso As String = dr("Nombre_Acceso").ToString()
                            Dim Ip_Camara As String = dr("IP_Camara").ToString()
                            Dim Tipo_EventoId As String = dr("Tipo_Evento").ToString()
                            Dim Tipo_Evento As String = ""
                            If dr("Tipo_Evento").ToString() = "True" Then
                                Tipo_Evento = "ENTRADA"
                            ElseIf dr("Tipo_Evento").ToString() = "False" Then
                                Tipo_Evento = "SALIDA"
                            Else
                                Tipo_Evento = ""
                            End If

                            ' Agregar los datos a la tabla
                            dt1.Rows.Add(Acceso, Ip_Camara, Tipo_EventoId, Tipo_Evento)
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

    Private Function Validar_Acceso(ByVal IdAcceso As String) As Boolean

        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para verificar si el dato existe
            Dim query As String = "SELECT COUNT(*) FROM Adm_Accesos WHERE Id_Acceso = @IdAcceso"

            ' Variable para almacenar el resultado de la consulta
            Dim existe As Boolean = False

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar los parámetros para la consulta
                    cmd.Parameters.AddWithValue("@IdAcceso", IdAcceso)

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

    Private Sub Guardar_Acceso()

        Try

            Dim cont As Integer = 0

            For Each r As GridViewRow In GridView1.Rows

                GridView1.SelectRow(cont)

                Dim Id_Acceso As Integer = Consultar_ID_Acceso(Convert.ToString(GridView1.SelectedRow.Cells(1).Text))

                If Not Validar_Acceso(Id_Acceso) Then

                    Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                        conn.Open()

                        ' Obtener el valor máximo de Id_Tercero y sumarle 1
                        Dim queryMaxId As String = "SELECT ISNULL(MAX(Id_Acceso), 0) + 1 FROM Adm_Accesos"
                        Dim newIdAcceso As Integer

                        Using cmdMaxId As New SqlCommand(queryMaxId, conn)
                            newIdAcceso = Convert.ToInt32(cmdMaxId.ExecuteScalar())
                        End Using

                        Dim queryInsert As String = "INSERT INTO dbo.Adm_Accesos (" &
                                                "Id_Acceso, " &
                                                "Nombre_Acceso, " &
                                                "IP_Camara, " &
                                                "Tipo_Evento, " &
                                                "Id_Sede) " &
                                                "VALUES (" &
                                                "@Id_Acceso, " &
                                                "@Nombre_Acceso, " &
                                                "@IP_Camara, " &
                                                "@Tipo_Evento, " &
                                                "@Id_Sede) "

                        Using cmdInsert As New SqlCommand(queryInsert, conn)
                            cmdInsert.Parameters.AddWithValue("@Id_Acceso", newIdAcceso)
                            cmdInsert.Parameters.AddWithValue("@Nombre_Acceso", Convert.ToString(GridView1.SelectedRow.Cells(1).Text))
                            Dim ipCamara = Convert.ToString(GridView1.SelectedRow.Cells(2).Text).Trim()
                            If String.IsNullOrWhiteSpace(ipCamara) OrElse ipCamara = "&nbsp;" Then
                                cmdInsert.Parameters.AddWithValue("@IP_Camara", DBNull.Value)
                            Else
                                cmdInsert.Parameters.AddWithValue("@IP_Camara", ipCamara)
                            End If

                            Dim tipoEvento = Convert.ToString(GridView1.SelectedRow.Cells(3).Text).Trim()
                            If String.IsNullOrWhiteSpace(tipoEvento) OrElse tipoEvento = "&nbsp;" Then
                                cmdInsert.Parameters.AddWithValue("@Tipo_Evento", DBNull.Value)
                            Else
                                cmdInsert.Parameters.AddWithValue("@Tipo_Evento", tipoEvento)
                            End If

                            cmdInsert.Parameters.AddWithValue("@Id_Sede", TxIdSede.ToolTip)

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

    Private Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Ocultar fila
            e.Row.Cells(3).Visible = False
        ElseIf e.Row.RowType = DataControlRowType.Header Then
            ' Ocultar encabezados
            e.Row.Cells(3).Visible = False
        End If
    End Sub

    Private Sub Eliminar_Acceso(ByVal IdAcceso As String)

        Try

            ' Cadena de conexión a la base de datos
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            ' Consulta SQL para eliminar el dato
            Dim query As String = "DELETE FROM Adm_Accesos WHERE Id_Acceso = @IdAcceso"

            ' Crear una conexión a la base de datos
            Using conn As New SqlConnection(connectionString)
                ' Crear un comando SQL con la consulta y la conexión
                Using cmd As New SqlCommand(query, conn)
                    ' Asignar el parámetro para el Id_Contacto_Cliente
                    cmd.Parameters.AddWithValue("@IdAcceso", IdAcceso)

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

    Private Sub LvSedes_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvSedes.ItemCommand
        Dim Id_SedeLabel As Label = DirectCast(e.Item.FindControl("Id_SedeLabel"), Label)
        Dim Nombre_SedeLabel As Label = DirectCast(e.Item.FindControl("Nombre_SedeLabel"), Label)
        Session("Id_Sede") = Id_SedeLabel.Text

        If e.CommandName = "Editar" Then
            PAccesos.Visible = True
            TxIdSede.Text = Nombre_SedeLabel.Text
            TxIdSede.ToolTip = Id_SedeLabel.Text
            Buscar_Detalle_Sede(Id_SedeLabel.Text)
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        Guardar_Acceso()

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Acceso Guardado Correctamente', 'success');")
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

    Private Sub Eliminar(ByRef index As Integer)

        Try
            Dim Id_Acceso As Integer

            Dim dt As New DataTable()

            If Session("datos1") Is Nothing Then
                dt = Session("datos1")
                ' Crear un DataTable para almacenar los datos

            End If
            dt.Columns.Add("NOMBRE ACCESO")
            dt.Columns.Add("IP CAMARA")
            dt.Columns.Add("ID TIPO EVENTO")
            dt.Columns.Add("TIPO EVENTO")

            ' Recorrer las filas del GridView y agregarlas al DataTable
            For Each row As GridViewRow In GridView1.Rows
                ' Crear una nueva fila para el DataTable
                Dim dr As DataRow = dt.NewRow()

                If row.RowIndex = index Then
                    Id_Acceso = Consultar_ID_Acceso(Convert.ToString(row.Cells(1).Text))
                End If

                ' Asignar los valores de las celdas al DataRow
                dr("NOMBRE ACCESO") = row.Cells(1).Text
                dr("IP CAMARA") = row.Cells(2).Text
                dr("ID TIPO EVENTO") = row.Cells(3).Text
                dr("TIPO EVENTO") = row.Cells(4).Text

                ' Agregar la fila al DataTable
                dt.Rows.Add(dr)
            Next

            ' Elimina la fila correspondiente de tu fuente de datos  
            dt.Rows(index).Delete()

            ' Vuelve a enlazar tu fuente de datos al GridView para reflejar los cambios
            Session("datos1") = dt
            GridView1.DataSource = dt
            GridView1.DataBind()


            If Validar_Acceso(Id_Acceso) Then

                Eliminar_Acceso(Id_Acceso)

            End If

        Catch ex As Exception
            Throw ex

        End Try

    End Sub

    Private Sub BtAceptar_Borrar_Click(sender As Object, e As EventArgs) Handles BtAceptar_Borrar.Click

        Try

            Eliminar(Session("Index"))
            Session("Index") = Nothing

        Catch ex As Exception
            ErrorOp = ex.Message
            EnviarCorreoError()
        End Try

    End Sub

    Private Sub TxBuscar_TextChanged(sender As Object, e As EventArgs) Handles TxBuscar.TextChanged
        If TxBuscar.Text = Nothing Then
            LvSedes.DataSourceID = "SqlSedes"
        Else
            LvSedes.DataSourceID = "Sql_Buscar_Sede"
        End If
        LvSedes.DataBind()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Accesos.aspx")
    End Sub

End Class