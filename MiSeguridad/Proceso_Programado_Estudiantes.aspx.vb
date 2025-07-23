Imports System.IO
Imports System.Net
Imports System.Data.SqlClient

Public Class Proceso_Programado_Estudiantes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ProcesarEstudiantes()
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

    Shared Function Obtener_IdHikvision_Estudiantes_Desde_BaseDeDatos_Unico() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Id_HikCamara = 14790")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
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

    Shared Function Obtener_IdHikvision_Estudiantes_Sin_ExtraClases() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Permiso_Salida = 1 AND Extra_Class = 0 AND Id_HikCamara IS NOT NULL AND Id_Rol = 11")

            ' Agregar las IPs al List
            For Each row As DataRow In dt.Rows
                Id_HikVision.Add(row("Id_HikCamara").ToString())
            Next
        Catch ex As Exception
            errores.Add("Error al obtener las Id_HikVision de las cámaras:" & ex.Message)
        End Try
        Return Id_HikVision
    End Function

    Shared Function Obtener_IdHikvision_Estudiantes_ExtraClases() As List(Of String)
        Dim Id_HikVision As New List(Of String)()
        Try
            ' Ejecutamos la consulta SQL para obtener las IPs desde la base de datos
            Dim dt As DataTable = EjecutarConsultaSQL("SELECT Id_HikCamara FROM Terceros WHERE Estado = 1 AND Permiso_Salida = 1 AND Extra_Class = 1 AND Id_HikCamara IS NOT NULL AND Id_Rol = 11")

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

    Shared Sub ProcesarEstudiantes()
        Dim ahora As DateTime = DateTime.Now
        Dim horaActual As TimeSpan = ahora.TimeOfDay

        ' Si hoy es miércoles, se activa en el horario especial de 1:10 PM a 1:30 PM
        If ahora.DayOfWeek = DayOfWeek.Wednesday Then
            Dim inicioMiercoles As TimeSpan = New TimeSpan(13, 10, 0)
            Dim finMiercoles As TimeSpan = New TimeSpan(13, 30, 0)

            If horaActual >= inicioMiercoles AndAlso horaActual < finMiercoles Then
                Dim estudiantesMiercoles As List(Of String) = Obtener_IdHikvision_Estudiantes_Sin_ExtraClases()
                Activar_Accesos_PorLista(estudiantesMiercoles)
            End If
        End If

        ' Activación accesos normales: entre 2:40 PM y 3:00 PM
        Dim inicioActivacionNormal As TimeSpan = New TimeSpan(14, 40, 0)
        Dim finActivacionNormal As TimeSpan = New TimeSpan(15, 0, 0)

        If horaActual >= inicioActivacionNormal AndAlso horaActual < finActivacionNormal Then
            Dim todosLosEstudiantes As List(Of String) = Obtener_IdHikvision_Estudiantes_Sin_ExtraClases()
            Activar_Accesos_PorLista(todosLosEstudiantes)
        End If

        ' Activación accesos extra clases: entre 4:40 PM y 5:00 PM
        Dim inicioActivacionExtra As TimeSpan = New TimeSpan(16, 40, 0)
        Dim finActivacionExtra As TimeSpan = New TimeSpan(17, 0, 0)

        If horaActual >= inicioActivacionExtra AndAlso horaActual < finActivacionExtra Then
            Dim todosLosEstudiantes As List(Of String) = Obtener_IdHikvision_Estudiantes_ExtraClases()
            Activar_Accesos_PorLista(todosLosEstudiantes)
        End If

        ' Desactivación general: entre 9:00 PM y 9:15 PM
        Dim inicioDesactivacion As TimeSpan = New TimeSpan(21, 0, 0)
        Dim finDesactivacion As TimeSpan = New TimeSpan(21, 20, 0)

        If horaActual >= inicioDesactivacion AndAlso horaActual < finDesactivacion Then
            Dim todosLosEstudiantes As List(Of String) = Obtener_IdHikvision_Estudiantes_Desde_BaseDeDatos()
            'Desactivar_Accesos_PorLista(todosLosEstudiantes)
        End If

        ' ACtivacion general: entre 9:00 PM y 9:15 PM
        Dim inicioActivacion As TimeSpan = New TimeSpan(18, 27, 0)
        Dim finActivacion As TimeSpan = New TimeSpan(18, 40, 0)

        If horaActual >= inicioActivacion AndAlso horaActual < finActivacion Then
            Dim todosLosEstudiantes As List(Of String) = Obtener_IdHikvision_Estudiantes_Desde_BaseDeDatos()
            Activar_Accesos_PorLista(todosLosEstudiantes)
        End If

    End Sub

End Class
