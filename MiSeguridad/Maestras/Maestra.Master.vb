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

                    End If
                End If
            Else
                Response.Redirect("../ErrorLicencia.aspx")
            End If
        End If
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

    Private Sub Cerrar_Sesion_ServerClick(sender As Object, e As EventArgs) Handles Cerrar_Sesion.ServerClick
        Session.Abandon()
        FormsAuthentication.SignOut()
        Session.Clear()
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Redirect("~/Login.aspx")
    End Sub

End Class