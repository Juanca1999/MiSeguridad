<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Empresa.aspx.vb" Inherits="MiSeguridad.Crear_Empresa" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR INMUEBLE</span>
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
            <button id="BtCrearEmpresa" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                CREAR EMPRESA
            </button>

            <button id="BtCrearSedes" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                CREAR SEDES
            </button>

            <button id="BtCrearAccesos" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                CREAR ACCESOS
            </button>

        </div>

        <div class="col-md-5 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px; padding-right: 0pt;">

            <div class="cd-form" style="padding-top: 5px; margin-top: 0px">
                <div class="content" style="padding-top: 0px; margin-top: 0px">
                    <div class="container-fluid" style="padding-top: 0px; margin-top: 0px">
                        <div class="card" style="border: none; border-color: transparent; padding-top: 0px; margin-top: -6px; background: #1e2833;">
                            <div class="card-header card-header-primary" style="width: 100%; margin: 0px; padding: 5px">
                                <h4 style="margin-bottom: 14px; margin-top: 14px;">
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR EMPRESA</asp:LinkButton>
                                </h4>
                                <p class="card-category">
                                </p>
                            </div>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="row" style="padding-top: 0px; margin-top: 0px;">

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNitEmpresa">Nit Empresa</label>
                                            <asp:TextBox ID="TxNitEmpresa" runat="server" Required="1" placeholder="Nit Empresa" CssClass="ciudad" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreEmpresa">Nombre Empresa</label>
                                            <asp:TextBox ID="TxNombreEmpresa" runat="server" Required="1" placeholder="Nombre Empresa" CssClass="ciudad"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTituloPersonal">Titulo Personal</label>
                                            <asp:TextBox ID="TxTituloPersonal" runat="server" Required="1" placeholder="Titulo Personal" CssClass="Area"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxDireccion">Direccion</label>
                                            <asp:TextBox ID="TxDireccion" runat="server" Required="1" placeholder="Direccion" CssClass="Direccion"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTelefono">Telefono</label>
                                            <asp:TextBox ID="TxTelefono" runat="server" Required="1" TextMode="Number" placeholder="Telefono" CssClass="Telefono"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxAdministrador">Administrador</label>
                                            <asp:TextBox ID="TxAdministrador" runat="server" Required="1" placeholder="Administrador" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTelefonoAdministrador">Telefono Administrador</label>
                                            <asp:TextBox ID="TxTelefonoAdministrador" runat="server" Required="1" TextMode="Number" placeholder="Telefono Administrador" CssClass="Telefono"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div class="col-md-12 mb-3">
                            <label class="cd-label" for="FLogo" style="margin-left: 41px; margin-top: 10px;">Logo</label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">image</i>
                            <asp:FileUpload ID="FLogo" runat="server" class="btn btn-outline-primary" Width="100%" />
                        </div>

                        <div class="col-md-12 mb-3">
                            <asp:Button ID="BtGuardar" runat="server" Text="GUARDAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                            <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                            <asp:Timer ID="Timer2" runat="server" Interval="999999999"></asp:Timer>
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
                                                <i class="material-icons" style="margin-right: 5px;">apartment</i> EMPRESA
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                                <div class="card-body table-responsive">

                                    <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvEmpresa" runat="server" DataSourceID="SqlEmpresa">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se han devuelto datos.</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Editar" runat="server" CommandName="Editar"><i class="material-icons" style="color: #1e2833;">edit</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Id_EmpresaLabel" runat="server" Text='<%# Eval("Id_Empresa") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="EmpresaLabel" runat="server" Text='<%# Eval("Empresa") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Administradorlabel" runat="server" Text='<%# Eval("Administrador") %>' />
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
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">NIT</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">NOMBRE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">ADMINISTRADOR</th>
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


                                                <asp:SqlDataSource ID="SqlEmpresa" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Empresa, Empresa, Direccion, Telefono, Logo, Administrador, Tel_Administrador FROM Adm_Empresa"></asp:SqlDataSource>

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

    </div>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>

</asp:Content>




