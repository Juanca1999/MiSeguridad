<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Registrar_Visita_Contratista.aspx.vb" Inherits="MiSeguridad.Registrar_Visita_Contratista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="https://optimus.miroseguridad.com:8282/Diseno/Numeros/Numeros.js"></script>
    <link rel="stylesheet" type="text/css" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700|Roboto+Slab:400,700|Material+Icons" />

    <link href="https://optimus.miroseguridad.com:8282/Diseno/Imagenes/Header.css" rel="stylesheet" />
    <script src="https://optimus.miroseguridad.com:8282/Diseno/Iconos/Iconos.js"></script>



    <link href='https://fonts.googleapis.com/css?family=Open+Sans:400,300,700' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/diseno/2019/Form/css/reset.css">
    <!-- CSS reset -->
    <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/diseno/2019/Form/css/style.css">
    <!-- Resource style -->
    <script src="https://optimus.miroseguridad.com:8282/diseno/2019/Form/js/modernizr.js"></script>
    <!-- Modernizr -->
    <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/Diseno/2018/Formularios/Bootstrap/css/bootstrap.min.css" />
    <%-- <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/Diseno/2018/Formularios/Bootstrap/css/bootstrap-theme.css" />--%>
    <link href="https://optimus.miroseguridad.com:8282/diseno/2019/Form/bootstrap-float-label/bootstrap-float-label.css" rel="stylesheet">
    <link href="https://optimus.miroseguridad.com:8282/diseno/2019/Form/bootstrap-float-label/bootstrap-float-label.min.css" rel="stylesheet">
    <script type="text/javascript" src="https://optimus.miroseguridad.com:8282/diseno/2019/sweetalert/jquery.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://optimus.miroseguridad.com:8282/diseno/2019/sweetalert/sweetalert2.min.css">
    <script type="text/javascript" src="https://optimus.miroseguridad.com:8282/diseno/2019/sweetalert/sweetalert2.min.js"></script>

    <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/diseno/2019/SubMenu/css/reset.css">
    <!-- CSS reset -->
    <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/diseno/2019/SubMenu/css/style.css">
    <!-- Resource style -->
    <script type="text/javascript" src="https://www.jquery-az.com/boots/js/bootstrap-filestyle.min.js"> </script>
    <link rel="stylesheet" href="https://optimus.miroseguridad.com:8282/diseno/2019/Menu/css/style.css" />
    <!-- Resource style -->

    <style>
        .cd-form {
            max-width: 100%;
            width: 100%;
            margin: 10px 11%;
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

        .custom-border {
            position: relative;
        }

            .custom-border::before {
                content: '';
                position: absolute;
                left: 7%;
                height: 90%; /* Ajusta esto para hacer la línea más corta o más larga */
                width: 2px;
                background-color: #6c757d; /* Color de la línea */
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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">OPERACIONES - GESTIONAR VISITAS CONTRATISTAS</span>
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

        <div class="col-md-12 mb-3" style="padding-bottom: 25px; margin-top: 0px; text-align: center">
            <button id="BtEntrada" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR ENTRADA
            </button>

            <button id="BtSalida" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR SALIDA
            </button>

            <button id="BtEntradaContratista" runat="server" type="button" class="btn btn-outline-primary" style="width: 200px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                REGISTRAR ENTRADA CONTRATISTA
            </button>

            <button id="BtSalidaContratista" runat="server" type="button" class="btn btn-primary" style="width: 200px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR SALIDA CONTRATISTA
            </button>
        </div>

        <div class="cd-form">

            <div class="row" style="padding-top: 0px; margin-top: 0px;">

                <div class="col-md-12 mb-3" style="display: flex; justify-content: center; padding-bottom: 4%; padding-top: 0%;">
                    <div class="icon has-float-label">
                        <label class="cd-label" for="TxBuscarCedulaContratista" style="font-size: 15px; font-weight: bold;">Buscar Cedula Contratista</label>
                        <asp:TextBox ID="TxBuscarCedulaContratista" runat="server" CssClass="Buscar" AutoPostBack="true"></asp:TextBox>
                    </div>
                    <asp:Button ID="BtBuscarContratista" runat="server" Text="BUSCAR" CssClass="btn btn-outline-primary" Style="background-color: #1e2833; border-color: #1e2833; padding: 15px 20px; margin-left: 20px;" />
                </div>

                <div id="Tabla" runat="server" class="col-md-12 mb-3" visible="false">

                    <div class="col-md-12 mb-3" style="margin-top: 0px; text-align: center;">
                        <asp:Label ID="TituloLabel" runat="server" Text="Autorizado" Font-Size="X-Large" ForeColor="Black" Font-Names="Trebuchet MS" Width="280px" Style="margin-bottom: 10px;"></asp:Label>
                    </div>

                    <div class="col-md-12 mb-3">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="always">
                            <ContentTemplate>
                                <asp:ListView ID="LvContratistas" runat="server" DataSourceID="SqlContratistas">
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
                                                <asp:Label ID="ContratistaLabel" runat="server" Text='<%# Eval("Contratista") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Id_InmuebleLabel" runat="server" Text='<%# Eval("Id_Inmueble") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Fecha_InicioLabel" runat="server" Text='<%# Eval("Fecha_Inicio") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Fecha_FinLabel" runat="server" Text='<%# Eval("Fecha_Fin") %>' Font-Size="8pt" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>

                                    <LayoutTemplate>
                                        <table runat="server" style="width: 100%">
                                            <tr runat="server">
                                                <td runat="server">
                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                        <tr runat="server">
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 35%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CONBTRATISTA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">INMUEBLE</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA INICIO</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA FIN</th>
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

                                <asp:SqlDataSource ID="SqlContratistas" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = C.Cedula_Contratista WHERE C.Cedula_Contratista = @Contratista ORDER BY C.Id_Control_Contratistas DESC">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="TxBuscarCedulaContratista" Name="Contratista" PropertyName="Text" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>

            </div>

            <div class="row">

                <div class="col-md-12" style="display: flex; justify-content: center;">
                    <asp:Button ID="BtGuardar" runat="server" Text="REGISTRAR ENTRADA" Visible="false" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" OnClientClick="return ValidateForm();" />
                    <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                </div>

            </div>

        </div>

    </div>


    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>

</asp:Content>




