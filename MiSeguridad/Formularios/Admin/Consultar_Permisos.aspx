<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Consultar_Permisos.aspx.vb" Inherits="MiSeguridad.Consultar_Permisos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="/../../Diseno/Numeros.js"></script>
    <link rel="stylesheet" type="text/css" href="/Diseno/material-icons.css" />

    
    <script src="/Diseno/Iconos.js"></script>



    <link href='/Diseno/open-sans.css' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" href="/Diseno/Form/css/reset.css">
    <!-- CSS reset -->
    <link rel="stylesheet" href="/Diseno/Form/css/style.css">
    <!-- Resource style -->
    <script src="/Diseno/Form/js/modernizr.js"></script>
    <!-- Modernizr -->
    <link rel="stylesheet" href="/Diseno/Formularios/Bootstrap/css/bootstrap.min.css" />
    
    <link href="/Diseno/Form/bootstrap-float-label/bootstrap-float-label.css" rel="stylesheet">
    <link href="/Diseno/Form/bootstrap-float-label/bootstrap-float-label.min.css" rel="stylesheet">
    <script type="text/javascript" src="/Diseno/2019/sweetalert/jquery.min.js"></script>
    <link rel="stylesheet" type="text/css" href="/Diseno/2019/sweetalert/sweetalert2.min.css">
    <script type="text/javascript" src="/Diseno/2019/sweetalert/sweetalert2.min.js"></script>

    <link rel="stylesheet" href="/Diseno/2019/SubMenu/css/reset.css">
    <!-- CSS reset -->
    <link rel="stylesheet" href="/Diseno/2019/SubMenu/css/style.css">
    <!-- Resource style -->
    <script type="text/javascript" src="/Diseno/bootstrap-filestyle.min.js"> </script>
    <link rel="stylesheet" href="/Diseno/2019/Menu/css/style.css" />
    <!-- Resource style -->

    <style>
        .cd-form {
            max-width: 100%;
            width: 100%;
            margin: 10px 7%;
        }

        @media only screen and (min-width: 600px) {
            .cd-form div {
                margin: 0;
            }
        }

        tr:nth-child(even) {
            background-color: #dddddd;
        }

        @media (min-width: 768px) {
            .p-md-5 {
                padding: 0rem !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <nav class="navbar navbar-expand-lg navbar-light bg-light" style="background-color: #0442A6 !important;">
        <div class="container-fluid">
            <button type="button" id="sidebarCollapse" class="btn btn-secondary">
                <i class="fa fa-bars"></i>
                <span class="sr-only">Toggle Menu</span>
            </button>

            <!-- Título centrado -->
            <div class="d-flex justify-content-center w-100 position-relative">
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CONSULTAR PERMISOS</span>
            </div>

            <!-- Nombres -->
            <asp:ListView ID="LvAccesos" runat="server" DataSourceID="SqlUsuario">
                <ItemTemplate>
                    <div style="white-space: nowrap; margin-right: 20px; text-align: right">
                        <span style="font-size: 10pt; color: white; font-weight: bold;"><%# Eval("Nombres") %></span>
                        <br />
                        <span style="font-size: 9pt; color: white;"><%# Eval("Nombre_Sede") %></span>
                        <br />
                        <span style="font-size: 9pt; color: white;"><%# Eval("Nombre_Acceso") %></span>
                    </div>
                </ItemTemplate>
            </asp:ListView>

            <asp:SqlDataSource ID="SqlUsuario" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT T.Nombres, T.Id_Sede, S.Nombre_Sede, T.Id_Acceso, A.Nombre_Acceso FROM Terceros T LEFT JOIN Adm_Accesos A ON A.Id_Acceso = T.Id_Acceso  LEFT JOIN Adm_Sedes S ON S.Id_Sede = T.Id_Sede WHERE T.Usuario = @Usuario">
                <SelectParameters>
                    <asp:SessionParameter Name="Usuario" SessionField="Usuario" />
                </SelectParameters>
            </asp:SqlDataSource>

            <!-- Logo a la derecha -->
            <div>
                <img src="/../../../Diseno/Imagenes/miro.png" class="ml-auto" alt="Logo" style="height: 50px; width: 40px;">
            </div>
        </div>
    </nav>

    <div class="row" style="padding-top: 0px; margin-top: 0px;">

        <div class="cd-form">

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                <ContentTemplate>

                    <div class="row" style="padding-top: 0px; margin-top: 0px;">

                        <div class="col-md-4 mb-3">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxBuscarPersona">Buscar Quien Solicita</label>
                                <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Quien Solicita" CssClass="Nombre_Usuario" AutoPostBack="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-4 mb-3">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxFecha">Fecha</label>
                                <asp:TextBox ID="TxFecha" runat="server" TextMode="Date" CssClass="Fecha" AutoPostBack="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="always">
                                <ContentTemplate>
                                    <asp:ListView ID="LvPermisos" runat="server" DataSourceID="SqlPermisos">
                                        <EmptyDataTemplate>
                                            <table runat="server" style="">
                                                <tr>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>

                                        <ItemTemplate>
                                            <tr style="">
                                                <asp:Label ID="EstadoLabel" runat="server" Text='<%# Eval("Estado") %>' Font-Size="0pt" />
                                                <asp:Label ID="AnuladaLabel" runat="server" Text='<%# Eval("Anulada") %>' Font-Size="0pt" />
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Id_PermisoLabel" runat="server" Text='<%# Eval("Id_Permiso") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Quien_SolicitaLabel" runat="server" Text='<%# Eval("Quien_Solicita") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_InicioLabel" runat="server" Text='<%# Eval("Fecha_Inicio", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_FinLabel" runat="server" Text='<%# Eval("Fecha_Fin", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="HoraLabel" runat="server" Text='<%# Eval("Hora") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="MotivoLabel" runat="server" Text='<%# Eval("Motivo") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:ImageButton ID="btnDetalle" runat="server" CommandName="Detalle" ImageUrl="~/Diseno/Imagenes/Estudiante.png" Width="20px" data-toggle="modal" data-target="#Detalle" CommandArgument='<%# Container.DataItemIndex %>' />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:ImageButton ID="BtReenviar" runat="server" CommandName="Reenviar" ImageUrl="~/Diseno/Imagenes/Reenviar.png" Width="20px" data-toggle="modal" data-target="#Notificar" CommandArgument='<%# Container.DataItemIndex %>' />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:ImageButton ID="BtConfirmar" runat="server" CommandName="Confirmar" ImageUrl="~/Diseno/Imagenes/Gris.png" Width="20px" data-toggle="modal" data-target="#Confirmar" CommandArgument='<%# Container.DataItemIndex %>' />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:ImageButton ID="BtAnulado" runat="server" CommandName="Anular" data-toggle="modal" data-target="#Anulado" ImageUrl="https://optimus.miroseguridad.com:8282/Diseno//Imagenes/CANCEL12.jpg" Width="20px" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>

                                        <LayoutTemplate>
                                            <table runat="server" style="width: 100%">
                                                <tr runat="server">
                                                    <td runat="server">
                                                        <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                            <tr runat="server">
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">ID PERMISO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">QUIEN SOLICITA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA INICIO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA FIN</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">HORA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">MOTIVO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">ESTUDIANTES</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">NOTIFICAR</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CONFIRMAR</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">ANULAR</th>
                                                            </tr>
                                                            <tr id="itemPlaceholder" runat="server">
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr runat="server">
                                                    <td runat="server" style=""></td>
                                                </tr>
                                            </table>
                                            <asp:DataPager ID="DataPager1" runat="server" PageSize="10">
                                                <Fields>
                                                    <asp:NumericPagerField />
                                                </Fields>
                                            </asp:DataPager>
                                        </LayoutTemplate>
                                    </asp:ListView>

                                    <asp:SqlDataSource ID="SqlPermisos" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT PE.Id_Permiso, PE.Id_Quien_Solicita, T.Nombres AS Quien_Solicita, PE.Fecha_Inicio, PE.Fecha_Fin, LEFT(CONVERT(VARCHAR(8), PE.Hora, 108), 5) AS Hora, PE.Motivo, PE.Estado, PE.Anulada, CAST(PE.Fecha_Inicio AS DATETIME) + CAST(PE.Hora AS DATETIME) AS FechaHoraPermiso FROM Adm_Permiso_Extemporaneo PE LEFT JOIN Terceros T ON T.Id_Tercero = PE.Id_Quien_Solicita WHERE PE.Anulada = 0 ORDER BY CASE WHEN PE.Estado IS NULL THEN 0 ELSE 1 END, FechaHoraPermiso ASC"></asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPermisoNombre" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT PE.Id_Permiso, PE.Id_Quien_Solicita, T.Nombres AS Quien_Solicita, PE.Fecha_Inicio, PE.Fecha_Fin, LEFT(CONVERT(VARCHAR(8), PE.Hora, 108), 5) AS Hora, PE.Motivo, PE.Estado, PE.Anulada, CAST(PE.Fecha_Inicio AS DATETIME) + CAST(PE.Hora AS DATETIME) AS FechaHoraPermiso FROM Adm_Permiso_Extemporaneo PE LEFT JOIN Terceros T ON T.Id_Tercero = PE.Id_Quien_Solicita WHERE PE.Anulada = 0 AND T.Nombres LIKE '%' + @Nombre + '%' ORDER BY CASE WHEN PE.Estado IS NULL THEN 0 ELSE 1 END, FechaHoraPermiso ASC">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="TxBuscarPersona" Name="Nombre" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPermisoFecha" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT PE.Id_Permiso, PE.Id_Quien_Solicita, T.Nombres AS Quien_Solicita, PE.Fecha_Inicio, PE.Fecha_Fin, LEFT(CONVERT(VARCHAR(8), PE.Hora, 108), 5) AS Hora, PE.Motivo, PE.Estado, PE.Anulada, CAST(PE.Fecha_Inicio AS DATETIME) + CAST(PE.Hora AS DATETIME) AS FechaHoraPermiso FROM Adm_Permiso_Extemporaneo PE LEFT JOIN Terceros T ON T.Id_Tercero = PE.Id_Quien_Solicita WHERE PE.Anulada = 0 AND PE.Fecha_Inicio = @Fecha ORDER BY CASE WHEN PE.Estado IS NULL THEN 0 ELSE 1 END, FechaHoraPermiso ASC">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="TxFecha" Name="Fecha" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

    </div>

    <%--SALIR--%>
    <div class="modal fade" id="Confirmar" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
        <div class="modal-dialog" role="document" style="text-align: center">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">CONFIRMAR PERMISO</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center">
                    <asp:Label ID="Label3" runat="server" Text="¿Seguro que desea confirmar el permiso?"></asp:Label>
                </div>
                <div class="modal-footer" style="text-align: center; align-self: center;">
                    <asp:Button ID="BtAceptar" runat="server" Text="ACEPTAR" class="btn btn-success" ForeColor="White" Width="160px" Font-Size="12pt" />
                    <asp:Button ID="BtRechazar" runat="server" Text="RECHAZAR" class="btn btn-danger" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div> 

    <%--EDITAR RESPONSABLE--%>
    <div class="modal" id="Notificar" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center;">
        <div class="modal-dialog" role="document" style="text-align: center; font-family: sans-serif;">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">NOTIFICAR PROFESOR</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center; padding: 1px;">
                    <asp:UpdatePanel ID="UpdatePanel9" runat="server" class="cd-form" style="width: 100%; margin: 0;" ChildrenAsTriggers="true">
                        <ContentTemplate>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxBuscarProfesor">Buscar Profesor A Notificar</label>
                                    <asp:TextBox ID="TxBuscarProfesor" runat="server" AutoPostBack="true" Visible="true" Placeholder="Buscar Profesor a Asignar" ToolTip="Buscar Profesor a Notificar" CssClass="Buscar"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxProfesor">Profesor A Notificar</label>
                                    <asp:DropDownList ID="TxProfesor" runat="server" DataSourceID="SqlProfesor" DataTextField="Nombres" DataValueField="Id_Tercero" CssClass="user">
                                        <asp:ListItem Value="">Profesor A Notificar</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlProfesor" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Tercero, Nombres FROM Terceros WHERE Estado = 1 AND Id_Rol = 8 AND Nombres LIKE '%' + @Persona + '%' ORDER BY Nombres ASC">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="TxBuscarProfesor" Name="Persona" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                </div>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtNotificar" runat="server" Text="NOTIFICAR" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal" id="Detalle" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center;">
        <div class="modal-dialog" role="document" style="text-align: center; font-family: sans-serif;">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">ESTUDIANTE A SALIR</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:ListView ID="LvDetalleEstudiante" runat="server" DataSourceID="SqlDetalleSalida">
                                        <EmptyDataTemplate>
                                            <table runat="server" style="">
                                                <tr>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>

                                        <ItemTemplate>
                                            <tr style="">
                                                <td style="width: 100%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Nombre_HijoLabel" runat="server" Text='<%# Eval("Nombre_Hijo") %>' Font-Size="8pt" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>

                                        <LayoutTemplate>
                                            <table runat="server" style="width: 100%">
                                                <tr runat="server">
                                                    <td runat="server">
                                                        <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                            <tr runat="server">
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 100%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">ESTUDIANTES</th>
                                                            </tr>
                                                            <tr id="itemPlaceholder" runat="server">
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr runat="server">
                                                    <td runat="server" style=""></td>
                                                </tr>
                                            </table>
                                        </LayoutTemplate>
                                    </asp:ListView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:SqlDataSource ID="SqlDetalleSalida" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                    </div>
                <div class="modal-footer" style="text-align: center">
                </div>
            </div>
        </div>
    </div>

    <%--ANULAR PERMISO--%>
    <div class="modal" id="Anulado" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
        <div class="modal-dialog" role="document" style="text-align: center">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center; padding-top: 15px">
                    <h5 class="modal-title" style="text-align: center">ANULAR PERMISO</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center">
                    <asp:Label ID="Label1" runat="server" Text="¿Seguro que desea anular el permiso?"></asp:Label>
                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtAnular" runat="server" Text="Anular" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>
</asp:Content>




