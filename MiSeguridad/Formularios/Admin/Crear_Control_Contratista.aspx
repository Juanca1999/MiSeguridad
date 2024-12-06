<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Control_Contratista.aspx.vb" Inherits="MiSeguridad.Crear_Control_Contratista" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR CONTROL CONTRATISTA</span>
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
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR CONTROL CONTRATISTA</asp:LinkButton>
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
                                            <label class="cd-label" for="TxBuscarPersona">Buscar Quien Autoriza</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                            <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Quien Autoriza" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombrePersona">Nombre Persona</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">person</i>
                                            <asp:DropDownList ID="TxNombrePersona" runat="server" Required="1" Style="height: 44px; padding: 12px; padding-left: 40px !important;">
                                                <asp:ListItem Value="">Nombre Persona</asp:ListItem>
                                            </asp:DropDownList>
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

                                    <div class="col-md-6 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCedulaContratista">Cedula Contratista</label>
                                            <asp:TextBox ID="TxCedulaContratista" runat="server" TextMode="Number" Required="1" placeholder="Cedula Contratista" CssClass="Cedula" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreContratista">Nombre Contratista</label>
                                            <asp:TextBox ID="TxNombreContratista" runat="server" Required="1" placeholder="Nombre Contratista" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxFechaInicio">Fecha Inicio</label>
                                            <asp:TextBox ID="TxFechaInicio" runat="server" Required="1" TextMode="Date" placeholder="Fecha Inicio" CssClass="Fecha"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxFechaFin">Fecha Fin</label>
                                            <asp:TextBox ID="TxFechaFin" runat="server" Required="1" TextMode="Date" placeholder="Fecha Fin" CssClass="Fecha"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 0px;">
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

                        <div class="col-md-12 mb-3">
                            <label class="cd-label" for="FLARL" style="margin-left: 41px; margin-top: 10px;">ARL</label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">attach_file</i>
                            <asp:FileUpload ID="FLARL" runat="server" class="btn btn-outline-primary" Width="100%" />
                        </div>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 15px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                <label class="cd-label" for="TxFechaARL">Fecha ARL</label>
                                <asp:TextBox ID="TxFechaARL" runat="server" TextMode="Date" CssClass="Fecha"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <label class="cd-label" for="FLSeguridadSocial" style="margin-left: 41px; margin-top: 10px;">Seguridad Social</label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">attach_file</i>
                            <asp:FileUpload ID="FLSeguridadSocial" runat="server" class="btn btn-outline-primary" Width="100%" />
                        </div>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 15px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                <label class="cd-label" for="TxFechaSeguridadSocial">Fecha Seguridad Social</label>
                                <asp:TextBox ID="TxFechaSeguridadSocial" runat="server" TextMode="Date" CssClass="Fecha"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <label class="cd-label" for="FLTrabajoAlturas" style="margin-left: 41px; margin-top: 10px;">Trabajo En Alturas</label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">attach_file</i>
                            <asp:FileUpload ID="FLTrabajoAlturas" runat="server" class="btn btn-outline-primary" Width="100%" />
                        </div>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 15px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                <label class="cd-label" for="TxFechaTrabajoAlturas">Fecha Trabajo Alturas</label>
                                <asp:TextBox ID="TxFechaTrabajoAlturas" runat="server" TextMode="Date" CssClass="Fecha"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <asp:Button ID="BtGuardar" runat="server" Text="GUARDAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                            <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
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
                                                <i class="material-icons" style="margin-right: 5px;">apartment</i> CONTROL CONTRATISTA
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                                <div class="card-body table-responsive">

                                    <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvControlContratista" runat="server" DataSourceID="SqlControlContratista">

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
                                                                <asp:Label ID="ContratistaLabel" runat="server" Text='<%# Eval("Contratista") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Id_InmuebleLabel" runat="server" Text='<%# Eval("Id_Inmueble") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_InicioLabel" runat="server" Text='<%# Eval("Fecha_Inicio") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_FinLabel" runat="server" Text='<%# Eval("Fecha_Fin") %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%; font-size: 8pt">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%" class="table table-hover table-fixed border">
                                                                        <tr runat="server" style="" class="card-header card-header-primary text-center content-center">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 35%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CONBTRATISTA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">INMUEBLE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA INICIO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA FIN</th>
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


                                                <asp:SqlDataSource ID="SqlControlContratista" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = C.Cedula_Contratista ORDER BY C.Id_Control_Contratistas DESC"></asp:SqlDataSource>

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




