<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Asignar_Per_Confianza.aspx.vb" Inherits="MiSeguridad.Asignar_Per_Confianza" %>

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
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>


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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - ASIGNAR PERSONAL CONFIANZA</span>
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
                        <div class="card" style="border: none; border-color: transparent; padding-top: 0px; margin-top: 10px; background: #1e2833;">
                            <div id="Div1" runat="server" class="card-header card-header-primary" style="width: 100%; margin: 0px; padding: 5px">
                                <h4 style="margin-bottom: 7px; margin-top: 7px;">
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>ASIGNAR PERSONAL DE CONFIANZA</asp:LinkButton>
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
                                            <label class="cd-label" for="TxBuscarPersona">Buscar Persona</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                            <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Persona" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombrePersona">Nombre Persona</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">person</i>
                                            <asp:DropDownList ID="TxNombrePersona" runat="server" Style="height: 44px; padding: 12px; padding-left: 40px !important;">
                                                <asp:ListItem Value="">Nombre Persona</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxBuscarInmueble">Buscar Inmueble</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                            <asp:TextBox ID="TxBuscarInmueble" runat="server" placeholder="Buscar Inmueble" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxInmueble">Inmueble</label>
                                            <asp:DropDownList ID="TxInmueble" runat="server" CssClass="ciudad" Required="1" AppendDataBoundItems="true" DataSourceID="SqlInmueble" DataTextField="Id_inmueble" DataValueField="Id_inmueble">
                                                <asp:ListItem Value="">Inmueble</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlInmueble" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_inmueble FROM Adm_Inmueble ORDER BY Id_inmueble"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxParentesco">Parentesco</label>
                                            <asp:DropDownList ID="TxParentesco" runat="server" CssClass="Tipo_Persona" Required="1" AppendDataBoundItems="true" DataSourceID="SqlParentesco" DataTextField="Parentesco" DataValueField="Id_Parentesco">
                                                <asp:ListItem Value="">Parentesco</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlParentesco" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Parentesco, Parentesco FROM Adm_Parentesco ORDER BY Id_Parentesco"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div class="col-md-12 mb-3">
                            <asp:Button ID="BtGuardar" runat="server" Text="GUARDAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                            <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                        </div>

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
                                        <div id="Encavezado_Dependencia" runat="server" class="card-header card-header-primary" style="background: #1e2833;">
                                            <ul class="nav nav-tabs md-tabs" id="myTabEx" role="tablist">
                                                <li class="nav-item" style="margin-bottom: 10px;">
                                                    <a style="font-size: 11pt; color: white;"><i class="material-icons">person</i>PERSONAL CONFIANZA</a>
                                                </li>
                                            </ul>
                                            <p class="card-category" style="text-align: right">
                                                <i class="material-icons">search</i>
                                                <asp:TextBox ID="TxBuscar" runat="server" placeholder="Buscar Persona" AutoPostBack="true" Width="30%" Font-Size="11pt" BackColor="White"></asp:TextBox>
                                            </p>
                                        </div>
                                        <div class="card-body table-responsive" id="Totales_Dependencia" runat="server">

                                            <div class="col-md-12 mb-3 scrollbar" id="PanelCss" style="height: 500px; padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">


                                                <asp:ListView ID="LvPersonalConfianza" runat="server" DataSourceID="SqlPersonalConfianza">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se han devuelto datos.</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' />
                                                                <br />
                                                                <asp:Label ID="Id_terceroLabel" runat="server" Text='<%# Eval("Id_tercero") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="ParentescoLabel" runat="server" Text='<%# Eval("Parentesco") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Id_inmuebleLabel" runat="server" Text='<%# Eval("Id_inmueble") %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%; font-size: 8pt">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%" class="table table-hover table-fixed border">
                                                                        <tr runat="server" style="" class="card-header card-header-primary text-center content-center">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 40px">PERSONA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 40px">PARENTESCO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 40px">INMUEBLE</th>
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


                                                <asp:SqlDataSource ID="SqlPersonalConfianza" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT IPC.Id_tercero, T.Nombres, P.Parentesco, IPC.Id_inmueble, IPC.Estado FROM Adm_Inmueble_Per_Confianza IPC LEFT JOIN Terceros T ON T.Id_Tercero = IPC.Id_tercero LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IPC.Id_inmueble LEFT JOIN Adm_Parentesco P ON P.Id_Parentesco = IPC.Id_Parentesco ORDER BY IPC.Id_inmueble ASC"></asp:SqlDataSource>

                                                <asp:SqlDataSource ID="Sql_Buscar_Persona" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT IPC.Id_tercero, T.Nombres, P.Parentesco, IPC.Id_inmueble, IPC.Estado FROM Adm_Inmueble_Per_Confianza IPC LEFT JOIN Terceros T ON T.Id_Tercero = IPC.Id_tercero LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IPC.Id_inmueble LEFT JOIN Adm_Parentesco P ON P.Id_Parentesco = IPC.Id_Parentesco WHERE T.Nombres LIKE '%' + @Nombre + '%' ORDER BY IPC.Id_inmueble ASC">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="TxBuscar" Name="Nombre" PropertyName="Text" />
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

        <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
</asp:Content>




