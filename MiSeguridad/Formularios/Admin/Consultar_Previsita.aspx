<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Consultar_Previsita.aspx.vb" Inherits="MiSeguridad.Consultar_Previsita" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CONSULTAR PREVISITA</span>
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

        <div class="col-md-12 mb-3" style="text-align: center">
            <button id="BtVisitantes" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                VISITANTES
            </button>

            <button id="BtContratistas" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                CONTRATISTAS
            </button>

        </div>

        <div class="cd-form">

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                <ContentTemplate>

                    <div class="row" style="padding-top: 0px; margin-top: 0px;">

                        <div class="col-md-4 mb-3">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxBuscarCedula">Buscar Cedula</label>
                                <asp:TextBox ID="TxBuscarCedula" runat="server" placeholder="Buscar Cedula" CssClass="Cedula" TextMode="Number" AutoPostBack="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-4 mb-3">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxBuscarPersona">Buscar Nombre Persona</label>
                                <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Nombre Persona" CssClass="Nombre_Usuario" AutoPostBack="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-4 mb-3">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxFechaIngreso">Fecha Ingreso</label>
                                <asp:TextBox ID="TxFechaIngreso" runat="server" TextMode="Date" CssClass="Fecha" AutoPostBack="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="always">
                                <ContentTemplate>
                                    <asp:ListView ID="LvPrevisitas" runat="server" DataSourceID="SqlPrevisitas">
                                        <EmptyDataTemplate>
                                            <table runat="server" style="">
                                                <tr>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>

                                        <ItemTemplate>
                                            <tr style="">
                                                <asp:Label ID="Id_Preregistro_VisitaLabel" runat="server" Text='<%# Eval("Id_Preregistro_Visita") %>' Font-Size="0pt" />
                                                <asp:Label ID="EstadoLabel" runat="server" Text='<%# Eval("Estado") %>' Font-Size="0pt" />
                                                <asp:Label ID="AnuladaLabel" runat="server" Text='<%# Eval("Anulada") %>' Font-Size="0pt" />
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="CedulaLabel" runat="server" Text='<%# Eval("Cedula") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_IngresoLabel" runat="server" Text='<%# Eval("Fecha_Ingreso", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                                                        <ContentTemplate>
                                                            <asp:ImageButton ID="LbFecha" runat="server" CommandName="Fecha" data-toggle="modal" data-target="#EditarFecha" ImageUrl="../../../Diseno/Imagenes/Fecha.png" Width="20px" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="EmpresaLabel" runat="server" Text='<%# Eval("Empresa") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Nombre_SedeLabel" runat="server" Text='<%# Eval("Nombre_Sede") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                                        <ContentTemplate>
                                                            <asp:ImageButton ID="LbResponsable2" runat="server" CommandName="Responsable" data-toggle="modal" data-target="#EditarResponsable" ImageUrl="https://optimus.miroseguridad.com:8282/Diseno/Imagenes/Persona_Lupa.png" Width="20px" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
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
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CEDULA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">NOMBRES</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA INGRESO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CAMBIAR FECHA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">EMPRESA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">SEDE</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">QUIEN AUTORIZA</th>
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

                                    <asp:SqlDataSource ID="SqlPrevisitas" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT P.Id_Preregistro_Visita, P.Cedula, T2.Nombres AS Nombres, P.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza, P.Empresa, P.Id_Sede, S.Nombre_Sede, P.Fecha_Ingreso, CASE WHEN P.Equipo_Computo = 1 THEN 'SI' ELSE 'NO' END Equipo_Computo, P.Serial_Computo, P.Id_Tipo_Vehiculo, V.Tipo_Vehiculo, P.Placa, P.Id_ARL, P.Id_EPS, P.Privacidad, P.Estado, P.Anulada FROM Adm_Preregistro_Visita P LEFT JOIN Terceros T ON T.Id_Tercero = P.Id_Quien_Autoriza LEFT JOIN Terceros T2 ON T2.Cedula = P.Cedula LEFT JOIN Adm_Sedes S ON S.Id_Sede = P.Id_Sede LEFT JOIN Adm_Tipo_Vehiculo V ON V.Id_Tipo_Vehiculo = P.Id_Tipo_Vehiculo WHERE P.Id_Quien_Autoriza = @Id_Tercero_Usuario AND P.Fecha_Ingreso >= CAST(GETDATE() AS DATE) ORDER BY CASE WHEN P.Estado IS NULL THEN 0 ELSE 1 END, CASE WHEN P.Fecha_Ingreso = CAST(GETDATE() AS DATE) THEN 0 ELSE DATEDIFF(DAY, GETDATE(), P.Fecha_Ingreso) END, P.Id_Preregistro_Visita ASC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPrevisitasCedula" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT P.Id_Preregistro_Visita, P.Cedula, T2.Nombres AS Nombres, P.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza, P.Empresa, P.Id_Sede, S.Nombre_Sede, P.Fecha_Ingreso, CASE WHEN P.Equipo_Computo = 1 THEN 'SI' ELSE 'NO' END Equipo_Computo, P.Serial_Computo, P.Id_Tipo_Vehiculo, V.Tipo_Vehiculo, P.Placa, P.Id_ARL, P.Id_EPS, P.Privacidad, P.Estado, P.Anulada FROM Adm_Preregistro_Visita P LEFT JOIN Terceros T ON T.Id_Tercero = P.Id_Quien_Autoriza LEFT JOIN Terceros T2 ON T2.Cedula = P.Cedula LEFT JOIN Adm_Sedes S ON S.Id_Sede = P.Id_Sede LEFT JOIN Adm_Tipo_Vehiculo V ON V.Id_Tipo_Vehiculo = P.Id_Tipo_Vehiculo WHERE P.Id_Quien_Autoriza = @Id_Tercero_Usuario AND P.Cedula = @Cedula ORDER BY CASE WHEN P.Estado IS NULL THEN 0 ELSE 1 END, CASE WHEN P.Fecha_Ingreso = CAST(GETDATE() AS DATE) THEN 0 ELSE DATEDIFF(DAY, GETDATE(), P.Fecha_Ingreso) END, P.Id_Preregistro_Visita ASC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                            <asp:ControlParameter ControlID="TxBuscarCedula" Name="Cedula" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPrevisitasNombre" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT P.Id_Preregistro_Visita, P.Cedula, T2.Nombres AS Nombres, P.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza, P.Empresa, P.Id_Sede, S.Nombre_Sede, P.Fecha_Ingreso, CASE WHEN P.Equipo_Computo = 1 THEN 'SI' ELSE 'NO' END Equipo_Computo, P.Serial_Computo, P.Id_Tipo_Vehiculo, V.Tipo_Vehiculo, P.Placa, P.Id_ARL, P.Id_EPS, P.Privacidad, P.Estado, P.Anulada FROM Adm_Preregistro_Visita P LEFT JOIN Terceros T ON T.Id_Tercero = P.Id_Quien_Autoriza LEFT JOIN Terceros T2 ON T2.Cedula = P.Cedula LEFT JOIN Adm_Sedes S ON S.Id_Sede = P.Id_Sede LEFT JOIN Adm_Tipo_Vehiculo V ON V.Id_Tipo_Vehiculo = P.Id_Tipo_Vehiculo WHERE P.Id_Quien_Autoriza = @Id_Tercero_Usuario AND T2.Nombres LIKE '%' + @Nombre + '%' ORDER BY CASE WHEN P.Estado IS NULL THEN 0 ELSE 1 END, CASE WHEN P.Fecha_Ingreso = CAST(GETDATE() AS DATE) THEN 0 ELSE DATEDIFF(DAY, GETDATE(), P.Fecha_Ingreso) END, P.Id_Preregistro_Visita ASC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                            <asp:ControlParameter ControlID="TxBuscarPersona" Name="Nombre" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPrevisitasFecha" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT P.Id_Preregistro_Visita, P.Cedula, T2.Nombres AS Nombres, P.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza, P.Empresa, P.Id_Sede, S.Nombre_Sede, P.Fecha_Ingreso, CASE WHEN P.Equipo_Computo = 1 THEN 'SI' ELSE 'NO' END Equipo_Computo, P.Serial_Computo, P.Id_Tipo_Vehiculo, V.Tipo_Vehiculo, P.Placa, P.Id_ARL, P.Id_EPS, P.Privacidad, P.Estado, P.Anulada FROM Adm_Preregistro_Visita P LEFT JOIN Terceros T ON T.Id_Tercero = P.Id_Quien_Autoriza LEFT JOIN Terceros T2 ON T2.Cedula = P.Cedula LEFT JOIN Adm_Sedes S ON S.Id_Sede = P.Id_Sede LEFT JOIN Adm_Tipo_Vehiculo V ON V.Id_Tipo_Vehiculo = P.Id_Tipo_Vehiculo WHERE P.Id_Quien_Autoriza = @Id_Tercero_Usuario AND P.Fecha_Ingreso = @Fecha ORDER BY CASE WHEN P.Estado IS NULL THEN 0 ELSE 1 END, CASE WHEN P.Fecha_Ingreso = CAST(GETDATE() AS DATE) THEN 0 ELSE DATEDIFF(DAY, GETDATE(), P.Fecha_Ingreso) END, P.Id_Preregistro_Visita ASC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                            <asp:ControlParameter ControlID="TxFechaIngreso" Name="Fecha" PropertyName="Text" />
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
                    <h5 class="modal-title" style="text-align: center">CONFIRMAR PREVISITA</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center">
                    <asp:Label ID="Label3" runat="server" Text="¿Seguro que desea confirmar la previsita?"></asp:Label>
                </div>
                <div class="modal-footer" style="text-align: center; align-self: center;">
                    <asp:Button ID="BtAceptar" runat="server" Text="ACEPTAR" class="btn btn-success" ForeColor="White" Width="160px" Font-Size="12pt" />
                    <asp:Button ID="BtRechazar" runat="server" Text="RECHAZAR" class="btn btn-danger" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <%--EDITAR RESPONSABLE--%>
    <div class="modal" id="EditarResponsable" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center;">
        <div class="modal-dialog" role="document" style="text-align: center; font-family: sans-serif;">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">CAMBIAR RESPONSABLE</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center; padding: 1px;">
                    <asp:UpdatePanel ID="UpdatePanel9" runat="server" class="cd-form" style="width: 100%; margin: 0;" ChildrenAsTriggers="true">
                        <ContentTemplate>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxBuscarPersonaResponsable">Buscar Persona</label>
                                    <asp:TextBox ID="TxBuscarPersonaResponsable" runat="server" AutoPostBack="true" Visible="true" Placeholder="Buscar Persona a Asignar" ToolTip="Buscar Ejecutivo a Asignar" CssClass="Buscar"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxReponsable">Persona A Asignar</label>
                                    <asp:DropDownList ID="TxReponsable" runat="server" DataSourceID="SqlResponsable" DataTextField="Nombres" DataValueField="Id_Tercero" CssClass="user">
                                        <asp:ListItem Value="">Persona A Asignar</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlResponsable" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Tercero, Nombres FROM Terceros WHERE Estado = 1 AND Id_Rol = 8 AND Nombres LIKE '%' + @Persona + '%' ORDER BY Nombres ASC">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="TxBuscarPersonaResponsable" Name="Persona" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                </div>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtEditarResponsable" runat="server" Text="REASIGNAR" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <%--EDITAR FECHA--%>
    <div class="modal" id="EditarFecha" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center;">
        <div class="modal-dialog" role="document" style="text-align: center; font-family: sans-serif;">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">CAMBIAR FECHA</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center; padding: 1px;">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" class="cd-form" style="width: 100%; margin: 0;" ChildrenAsTriggers="true">
                        <ContentTemplate>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxFechaIngresoEditar">Fecha Ingreso</label>
                                    <asp:TextBox ID="TxFechaIngresoEditar" runat="server" TextMode="Date" placeholder="Fecha Ingreso" CssClass="Fecha"></asp:TextBox>
                                </div>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtEditarFecha" runat="server" Text="REAGENDAR" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <%--ANULAR COTIZACION--%>
    <div class="modal" id="Anulado" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
        <div class="modal-dialog" role="document" style="text-align: center">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center; padding-top: 15px">
                    <h5 class="modal-title" style="text-align: center">ANULAR PREVISITA</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center">
                    <asp:Label ID="Label1" runat="server" Text="¿Seguro que desea anular la previsita?"></asp:Label>
                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtAnular" runat="server" Text="Anular" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>
</asp:Content>




