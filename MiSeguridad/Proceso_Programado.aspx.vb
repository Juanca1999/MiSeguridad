Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class Proceso_Programado
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Activar_Accesos_Empleados()
        Borrar_Visitantes_Facial()
    End Sub

    Shared errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Shared Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Inmueble = 'COMEDOR'")

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
    Shared Function EjecutarConsultaSQL(query As String) As DataTable
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

    ' Método para obtener las Id_HikVision de empleados de las cámaras desde la base de datos
    Shared Function ObtenerIdHikvisionEmpleadosDesdeBaseDeDatos() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Id_HikCamara IS NOT NULL")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
    End Function

    ' Método para obtener las Id_HikVision de visitantes de las cámaras desde la base de datos
    Shared Function ObtenerIdHikvisionVisitantesDesdeBaseDeDatos() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Adm_Control_Restaurante WHERE Id_HikCamara IS NOT NULL AND Fecha_Activacion = CAST(GETDATE() AS DATE)")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
    End Function

    Shared Sub Activar_Accesos_Empleados()

        ' Listas de cámaras y usuarios obtenidas desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()
        Dim Id_HikVision As List(Of String) = ObtenerIdHikvisionEmpleadosDesdeBaseDeDatos()

        ' Iterar sobre cada usuario
        For Each idUsuario As String In Id_HikVision
            ' Iterar sobre cada cámara
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
        Next
    End Sub

    Shared Sub Borrar_Visitantes_Facial()

        ' Listas de cámaras y usuarios obtenidas desde la base de datos
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()
        Dim Id_HikVision As List(Of String) = ObtenerIdHikvisionVisitantesDesdeBaseDeDatos()

        ' Iterar sobre cada usuario
        For Each idUsuario As String In Id_HikVision
            ' Iterar sobre cada cámara
            For Each ipCamara As String In ipCamaras
                Try
                    ' Construir la URL de la solicitud para la cámara
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Delete?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    Dim credentials As New NetworkCredential("admin", "Miro-321")
                    requestDocument.Credentials = credentials
                    requestDocument.PreAuthenticate = True

                    ' JSON que se enviará en el cuerpo de la solicitud
                    Dim jsonBody As String = "
                {
                   ""UserInfoDelCond"": {
                      ""EmployeeNoList"": [
                         {
                            ""employeeNo"": """ & idUsuario & """
                         }
                      ]
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

                        Actualizar_Hikvision(idUsuario)
                    End Using

                Catch ex As Exception
                    errores.Add("Error al eliminar al usuario " & idUsuario & " en la cámara " & ipCamara & ": " & ex.Message)
                End Try
            Next
        Next
    End Sub

    Shared Sub Actualizar_Hikvision(Id_HikVision As Long)
        Dim sql As String = "UPDATE Adm_Control_Restaurante SET Id_HikCamara = NULL WHERE Id_HikCamara = @Id_HikCamara"

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Id_HikCamara", Id_HikVision)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Class
