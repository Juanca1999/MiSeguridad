Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Data.SqlClient

Public Class Proceso_Programado_Salidas_Ext
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ProcesarPermisosExtemporaneos()
    End Sub

    Shared errores As New List(Of String)

    ' Método para obtener las IPs de las cámaras desde la base de datos
    Shared Function ObtenerIPCamaraDesdeBaseDeDatos() As List(Of String)
        Dim ipCamaras As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT IP_Camara FROM Adm_Accesos WHERE IP_Camara IS NOT NULL AND Id_Tipo_Acceso = 1 AND Tipo_Evento = 0")

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

    Shared Function Obtener_IdHikvision_Estudiantes_Desde_BaseDeDatos() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Id_HikCamara IS NOT NULL AND Id_Rol = 11")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
    End Function

    Shared Sub Activar_Accesos_PorLista(idUsuarios As List(Of String))
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each idUsuario As String In idUsuarios
            For Each ipCamara As String In ipCamaras
                Try
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    Dim credentials As New NetworkCredential("admin", "Miro-321")
                    requestDocument.Credentials = credentials
                    requestDocument.PreAuthenticate = True

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

                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                    requestDocument.ContentLength = byteArray.Length

                    Using dataStream As Stream = requestDocument.GetRequestStream()
                        dataStream.Write(byteArray, 0, byteArray.Length)
                    End Using

                    Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseJson As String = streamReader.ReadToEnd()
                    End Using

                Catch ex As Exception
                    errores.Add("Error al ACTIVAR acceso del usuario " & idUsuario & " en cámara " & ipCamara & ": " & ex.Message)
                End Try
            Next
        Next
    End Sub

    Shared Sub Desactivar_Accesos_PorLista(idUsuarios As List(Of String))
        Dim ipCamaras As List(Of String) = ObtenerIPCamaraDesdeBaseDeDatos()

        For Each idUsuario As String In idUsuarios
            For Each ipCamara As String In ipCamaras
                Try
                    Dim HikvisionURL As String = "http://" & ipCamara & "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                    Dim requestDocument As HttpWebRequest = DirectCast(WebRequest.Create(HikvisionURL), HttpWebRequest)
                    requestDocument.Method = "PUT"
                    requestDocument.ContentType = "application/json"
                    Dim credentials As New NetworkCredential("admin", "Miro-321")
                    requestDocument.Credentials = credentials
                    requestDocument.PreAuthenticate = True

                    Dim jsonBody As String = "
                {
                   ""UserInfo"": {
                      ""employeeNo"": """ & idUsuario & """,
                      ""doorRight"": """",
                      ""RightPlan"": [
                         {
                            ""doorNo"": 1,
                            ""planTemplateNo"": ""1""
                         }
                      ]
                   }
                }"

                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonBody)
                    requestDocument.ContentLength = byteArray.Length

                    Using dataStream As Stream = requestDocument.GetRequestStream()
                        dataStream.Write(byteArray, 0, byteArray.Length)
                    End Using

                    Dim response As HttpWebResponse = DirectCast(requestDocument.GetResponse(), HttpWebResponse)
                    Using streamReader As New StreamReader(response.GetResponseStream())
                        Dim responseJson As String = streamReader.ReadToEnd()
                    End Using

                Catch ex As Exception
                    errores.Add("Error al DESACTIVAR acceso del usuario " & idUsuario & " en cámara " & ipCamara & ": " & ex.Message)
                End Try
            Next
        Next
    End Sub

    Shared Sub ProcesarPermisosExtemporaneos()
        Dim ahora As DateTime = DateTime.Now

        ' 🔸 Activar si la salida es entre ahora -15 y ahora +15
        Dim activarDesde As DateTime = ahora.AddMinutes(-15)
        Dim activarHasta As DateTime = ahora.AddMinutes(15)
        Dim idsActivar As List(Of String) = ObtenerIdsHikvisionPorRangoHora(activarDesde, activarHasta)

        If idsActivar.Count > 0 Then
            Activar_Accesos_PorLista(idsActivar)
        End If

        ' 🔸 Desactivar si ya pasaron entre 13 y 17 minutos de la hora del permiso
        Dim desactivarDesde As DateTime = ahora.AddMinutes(-17)
        Dim desactivarHasta As DateTime = ahora.AddMinutes(-13)
        Dim idsDesactivar As List(Of String) = ObtenerIdsHikvisionPorRangoDesactivacion(desactivarDesde, desactivarHasta)

        If idsDesactivar.Count > 0 Then
            Desactivar_Accesos_PorLista(idsDesactivar)
        End If
    End Sub

    ' 🔹 Activación por rango
    Shared Function ObtenerIdsHikvisionPorRangoHora(desde As DateTime, hasta As DateTime) As List(Of String)
        Dim lista As New List(Of String)
        Try
            Dim query As String = "
        SELECT DISTINCT T.Id_HikCamara 
        FROM Adm_Permiso_Extemporaneo_Detalle PD
        LEFT JOIN Adm_Permiso_Extemporaneo P ON P.Id_Permiso = PD.Id_Permiso
        LEFT JOIN Terceros T ON T.Id_Tercero = PD.Id_Tercero_Hijo
        WHERE P.Estado = 1 AND CONVERT(DATE, GETDATE()) BETWEEN CONVERT(DATE, P.Fecha_Inicio) AND CONVERT(DATE, P.Fecha_Fin)
        AND CAST(P.Hora AS TIME) BETWEEN @HoraDesde AND @HoraHasta
        AND T.Id_HikCamara IS NOT NULL"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@HoraDesde", desde.ToString("HH:mm"))
                    cmd.Parameters.AddWithValue("@HoraHasta", hasta.ToString("HH:mm"))

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(reader("Id_HikCamara").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al consultar Id_HikCamara (rango activación): " & ex.Message)
        End Try
        Return lista
    End Function

    ' 🔹 Desactivación por rango
    Shared Function ObtenerIdsHikvisionPorRangoDesactivacion(desde As DateTime, hasta As DateTime) As List(Of String)
        Dim lista As New List(Of String)
        Try
            Dim query As String = "
        SELECT DISTINCT T.Id_HikCamara 
        FROM Adm_Permiso_Extemporaneo_Detalle PD
        LEFT JOIN Adm_Permiso_Extemporaneo P ON P.Id_Permiso = PD.Id_Permiso
        LEFT JOIN Terceros T ON T.Id_Tercero = PD.Id_Tercero_Hijo
        WHERE P.Estado = 1 AND CONVERT(DATE, GETDATE()) BETWEEN CONVERT(DATE, P.Fecha_Inicio) AND CONVERT(DATE, P.Fecha_Fin)
        AND CAST(P.Hora AS TIME) BETWEEN @HoraDesde AND @HoraHasta
        AND T.Id_HikCamara IS NOT NULL"

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MiSeguridadConnectionString").ToString())
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@HoraDesde", desde.ToString("HH:mm"))
                    cmd.Parameters.AddWithValue("@HoraHasta", hasta.ToString("HH:mm"))

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(reader("Id_HikCamara").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            errores.Add("Error al consultar Id_HikCamara (rango desactivación): " & ex.Message)
        End Try
        Return lista
    End Function

End Class
