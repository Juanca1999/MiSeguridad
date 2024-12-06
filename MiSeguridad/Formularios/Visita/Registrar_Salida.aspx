<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Registrar_Salida.aspx.vb" Inherits="MiSeguridad.Registrar_Salida" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">OPERACIONES - GESTIONAR VISITAS</span>
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

        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 25px; margin-bottom: 0px; margin-top: 0px; padding-right: 0pt; text-align: center">
            <button id="BtEntrada" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR ENTRADA
            </button>

            <button id="BtSalida" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                REGISTRAR SALIDA
            </button>

            <button id="BtEntradaContratista" runat="server" type="button" class="btn btn-primary" style="width: 200px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR ENTRADA CONTRATISTA
            </button>

            <button id="BtSalidaContratista" runat="server" type="button" class="btn btn-primary" style="width: 200px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR SALIDA CONTRATISTA
            </button>

        </div>

        <div class="cd-form">

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                <ContentTemplate>

                    <div class="row" style="padding-top: 0px; margin-top: 0px;">

                        <div class="col-md-4 mb-3" style="margin-top: 0px;">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxBuscarInmueble" style="font-size: 2rem; font-weight: bold;">Buscar Inmueble</label>
                                <i class="material-icons" style="position: absolute; top: 25%; left: 18px; font-size: 24px;">search</i>
                                <asp:DropDownList ID="TxBuscarInmueble" runat="server" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlInmueble" DataTextField="Id_inmueble" DataValueField="Id_inmueble" Style="width: 80%; border: 2px solid #007bff; font-size: 15pt; padding: 9px;">
                                    <asp:ListItem Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlInmueble" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_inmueble FROM Adm_Inmueble ORDER BY Id_inmueble"></asp:SqlDataSource>
                            </div>
                        </div>

                        <div class="col-md-4 mb-3" style="margin-top: 0px;">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxBuscarCedula" style="font-size: 2rem; font-weight: bold;">Buscar Cedula</label>
                                <i class="material-icons" style="position: absolute; top: 25%; left: 18px; font-size: 24px;">search</i>
                                <asp:TextBox ID="TxBuscarCedula" runat="server" AutoPostBack="true" Style="width: 80%; border: 2px solid #007bff; font-size: 15pt;"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-4 mb-3" style="margin-top: 0px;">
                            <div class="icon has-float-label" style="margin-top: 0px;">
                                <label class="cd-label" for="TxBuscarPersona" style="font-size: 2rem; font-weight: bold;">Buscar Nombre Persona</label>
                                <i class="material-icons" style="position: absolute; top: 25%; left: 18px; font-size: 24px;">search</i>
                                <asp:TextBox ID="TxBuscarPersona" runat="server" AutoPostBack="true" Style="width: 100%; border: 2px solid #007bff; font-size: 15pt;"></asp:TextBox>
                            </div>
                        </div>

                        <div id="Tabla" runat="server" class="col-md-12 mb-3" visible="false">

                            <div class="col-md-12 mb-3" style="margin-top: 35px; text-align: center;">
                                <asp:Label ID="Titulo2Label" runat="server" Text="Visitantes" Font-Size="X-Large" ForeColor="Black" Font-Names="Trebuchet MS" Width="280px"></asp:Label>
                            </div>

                            <div class="col-md-12 mb-3">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" style="margin-top: -15px;" UpdateMode="always">
                                    <ContentTemplate>
                                        <asp:ListView ID="LvVisitantes" runat="server" DataSourceID="SqlVisitantes">
                                            <EmptyDataTemplate>
                                                <table runat="server" style="">
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                </table>
                                            </EmptyDataTemplate>

                                            <ItemTemplate>
                                                <tr style="">
                                                    <asp:Label ID="Id_VisitaLabel" runat="server" Text='<%# Eval("Id_Visita") %>' Font-Size="0pt" />
                                                    <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                        <asp:Label ID="CedulaLabel" runat="server" Text='<%# Eval("Cedula") %>' Font-Size="8pt" />
                                                    </td>
                                                    <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                        <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' Font-Size="8pt" />
                                                    </td>
                                                    <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                        <asp:Label ID="Id_InmuebleLabel" runat="server" Text='<%# Eval("Id_Inmueble") %>' Font-Size="8pt" />
                                                    </td>
                                                    <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                        <asp:Label ID="Fecha_Inicio_VisitaLabel" runat="server" Text='<%# Eval("Fecha_Inicio_Visita") %>' Font-Size="8pt" />
                                                        <br />
                                                        <asp:Label ID="Hora_Inicio_VisitaLabel" runat="server" Text='<%# Eval("Hora_Inicio_Visita") %>' Font-Size="8pt" />
                                                    </td>
                                                    <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                        <asp:ImageButton ID="BtSalir" runat="server" CommandName="Salir" ImageUrl="~/Diseno/Imagenes/salida.png" Width="25px" data-toggle="modal" data-target="#Salir" CommandArgument='<%# Container.DataItemIndex %>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>

                                            <LayoutTemplate>
                                                <table runat="server" style="width: 100%">
                                                    <tr runat="server">
                                                        <td runat="server">
                                                            <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                                <tr runat="server">
                                                                    <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CEDULA</th>
                                                                    <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 50%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">NOMBRES</th>
                                                                    <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">INMUEBLE</th>
                                                                    <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA HORA INGRESO</th>
                                                                    <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">REGISTRAR SALIDA</th>
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

                                        <asp:SqlDataSource ID="SqlVisitantes" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND V.Id_Inmueble = @Inmueble ORDER BY T.Nombres">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="TxBuscarInmueble" Name="Inmueble" PropertyName="Text" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>

                                        <asp:SqlDataSource ID="SqlVisitantesCedula" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND T.Cedula = @Cedula ORDER BY T.Nombres">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="TxBuscarCedula" Name="Cedula" PropertyName="Text" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>

                                        <asp:SqlDataSource ID="SqlVisitantesNombre" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT V.Id_Visita, T.Cedula, T.Nombres, V.Id_Inmueble, V.Fecha_Inicio_Visita, V.Hora_Inicio_Visita FROM Adm_Visita V LEFT JOIN Terceros T ON T.Id_Tercero = V.Id_Quien_Ingresa WHERE V.Estado = 1 AND T.Nombres LIKE '%' + @Nombre + '%' ORDER BY T.Nombres">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="TxBuscarPersona" Name="Nombre" PropertyName="Text" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>

                        </div>

                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

    </div>

    <%--SALIR--%>
    <div class="modal fade" id="Salir" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
        <div class="modal-dialog" role="document" style="text-align: center">
            <div class="modal-content" style="text-align: center">
                <div class="modal-header" style="text-align: center">
                    <h5 class="modal-title" style="text-align: center">REGISTRAR SALIDA</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="text-align: center">
                    <asp:Label ID="Label3" runat="server" Text="¿Seguro que desea registrar esta salida?"></asp:Label>


                </div>
                <div class="modal-footer" style="text-align: center">
                    <asp:Button ID="BtSalir" runat="server" Text="Salida" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="12pt" />
                </div>
            </div>
        </div>
    </div>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
</asp:Content>




