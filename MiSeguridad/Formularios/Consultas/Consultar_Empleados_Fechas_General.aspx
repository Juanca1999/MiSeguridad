<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Consultar_Empleados_Fechas_General.aspx.vb" Inherits="MiSeguridad.Consultar_Empleados_Fechas_General" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">CONSULTAS - CONSULTAR VISITAS GENERAL POR FECHA</span>
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
                            <label class="cd-label" for="TxFechaInicio" style="font-size: 12pt; font-weight: bold;">Fecha Inicio</label>
                            <asp:TextBox ID="TxFechaInicio" runat="server" TextMode="Date" CssClass="Fecha" AutoPostBack="true" Width="90%"></asp:TextBox>
                        </div>
                        <div class="icon has-float-label">
                            <label class="cd-label" for="TxFechaFin" style="font-size: 12pt; font-weight: bold;">Fecha Fin</label>
                            <asp:TextBox ID="TxFechaFin" runat="server" TextMode="Date" CssClass="Fecha" AutoPostBack="true" Width="90%"></asp:TextBox>
                        </div>
                        <asp:Button ID="BtBuscarFecha" runat="server" Text="BUSCAR" CssClass="btn btn-outline-primary" Style="background-color: #1e2833; border-color: #1e2833; padding: 15px 20px;" />
                    </div>

                    <div id="Tabla" runat="server" class="col-md-12 mb-3" visible="false">

                        <div class="row mb-3" style="text-align: center;">
                            <div class="col-md-10">
                                <asp:Label ID="Titulo2Label" runat="server" Text="Empleados" Font-Size="X-Large" ForeColor="Black" Font-Names="Trebuchet MS" Width="280px"></asp:Label>
                            </div>
                            <div class="col-md-2">
                                <asp:ImageButton ID="BtExportarExcel" runat="server" ImageUrl="https://optimus.miroseguridad.com:8282/Diseno/2024/Logo_Excel.png" Width="30px" Style="margin-top: 10px;" />
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" style="margin-top: -15px;" UpdateMode="always">
                                <ContentTemplate>
                                    <asp:ListView ID="LvEmpleados" runat="server" DataSourceID="SqlEmpleados">
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
                                                    <asp:Label ID="CedulaLabel" runat="server" Text='<%# Eval("Cedula") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_Inicio_VisitaLabel" runat="server" Text='<%# Eval("Fecha_Inicio_Visita") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Hora_Inicio_VisitaLabel" runat="server" Text='<%# Eval("Hora_Inicio_Visita") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Fecha_Fin_VisitaLabel" runat="server" Text='<%# Eval("Fecha_Fin_Visita") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="Hora_fin_VisitaLabel" runat="server" Text='<%# Eval("Hora_fin_Visita") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="RolLabel" runat="server" Text='<%# Eval("Rol") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="CargoLabel" runat="server" Text='<%# Eval("Cargo") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="EmpresaLabel" runat="server" Text='<%# Eval("Empresa") %>' Font-Size="10pt" />
                                                </td>
                                                <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="EstadoLabel" runat="server" Text='<%# Eval("Estado") %>' Font-Size="10pt" />
                                                </td>
                                                <td id="Td_Restaurante" runat="server" Visible="false" style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                    <asp:Label ID="TipoAlimentacionLabel" runat="server" Text='<%# Eval("Tipo_Comida") %>' Font-Size="10pt" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>

                                        <LayoutTemplate>
                                            <table runat="server" style="width: 100%">
                                                <tr runat="server">
                                                    <td runat="server">
                                                        <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                            <tr runat="server">
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">CEDULA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">NOMBRES</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">FECHA INGRESO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">HORA INGRESO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">FECHA SALIDA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">HORA SALIDA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">ROL</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">CARGO</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">EMPRESA</th>
                                                                <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">ESTADO</th>
                                                                <th id="Encabezado_Restaurante" runat="server" Visible="false" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">TIPO ALIMENTACION</th>
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

                                    <asp:SqlDataSource ID="SqlEmpleados" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT T.Cedula, T.Nombres, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita, V.Fecha_Fin_Visita, Hora_fin_Visita, R.Rol, T.Cargo, E.Empresa, CASE WHEN V.Estado = 1 THEN 'EN CURSO' ELSE 'FINALIZADA' END Estado, CASE WHEN V.Hora_Inicio_Visita BETWEEN '06:00:00' AND '08:30:00' THEN 'DESAYUNO' WHEN V.Hora_Inicio_Visita BETWEEN '10:30:00' AND '14:30:00' THEN 'ALMUERZO' WHEN V.Hora_Inicio_Visita BETWEEN '16:30:00' AND '19:30:00' THEN 'CENA' ELSE 'FUERA DE RANGO' END AS Tipo_Comida FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Empresas E ON E.Id_Empresa = T.Id_Empresa WHERE V.Id_Sede = @Sede AND T.Id_Rol = 8 AND V.Fecha_Inicio_Visita BETWEEN @Fecha_Inicio AND @Fecha_Fin ORDER BY V.Fecha_Inicio_Visita DESC, V.Hora_Inicio_Visita DESC">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Sede" SessionField="Sucursal_Usuario" />
                                            <asp:ControlParameter ControlID="TxFechaInicio" Name="Fecha_Inicio" PropertyName="Text" />
                                            <asp:ControlParameter ControlID="TxFechaFin" Name="Fecha_Fin" PropertyName="Text" />
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

    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>
</asp:Content>




