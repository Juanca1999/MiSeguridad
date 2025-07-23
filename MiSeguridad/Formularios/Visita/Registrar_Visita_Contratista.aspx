<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Registrar_Visita_Contratista.aspx.vb" Inherits="MiSeguridad.Registrar_Visita_Contratista" %>

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

        .alerta-vencimiento {
            width: 600px !important;
            max-width: 80% !important;
            background-color: #fff8e1 !important;
            border: 2px solid #f39c12 !important;
            font-family: 'Arial' !important;
            height: 400px !important;
            font-size: 14pt !important;
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

        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 25px; margin-bottom: 0px; margin-top: 0px; padding-right: 0pt; text-align: center">
            <button id="BtEntrada" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                INGRESO VISITANTES
            </button>

            <button id="BtVisitaProveedor" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                INGRESO PROVEEDOR
            </button>

            <button id="BtEntradaContratista" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                INGRESO CONTRATISTA
            </button>

            <button id="BtSalidaContratista" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                SALIDA CONTRATISTA
            </button>

            <button id="BtSalida" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR SALIDA
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
                                                <asp:Label ID="Cedula_ContratistaLabel" runat="server" Text='<%# Eval("Cedula_Contratista") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="ContratistaLabel" runat="server" Text='<%# Eval("Contratista") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="EmpresaLabel" runat="server" Text='<%# Eval("Empresa") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Quien_AutorizaLabel" runat="server" Text='<%# Eval("Quien_Autoriza") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Fecha_InicioLabel" runat="server" Text='<%# Eval("Fecha_Inicio", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="Fecha_FinLabel" runat="server" Text='<%# Eval("Fecha_Fin", "{0:yyyy-MM-dd}") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 5%; text-align: center; vertical-align: middle; border: thin solid darkgray; padding: 5px;">
                                                <a href="#" id="ARL" runat="server" title='<%# Eval("Fecha_ARL", "{0:yyyy-MM-dd}") %>' onclick='<%# "Mostrar_Documento_Modal(""" & Eval("ARL") & """); return false;" %>'>
                                                    <i class="material-icons" style="color: blue">description</i>
                                                </a>
                                            </td>
                                            <td style="width: 5%; text-align: center; vertical-align: middle; border: thin solid darkgray; padding: 5px;">
                                                <a href="#" id="Seguridad_Social" runat="server" title='<%# Eval("Fecha_Seguridad_Social", "{0:yyyy-MM-dd}") %>' onclick='<%# "Mostrar_Documento_Modal(""" & Eval("Seguridad_Social") & """); return false;" %>'>
                                                    <i class="material-icons" style="color: blue">description</i>
                                                </a>
                                            </td>
                                            <td style="width: 5%; text-align: center; vertical-align: middle; border: thin solid darkgray; padding: 5px;">
                                                <a href="#" id="Trabajo_Alturas" runat="server" title='<%# Eval("Fecha_Trabajo_Alturas", "{0:yyyy-MM-dd}") %>' onclick='<%# "Mostrar_Documento_Modal(""" & Eval("Trabajo_Alturas") & """); return false;" %>'>
                                                    <i class="material-icons" style="color: blue">description</i>
                                                </a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>

                                    <LayoutTemplate>
                                        <table runat="server" style="width: 100%">
                                            <tr runat="server">
                                                <td runat="server">
                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                        <tr runat="server">
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CEDULA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CONBTRATISTA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">EMPRESA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">QUIEN AUTORIZA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA INICIO</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">FECHA FIN</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CURSO SST SURA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">SEGURIDAD SOCIAL</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">TRABAJO ALTURAS</th>
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

                                <asp:SqlDataSource ID="SqlContratistas" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT C.Id_Control_Contratistas, C.Id_Quien_Registra, C.Id_Quien_Autoriza, T.Nombres AS Quien_Autoriza, E.Empresa, C.Id_Inmueble, C.Cedula_Contratista, TC.Nombres AS Contratista, C.Fecha_Inicio, C.Fecha_Fin, C.Lunes, C.Martes, C.Miercoles, C.Jueves, C.Viernes, C.Sabado, C.Domingo, C.Festivo, C.ARL, C.Fecha_ARL, C.Seguridad_Social, C.Fecha_Seguridad_Social, C.Trabajo_Alturas, C.Fecha_Trabajo_Alturas FROM Adm_Control_Contratistas C LEFT JOIN Terceros T ON T.Id_Tercero = C.Id_Quien_Autoriza LEFT JOIN Terceros TC ON TC.Cedula = C.Cedula_Contratista LEFT JOIN Empresas E ON E.Id_Empresa = TC.Id_Empresa WHERE C.Cedula_Contratista = @Contratista ORDER BY C.Id_Control_Contratistas DESC">
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
                    <asp:Button ID="BtGuardar" runat="server" Text="REGISTRAR ENTRADA" Visible="false" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                </div>

            </div>

        </div>

        <!-- Modal Documento Detalle -->
        <div class="modal fade" id="Foto_Detalle" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">DOCUMENTO DETALLE</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body text-center">
                        <!-- Contenedor para imagen -->
                        <img id="Img_Detalle" class="img-fluid rounded" alt="Documento Detalle" style="display: none; max-width: 100%; height: auto;" />

                        <!-- Contenedor para PDF -->
                        <iframe id="Pdf_Detalle" style="display: none; width: 100%; height: 500px; border: none;"></iframe>
                    </div>
                </div>
            </div>
        </div>

    </div>


    <script src="/Diseno/jquery.min.js-2.js"></script>

    <script type="text/javascript">

        function Mostrar_Documento_Modal(nombreArchivo) {
            // Construir la URL completa del archivo
            var url = "/Adjunto/Documentos_Contratistas/" + nombreArchivo;

            // Obtener referencias a los elementos del modal
            var imgElement = document.getElementById('Img_Detalle');
            var pdfElement = document.getElementById('Pdf_Detalle');

            // Determinar la extensión del archivo
            var extension = nombreArchivo.split('.').pop().toLowerCase();

            if (["jpg", "jpeg", "png", "gif", "webp"].includes(extension)) {
                // Mostrar imagen
                imgElement.src = url;
                imgElement.style.display = "block";
                pdfElement.style.display = "none";
            } else if (extension === "pdf") {
                // Mostrar PDF
                pdfElement.src = url;
                pdfElement.style.display = "block";
                imgElement.style.display = "none";
            } else if (["xls", "xlsx", "csv"].includes(extension)) {
                // Si es Excel, abrir en nueva pestaña o descargar
                window.open(url, "_blank");
                return; // No abrir el modal
            } else {
                // Si el formato no es soportado, ocultar ambos
                imgElement.style.display = "none";
                pdfElement.style.display = "none";
            }

            // Abrir el modal solo si es imagen o PDF
            $('#Foto_Detalle').modal('show');
        }

    </script>

</asp:Content>




