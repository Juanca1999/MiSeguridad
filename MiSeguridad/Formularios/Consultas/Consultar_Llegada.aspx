<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Consultar_Llegada.aspx.vb" Inherits="MiSeguridad.Consultar_Llegada" %>

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
            max-width: 1080px;
            width: 100%;
            margin: 0;
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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">CONSULTAS - CONSULTAR LLEGADA</span>
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

        <div class="col-md-1 mb-3">
        </div>

        <div class="col-md-10 mb-3">

            <div class="cd-form">

                <div class="row" style="padding-top: 0px; margin-top: 0px;">

                    <div class="col-md-12 mb-3" style="display: flex; justify-content: center; padding-bottom: 7%; padding-top: 3%;">
                        <div class="icon has-float-label">
                            <label class="cd-label" for="TxNombreAsignado" style="font-size: 12pt; font-weight: bold;">Buscar Nombre Asignado</label>
                            <asp:TextBox ID="TxNombreAsignado" runat="server" CssClass="Buscar" AutoPostBack="true" Width="100%"></asp:TextBox>
                        </div>
                        <asp:Button ID="BtBuscarAsignado" runat="server" Text="BUSCAR" CssClass="btn btn-outline-primary" Style="background-color: #1e2833; border-color: #1e2833; padding: 15px 20px; margin-left: 20px;" />
                    </div>

                    <div id="Tabla" runat="server" class="col-md-12 mb-3" visible="false">

                        <div class="col-md-12 mb-3">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" style="margin-top: -15px;" UpdateMode="always">
                                <ContentTemplate>
                                    <asp:ListView ID="LvAsignados" runat="server" DataSourceID="SqlAsignados">
                                        <EmptyDataTemplate>
                                            <table runat="server" style="">
                                                <tr>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>

                                        <ItemTemplate>
                                            <tr style="">
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Nombre_AsignadoLabel" runat="server" Text='<%# Eval("Nombre_Asignado") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 5%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:ImageButton ID="BtDetalle" runat="server" CommandName="Detalle" data-toggle="modal" data-target="#Detalle" ImageUrl="/../../../Diseno/Imagenes/Detalle.png" Width="20px" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>

                                        <LayoutTemplate>
                                            <table runat="server" style="width: 100%">
                                                <tr runat="server">
                                                    <td runat="server">
                                                        <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                            <tr runat="server">
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">PLACA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 50%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">NOMBRE ASIGNADO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">DETALLE</th>
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

                                    <asp:SqlDataSource ID="SqlAsignados" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Placa, Nombre_Asignado FROM Adm_Vehiculo_Asignado_A WHERE Nombre_Asignado LIKE '%' + @Nombre_Asignado + '%' ORDER BY Nombre_Asignado DESC">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="TxNombreAsignado" Name="Nombre_Asignado" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                    </div>

                </div>

            </div>

        </div>

        <div class="col-md-1 mb-3">
        </div>

        <%--DETALLE RONDA AGRUPADA--%>
        <div class="modal fade" id="Detalle" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog modal-lg" role="document" style="text-align: center;">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center; height: 45px;">
                        <h5 class="modal-title" style="text-align: center">DETALLE</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body" style="text-align: center">
                        <asp:UpdatePanel ID="UpdatePanel15" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <div style="margin-bottom: 16px;">
                                    <asp:Label ID="NombreEstudianteLabel" runat="server" Font-Bold="true" Text="" />
                                </div>
                                <asp:ListView ID="LvDetallePlaca" runat="server" DataSourceID="Sql_Detalle_Placa">
                                    <EmptyDataTemplate>
                                        <table runat="server" style="">
                                            <tr>
                                                <td></td>
                                            </tr>
                                        </table>
                                    </EmptyDataTemplate>

                                    <ItemTemplate>
                                        <tr style="">
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' Font-Size="10pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Fecha_Hora_EventoLabel" runat="server" Text='<%# Eval("Fecha_Hora_Evento") %>' Font-Size="10pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Tipo_EventoLabel" runat="server" Text='<%# Eval("Tipo_Evento") %>' Font-Size="10pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Nombre_AccesoLabel" runat="server" Text='<%# Eval("Nombre_Acceso") %>' Font-Size="10pt" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>

                                    <LayoutTemplate>
                                        <table runat="server" style="width: 100%">
                                            <tr runat="server">
                                                <td runat="server">
                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                        <tr runat="server">
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">PLACA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 25%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">FECHA HORA EVENTO</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">TIPO EVENTO</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">NOMBRE ACCESO</th>
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

                                <asp:SqlDataSource ID="Sql_Detalle_Placa" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                    </div>
                </div>
            </div>
        </div>

    </div>


    <script src="/Diseno/jquery.min.js-2.js"></script>
</asp:Content>




