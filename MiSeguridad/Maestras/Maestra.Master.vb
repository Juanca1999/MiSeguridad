Imports System.IO
Imports System.Data.SqlClient
Imports System.Security.Cryptography

Public Class Maestra
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            EstablecerLogo()

            Dim fechaExpiracion As DateTime
            Dim rutaLicencia As String = HttpContext.Current.Request.ApplicationPath & "/Diseno/2024/li.dat"
            Dim fechaLicencia As String = DesencriptarArchivo(Server.MapPath(rutaLicencia), "M1r0@cc3$$P0int!2024")


            If DateTime.TryParse(fechaLicencia, fechaExpiracion) Then
                If DateTime.Now > fechaExpiracion Then
                    Response.Redirect("../RenovarLicencia.aspx")
                Else
                    If Session("MensajeLicencia") IsNot Nothing Then
                        Dim mensajeLicencia As String = Session("MensajeLicencia").ToString()

                        Dim script As String = String.Format("swal({{ title: 'OJO!', text: '{0}', type: 'warning', allowOutsideClick: false }});", mensajeLicencia)
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(System.Web.UI.Page), "redirect", script, True)

                        Session.Remove("MensajeLicencia")
                    Else
                        'SE CONSULTA LA SEDE DONDE ESTA EL USUARIO 
                        Consultar_Info_Usuario()

                        If Session("Id_Rol") = "10" Then
                            lblVisitantes_sin_salida.Visible = False
                            lblEmpleados_sin_salida.Visible = False
                            lblContratistas_sin_salida.Visible = False
                            lblGeneral_sin_salida.Visible = False
                        End If

                        'SE AGREGA CONSULTA PARA LA CANTIDAD DE VISITANTES
                        Dim view As DataView = CType(SqlCantidadVisitantes.Select(DataSourceSelectArguments.Empty), DataView)
                            If view IsNot Nothing AndAlso view.Count > 0 Then
                                Dim cantidad As Integer = Convert.ToInt32(view(0)("CANTIDAD"))
                                LabelCantVisit.Text = cantidad.ToString()
                            Else
                                LabelCantVisit.Text = 0
                            End If

                            'SE AGREGA CONSULTA PARA LA CANTIDAD DE EMPLEADOS
                            Dim view2 As DataView = CType(SqlCantidadEmpleados.Select(DataSourceSelectArguments.Empty), DataView)
                            If view2 IsNot Nothing AndAlso view2.Count > 0 Then
                                Dim cantidad As Integer = Convert.ToInt32(view2(0)("CANTIDAD"))
                                LabelCantEmp.Text = cantidad.ToString()
                            Else
                                LabelCantEmp.Text = 0
                            End If

                            'SE AGREGA CONSULTA PARA LA CANTIDAD DE CONTRATISTAS
                            Dim view3 As DataView = CType(SqlCantidadContratistas.Select(DataSourceSelectArguments.Empty), DataView)
                            If view3 IsNot Nothing AndAlso view3.Count > 0 Then
                                Dim cantidad As Integer = Convert.ToInt32(view3(0)("CANTIDAD"))
                                LabelCantCont.Text = cantidad.ToString()
                            Else
                                LabelCantCont.Text = 0
                            End If

                            'SE AGREGA CONSULTA PARA LA CANTIDAD GENERAL
                            Dim view4 As DataView = CType(SqlCantidadGeneral.Select(DataSourceSelectArguments.Empty), DataView)
                            If view4 IsNot Nothing AndAlso view4.Count > 0 Then
                                Dim cantidad As Integer = Convert.ToInt32(view4(0)("CANTIDAD"))
                                LabelCantGen.Text = cantidad.ToString()
                            Else
                                LabelCantGen.Text = 0
                            End If

                        End If
                    End If
            Else
                Response.Redirect("../ErrorLicencia.aspx")
            End If
        End If
    End Sub

    Private Sub Consultar_Info_Usuario()
        Try
            Dim query As String = "SELECT Id_Tercero, Nombres, Id_Sede, Id_Acceso, Id_Rol FROM Terceros WHERE Usuario = @Usuario"

            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                connection.Open()

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Usuario", Session("Usuario"))

                    Using dr As SqlDataReader = command.ExecuteReader()
                        If dr.HasRows Then
                            dr.Read()

                            ' Asignación de Id_Sede verificando si es DBNull
                            If IsDBNull(dr("Id_Sede")) Then
                                Session("Sucursal_Usuario") = Nothing
                            Else
                                Session("Sucursal_Usuario") = dr("Id_Sede").ToString()
                            End If

                            If IsDBNull(dr("Id_Rol")) Then
                                Session("Id_Rol") = Nothing
                            Else
                                Session("Id_Rol") = dr("Id_Rol").ToString()
                            End If
                        Else
                            ' Si no hay registros
                            Session("Sucursal_Usuario") = Nothing
                            Session("Id_Rol") = Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function DesencriptarArchivo(ByVal inputFile As String, ByVal password As String) As String
        Dim key As Byte() = Encoding.UTF8.GetBytes(password.PadRight(32))

        Using aes As Aes = Aes.Create()
            aes.Key = key

            Using fileStream As New FileStream(inputFile, FileMode.Open)
                Dim iv(15) As Byte
                fileStream.Read(iv, 0, iv.Length)

                aes.IV = iv
                Using cryptoStream As New CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read)
                    Using reader As New StreamReader(cryptoStream)
                        Return reader.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using
    End Function

    Private Function ObtenerIdTercero() As Integer
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener el Id_Rol del usuario actual
        Dim query As String = "SELECT Id_Tercero FROM Terceros WHERE Cedula = @Usuario"
        Dim usuario As String = HttpContext.Current.User.Identity.Name
        Dim idTercero As Integer = 0

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Usuario", usuario)

                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener el Id_Rol
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    idTercero = Convert.ToInt64(result)
                End If
            End Using
        End Using

        ' Devolver el Id_Rol obtenido
        Return idTercero
    End Function

    Private Sub EstablecerLogo()
        ' Obtener la cadena de conexión desde la configuración
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        ' Consulta SQL para obtener la ruta del logo
        Dim query As String = "SELECT TOP 1 Logo FROM Adm_Empresa"

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                ' Abrir la conexión
                connection.Open()

                ' Ejecutar la consulta y obtener la ruta de la imagen
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing Then
                    Logo.ImageUrl = "/../../../Adjunto/Logo/" + result.ToString()
                Else
                    Logo.ImageUrl = "/../../../Adjunto/Logo/miro.png"
                End If
            End Using
        End Using
    End Sub

    Private Function Consultar_Control_Restaurante() As Boolean
        Try
            ' Obtener la cadena de conexión desde la configuración
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

            Using connection As New SqlConnection(connectionString)
                connection.Open()

                ' Consulta SQL
                Dim query As String = "SELECT TOP 1 Control_Restaurante FROM Adm_Empresa"
                Using command As New SqlCommand(query, connection)

                    ' Ejecutar la consulta y obtener el resultado
                    Dim result As Object = command.ExecuteScalar()

                    ' Validar si el resultado no es NULL y devolver el valor como Boolean
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return Convert.ToBoolean(result)
                    Else
                        ' Si el resultado es NULL, devolver False
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub lvMenu_ItemDataBound(sender As Object, e As ListViewItemEventArgs) Handles lvMenu.ItemDataBound
        ' Obtener el módulo actual
        Dim dataItem As ListViewDataItem = DirectCast(e.Item, ListViewDataItem)
        Dim idModulo As String = DataBinder.Eval(dataItem.DataItem, "Id_Modulo").ToString()
        Dim idTercero As Integer = ObtenerIdTercero()

        ' Configurar SqlFormularios para el módulo actual
        Dim SqlFormularios As SqlDataSource = DirectCast(e.Item.FindControl("SqlFormularios"), SqlDataSource)
        If SqlFormularios IsNot Nothing Then
            SqlFormularios.SelectCommand = "SELECT P.Id_Tercero, P.Id_Formulario, F.URL, F.Icono, P.Id_submenu, F.Formulario, REPLACE(F.Formulario, ' ', '') AS Formulario_Menu, F.Id_Modulo, M.Modulo FROM Sys_Permisos P INNER JOIN Sys_Formularios F ON P.Id_Formulario = F.Id_Formulario LEFT JOIN Sys_Modulos M ON M.Id_Modulo = F.Id_Modulo WHERE (P.Id_Tercero = @Usuario) AND (F.Id_Modulo = @IdModulo) AND (P.Id_submenu = 0)"
            SqlFormularios.SelectParameters.Clear()
            SqlFormularios.SelectParameters.Add("Usuario", idTercero)
            SqlFormularios.SelectParameters.Add("IdModulo", idModulo)
        End If
    End Sub

    Protected Sub BtExportarExcelEmpleados_Click(sender As Object, e As EventArgs) Handles BtExportarExcelEmpleados.Click
        ' Encuentra el DataPager
        Dim pager As DataPager = TryCast(LvEmpleados.FindControl("DataPager1"), DataPager)

        ' Guarda el estado original de la paginación
        Dim originalPageSize = pager.PageSize
        Dim originalStartRowIndex = pager.StartRowIndex

        ' Desactiva la paginación
        pager.SetPageProperties(0, Integer.MaxValue, False)

        ' Actualiza el ListView
        LvEmpleados.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=Empleados_Sin_Salida.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        ' Crear un formulario HTML temporal para encapsular el ListView
        Dim form As New HtmlForm()
        Controls.Add(form)
        form.Controls.Add(LvEmpleados)

        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            form.RenderControl(hw) ' Renderiza el formulario temporal con el ListView
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End Using

        ' Restaura el estado original de la paginación
        pager.SetPageProperties(originalStartRowIndex, originalPageSize, False)

        ' Actualiza el ListView
        LvEmpleados.DataBind()
    End Sub

    Protected Sub BtExportarExcelVisitantes_Click(sender As Object, e As EventArgs) Handles BtExportarExcelVisitantes.Click
        ' Encuentra el DataPager
        Dim pager As DataPager = TryCast(LvVisitantes.FindControl("DataPager1"), DataPager)

        ' Guarda el estado original de la paginación
        Dim originalPageSize = pager.PageSize
        Dim originalStartRowIndex = pager.StartRowIndex

        ' Desactiva la paginación
        pager.SetPageProperties(0, Integer.MaxValue, False)

        ' Actualiza el ListView
        LvVisitantes.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=Visitantes_Sin_Salida.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        ' Crear un formulario HTML temporal para encapsular el ListView
        Dim form As New HtmlForm()
        Controls.Add(form)
        form.Controls.Add(LvVisitantes)

        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            form.RenderControl(hw) ' Renderiza el formulario temporal con el ListView
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End Using

        ' Restaura el estado original de la paginación
        pager.SetPageProperties(originalStartRowIndex, originalPageSize, False)

        ' Actualiza el ListView
        LvVisitantes.DataBind()
    End Sub

    Protected Sub BtExportarExcelContratistas_Click(sender As Object, e As EventArgs) Handles BtExportarExcelContratistas.Click
        ' Encuentra el DataPager
        Dim pager As DataPager = TryCast(LvContratistas.FindControl("DataPager1"), DataPager)

        ' Guarda el estado original de la paginación
        Dim originalPageSize = pager.PageSize
        Dim originalStartRowIndex = pager.StartRowIndex

        ' Desactiva la paginación
        pager.SetPageProperties(0, Integer.MaxValue, False)

        ' Actualiza el ListView
        LvContratistas.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=Contratistas_Sin_Salida.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        ' Crear un formulario HTML temporal para encapsular el ListView
        Dim form As New HtmlForm()
        Controls.Add(form)
        form.Controls.Add(LvContratistas)

        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            form.RenderControl(hw) ' Renderiza el formulario temporal con el ListView
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End Using

        ' Restaura el estado original de la paginación
        pager.SetPageProperties(originalStartRowIndex, originalPageSize, False)

        ' Actualiza el ListView
        LvContratistas.DataBind()
    End Sub

    Protected Sub BtExportarExcelGeneral_Click(sender As Object, e As EventArgs) Handles BtExportarExcelGeneral.Click
        ' Encuentra el DataPager
        Dim pager As DataPager = TryCast(LvGeneral.FindControl("DataPager1"), DataPager)

        ' Guarda el estado original de la paginación
        Dim originalPageSize = pager.PageSize
        Dim originalStartRowIndex = pager.StartRowIndex

        ' Desactiva la paginación
        pager.SetPageProperties(0, Integer.MaxValue, False)

        ' Actualiza el ListView
        LvGeneral.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=General_Sin_Salida.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        ' Crear un formulario HTML temporal para encapsular el ListView
        Dim form As New HtmlForm()
        Controls.Add(form)
        form.Controls.Add(LvGeneral)

        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            form.RenderControl(hw) ' Renderiza el formulario temporal con el ListView
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End Using

        ' Restaura el estado original de la paginación
        pager.SetPageProperties(originalStartRowIndex, originalPageSize, False)

        ' Actualiza el ListView
        LvGeneral.DataBind()
    End Sub

    Private Sub Cerrar_Sesion_ServerClick(sender As Object, e As EventArgs) Handles Cerrar_Sesion.ServerClick
        Session.Abandon()
        FormsAuthentication.SignOut()
        Session.Clear()
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Redirect("~/Login.aspx")
    End Sub

End Class