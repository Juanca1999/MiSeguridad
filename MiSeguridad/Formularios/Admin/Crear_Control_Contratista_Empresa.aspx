<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Control_Contratista_Empresa.aspx.vb" Inherits="MiSeguridad.Crear_Control_Contratista_Empresa" %>

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
            width: 100%;
            max-width: 1100px;
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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">CREAR CONTRATISTA</span>
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

            <div class="cd-form" style="margin-top: 0px">
                <div class="content" style="padding-top: 0px; margin-top: 0px">
                    <div class="container-fluid" style="padding-top: 0px; margin-top: 0px">
                        <div class="card" style="border: none; border-color: transparent; padding-top: 0px; margin-top: -6px; background: #1e2833;">
                            <div class="card-header card-header-primary" style="width: 100%; margin: 0px; padding: 5px">
                                <h4 style="margin-bottom: 14px; margin-top: 14px;">
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR CONTRATISTA</asp:LinkButton>
                                </h4>
                                <p class="card-category">
                                </p>
                            </div>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="row" style="padding-top: 0px; margin-top: 0px;">

                                    <div class="col-md-4 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCedulaContratista">Cedula Contratista<span style="color: red;">*</span></label>
                                            <asp:TextBox ID="TxCedulaContratista" runat="server" TextMode="Number" Required="1" placeholder="Cedula Contratista" CssClass="Cedula" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-8 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreContratista">Nombre Contratista<span style="color: red;">*</span></label>
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
                                            <label class="cd-label" for="TxBuscarPersona">Buscar Quien Autoriza<span style="color: red;">*</span></label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                            <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Quien Autoriza" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombrePersona">Nombre Persona<span style="color: red;">*</span></label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">person</i>
                                            <asp:DropDownList ID="TxNombrePersona" runat="server" Required="1" Style="height: 44px; padding: 12px; padding-left: 40px !important;">
                                                <asp:ListItem Value="">Nombre Persona</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxBuscarEmpresa">Buscar Empresa</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                            <asp:TextBox ID="TxBuscarEmpresa" runat="server" placeholder="Buscar Empresa" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxEmpresa">Empresa</label>
                                            <asp:DropDownList ID="TxEmpresa" runat="server" CssClass="ciudad">
                                                <asp:ListItem Value="">Empresa</asp:ListItem>
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

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxFechaInicio">Fecha Inicio<span style="color: red;">*</span></label>
                                            <asp:TextBox ID="TxFechaInicio" runat="server" Required="1" TextMode="Date" placeholder="Fecha Inicio" CssClass="Fecha"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxFechaFin">Fecha Fin<span style="color: red;">*</span></label>
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
                            <label class="cd-label" for="FLARL" style="margin-left: 41px; margin-top: 10px;">Curso SST SURA<span style="color: red;">*</span></label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">attach_file</i>
                            <asp:FileUpload ID="FLARL" runat="server" class="btn btn-outline-primary" Required="1" Width="100%" />
                        </div>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 15px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                <label class="cd-label" for="TxFechaARL">Fecha Vencimiento Curso SST SURA<span style="color: red;">*</span></label>
                                <asp:TextBox ID="TxFechaARL" runat="server" TextMode="Date" Required="1" CssClass="Fecha"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-12 mb-3">
                            <label class="cd-label" for="FLSeguridadSocial" style="margin-left: 41px; margin-top: 10px;">Seguridad Social<span style="color: red;">*</span></label>
                            <i class="material-icons" style="margin-top: -33px; margin-left: 10px; position: absolute;">attach_file</i>
                            <asp:FileUpload ID="FLSeguridadSocial" runat="server" Required="1" class="btn btn-outline-primary" Width="100%" />
                        </div>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 15px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                <label class="cd-label" for="TxFechaSeguridadSocial">Fecha Vencimiento Seguridad Social<span style="color: red;">*</span></label>
                                <asp:TextBox ID="TxFechaSeguridadSocial" runat="server" Required="1" TextMode="Date" CssClass="Fecha"></asp:TextBox>
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

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                            <div style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                <asp:Button ID="btnAbrirModal" runat="server" Text="Leer Política de Tratamiento de Datos" CssClass="btn btn-secondary" data-toggle="modal" data-target="#modalPolitica" UseSubmitBehavior="false" OnClientClick="return false;" Style="color: white" />
                            </div>
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


                                                <asp:SqlDataSource ID="SqlControlContratista" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.Fecha_ARL, C.Fecha_Seguridad_Social, C.Fecha_Trabajo_Alturas, TC.Id_HikCamara FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = CONVERT(NVARCHAR, C.Cedula_Contratista) WHERE C.Id_Quien_Registra = @Usuario ORDER BY C.Id_Control_Contratistas DESC">
                                                    <SelectParameters>
                                                        <asp:SessionParameter Name="Usuario" SessionField="Usuario" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>

                                                <asp:SqlDataSource ID="SqlBuscarControlContratista" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.Fecha_ARL, C.Fecha_Seguridad_Social, C.Fecha_Trabajo_Alturas, TC.Id_HikCamara FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = CONVERT(NVARCHAR, C.Cedula_Contratista) WHERE C.Id_Quien_Registra = @Usuario AND TC.Nombres LIKE '%' + @Contratista + '%' ORDER BY C.Id_Control_Contratistas DESC">
                                                    <SelectParameters>
                                                        <asp:SessionParameter Name="Usuario" SessionField="Usuario" />
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

        <!-- Modal Política -->
        <div class="modal fade" id="modalPolitica" tabindex="-1" aria-labelledby="labelPolitica" aria-hidden="true">
            <div class="modal-dialog modal-dialog-scrollable modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title w-100 text-center" id="labelPolitica">
                            <strong>AUTORIZACIÓN DE TRATAMIENTO DE DATOS, DECLARACIÓN DE COMPROMISO Y VERACIDAD DE LA INFORMACIÓN.</strong>
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body" id="contenidoPolitica" style="max-height: 500px; overflow-y: auto; text-align: justify; padding: 20px; font-size: 14px;">
                        Con la firma en el presente documento, autorizo de manera previa y expresa a las empresas del <strong>GRUPO EMPRESARIAL AUTECO COLOMBIA</strong>, para que, cada una en calidad de Responsable puedan llevar a cabo la recolección, almacenamiento, uso, circulación y supresión de mis datos personales, de acuerdo con la regulación aplicable en Protección de Datos personales de Colombia y conforme a las políticas de tratamiento de datos personales del GRUPO EMPRESARIAL AUTECO COLOMBIA que se encuentra publicada en 
        <a href="https://www.auteco.com.co/manual-de-politicas-y-procedimientos-de-datos" target="_blank">https://www.auteco.com.co/manual-de-politicas-y-procedimientos-de-datos</a> o que pueden ser solicitadas en el correo: 
        <a href="mailto:servicioalcliente@auteco.com.co">servicioalcliente@auteco.com.co</a>.

        <br>
                        <br>
                        Asimismo, me fue informado cada una de las empresas que conforman el GRUPO EMPRESARIAL AUTECO COLOMBIA como Responsables, que se identifican en los siguientes términos:
        <br>
                        i) Auteco S.A.S identificada con NIT 890.900.317-0 domiciliada en Itagüí,
        <br>
                        ii) Auteco Mobility S.A.S identificada con NIT 901.249.413-7 domiciliada en Envigado,
        <br>
                        iii) Synergix S.A.S. identificada con NIT 901.259.188-7 domiciliada en Itagüí.
        <br>
                        En concreto, autorizo a que mis datos personales sean tratados para las siguientes finalidades:

        <br>
                        <br>
                        (i) Soliciten, almacenen, consulten, compartan, informen, reporten, rectifiquen, procesen, modifiquen, actualicen, aclaren, retiren o divulguen mi información ante cualquier entidad o central de información, de riesgos o administradora de bases de datos legalmente constituida en el país, sea pública o privada, nacional o extranjera, cualquier información referente al comportamiento crediticio, financiero, tributario, contable, contractual y de servicios, incluso cuando haya finalizado la relación durante el tiempo permitido por la Ley;
        <br>
                        (ii) Para que compartan, divulguen, transfieran, transmitan y actualicen mi información con proveedores, aliados o entidades con las que el GRUPO EMPRESARIAL AUTECO COLOMBIA celebren ahora o en el futuro contratos para el desarrollo del objeto social;
        <br>
                        (iii) Para el cumplimiento de las obligaciones legales que involucren datos personales;
        <br>
                        (iv) Para generar encuestas y reportes de satisfacción en lo concerniente a los servicios prestados;
        <br>
                        (v) Para ofrecer y gestionar productos y servicios, incluyendo la gestión de cobros, a través de cualquier medio o canal;
        <br>
                        (vi) Para el desarrollo de la relación comercial, contractual o negocial entre las partes;
        <br>
                        (vii) Para gestionar y llevar a cabo la atención y seguimiento de las Solicitudes Peticiones Quejas y Reclamos de todo tipo en los canales de atención de la compañía;
        <br>
                        (viii) Para llevar a cabo las actividades que permitan el proceso de acceso biométrico al nuevo sistema del GRUPO EMPRESARIAL AUTECO COLOMBIA;
        <br>
                        (ix) Para llevar a cabo el ingreso;
        <br>
                        (x) Para realizar contacto directo o por medio de cualquier tercero debidamente acreditado por el GRUPO EMPRESARIAL AUTECO COLOMBIA con fines comerciales, promocionales, información de productos de interés, invitación a eventos comerciales, de mercadeo y de servicios venta;
        <br>
                        (xi) Para el envío de información comercial, educativa, de importancia para la industria o el sector y de servicio;
        <br>
                        (xii) Para la actualización de los datos y para las demás finalidades detalladas en la política de tratamiento de datos.

        <br>
                        <br>
                        En mi calidad de Titular tengo derecho a:
        <br>
                        (i) conocer, actualizar y rectificar mis datos personales,
        <br>
                        (ii) solicitar prueba de la autorización otorgada,
        <br>
                        (iii) Recibir información sobre el tratamiento de mis datos personales,
        <br>
                        (iv) Presentar quejas ante la Superintendencia de Industria y Comercio (SIC),
        <br>
                        (v) Revocar la autorización otorgada y/o solicitar la supresión de mis datos personales en los casos en que sea procedente,
        <br>
                        (vi) Conocer el carácter facultativo de mis respuestas a preguntas sobre datos sensibles o de menores de edad,
        <br>
                        (vii) Negarme a entregar o autorizar el tratamiento de datos sensibles y
        <br>
                        (viii) Acceder de forma gratuita a mis datos personales que hayan sido objeto de tratamiento.

        <br>
                        <br>
                        Reconozco que fui informado que, para el tratamiento de mis datos sensibles:
        <br>
                        (i) no estoy obligado a dar mi Autorización o consentimiento,
        <br>
                        (ii) me fue informado de manera previa y explicita los datos sensibles solicitados frente al acceso biométrico y
        <br>
                        (iii) se me comunicó el tratamiento y finalidades que se le darán a estos datos sensibles.

        <br>
                        <br>
                        Esta autorización se mantendrá vigente mientras subsista las finalidades para el tratamiento de mis datos. Asimismo, podré ejercer mis derechos en <a href="mailto:servicioalcliente@auteco.com.co">servicioalcliente@auteco.com.co</a>.

        <br>
                        <br>
                        En esta misma línea, manifiesto de manera expresa mi compromiso y veracidad en relación con la información y documentación suministrada para el proceso de acceso biométrico a la compañía. Declaro que:
        <br>
                        • He verificado toda la información registrada y la documentación cargada en el formulario correspondiente.
        <br>
                        • La documentación proporcionada se encuentra vigente, es veraz y cumple con las fechas y requisitos establecidos por la compañía.
        <br>
                        • Entiendo que la información suministrada será utilizada exclusivamente para gestionar mi acceso biométrico.
        <br>
                        • Soy consciente que cualquier inconsistencia de la información suministrada, podrá acarrear consecuencias legales o contractuales según el caso.

        <br>
                        <br>
                        Esta declaración es emitida con plena conciencia de su alcance y responsabilidad legal.

                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="BtAceptarPolitica" runat="server" Text="ACEPTO" UseSubmitBehavior="false" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" OnClientClick="aceptarPolitica(); return false;" Style="background-color: #1e2833; border-color: #1e2833;" />
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

        document.addEventListener("DOMContentLoaded", function () {
            var modalBody = document.getElementById("contenidoPolitica"); // Asegúrate que tenga este ID
            var btnAceptar = document.getElementById("<%= BtAceptarPolitica.ClientID %>");
             var btnAbrirModal = document.getElementById("<%= btnAbrirModal.ClientID %>");
             var btnGuardar = document.getElementById("<%= BtGuardar.ClientID %>");

            // Al iniciar, desactivar botones aceptar y guardar
            btnAceptar.disabled = true;

            // Detectar scroll hasta el final del modal
            modalBody.addEventListener("scroll", function () {
                if (modalBody.scrollTop + modalBody.clientHeight >= modalBody.scrollHeight - 5) {
                    btnAceptar.disabled = false;
                }
            });
        });

        function aceptarPolitica() {
            // Cambiar la clase del botón que abre el modal
            document.getElementById("<%= btnAbrirModal.ClientID %>").className = "btn btn-success";

            // Habilitar el botón de guardar
            document.getElementById("<%= BtGuardar.ClientID %>").disabled = false;

            $('#modalPolitica').modal('hide');
        }
    </script>

</asp:Content>




