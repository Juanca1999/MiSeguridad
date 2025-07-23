<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Control_Contratista.aspx.vb" Inherits="MiSeguridad.Crear_Control_Contratista" %>

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
                                            <label class="cd-label" for="TxTipoRegistro">Tipo De Registro</label>
                                            <asp:DropDownList ID="TxTipoRegistro" runat="server" Required="1" CssClass="tipo" AppendDataBoundItems="true">
                                                <asp:ListItem Value="">Tipo De Registro</asp:ListItem>
                                                <asp:ListItem Value="1">MANUAL</asp:ListItem>
                                                <asp:ListItem Value="0">FACIAL</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

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

                                    <div class="col-md-5 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCedulaContratista">Cedula Contratista</label>
                                            <asp:TextBox ID="TxCedulaContratista" runat="server" TextMode="Number" Required="1" placeholder="Cedula Contratista" CssClass="Cedula" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-7 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreContratista">Nombre Contratista</label>
                                            <asp:TextBox ID="TxNombreContratista" runat="server" Required="1" placeholder="Nombre Contratista" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCargo">Cargo Contratista</label>
                                            <asp:TextBox ID="TxCargo" runat="server" placeholder="Cargo Contratista" CssClass="Nombre_Usuario"></asp:TextBox>
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
                                        <asp:ImageButton ID="BtnAgregarEmpresa" UseSubmitBehavior="false" runat="server" ImageUrl="/Diseno/Imagenes/Mas.png" Width="35px" data-toggle="modal" data-target="#Crear_Empresa" />
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

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxObservacion">Observaciones</label>
                                            <asp:TextBox ID="TxObservacion" runat="server" class="des" TextMode="MultiLine" Height="130px" placeholder="Observaciones"></asp:TextBox>
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
                                <label class="cd-label" for="TxFechaARL">Fecha Vencimiento ARL</label>
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
                                <label class="cd-label" for="TxFechaSeguridadSocial">Fecha Vencimiento Seguridad Social</label>
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
                                <label class="cd-label" for="TxFechaTrabajoAlturas">Fecha Vencimiento Trabajo Alturas</label>
                                <asp:TextBox ID="TxFechaTrabajoAlturas" runat="server" TextMode="Date" CssClass="Fecha"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 15px;">
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
                                                <i class="material-icons" style="margin-right: 5px;">apartment</i> CONTROL CONTRATISTA
                                            </a>
                                        </li>
                                    </ul>
                                    <p class="card-category" style="text-align: right">
                                        <i class="material-icons" style="color: white; position: absolute; margin-top: 2px; margin-left: -25px;">search</i>
                                        <asp:TextBox ID="TxBuscar" runat="server" placeholder="Buscar Contratista" AutoPostBack="true" Width="30%" Font-Size="11pt" BackColor="White"></asp:TextBox>
                                    </p>
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
                                                            <asp:Label ID="Id_Control_ContratistasLabel" runat="server" Text='<%# Eval("Id_Control_Contratistas") %>' Font-Size="0pt" />
                                                            <asp:Label ID="Cedula_ContratistaLabel" runat="server" Text='<%# Eval("Cedula_Contratista") %>' Font-Size="0pt" />
                                                            <asp:Label ID="Id_HikCamaraLabel" runat="server" Text='<%# Eval("Id_HikCamara") %>' Font-Size="0pt" />
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Editar" runat="server" CommandName="Editar"><i class="material-icons" style="color: #1e2833;">edit</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="ContratistaLabel" runat="server" Text='<%# Eval("Contratista") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_InicioLabel" runat="server" Text='<%# Eval("Fecha_Inicio", "{0:yyyy-MM-dd}") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_FinLabel" runat="server" Text='<%# Eval("Fecha_Fin", "{0:yyyy-MM-dd}") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_ARLLabel" runat="server" Text='<%# Eval("Fecha_ARL", "{0:yyyy-MM-dd}") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_Seguridad_SocialLabel" runat="server" Text='<%# Eval("Fecha_Seguridad_Social", "{0:yyyy-MM-dd}") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Fecha_Trabajo_AlturasLabel" runat="server" Text='<%# Eval("Fecha_Trabajo_Alturas", "{0:yyyy-MM-dd}") %>' />
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
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 35%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CONTRATISTA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA INICIO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA FIN</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA ARL</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA SEGURIDAD SOCIAL</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">FECHA TRABAJO ALTURAS</th>
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


                                                <asp:SqlDataSource ID="SqlControlContratista" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.Fecha_ARL, C.Fecha_Seguridad_Social, C.Fecha_Trabajo_Alturas, TC.Id_HikCamara FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = CONVERT(NVARCHAR, C.Cedula_Contratista) ORDER BY C.Id_Control_Contratistas DESC"></asp:SqlDataSource>

                                                <asp:SqlDataSource ID="SqlBuscarControlContratista" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.Fecha_ARL, C.Fecha_Seguridad_Social, C.Fecha_Trabajo_Alturas, TC.Id_HikCamara FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = CONVERT(NVARCHAR, C.Cedula_Contratista) WHERE TC.Nombres LIKE '%' + @Contratista + '%' ORDER BY C.Id_Control_Contratistas DESC">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="TxBuscar" Name="Contratista" PropertyName="Text" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>

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
                        <asp:Button ID="BtCrearEmpresa" runat="server" Text="CREAR" UseSubmitBehavior="false" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" />
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

</asp:Content>




