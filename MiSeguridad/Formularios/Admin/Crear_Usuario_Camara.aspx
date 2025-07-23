<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Usuario_Camara.aspx.vb" Inherits="MiSeguridad.Crear_Usuario_Camara" %>

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
    <script type="text/javascript">

        function ValidarLockers() {
            var Vestier = document.getElementById("<%=TxVestier.ClientID%>").value;
            var Locker = document.getElementById("<%=TxLocker.ClientID%>").value;

            if (Vestier == "") {
                alert("Debe seleccionar un vestier.");
                return false;
            } else if (Locker == "") {
                alert("Debe seleccionar un locker");
                return false;
            }

            __doPostBack('<%= BtAsignarLocker.UniqueID %>', '');
            return false;
        }


        function ValidarEquipos() {
            var Equipo = document.getElementById("<%=TxDetalleEquipo.ClientID%>").value;
            if (Equipo == "") {
                alert("Debe agregar algun equipo");
                return false;
            }

            __doPostBack('<%= BtAsignarEquipo.UniqueID %>', '');
            return false;
        }

    </script>

    <style>
        .cd-form {
            max-width: 980px;
        }

        tr:nth-child(even) {
            background-color: #dddddd;
        }

        @media only screen and (min-width: 600px) {
            .cd-form div {
                margin: 20px 0px;
            }
        }

        .card-body {
            padding: 0.65rem;
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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR USUARIO CAMARA</span>
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

        <div class="col-md-5 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px; padding-right: 0pt;">

            <div class="cd-form" style="padding-top: 5px; margin-top: 0px">
                <div class="content" style="padding-top: 0px; margin-top: 0px">
                    <div class="container-fluid" style="padding-top: 0px; margin-top: 0px">
                        <div class="card" style="border: none; border-color: transparent; padding-top: 0px; margin-top: -6px; margin-bottom: 12px; background: #1e2833;">
                            <div class="card-header card-header-primary" style="width: 100%; margin: 0px; padding: 5px">
                                <h4 style="margin-bottom: 14px; margin-top: 14px;">
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR USUARIO CAMARA</asp:LinkButton>
                                </h4>
                                <p class="card-category">
                                </p>
                            </div>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" style="margin: 2px 0px;">
                            <ContentTemplate>

                                <div class="row" style="padding-top: 0px; margin-top: 0px;">

                                    <div id="Vehiculos" runat="server" visible="false" class="col-md-12 mb-3" style="margin: 0px 0px;">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always" style="margin: 0px 0px;">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvVehiculosAsignados" runat="server" DataSourceID="SqlVehiculosAsignados">
                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td></td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <td style="width: 10%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="Tipo_VehiculoLabel" runat="server" Text='<%# Eval("Tipo_Vehiculo") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 20%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 20%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="MarcaLabel" runat="server" Text='<%# Eval("Marca") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 20%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="ModeloLabel" runat="server" Text='<%# Eval("Modelo") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 20%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="ColorLabel" runat="server" Text='<%# Eval("Color") %>' Font-Size="8pt" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>

                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                                        <tr runat="server">
                                                                            <th runat="server" colspan="5" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">VEHICULOS ASIGNADOS</th>
                                                                        </tr>
                                                                        <tr runat="server">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">TIPO VEHICULO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">PLACA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">MARCA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">MODELO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">COLOR</th>
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
                                        <asp:SqlDataSource ID="SqlVehiculosAsignados" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                                    </div>

                                    <div id="Lockers" runat="server" visible="false" class="col-md-12 mb-3" style="margin: 0px 0px;">
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Always" style="margin: 0px 0px;">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvLockerAsignados" runat="server" DataSourceID="SqlLockersAsignados">
                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td></td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <td style="width: 50%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="Id_LockerLabel" runat="server" Text='<%# Eval("Id_Locker") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 50%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="Nombre_VestierLabel" runat="server" Text='<%# Eval("Nombre_Vestier") %>' Font-Size="8pt" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>

                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                                        <tr runat="server">
                                                                            <th runat="server" colspan="2" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">LOCKERS ASIGNADOS</th>
                                                                        </tr>
                                                                        <tr runat="server">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 50%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">LOCKER</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 50%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">VESTIER</th>
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
                                        <asp:SqlDataSource ID="SqlLockersAsignados" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                                    </div>

                                    <div id="Equipos" runat="server" visible="false" class="col-md-12 mb-3" style="margin: 0px 0px;">
                                        <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Always" style="margin: 0px 0px;">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvEquipos" runat="server" DataSourceID="SqlEquipos">
                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td></td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <td style="width: 40%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="Detalle_EquipoLabel" runat="server" Text='<%# Eval("Detalle_Equipo") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 30%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="MarcaLabel" runat="server" Text='<%# Eval("Marca") %>' Font-Size="8pt" />
                                                            </td>
                                                            <td style="width: 30%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                                <asp:Label ID="SerialLabel" runat="server" Text='<%# Eval("Serial") %>' Font-Size="8pt" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>

                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                                        <tr runat="server">
                                                                            <th runat="server" colspan="3" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; background-color: #1e2833; color: white; font-size: 10pt; height: 30px">EQUIPOS ASIGNADOS</th>
                                                                        </tr>
                                                                        <tr runat="server">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">EQUIPO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">MARCA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">SERIAL</th>
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
                                        <asp:SqlDataSource ID="SqlEquipos" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 10px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCedula">Cedula</label>
                                            <asp:TextBox ID="TxCedula" runat="server" placeholder="Cedula" CssClass="Cedula" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombres">Nombres</label>
                                            <asp:TextBox ID="TxNombres" runat="server" placeholder="Nombres" CssClass="Nombre_Usuario" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCorreo">Correo</label>
                                            <asp:TextBox ID="TxCorreo" runat="server" placeholder="Correo" TextMode="Email" CssClass="email"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxUsuario">Usuario</label>
                                            <asp:TextBox ID="TxUsuario" runat="server" placeholder="Usuario" CssClass="Nombre_Usuario"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPassword">Contraseña</label>
                                            <asp:TextBox ID="TxPassword" runat="server" placeholder="Contraseña" CssClass="Cod"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCargo">Cargo</label>
                                            <asp:TextBox ID="TxCargo" runat="server" placeholder="Cargo" CssClass="Nombre_Usuario"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxGenero">Genero</label>
                                            <asp:DropDownList ID="TxGenero" runat="server" CssClass="tipo" AppendDataBoundItems="true" Required="1">
                                                <asp:ListItem Value="">Genero</asp:ListItem>
                                                <asp:ListItem Value="male">MASCULINO</asp:ListItem>
                                                <asp:ListItem Value="female">FEMENINO</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTipoUsuario">Tipo Usuario</label>
                                            <asp:DropDownList ID="TxTipoUsuario" runat="server" CssClass="tipo" AppendDataBoundItems="true" Required="1">
                                                <asp:ListItem Value="">Tipo Usuario</asp:ListItem>
                                                <asp:ListItem Value="normal">NORMAL</asp:ListItem>
                                                <asp:ListItem Value="visitor">VISITANTE</asp:ListItem>
                                                <asp:ListItem Value="blackList">LISTA NEGRA</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxIdRol">Rol</label>
                                            <asp:DropDownList ID="TxIdRol" runat="server" CssClass="tipo" AppendDataBoundItems="true" DataSourceID="SqlRol" DataTextField="Rol" DataValueField="Id_Rol">
                                                <asp:ListItem Value="">Rol</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlRol" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Rol, Rol FROM Adm_Rol ORDER BY Rol"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCelular">Celular</label>
                                            <asp:TextBox ID="TxCelular" runat="server" placeholder="Celular" TextMode="Number" CssClass="Telefono"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 15px; margin-top: 0px;">
                                            <label class="cd-label" for="TxEstado">Estado</label>
                                            <asp:DropDownList ID="TxEstado" runat="server" AppendDataBoundItems="true" CssClass="Calificacion">
                                                <asp:ListItem Value="">Estado</asp:ListItem>
                                                <asp:ListItem Value="1">ACTIVO</asp:ListItem>
                                                <asp:ListItem Value="0">INACTIVO</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-5 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxBuscarResponsable">Buscar Responsable</label>
                                            <asp:TextBox ID="TxBuscarResponsable" runat="server" CssClass="Buscar" placeholder="Buscar Responsable" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-7 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreResponsable">Nombre Responsable</label>
                                            <asp:DropDownList ID="TxNombreResponsable" runat="server" CssClass="Cedula">
                                                <asp:ListItem Value="">Nombre Responsable</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-10 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxEmpresa">Empresa</label>
                                            <asp:DropDownList ID="TxEmpresa" runat="server" CssClass="ciudad" AppendDataBoundItems="true" DataSourceID="SqlEmpresas" DataTextField="Empresa" DataValueField="Id_Empresa">
                                                <asp:ListItem Value="">Empresa</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlEmpresas" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Empresa, Empresa FROM Empresas ORDER BY Id_Empresa"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-2 mb-3" style="margin-top: -10px; align-content: center;">
                                        <asp:ImageButton ID="BtnAgregarEmpresa" UseSubmitBehavior="false" runat="server" ImageUrl="https://cdn-icons-png.flaticon.com/512/6325/6325028.png" Width="35px" data-toggle="modal" data-target="#Crear_Empresa" />
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxSede">Sede</label>
                                            <asp:DropDownList ID="TxSede" runat="server" CssClass="ciudad" AppendDataBoundItems="true" DataSourceID="SqlSedes" DataTextField="Nombre_Sede" DataValueField="Id_Sede">
                                                <asp:ListItem Value="">Sede</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlSedes" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Sede, Nombre_Sede FROM Adm_Sedes ORDER BY Id_Sede"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 15px; margin-top: -15px;">
                                <label class="cd-label" for="TxTipoSubida">Forma de Subir</label>
                                <asp:DropDownList ID="TxTipoSubida" runat="server" AppendDataBoundItems="true" CssClass="Calificacion">
                                    <asp:ListItem Value="">Forma de Subir</asp:ListItem>
                                    <asp:ListItem Value="1">ARCHIVO</asp:ListItem>
                                    <asp:ListItem Value="0">TOMAR FOTO</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div id="Archivo" runat="server" style="display: none" class="col-md-12 mb-3">
                            <label class="cd-label" for="FlFoto" style="margin-left: 41px; margin-top: 10px;">Foto</label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">image</i>
                            <asp:FileUpload ID="FlFoto" runat="server" class="btn btn-outline-primary" Width="100%" />
                        </div>

                        <div id="Tomar_Foto" runat="server" style="display: none" class="col-md-12 custom-border">

                            <h2 style="text-align: center">Seleccione una camara</h2>
                            <div style="display: flex; justify-content: center">
                                <select id="cameraSelect" style="width: 70%;"></select>
                            </div>
                            <br>

                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

                                    <div style="text-align: center">
                                        <img id="photo" runat="server" src="~/Diseno/Imagenes/Blanco.jpg" width="700" height="700" style="border: 2px solid #6c757d;" />
                                    </div>
                                    <br>

                                    <div style="text-align: center">
                                        <canvas id="canvas" width="700" height="700" style="display: none;"></canvas>
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <hr style="border: 1px solid #6c757d; width: 80%" />

                            <h2 style="text-align: center">Capturar Foto</h2>
                            <br>

                            <div style="text-align: center">
                                <video id="video" width="700" height="700" style="border: 2px solid #6c757d;" autoplay></video>
                            </div>

                            <div style="text-align: center">
                                <asp:Button ID="BtCapturar" runat="server" Text="Tomar Foto" CssClass="btn btn-primary" UseSubmitBehavior="false" Style="color: white" OnClientClick="return ValidateForm();" />
                            </div>

                            <br>

                            <!-- Campo oculto para enviar la imagen en Base64 -->
                            <asp:HiddenField ID="base64image" runat="server" />

                        </div>

                        <div class="col-md-12 mb-3">
                            <asp:Button ID="BtGuardar" runat="server" Text="GUARDAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                        </div>

                    </div>

                </div>

            </div>

        </div>

        <div class="col-md-7 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">

            <div class="row" style="text-align: center; padding: 0px; margin: 0px">
                <div class="col-lg-12 col-md-12">
                    <div class="content">
                        <div class="container-fluid">
                            <div class="card">
                                <div class="card-header card-header-primary" style="background: #1e2833;">
                                    <ul class="nav nav-tabs md-tabs" id="myTabEx" role="tablist">
                                        <li class="nav-item" style="margin-bottom: 10px; display: flex; align-items: center;">
                                            <a style="font-size: 11pt; color: white; display: flex; align-items: center;">
                                                <i class="material-icons" style="margin-right: 5px;">apartment</i> USUARIOS
                                            </a>
                                        </li>
                                    </ul>
                                    <p class="card-category" style="text-align: right">
                                        <i class="material-icons" style="color: white; position: absolute; margin-top: 2px; margin-left: -25px;">search</i>
                                        <asp:TextBox ID="TxBuscar" runat="server" placeholder="Buscar Usuario" AutoPostBack="true" Width="30%" Font-Size="11pt" BackColor="White"></asp:TextBox>
                                    </p>
                                </div>

                                <div class="card-body table-responsive">

                                    <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvUsuarios" runat="server">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se encuentran usuarios</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <asp:Label ID="genderLabel" runat="server" Text='<%# Eval("gender") %>' Font-Size="0pt" />
                                                            <asp:Label ID="numOfFaceLabel" runat="server" Text='<%# Eval("numOfFace") %>' Font-Size="0pt" />
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Editar" runat="server" CommandName="Editar"><i class="material-icons" style="color: #1e2833;">edit</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="employeeNoLabel" runat="server" Text='<%# Eval("employeeNo") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="nameLabel" runat="server" Text='<%# Eval("name") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="userTypelabel" runat="server" Text='<%# Eval("userType") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:LinkButton ID="Foto" runat="server" CommandName="Foto" OnClientClick='<%# "obtenerFoto(" + Eval("employeeNo") + "); return false;" %>'><i class="material-icons" style="color: #1e2833;">contact_emergency</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:ImageButton ID="Tiene_Vehiculo" runat="server" CommandName="Vehiculo" data-employeeNo='<%# Eval("employeeNo") %>' CssClass="Crear_Vehiculo" data-toggle="modal" data-target="#Crear_Vehiculo" ImageUrl="~/Diseno/Imagenes/Gris.png" Width="20px" UseSubmitBehavior="false" />
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:ImageButton ID="Tiene_Locker" runat="server" CommandName="Locker" data-employeeNo='<%# Eval("employeeNo") %>' CssClass="Asignar_Locker" data-toggle="modal" data-target="#Asignar_Locker" ImageUrl="~/Diseno/Imagenes/Gris.png" Width="20px" UseSubmitBehavior="false" />
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:ImageButton ID="Tiene_Equipo" CommandName="Equipo" data-employeeNo='<%# Eval("employeeNo") %>' CssClass="Asignar_Equipo" data-toggle="modal" data-target="#Asignar_Equipo" runat="server" ImageUrl="~/Diseno/Imagenes/Gris.png" Width="20px" UseSubmitBehavior="false" />
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:ImageButton ID="Tiene_Responsable" runat="server" ImageUrl="~/Diseno/Imagenes/Gris.png" Width="20px" />
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Eliminar" runat="server" CommandName="Eliminar" data-employeeNo='<%# Eval("employeeNo") %>' CssClass="Eliminar_Boton" data-toggle="modal" data-target="#Confirmacion"><i class="material-icons" style="color: #1e2833;">delete</i></asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%; font-size: 8pt">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%" class="table table-hover table-fixed border">
                                                                        <tr runat="server" style="" class="card-header card-header-primary text-center content-center">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons">create</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">ID EMPLEADO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">NOMBRE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">TIPO USUARIO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons">contact_emergency</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons" style="font-size: 20px">directions_car</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons" style="font-size: 20px">lock</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons" style="font-size: 20px">computer</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons" style="font-size: 20px">supervisor_account</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons">delete</i></th>
                                                                        </tr>
                                                                        <tr id="itemPlaceholder" runat="server">
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr runat="server">
                                                                <td runat="server" style="">
                                                                    <asp:DataPager ID="DataPager1" runat="server" PageSize="20" style="font-size: 12pt">
                                                                        <Fields>
                                                                            <asp:NumericPagerField />
                                                                        </Fields>
                                                                    </asp:DataPager>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </LayoutTemplate>

                                                </asp:ListView>

                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Foto Usuario -->
        <div class="modal fade" id="Foto_Usuario" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" id="MensajeModalLabel" style="text-align: center">FOTO USUARIO</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body text-center">
                        <img id="Foto_User" class="img-fluid rounded" alt="Foto Usuario" />
                    </div>
                    <div class="modal-footer" style="text-align: center">
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Confirmacion -->
        <div class="modal fade" id="Confirmacion" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" style="text-align: center">BORRAR USUARIO DE FACIAL</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body" style="text-align: center">
                        <asp:Label ID="Label1" runat="server" Text="OJO: ¿DESEA BORRAR EL USUARIO DE LA CAMARA FACIAL?"></asp:Label>
                        <asp:HiddenField ID="HiddenEmployeeNo" runat="server" />
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtAceptar_Borrar" runat="server" Text="Aceptar" class="btn btn-danger" BackColor="IndianRed" UseSubmitBehavior="false" ForeColor="White" Width="160px" Font-Size="11pt" />
                        <asp:Button ID="BtCancelar_Borrar" runat="server" Text="Cancelar" class="btn btn-primary" ForeColor="White" UseSubmitBehavior="false" Width="160px" Font-Size="11pt" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Crear Empresa -->
        <div class="modal fade" id="Crear_Empresa" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" style="text-align: center">CREAR EMPRESA</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body cd-form" style="text-align: center; margin: 0em auto; width: 100%; margin-top: 0px;">
                        <div class="row" style="margin: 0px 0px;">
                            <div class="col-md-12 mb-3" style="margin: 0px 0px;">
                                <div class="icon has-float-label" style="margin: 0px 0px;">
                                    <label class="cd-label" for="TxNombreEmpresa">Nombre Empresa</label>
                                    <asp:TextBox ID="TxNombreEmpresa" runat="server" placeholder="Nombre Empresa" CssClass="ciudad"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtCrearEmpresa" runat="server" Text="CREAR" UseSubmitBehavior="false" class="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" ForeColor="White" Width="160px" Font-Size="11pt" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Crear Vehiculo -->
        <div class="modal fade" id="Crear_Vehiculo" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" style="text-align: center">CREAR VEHICULO</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body cd-form" style="text-align: center; margin: 0em auto; width: 100%; margin-top: 0px;">

                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="row" style="margin: 0px 0px;">
                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTipoVehiculo">Tipo Vehiculo</label>
                                            <asp:DropDownList ID="TxTipoVehiculo" runat="server" CssClass="Sedes" AutoPostBack="true" OnChange="updatePlacaState()" AppendDataBoundItems="true" DataSourceID="SqlTipoVehiculo" DataTextField="Tipo_Vehiculo" DataValueField="Id_Tipo_Vehiculo">
                                                <asp:ListItem Value="">NINGUNO</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlTipoVehiculo" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Tipo_Vehiculo, Tipo_Vehiculo FROM Adm_Tipo_Vehiculo ORDER BY Tipo_Vehiculo"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="margin-top: 0px;">
                                        <div id="Placa" runat="server" class="icon has-float-label" style="margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPlaca">Placa</label>
                                            <asp:TextBox ID="TxPlaca" runat="server" placeholder="Placa" CssClass="Placa" AutoPostBack="true" oninput="handleInput()"></asp:TextBox>
                                            <asp:Label ID="lblPlacaError" runat="server" ForeColor="Red" Style="display: none; margin-top: 60px;"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxMarca">Marca</label>
                                            <asp:TextBox ID="TxMarca" runat="server" placeholder="Marca" CssClass="tipo"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxModelo">Modelo</label>
                                            <asp:TextBox ID="TxModelo" runat="server" placeholder="Modelo" CssClass="tipo"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxColor">Color</label>
                                            <asp:TextBox ID="TxColor" runat="server" placeholder="Color" CssClass="Color"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:HiddenField ID="HiddenFieldVehiculo" runat="server" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtGuardarVehiculo" runat="server" Text="CREAR" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" Style="background-color: #1e2833; border-color: #1e2833;" UseSubmitBehavior="false" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Asignar Locker -->
        <div class="modal fade" id="Asignar_Locker" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" style="text-align: center">ASIGNAR LOCKER</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body cd-form" style="text-align: center; margin: 0em auto; width: 100%; margin-top: 0px;">

                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="row" style="margin: 0px 0px;">

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxVestier">Vestier</label>
                                            <asp:DropDownList ID="TxVestier" runat="server" CssClass="ciudad" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlVestier" DataTextField="Nombre_Vestier" DataValueField="Id_Vestier">
                                                <asp:ListItem Value="">Vestier</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlVestier" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Vestier, Nombre_Vestier FROM Adm_Vestier ORDER BY Id_Vestier"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxBuscarLocker">Buscar Locker</label>
                                            <asp:TextBox ID="TxBuscarLocker" runat="server" placeholder="Buscar Locker" CssClass="Buscar" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxLocker">Locker</label>
                                            <asp:DropDownList ID="TxLocker" runat="server" CssClass="tipo">
                                                <asp:ListItem Value="">Locker</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <asp:HiddenField ID="HiddenFieldLocker" runat="server" />

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtAsignarLocker" runat="server" Text="ASIGNAR LOCKER" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" Style="background-color: #1e2833; border-color: #1e2833;" UseSubmitBehavior="false" OnClientClick="return ValidarLockers()" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Asignar Equipo -->
        <div class="modal fade" id="Asignar_Equipo" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" style="text-align: center">ASIGNAR EQUIPO</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body cd-form" style="text-align: center; margin: 0em auto; width: 100%; margin-top: 0px;">

                        <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="row" style="margin: 0px 0px;">

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxDetalleEquipo">Equipo</label>
                                            <asp:TextBox ID="TxDetalleEquipo" runat="server" placeholder="Equipo" CssClass="Vendedor"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxMarcaEquipo">Marca</label>
                                            <asp:TextBox ID="TxMarcaEquipo" runat="server" placeholder="Marca" CssClass="tipo"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxSerial">Serial</label>
                                            <asp:TextBox ID="TxSerial" runat="server" placeholder="Serial" CssClass="Cod"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:HiddenField ID="HiddenFieldEquipo" runat="server" />

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtAsignarEquipo" runat="server" Text="ASIGNAR EQUIPO" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" Style="background-color: #1e2833; border-color: #1e2833;" UseSubmitBehavior="false" OnClientClick="return ValidarEquipos()" />
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>

    <script type="text/javascript">

        document.addEventListener("DOMContentLoaded", function () {
            const tipoSubida = document.getElementById('<%= TxTipoSubida.ClientID %>');
            const divArchivo = document.getElementById('<%= Archivo.ClientID %>');
            const divTomarFoto = document.getElementById('<%= Tomar_Foto.ClientID %>');

            const actualizarVisibilidad = () => {
                const valor = tipoSubida.value;
                divArchivo.style.display = valor === "1" ? "block" : "none";
                divTomarFoto.style.display = valor === "0" ? "block" : "none";
            };

            tipoSubida.addEventListener("change", actualizarVisibilidad);
            actualizarVisibilidad(); // Llamar al cargar la página

            // ** Configuración de la cámara y manejo de dispositivos **
            const video = document.getElementById('video');
            const canvas = document.getElementById('canvas');
            const photo = document.getElementById('<%= photo.ClientID %>');
            const base64image = document.getElementById('<%= base64image.ClientID %>');
            const cameraSelect = document.getElementById('cameraSelect');

            // Función para iniciar la cámara seleccionada
            function startCamera(deviceId) {
                navigator.mediaDevices.getUserMedia({
                    video: { deviceId: deviceId ? { exact: deviceId } : undefined }
                })
                    .then(stream => {
                        video.srcObject = stream;
                    })
                    .catch(error => {
                        console.error('Error al acceder a la cámara:', error);
                    });
            }

            // ** Enumerar dispositivos (versión corregida) **
            navigator.mediaDevices.enumerateDevices()
                .then(devices => {
                    devices.forEach(device => {
                        if (device.kind === 'videoinput') {
                            const option = document.createElement('option');

                            // Eliminar el texto entre paréntesis del nombre de la cámara
                            const cameraName = device.label.replace(/\s*\(.*?\)\s*/g, '');

                            option.value = device.deviceId;
                            option.text = cameraName || 'Sin cámaras disponibles';
                            cameraSelect.appendChild(option);
                        }
                    });

                    // Comenzar con la primera cámara si está disponible
                    if (cameraSelect.options.length > 0) {
                        startCamera(cameraSelect.value);
                    }
                })
                .catch(error => {
                    console.error('Error al enumerar dispositivos:', error);
                });

            // Cambiar la cámara al seleccionar otra
            cameraSelect.addEventListener("change", () => {
                startCamera(cameraSelect.value);
            });

            // ** Captura de foto desde la cámara **
            document.getElementById('<%= BtCapturar.ClientID %>').addEventListener('click', () => {
                const context = canvas.getContext('2d');
                context.drawImage(video, 0, 0, 700, 700); // Tamaño ajustado al canvas
                const dataURL = canvas.toDataURL('image/png');

                // Asignar la imagen capturada
                base64image.value = dataURL;
                photo.src = dataURL; // Mostrar en el <img>
            });
        });
    </script>

    <script type="text/javascript">
        function obtenerFoto(Id) {
            fetch("Crear_Usuario_Camara.aspx/Consultar_Foto", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8",
                },
                body: JSON.stringify({ Id: Id }),
            })
                .then(response => response.json())
                .then(data => {
                    var result = data.d;

                    // Abrir el modal
                    $('#Foto_Usuario').modal('show');

                    // Asignar la URL directamente al src del img
                    document.getElementById('Foto_User').src = result;
                })
                .catch(error => {
                    console.log("Hubo un error al realizar la solicitud: " + error.message);
                });
        }

        document.querySelectorAll(".Eliminar_Boton").forEach(button => {
            button.addEventListener("click", function () {
                const employeeNo = this.getAttribute("data-employeeNo");
                document.getElementById('<%= HiddenEmployeeNo.ClientID %>').value = employeeNo;
            });
        });

        document.querySelectorAll(".Crear_Vehiculo").forEach(button => {
            button.addEventListener("click", function () {
                const employeeNo = this.getAttribute("data-employeeNo");
                document.getElementById('<%= HiddenFieldVehiculo.ClientID %>').value = employeeNo;
            });
        });

        document.querySelectorAll(".Asignar_Locker").forEach(button => {
            button.addEventListener("click", function () {
                const employeeNo = this.getAttribute("data-employeeNo");
                document.getElementById('<%= HiddenFieldLocker.ClientID %>').value = employeeNo;
            });
        });

        document.querySelectorAll(".Asignar_Equipo").forEach(button => {
            button.addEventListener("click", function () {
                const employeeNo = this.getAttribute("data-employeeNo");
                document.getElementById('<%= HiddenFieldEquipo.ClientID %>').value = employeeNo;
            });
        });

        function handleInput() {
            var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
            var btGuardar = document.getElementById('<%= BtGuardarVehiculo.ClientID %>');
            var tipoVehiculoElement = document.getElementById('<%= TxTipoVehiculo.ClientID %>');
            var tipoVehiculo = tipoVehiculoElement ? tipoVehiculoElement.value.trim() : null;
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

            // Convertir el valor del input a mayúsculas antes de obtenerlo
            txPlaca.value = txPlaca.value.toUpperCase();
            var placa = txPlaca.value.trim();  // Ahora sí se obtiene en mayúsculas

            var isValid = false;

            // Validación de placa
            if (placa) {
                if (tipoVehiculo === "1") { // Carro
                    isValid = /^[A-Z]{3}[0-9]{3}$/.test(placa);
                } else if (tipoVehiculo === "2") { // Moto
                    isValid = /^[A-Z]{3}[0-9]{2}[A-Z]{1}$/.test(placa);
                } else {
                    console.warn("Tipo de vehículo no reconocido:", tipoVehiculo);
                }
            } else {
                isValid = true; // Permitir que el campo esté vacío
            }

            // Habilitar o deshabilitar el botón basado en la validación
            btGuardar.disabled = !isValid;

            // Mostrar u ocultar el mensaje de error
            if (isValid) {
                lblPlacaError.style.display = 'none';
            } else {
                lblPlacaError.style.display = 'block';
                lblPlacaError.innerText = "Placa inválida. " + getValidationMessage(tipoVehiculo);
            }
        }

        function getValidationMessage(tipoVehiculo) {
            if (tipoVehiculo === "1") { // Carro
                return "Debe ser 3 letras y 3 números (ej: ABC123)";
            } else if (tipoVehiculo === "2") { // Moto
                return "Debe ser 3 letras, 2 números y 1 letra (ej: ABC12D)";
            } else {
                return "";
            }
        }

        function updatePlacaState() {
            var tipoVehiculo = document.getElementById('<%= TxTipoVehiculo.ClientID %>').value;
            var btGuardar = document.getElementById('<%= BtGuardarVehiculo.ClientID %>');
            var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

            if (tipoVehiculo === "1" || tipoVehiculo === "2") { // Carro o Moto
                txPlaca.disabled = false; // Habilitar
                // Si ya hay una placa ingresada, validarla al cambiar el tipo de vehículo
                if (txPlaca.value.trim() !== "") {
                    handleInput(); // Llamar a la validación de la placa
                }
            } else {
                txPlaca.value = ""; // Vaciar
                lblPlacaError.style.display = 'none';
            }
        }

    </script>


</asp:Content>




