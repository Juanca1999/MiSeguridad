<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Usuarios.aspx.vb" Inherits="MiSeguridad.Crear_Usuarios" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR USUARIOS</span>
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
                        <div class="card" style="border: none; border-color: transparent; padding-top: 0px; margin-top: -6px; background: #1e2833;">
                            <div class="card-header card-header-primary" style="width: 100%; margin: 0px; padding: 5px">
                                <h4 style="margin-bottom: 14px; margin-top: 14px;">
                                    <asp:LinkButton ID="LbNueva_Persona" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR USUARIOS</asp:LinkButton>
                                </h4>
                                <p class="card-category">
                                </p>
                            </div>
                        </div>

                        <%--FORMULARIO--%>
                        <asp:Panel ID="PUsuario" runat="server" Visible="True" Style="margin: 0px; padding: 0px;" Width="100%">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                <ContentTemplate>

                                    <div class="row" style="padding-top: 0px; margin-top: 0px;">

                                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                                <label class="cd-label" for="TxCedula">Cedula</label>
                                                <asp:TextBox ID="TxCedula" runat="server" placeholder="Cedula" CssClass="Cedula" AutoPostBack="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                                <label class="cd-label" for="TxNombres">Nombres</label>
                                                <asp:TextBox ID="TxNombres" runat="server" placeholder="Nombres" CssClass="Nombre_Usuario"></asp:TextBox>
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

                            <div class="col-md-12 mb-3">
                                <asp:Button ID="BtGuardar" runat="server" Text="GUARDAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                                <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                            </div>
                        </asp:Panel>
                        <%--ACTA--%>
                        <asp:Panel ID="PPermisos" runat="server" Visible="False" Style="margin: 0px; padding: 0px;" Width="100%">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <div class="col-md-12 mb-3 cd-form" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxModulos">Modulos</label>
                                            <asp:DropDownList ID="TxModulos" runat="server" AppendDataBoundItems="true" AutoPostBack="true" CssClass="tipo" DataSourceID="SqlModulos" DataTextField="Modulo" DataValueField="Id_Modulo">
                                                <asp:ListItem Value="">Modulos</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlModulos" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Modulo, Modulo FROM Sys_Modulos"></asp:SqlDataSource>

                                        </div>
                                    </div>

                                    <div class="row">

                                        <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <asp:ListView ID="LvFormularios" runat="server" DataSourceID="SqlPermisos" DataKeyNames="Id_Formulario">
                                                <ItemTemplate>
                                                    <asp:Label ID="Id_Formulario" runat="server" Text='<%# Eval("Id_Formulario") %>' Font-Size="0pt" />
                                                    <asp:Label ID="Submenu" runat="server" Text='<%# Eval("Submenu") %>' Font-Size="0pt" />
                                                    <asp:Button ID="Formulario" runat="server" Text='<%# Eval("Formulario") %>' CssClass="btn bg-primary" Width="100%" Style="white-space: normal;" />
                                                </ItemTemplate>
                                            </asp:ListView>
                                            <asp:SqlDataSource ID="SqlPermisos" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Formulario, Formulario, Id_Modulo, Submenu FROM Sys_Formularios WHERE (Id_Modulo = @Modulo)">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="TxModulos" Name="Modulo" PropertyName="SelectedValue" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </div>

                                        <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <asp:ListView ID="LvSubmenu" runat="server" DataSourceID="SqlSubMenu" DataKeyNames="Id_Submenu">
                                                <ItemTemplate>
                                                    <asp:Label ID="Id_Submenu" runat="server" Text='<%# Eval("Id_Submenu") %>' Font-Size="0pt" />
                                                    <asp:Label ID="Id_formulario" runat="server" Text='<%# Eval("Id_formulario") %>' Font-Size="0pt" />
                                                    <asp:Button ID="Submenu" runat="server" Text='<%# Eval("Submenu") %>' CssClass="btn bg-danger" Width="100%" Style="white-space: normal;" />
                                                </ItemTemplate>
                                            </asp:ListView>
                                            <asp:SqlDataSource ID="SqlSubMenu" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Submenu, Submenu, Id_formulario FROM Sys_Submenu WHERE (Id_formulario = @Formulario)">
                                                <SelectParameters>
                                                    <asp:SessionParameter Name="Formulario" SessionField="Id_Formulario" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>

                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-7 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="row" style="text-align: center; padding: 0px; margin: 0px">

                        <div class="col-lg-12 col-md-12">
                            <div class="content">
                                <div class="container-fluid">
                                    <div class="card">
                                        <div class="card-header card-header-primary" style="background: #1e2833;">
                                            <ul class="nav nav-tabs md-tabs" id="myTabEx" role="tablist">
                                                <li class="nav-item" style="margin-bottom: 10px; display: flex; align-items: center;">
                                                    <a style="font-size: 11pt; color: white; display: flex; align-items: center;">
                                                        <i class="material-icons" style="margin-right: 5px;">person</i>USUARIOS
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

                                                <asp:ListView ID="LvUsuarios" runat="server" DataSourceID="SqlUsuarios">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se han devuelto datos.</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <asp:Label ID="Id_TerceroLabel" runat="server" Text='<%# Eval("Id_Tercero") %>' Font-Size="0pt" />
                                                            <asp:Label ID="Estado" runat="server" Text='<%# Eval("Estado") %>' Font-Size="0pt" />
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="LibEditar" runat="server" CommandName="Editar" CssClass="text-primary"><i class="material-icons">create</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="LibPermisos" runat="server" CommandName="Permisos" CssClass="text-primary"><i class="material-icons">add_task</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="CedulaLabel" runat="server" Text='<%# Eval("Cedula") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="RolLabel" runat="server" Text='<%# Eval("Rol") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="CargoLabel" runat="server" Text='<%# Eval("Cargo") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="CelularLabel" runat="server" Text='<%# Eval("Celular") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Nombre_SedeLabel" runat="server" Text='<%# Eval("Nombre_Sede") %>' />
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
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons">add_task</i></th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CEDULA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">NOMBRES</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">ROL</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CARGO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CELULAR</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">SEDE</th>
                                                                        </tr>
                                                                        <tr id="itemPlaceholder" runat="server">
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr runat="server">
                                                                <td runat="server" style="">
                                                                    <asp:DataPager ID="DataPager1" runat="server" PageSize="20">
                                                                        <Fields>
                                                                            <asp:NumericPagerField />
                                                                        </Fields>
                                                                    </asp:DataPager>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </LayoutTemplate>

                                                </asp:ListView>


                                                <asp:SqlDataSource ID="SqlUsuarios" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT T.Id_Tercero, T.Cedula, T.Nombres, T.Id_Rol, R.Rol, T.Correo, T.Usuario, T.Password, T.Cargo, T.Celular, T.Id_Sede, S.Nombre_Sede, T.Estado FROM Terceros T LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Adm_Sedes S ON S.Id_Sede = T.Id_Sede ORDER BY T.Id_Tercero ASC"></asp:SqlDataSource>

                                                <asp:SqlDataSource ID="Sql_Buscar_Persona" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT T.Id_Tercero, T.Cedula, T.Nombres, T.Id_Rol, R.Rol, T.Correo, T.Usuario, T.Password, T.Cargo, T.Celular, T.Id_Sede, S.Nombre_Sede, T.Estado FROM Terceros T LEFT JOIN Adm_Rol R ON R.Id_Rol = T.Id_Rol LEFT JOIN Adm_Sedes S ON S.Id_Sede = T.Id_Sede WHERE T.Nombres LIKE '%' + @Persona + '%' ORDER BY T.Id_Tercero ASC">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="TxBuscar" Name="Persona" PropertyName="Text" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>


                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
    
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    </div>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
</asp:Content>




