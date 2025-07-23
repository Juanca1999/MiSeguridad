Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class Proceso_Programado_Usuarios
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Ejecutar_Proceso_Empleados()
    End Sub

    Dim errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Private Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                ipCamaras.Add(row("IP_Camara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las IPs de las cámaras:" & ex.Message)
        End Try
        Return ipCamaras
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

    Private Sub Consultar_Id_Persona()
        Dim Valor As Integer = ObtenerMaxId("Id_Tercero", "Terceros")
        Session("Id_Tercero") = Valor + 1
    End Sub

    Private Sub Consultar_Id_HikCamara()
        Dim Valor As Integer = ObtenerMaxId("Id_HikCamara", "Terceros")
        Session("Id_HikCamara") = Valor + 1
    End Sub

    Private Function ObtenerMaxId(campo As String, tabla As String) As Integer
        Dim Valor As Integer = 0
        Dim query As String = $"SELECT MAX({campo}) FROM {tabla}"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                Dim resultado = cmd.ExecuteScalar()
                If resultado IsNot Nothing AndAlso Not IsDBNull(resultado) Then
                    Valor = Convert.ToInt32(resultado)
                End If
            End Using
        End Using

        Return Valor
    End Function

    Private Sub Guardar_Persona(ByVal Cedula As String, ByVal Nombres As String, ByVal Cargo As String, ByVal Correo As String, ByVal IdSede As String, ByVal IdEmpresa As String)
        Try
            Dim query As String = "
        INSERT INTO dbo.Terceros 
        (Id_Tercero, Id_HikCamara, Cedula, Nombres, Id_Rol, Correo, Usuario, Password, Cargo, Celular, Id_Empresa, Id_Sede, Estado) 
        VALUES 
        (@Id_Tercero, @Id_HikCamara, @Cedula, @Nombres, @Id_Rol, @Correo, @Usuario, @Password, @Cargo, @Celular, @Id_Empresa, @Id_Sede, @Estado)"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    ' Agregar parámetros con valores
                    cmd.Parameters.AddWithValue("@Id_Tercero", Session("Id_Tercero"))
                    cmd.Parameters.AddWithValue("@Id_HikCamara", Session("Id_HikCamara"))
                    cmd.Parameters.AddWithValue("@Cedula", If(Cedula Is Nothing, DBNull.Value, Cedula))
                    cmd.Parameters.AddWithValue("@Nombres", Nombres.ToUpper())
                    cmd.Parameters.AddWithValue("@Id_Rol", "8")
                    cmd.Parameters.AddWithValue("@Correo", If(String.IsNullOrWhiteSpace(Correo), DBNull.Value, Correo))
                    cmd.Parameters.AddWithValue("@Usuario", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Cargo", If(String.IsNullOrWhiteSpace(Cargo), DBNull.Value, Cargo.ToUpper()))
                    cmd.Parameters.AddWithValue("@Celular", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Id_Empresa", If(String.IsNullOrWhiteSpace(IdEmpresa), DBNull.Value, Convert.ToDecimal(IdEmpresa)))
                    cmd.Parameters.AddWithValue("@Id_Sede", If(String.IsNullOrWhiteSpace(IdSede), DBNull.Value, Convert.ToDecimal(IdSede)))
                    cmd.Parameters.AddWithValue("@Estado", 1)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al guardar en BD:" & ex.Message)
        End Try
    End Sub

    Private Sub Crear_Usuario(ByVal Nombres As String)

        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Usar la IP obtenida para construir la URL de la solicitud
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Record?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "POST"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que quieres enviar en el cuerpo de la solicitud
                Dim jsonBody As String = "
        {
           ""UserInfo"": {
              ""employeeNo"": """ & Session("Id_HikCamara").ToString & """,
              ""name"": """ & Nombres.ToUpper() & """,
              ""userType"": ""normal"",
               ""Valid"": {
                 ""enable"": true,
                 ""beginTime"": ""2024-12-04T00:00:00"",
                 ""endTime"": ""2030-12-31T23:59:59""
              },
              ""doorRight"": ""1"",
              ""RightPlan"": [
                 {
                    ""doorNo"": 1,
                    ""planTemplateNo"": ""1""
                 }
              ]
           }
        }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Obtener el flujo de escritura para escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                    Session("Status_Code_Usuario") = jsonResponse("statusCode").ToString()
                End Using

            Catch ex As Exception
                Session("Status_Code_Usuario") = 0
                errores.Add("Error al crear usuario en hikvision " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub Actualizar_Usuario(idUsuario As String, tipo_Usuario As String)

        ' Listas de cámaras y usuarios obtenidas desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each ipCamara As String In ipCamaras
            Try
                ' Construir la URL de la solicitud para la cámara
                Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                requestDocument.Method = "PUT"
                requestDocument.ContentType = "application/json"
                Dim credentials As New NetworkCredential("admin", "Miro-321")
                requestDocument.Credentials = credentials
                requestDocument.PreAuthenticate = True

                ' JSON que se enviará en el cuerpo de la solicitud
                Dim jsonBody As String = "
                {
                   ""UserInfo"": {
                      ""employeeNo"": """ & idUsuario & """,
                      ""userType"": """ & tipo_Usuario & """
                   }
                }"

                ' Convertir el JSON a un array de bytes
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)

                ' Establecer la longitud del cuerpo de la solicitud
                requestDocument.ContentLength = byteArray.Length

                ' Escribir en el cuerpo de la solicitud
                Using dataStream As Stream = requestDocument.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                End Using

                ' Enviar la solicitud y obtener la respuesta
                Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)

                ' Leer la respuesta
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseJson As String = streamReader.ReadToEnd()

                    ' Analizar la respuesta JSON si es necesario
                    Dim jsonResponse As JObject = JObject.Parse(responseJson)
                End Using

            Catch ex As Exception
                errores.Add("Error al dar acceso al usuario " & idUsuario & " en la cámara " & ipCamara & ": " & ex.Message)
            End Try
        Next
    End Sub

    Public Sub Actualizar_Estado(Cedula As Long, Estado As Integer)
        Dim sql As String = "UPDATE Terceros SET Estado = @Estado WHERE Cedula = @Cedula"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Cedula", Cedula)
                cmd.Parameters.AddWithValue("@Estado", Estado)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Class DatosEmpleado
        Public Property Nombre As String
        Public Property Cargo As String
        Public Property Correo As String
        Public Property IdSede As String
        Public Property IdEmpresa As String
    End Class

    Public Sub Ejecutar_Proceso_Empleados()

        Dim filePath As String = Server.MapPath("Adjunto/Excel_Usuarios/Empleados_Activos.xlsx")

        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial

        ' Paso 1: Leer datos del Excel
        Dim cedulasExcel As New HashSet(Of String)()
        Dim datosExcel As New Dictionary(Of String, DatosEmpleado)()

        Using package As New OfficeOpenXml.ExcelPackage(New FileInfo(filePath))
            Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets("Hoja1")
            Dim startRow As Integer = worksheet.Dimension.Start.Row
            Dim endRow As Integer = worksheet.Dimension.End.Row

            For row As Integer = startRow + 1 To endRow
                Dim cedula As String = worksheet.Cells(row, 2).Text.Trim()
                Dim nombre As String = worksheet.Cells(row, 3).Text.Trim()
                Dim cargo As String = worksheet.Cells(row, 8).Text.Trim()
                Dim correo As String = worksheet.Cells(row, 5).Text.Trim()
                Dim idSede As String = worksheet.Cells(row, 12).Text.Trim()
                Dim idEmpresa As String = worksheet.Cells(row, 14).Text.Trim()

                If Not String.IsNullOrEmpty(cedula) Then
                    cedulasExcel.Add(cedula)

                    If Not datosExcel.ContainsKey(cedula) Then
                        Dim empleado As New DatosEmpleado With {
                            .Nombre = nombre,
                            .Cargo = cargo,
                            .Correo = correo,
                            .IdSede = idSede,
                            .IdEmpresa = idEmpresa
                        }
                        datosExcel.Add(cedula, empleado)
                    End If
                End If
            Next
        End Using

        ' Paso 2: Leer usuarios desde la base de datos
        Dim cedulasBD As New Dictionary(Of String, String)() ' Cedula => Id_HikCamara
        Dim connStr As String = ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString()

        Using conn As New SqlConnection(connStr)
            Dim sql As String = "SELECT Cedula, Id_HikCamara FROM Terceros"
            Using cmd As New SqlCommand(sql, conn)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim cedulaBD As String = reader("Cedula").ToString().Trim()
                        Dim idHikvision As String = If(reader("Id_HikCamara") IsNot DBNull.Value, reader("Id_HikCamara").ToString().Trim(), "")
                        cedulasBD(cedulaBD) = idHikvision
                    End While
                End Using
            End Using
        End Using

        'Paso 2.1: Inactivar usuarios que están en la BD pero no en el Excel
        'For Each kvp As KeyValuePair(Of String, String) In cedulasBD
        '    Dim cedulaBD = kvp.Key
        '    Dim idHikvision = kvp.Value

        '    If Not cedulasExcel.Contains(cedulaBD) Then
        '        Try
        '            Actualizar_Estado(Convert.ToInt64(cedulaBD), 0)
        '            If Not String.IsNullOrWhiteSpace(idHikvision) Then
        '                Actualizar_Usuario(idHikvision, "blackList")
        '            Else
        '                errores.Add("No se encontró Id_HikCamara para cédula " & cedulaBD)
        '            End If
        '        Catch ex As Exception
        '            errores.Add("Error al inactivar cédula " & cedulaBD & ": " & ex.Message)
        '        End Try
        '    End If
        'Next

        'Paso 2.2: Activar usuarios que están tanto en la BD como en el Excel
        'For Each cedulaBD In cedulasBD.Keys
        '    If cedulasExcel.Contains(cedulaBD) Then
        '        Try
        '            Actualizar_Estado(Convert.ToInt64(cedulaBD), 1)
        '            Dim idHikvision As String = cedulasBD(cedulaBD)
        '            If Not String.IsNullOrWhiteSpace(idHikvision) Then
        '                Actualizar_Usuario(idHikvision, "normal")
        '            Else
        '                errores.Add("No se encontró Id_HikCamara para cédula " & cedulaBD)
        '            End If
        '        Catch ex As Exception
        '            errores.Add("Error al activar cédula " & cedulaBD & ": " & ex.Message)
        '        End Try
        '    End If
        'Next

        ' Paso 3: Crear usuarios que están en el Excel pero no en la BD
        For Each cedulaExcel In cedulasExcel
            If Not cedulasBD.ContainsKey(cedulaExcel) Then
                Try
                    Dim datos As DatosEmpleado = datosExcel(cedulaExcel)
                    Consultar_Id_Persona()
                    Consultar_Id_HikCamara()
                    Guardar_Persona(cedulaExcel, datos.Nombre, datos.Cargo, datos.Correo, datos.IdSede, datos.IdEmpresa)
                    Crear_Usuario(datos.Nombre)
                Catch ex As Exception
                    errores.Add("Error al crear usuario con cédula " & cedulaExcel & ": " & ex.Message)
                End Try
            End If
        Next
    End Sub

End Class
