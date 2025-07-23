Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Data.SqlClient
Imports System.Security.Cryptography.X509Certificates

Public Class Crear_Empresa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("Usuario") = User.Identity.Name
    End Sub

    Public Function HayRegistrosEnAdmEmpresa() As Boolean
        Dim hayRegistros As Boolean = False

        ' Cadena de conexión a tu base de datos
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para verificar si existe al menos un registro en la tabla Adm_Empresa
        Dim query As String = "SELECT COUNT(*) FROM Adm_Empresa"

        ' Crear la conexión y el comando SQL
        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                Try
                    ' Abrir la conexión
                    connection.Open()

                    ' Ejecutar el comando y obtener el número de registros
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                    ' Si el recuento es mayor que 0, entonces hay registros
                    If count > 0 Then
                        hayRegistros = True
                    End If
                Catch ex As Exception
                Finally
                    ' Asegúrate de cerrar la conexión
                    connection.Close()
                End Try
            End Using
        End Using

        Return hayRegistros
    End Function

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

    Dim errores As New List(Of String)

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

    Private Sub Guardar_Empresa()
        Session("sql") = "INSERT INTO Adm_Empresa ("
        Session("sql") += "Id_Empresa, "
        Session("sql") += "Empresa, "
        Session("sql") += "Direccion, "
        Session("sql") += "Telefono, "
        Session("sql") += "Administrador, "
        Session("sql") += "Titulo_Personas, "
        Session("sql") += "Segundos_Facial, "
        Session("sql") += "Tiene_Faciales, "
        Session("sql") += "Tiene_LPR, "
        Session("sql") += "Control_Restaurante, "
        Session("sql") += "Control_Lockers, "
        Session("sql") += "Control_Llaves, "
        Session("sql") += "Control_Mensajeria, "
        Session("sql") += "Control_Previsita, "
        Session("sql") += "Visita_Proveedores, "
        Session("sql") += "Visitantes_Facial, "
        If FLogo.HasFile = True Then
            Session("sql") += "Logo, "
        End If
        Session("sql") += "Tel_Administrador) VALUES ("

        Session("sql") += "" & TxNitEmpresa.Text & ", "
        Session("sql") += "UPPER('" & TxNombreEmpresa.Text & "'), "
        Session("sql") += "'" & TxDireccion.Text & "', "
        Session("sql") += "" & TxTelefono.Text & ", "
        Session("sql") += "UPPER('" & TxAdministrador.Text & "'), "
        Session("sql") += "UPPER('" & TxTituloPersonal.Text & "'), "
        Session("sql") += "" & TxSegundosFacial.Text & ", "
        If TxTieneFaciales.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxTieneLPR.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxControlRestaurante.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxControlLockers.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxControlLlaves.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxControlMensajeria.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxControlPrevisita.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxControlProveedores.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If TxVisitantesFacial.Checked = True Then
            Session("sql") += "1, "
        Else
            Session("sql") += "0, "
        End If
        If FLogo.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLogo.FileName)

            FLogo.SaveAs(Server.MapPath("../../Adjunto/Logo/" + TxNitEmpresa.Text + Ext.ToString))
            Session("sql") += "'" & TxNitEmpresa.Text + Ext.ToString & "', "
        End If
        Session("sql") += "" & TxTelefonoAdministrador.Text & ") "
        Guardar_Datos()
    End Sub

    Private Sub Actualizar_Empresa()
        Session("sql") = "UPDATE dbo.Adm_Empresa SET "
        Session("sql") += "Id_Empresa = " & TxNitEmpresa.Text & ", "
        Session("sql") += "Empresa = UPPER('" & TxNombreEmpresa.Text & "'), "
        Session("sql") += "Titulo_Personas = UPPER('" & TxTituloPersonal.Text & "'), "
        Session("sql") += "Direccion = '" & TxDireccion.Text & "', "
        Session("sql") += "Telefono = " & TxTelefono.Text & ", "
        Session("sql") += "Segundos_Facial = " & TxSegundosFacial.Text & ", "
        Session("sql") += "Administrador = UPPER('" & TxAdministrador.Text & "'), "
        Session("sql") += "Tiene_Faciales = " & If(TxTieneFaciales.Checked, "1", "0") & ", "
        Session("sql") += "Tiene_LPR = " & If(TxTieneLPR.Checked, "1", "0") & ", "
        Session("sql") += "Control_Restaurante = " & If(TxControlRestaurante.Checked, "1", "0") & ", "
        Session("sql") += "Control_Lockers = " & If(TxControlLockers.Checked, "1", "0") & ", "
        Session("sql") += "Control_Llaves = " & If(TxControlLlaves.Checked, "1", "0") & ", "
        Session("sql") += "Control_Mensajeria = " & If(TxControlMensajeria.Checked, "1", "0") & ", "
        Session("sql") += "Control_Previsita = " & If(TxControlPrevisita.Checked, "1", "0") & ", "
        Session("sql") += "Visita_Proveedores = " & If(TxControlProveedores.Checked, "1", "0") & ", "
        Session("sql") += "Visitantes_Facial = " & If(TxVisitantesFacial.Checked, "1", "0") & ", "
        If FLogo.HasFile = True Then
            Dim Ext As String = System.IO.Path.GetExtension(FLogo.FileName)

            FLogo.SaveAs(Server.MapPath("../../Adjunto/Logo/" + TxNitEmpresa.Text + Ext.ToString))
            Session("sql") += "Logo = '" & TxNitEmpresa.Text + Ext.ToString & "', "
        End If
        Session("sql") += "Tel_Administrador = " & TxTelefonoAdministrador.Text & " "
        Session("sql") += "WHERE Id_Empresa = " & Session("Id_Empresa") & ""
        Guardar_Datos()
    End Sub

    Private Sub Consultar_Empresa(ByVal Nit As Long)
        Session("sql") = "SELECT Id_Empresa, Empresa, Direccion, Telefono, Logo, Administrador, Titulo_Personas, Segundos_Facial, Tel_Administrador, Tiene_Faciales, Tiene_LPR, Control_Restaurante, Control_Lockers, Control_Llaves, Control_Mensajeria, Control_Previsita, Visita_Proveedores, Visitantes_Facial FROM Adm_Empresa "
        Session("sql") += "WHERE Id_Empresa = " & Nit & " "

        Ejecutar_Query()
        If dr.HasRows Then
            dr.Read()
            TxNitEmpresa.Text = dr("Id_Empresa").ToString
            TxNombreEmpresa.Text = dr("Empresa").ToString
            TxTituloPersonal.Text = dr("Titulo_Personas").ToString
            TxDireccion.Text = dr("Direccion").ToString
            TxTelefono.Text = dr("Telefono").ToString
            TxAdministrador.Text = dr("Administrador").ToString
            TxSegundosFacial.Text = dr("Segundos_Facial").ToString
            TxTelefonoAdministrador.Text = dr("Tel_Administrador").ToString
            TxTieneFaciales.Checked = If(dr("Tiene_Faciales").ToString() = "True", True, False)
            TxTieneLPR.Checked = If(dr("Tiene_LPR").ToString() = "True", True, False)
            TxControlRestaurante.Checked = If(dr("Control_Restaurante").ToString() = "True", True, False)
            TxControlLockers.Checked = If(dr("Control_Lockers").ToString() = "True", True, False)
            TxControlLlaves.Checked = If(dr("Control_Llaves").ToString() = "True", True, False)
            TxControlMensajeria.Checked = If(dr("Control_Mensajeria").ToString() = "True", True, False)
            TxControlPrevisita.Checked = If(dr("Control_Previsita").ToString() = "True", True, False)
            TxControlProveedores.Checked = If(dr("Visita_Proveedores").ToString() = "True", True, False)
            TxVisitantesFacial.Checked = If(dr("Visitantes_Facial").ToString() = "True", True, False)
            If dr("Visitantes_Facial").ToString() = "True" Then
                Cargar_Camaras()
                Visitantes_Facial.Visible = True
                Consultar_Permisos()
            End If
            BtGuardar.Text = "ACTUALIZAR"
            dr.Close()
        End If

        conn_Administracion1.Close()
        conn_Administracion2.Close()
    End Sub

    Private Sub LvEmpresa_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles LvEmpresa.ItemCommand
        Dim Id_EmpresaLabel As Label = DirectCast(e.Item.FindControl("Id_EmpresaLabel"), Label)
        Session("Id_Empresa") = Id_EmpresaLabel.Text
        If e.CommandName = "Editar" Then
            Consultar_Empresa(Id_EmpresaLabel.Text)
        End If
    End Sub

    ' Clase para almacenar los datos de la cámara
    Public Class Camara
        Public Property Nombre_Acceso As String
        Public Property IP_Camara As String
    End Class

    Private Function ObtenerCamarasDesdeBaseDeDatos() As List(Of Camara)
        Dim camaras As New List(Of Camara)()
        Try
            ' Ejecutamos la consulta SQL para obtener los datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Nombre_Acceso, IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Sede = " & Session("Sucursal_Usuario").ToString & "")

            ' Agregar los datos al List
            For Each row As DataRow In dt.Rows
                camaras.Add(New Camara() With {
                .Nombre_Acceso = row("Nombre_Acceso").ToString(),
                .IP_Camara = row("IP_Camara").ToString()
            })
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las cámaras:" & ex.Message)
        End Try
        Return camaras
    End Function

    ' Método para ejecutar una consulta SQL y retornar un DataTable
    Private Function EjecutarConsultaSQL(query As String) As DataTable
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    Try
                        conn.Open()
                        da.Fill(dt)
                    Catch ex As Exception
                        errores.Add("Error al ejecutar la consulta SQL:" & ex.Message)
                    End Try
                    Return dt
                End Using
            End Using
        End Using
    End Function

    Private Sub Cargar_Camaras()
        Dim camaras As List(Of Camara) = ObtenerCamarasDesdeBaseDeDatos()

        If camaras IsNot Nothing Then
            ' Crear la lista con los valores necesarios para mostrar en el ListView
            Dim camarasMostrar As New List(Of Object)()
            For Each camara In camaras
                camarasMostrar.Add(New With {.Texto = camara.Nombre_Acceso, .Tooltip = camara.IP_Camara})
            Next

            ' Asignar la fuente de datos al ListView
            LvCamaras.DataSource = camarasMostrar
            LvCamaras.DataBind()
        Else
            Dim Script As String = $"swal('OJO!', 'No se pudieron cargar las cámaras.', 'warning');"
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", Script, True)
        End If
    End Sub

    Protected Sub LvCamaras_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles LvCamaras.ItemDataBound
        If e.Item.ItemType = ListViewItemType.DataItem Then
            ' Obtener el CheckBox dentro del item
            Dim chk As CheckBox = CType(e.Item.FindControl("CheckBox1"), CheckBox)

            ' Obtener los valores del DataItem
            Dim item As Object = e.Item.DataItem
            Dim texto As String = item.Texto
            Dim tooltip As String = item.Tooltip

            ' Asignar el texto y el tooltip
            If chk IsNot Nothing Then
                chk.Text = texto
                chk.ToolTip = tooltip
            End If
        End If
    End Sub

    Private Function ObtenerIdAcceso(IP As String) As Long
        ' Consulta SQL para obtener el Id_Tercero del usuario proporcionado
        Dim query As String = "SELECT Id_Acceso FROM Adm_Accesos WHERE IP_Camara = @IP"
        Dim Id_Acceso As Long = 0

        Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@IP", IP)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Tercero
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    Id_Acceso = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Tercero obtenido
        Return Id_Acceso
    End Function

    Private Sub Consultar_Permisos()
        ' Obtener la lista de IPs que tienen Visitante = 1
        Dim query As String = "SELECT IP_Camara FROM Adm_Accesos WHERE Visitante = 1"

        Dim ipsConPermiso As New List(Of String)()

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(query, conn)
                conn.Open()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        If Not reader.IsDBNull(reader.GetOrdinal("IP_Camara")) Then
                            ipsConPermiso.Add(reader("IP_Camara").ToString())
                        End If
                    End While
                End Using
            End Using
        End Using

        ' Recorrer los ítems del ListView y marcar los checkboxes según IP
        For Each item As ListViewItem In LvCamaras.Items
            Dim chk As CheckBox = CType(item.FindControl("CheckBox1"), CheckBox)

            If chk IsNot Nothing Then
                Dim ipCamara As String = chk.ToolTip

                If ipsConPermiso.Contains(ipCamara) Then
                    chk.Checked = True
                Else
                    chk.Checked = False
                End If
            End If
        Next
    End Sub

    Private Sub Guardar_Permiso_Camaras()
        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()

                For Each item As ListViewItem In LvCamaras.Items
                    Dim chk As CheckBox = CType(item.FindControl("CheckBox1"), CheckBox)

                    If chk IsNot Nothing Then
                        Dim IdAcceso As Long = ObtenerIdAcceso(chk.ToolTip) ' o desde otro campo

                        Dim updateQuery As String = "UPDATE Adm_Accesos SET Visitante = @Visitante WHERE Id_Acceso = @Id_Acceso"

                        Using cmd As New SqlCommand(updateQuery, conn)
                            ' Asignar el valor de Visitante dependiendo del estado del checkbox
                            If chk.Checked Then
                                cmd.Parameters.AddWithValue("@Visitante", 1)
                            Else
                                cmd.Parameters.AddWithValue("@Visitante", DBNull.Value)
                            End If

                            cmd.Parameters.AddWithValue("@Id_Acceso", IdAcceso)

                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next
            End Using
        Catch ex As Exception
            errores.Add("Error en Guardar_Permiso_Camaras(): " & ex.Message)
        End Try
    End Sub

    Private Sub Actualizar_Permiso_Camaras()
        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()

                For Each item As ListViewItem In LvCamaras.Items
                    Dim chk As CheckBox = CType(item.FindControl("CheckBox1"), CheckBox)

                    If chk IsNot Nothing Then
                        Dim IdAcceso As Long = ObtenerIdAcceso(chk.ToolTip) ' o de otro campo del item

                        Dim updateQuery As String = "UPDATE Adm_Accesos SET Visitante = @Visitante WHERE Id_Acceso = @Id_Acceso"

                        Using cmd As New SqlCommand(updateQuery, conn)
                            If chk.Checked Then
                                cmd.Parameters.AddWithValue("@Visitante", 1)
                            Else
                                cmd.Parameters.AddWithValue("@Visitante", DBNull.Value)
                            End If

                            cmd.Parameters.AddWithValue("@Id_Acceso", IdAcceso)

                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next
            End Using
        Catch ex As Exception
            errores.Add("Error en Actualizar_Permiso_Camaras(): " & ex.Message)
        End Try
    End Sub

    Protected Sub TxVisitantesFacial_CheckedChanged(sender As Object, e As EventArgs) Handles TxVisitantesFacial.CheckedChanged
        If TxVisitantesFacial.Checked = True Then
            Cargar_Camaras()
            Visitantes_Facial.Visible = True
        Else
            Visitantes_Facial.Visible = False
        End If
    End Sub

    Private Sub BtGuardar_Click(sender As Object, e As EventArgs) Handles BtGuardar.Click
        If BtGuardar.Text = "GUARDAR" Then
            If HayRegistrosEnAdmEmpresa() = True Then
                ' Si ya hay registros, mostrar un mensaje de advertencia
                Dim script As String = String.Format("swal('OJO!', 'Ya hay una empresa creada', 'warning');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Else
                ' Si no hay registros, proceder a guardar la empresa
                Guardar_Empresa()
                If TxVisitantesFacial.Checked = True Then
                    Guardar_Permiso_Camaras()
                End If
                Dim script As String = String.Format("swal('Excelente!', 'Empresa Guardada Correctamente', 'success');")
                ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
                Timer2.Interval = 3000
            End If
        ElseIf BtGuardar.Text = "ACTUALIZAR" Then
            Actualizar_Empresa()
            If TxVisitantesFacial.Checked = True Then
                Actualizar_Permiso_Camaras()
            End If
        End If

        If ErrorOp = Nothing Then
            Dim script As String = String.Format("swal('Excelente!', 'Empresa Guardada Correctamente', 'success');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            Timer1.Interval = 3000
        Else
            Dim script As String = String.Format("swal('Error!', 'Verificar Formulario y volver a intentar', 'Warning');")
            ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)
            EnviarCorreoError()
        End If

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Empresa.aspx")
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Timer1.Interval = 999999999
        Response.Redirect("Crear_Sedes.aspx")
    End Sub

End Class