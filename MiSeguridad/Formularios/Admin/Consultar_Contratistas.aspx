<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Consultar_Contratistas.aspx.vb" Inherits="MiSeguridad.Consultar_Contratistas" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CONSULTAR CONTRATISTAS</span>
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
            <button id="BtVisitantes" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                VISITANTES
            </button>

            <button id="BtContratistas" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
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
                                                <asp:Label ID="Id_Control_ContratistasLabel" runat="server" Text='<%# Eval("Id_Control_Contratistas") %>' Font-Size="0pt" />
                                                <asp:Label ID="Id_HikCamaraLabel" runat="server" Text='<%# Eval("Id_HikCamara") %>' Font-Size="0pt" />
                                                <asp:Label ID="ARLLabel" runat="server" Text='<%# Eval("ARL") %>' Font-Size="0pt" />
                                                <asp:Label ID="Seguridad_SocialLabel" runat="server" Text='<%# Eval("Seguridad_Social") %>' Font-Size="0pt" />
                                                <asp:Label ID="Trabajo_AlturasLabel" runat="server" Text='<%# Eval("Trabajo_Alturas") %>' Font-Size="0pt" />
                                                <asp:Label ID="EstadoLabel" runat="server" Text='<%# Eval("Estado") %>' Font-Size="0pt" />
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Cedula_ContratistaLabel" runat="server" Text='<%# Eval("Cedula_Contratista") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="ContratistaLabel" runat="server" Text='<%# Eval("Contratista") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_InicioLabel" runat="server" Text='<%# Eval("Fecha_Inicio", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_FinLabel" runat="server" Text='<%# Eval("Fecha_Fin", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                                                        <ContentTemplate>
                                                            <asp:ImageButton ID="LbFecha" runat="server" CommandName="Fecha" data-toggle="modal" data-target="#EditarFecha" ImageUrl="../../../Diseno/Imagenes/Fecha.png" Width="20px" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Always">
                                                        <ContentTemplate>
                                                            <asp:ImageButton ID="LbDias" runat="server" CommandName="Dias" data-toggle="modal" data-target="#Dias" ImageUrl="../../../Diseno/Imagenes/Detalle.png" Width="20px" />
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
                                                <td style="width: 5%; text-align: center; vertical-align: middle; border: thin solid darkgray; padding: 5px;">
                                                    <a href="#" id="ARL" runat="server" title='<%# Eval("Fecha_ARL") %>' onclick='<%# "Mostrar_Documento_Modal(""" & Eval("ARL") & """); return false;" %>'>
                                                        <i class="material-icons" style="color: blue">description</i>
                                                    </a>
                                                </td>
                                                <td style="width: 5%; text-align: center; vertical-align: middle; border: thin solid darkgray; padding: 5px;">
                                                    <a href="#" id="Seguridad_Social" runat="server" title='<%# Eval("Fecha_Seguridad_Social") %>' onclick='<%# "Mostrar_Documento_Modal(""" & Eval("Seguridad_Social") & """); return false;" %>'>
                                                        <i class="material-icons" style="color: blue">description</i>
                                                    </a>
                                                </td>
                                                <td style="width: 5%; text-align: center; vertical-align: middle; border: thin solid darkgray; padding: 5px;">
                                                    <a href="#" id="Trabajo_Alturas" runat="server" title='<%# Eval("Fecha_Trabajo_Alturas") %>' onclick='<%# "Mostrar_Documento_Modal(""" & Eval("Trabajo_Alturas") & """); return false;" %>'>
                                                        <i class="material-icons" style="color: blue">description</i>
                                                    </a>
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:ImageButton ID="BtConfirmar" runat="server" CommandName="Confirmar" ImageUrl="~/Diseno/Imagenes/Gris.png" Width="20px" data-toggle="modal" data-target="#Confirmar" CommandArgument='<%# Container.DataItemIndex %>' />
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
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA INICIO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA FIN</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CAMBIAR FECHA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">DIAS</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">EMPRESA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">SEDE</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">QUIEN AUTORIZA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CURSO SST SURA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">SEGURIDAD SOCIAL</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">TRABAJO ALTURAS</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CONFIRMAR</th>
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

                                    <asp:SqlDataSource ID="SqlPrevisitas" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Id_Empresa, E.Empresa, S.Nombre_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, T.Id_HikCamara, CC.Fecha_Inicio, CC.Fecha_Fin, CC.ARL, CC.Fecha_ARL, CC.Seguridad_Social, CC.Fecha_Seguridad_Social, CC.Trabajo_Alturas, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo, CC.Estado FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T on T.Cedula = CC.Cedula_Contratista LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza LEFT JOIN Empresas E on E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Sedes S on S.Id_Sede = T.Id_Sede WHERE CC.Id_Quien_Autoriza = @Id_Tercero_Usuario ORDER BY CC.Id_Control_Contratistas DESC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPrevisitasCedula" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Id_Empresa, E.Empresa, S.Nombre_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, T.Id_HikCamara, CC.Fecha_Inicio, CC.Fecha_Fin, CC.ARL, CC.Fecha_ARL, CC.Seguridad_Social, CC.Fecha_Seguridad_Social, CC.Trabajo_Alturas, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo, CC.Estado FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T on T.Cedula = CC.Cedula_Contratista LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza LEFT JOIN Empresas E on E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Sedes S on S.Id_Sede = T.Id_Sede WHERE CC.Id_Quien_Autoriza = @Id_Tercero_Usuario AND CC.Cedula_Contratista = @Cedula ORDER BY CC.Id_Control_Contratistas DESC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                            <asp:ControlParameter ControlID="TxBuscarCedula" Name="Cedula" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPrevisitasNombre" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Id_Empresa, E.Empresa, S.Nombre_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, T.Id_HikCamara, CC.Fecha_Inicio, CC.Fecha_Fin, CC.ARL, CC.Fecha_ARL, CC.Seguridad_Social, CC.Fecha_Seguridad_Social, CC.Trabajo_Alturas, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo, CC.Estado FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T on T.Cedula = CC.Cedula_Contratista LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza LEFT JOIN Empresas E on E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Sedes S on S.Id_Sede = T.Id_Sede WHERE CC.Id_Quien_Autoriza = @Id_Tercero_Usuario AND T.Nombres LIKE '%' + @Nombre + '%' ORDER BY CC.Id_Control_Contratistas DESC;">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Id_Tercero_Usuario" SessionField="Id_Tercero_Usuario" />
                                            <asp:ControlParameter ControlID="TxBuscarPersona" Name="Nombre" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>

                                    <asp:SqlDataSource ID="SqlPrevisitasFecha" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT CC.Id_Control_Contratistas, CC.Id_Quien_Autoriza, T2.Nombres AS Quien_Autoriza, CC.Id_Inmueble, T.Id_Empresa, E.Empresa, S.Nombre_Sede, CC.Cedula_Contratista, T.Nombres AS Contratista, T.Id_HikCamara, CC.Fecha_Inicio, CC.Fecha_Fin, CC.ARL, CC.Fecha_ARL, CC.Seguridad_Social, CC.Fecha_Seguridad_Social, CC.Trabajo_Alturas, CC.Fecha_Trabajo_Alturas, CC.Lunes, CC.Martes, CC.Miercoles, CC.Jueves, CC.Viernes, CC.Sabado, CC.Domingo, CC.Festivo, CC.Estado FROM Adm_Control_Contratistas CC LEFT JOIN Terceros T on T.Cedula = CC.Cedula_Contratista LEFT JOIN Terceros T2 on T2.Id_Tercero = CC.Id_Quien_Autoriza LEFT JOIN Empresas E on E.Id_Empresa = T.Id_Empresa LEFT JOIN Adm_Sedes S on S.Id_Sede = T.Id_Sede WHERE CC.Id_Quien_Autoriza = @Id_Tercero_Usuario AND CC.Fecha_Inicio = @Fecha ORDER BY CC.Id_Control_Contratistas DESC;">
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
                    <h5 class="modal-title" style="text-align: center">REASIGNAR RESPONSABLE</h5>
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
                    <h5 class="modal-title" style="text-align: center">REAGENDAR FECHA</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center; padding: 1px;">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" class="cd-form" style="width: 100%; margin: 0;" ChildrenAsTriggers="true">
                        <ContentTemplate>

                            <div class="row" style="margin-top: 10px;">

                                <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                    <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                        <label class="cd-label" for="TxFechaIngresoEditar">Fecha Ingreso</label>
                                        <asp:TextBox ID="TxFechaIngresoEditar" runat="server" TextMode="Date" placeholder="Fecha Ingreso" CssClass="Fecha"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                    <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                        <label class="cd-label" for="TxFechaSalidaEditar">Fecha Salida</label>
                                        <asp:TextBox ID="TxFechaSalidaEditar" runat="server" TextMode="Date" placeholder="Fecha Salida" CssClass="Fecha"></asp:TextBox>
                                    </div>
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

    <%--DIAS--%>
    <div class="modal" id="Dias" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center;">
        <div class="modal-dialog" role="document" style="text-align: center; font-family: sans-serif;">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">DIAS</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center; padding: 1px;">
                    <asp:UpdatePanel ID="UpdatePanel7" runat="server" class="cd-form" style="width: 100%; margin: 0;" ChildrenAsTriggers="true">
                        <ContentTemplate>

                            <div class="row" style="margin-top: 16px;">

                                <div class="col-md-12 mb-3" style="margin-top: 5px;">
                                    <div id="DvDias" style="margin: -10px 1px;">
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxLunes" runat="server" Text="Lu" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxMartes" runat="server" Text="Ma" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxMiercoles" runat="server" Text="Mi" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxJueves" runat="server" Text="Ju" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxViernes" runat="server" Text="Vi" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxSabado" runat="server" Text="Sa" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxDomingo" runat="server" Text="Do" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; width: 12.5%">
                                                    <div>
                                                        <asp:CheckBox ID="TxFestivo" runat="server" Text="Fes" Font-Size="12pt" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>

                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtEditarDias" runat="server" Text="REAGENDAR" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Documento Detalle -->
    <div class="modal fade" id="Foto_Detalle" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">DOCUMENTO DETALLE</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body text-center">
                    <!-- Contenedor para imagen -->
                    <img id="Img_Detalle" class="img-fluid rounded" alt="Documento Detalle" style="display: none; max-width: 100%; height: auto;" />

                    <!-- Contenedor para PDF -->
                    <iframe id="Pdf_Detalle" style="display: none; width: 100%; height: 500px; border: none;"></iframe>
                </div>
            </div>
        </div>
    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>

    <script type="text/javascript">

        function Mostrar_Documento_Modal(nombreArchivo) {
            // Construir la URL completa del archivo
            var url = "/Adjunto/Documentos_Contratistas/" + nombreArchivo;

            // Obtener referencias a los elementos del modal
            var imgElement = document.getElementById('Img_Detalle');
            var pdfElement = document.getElementById('Pdf_Detalle');

            // Determinar la extensión del archivo
            var extension = nombreArchivo.split('.').pop().toLowerCase();

            if (["jpg", "jpeg", "png", "gif", "webp"].includes(extension)) {
                // Mostrar imagen
                imgElement.src = url;
                imgElement.style.display = "block";
                pdfElement.style.display = "none";
            } else if (extension === "pdf") {
                // Mostrar PDF
                pdfElement.src = url;
                pdfElement.style.display = "block";
                imgElement.style.display = "none";
            } else if (["xls", "xlsx", "csv"].includes(extension)) {
                // Si es Excel, abrir en nueva pestaña o descargar
                window.open(url, "_blank");
                return; // No abrir el modal
            } else {
                // Si el formato no es soportado, ocultar ambos
                imgElement.style.display = "none";
                pdfElement.style.display = "none";
            }

            // Abrir el modal solo si es imagen o PDF
            $('#Foto_Detalle').modal('show');
        }

    </script>

</asp:Content>




