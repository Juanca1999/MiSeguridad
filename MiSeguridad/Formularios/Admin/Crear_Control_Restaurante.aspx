<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Control_Restaurante.aspx.vb" Inherits="MiSeguridad.Crear_Control_Restaurante" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR CONTROL RESTAURANTE</span>
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
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR CONTROL RESTAURANTE</asp:LinkButton>
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
                                            <label class="cd-label" for="TxCedula">Cedula</label>
                                            <asp:TextBox ID="TxCedula" runat="server" placeholder="Cedula" CssClass="Cedula" AutoPostBack="true" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombres">Nombres</label>
                                            <asp:TextBox ID="TxNombres" runat="server" placeholder="Nombres" CssClass="Nombre_Usuario" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCorreo">Correo</label>
                                            <asp:TextBox ID="TxCorreo" runat="server" placeholder="Correo" TextMode="Email" CssClass="email"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCelular">Celular</label>
                                            <asp:TextBox ID="TxCelular" runat="server" placeholder="Celular" TextMode="Number" CssClass="Telefono"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxFechaActivacion">Fecha Activacion</label>
                                            <asp:TextBox ID="TxFechaActivacion" runat="server" Required="1" TextMode="Date" placeholder="Fecha Activacion" CssClass="Fecha"></asp:TextBox>
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

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCantidadAccesos">Cantidad Accesos</label>
                                            <asp:TextBox ID="TxCantidadAccesos" runat="server" TextMode="Number" placeholder="Cantidad Accesos" CssClass="Tag" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNumeroFactura">Area o Numero Factura</label>
                                            <asp:TextBox ID="TxNumeroFactura" runat="server" placeholder="Area o Numero Factura" CssClass="Tipo_Novedad" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3">
                                        <asp:Button ID="BtAbrirQR" runat="server" Text="GUARDAR" CssClass="btn btn-primary" OnClientClick="return validarFormulario();" Style="background-color: #1e2833; border-color: #1e2833;" />
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

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
                                                <i class="material-icons" style="margin-right: 5px;">restaurant</i> CONTROL RESTAURANTE
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                                <div class="card-body table-responsive">

                                    <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvControlRestaurante" runat="server" DataSourceID="SqlControlRestaurante">

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
                                                                <asp:Label ID="CedulaLabel" runat="server" Text='<%# Eval("Cedula") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="CorreoLabel" runat="server" Text='<%# Eval("Correo") %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <LayoutTemplate>
                                                        <table runat="server" style="width: 100%; font-size: 8pt">
                                                            <tr runat="server">
                                                                <td runat="server">
                                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%" class="table table-hover table-fixed border">
                                                                        <tr runat="server" style="" class="card-header card-header-primary text-center content-center">
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 15%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CEDULA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">NOMBRE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">CORREO</th>
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


                                                <asp:SqlDataSource ID="SqlControlRestaurante" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT DISTINCT T.Cedula, T.Nombres, T.Correo FROM Terceros T INNER JOIN Adm_Control_Restaurante CR ON CR.Cedula = T.Cedula"></asp:SqlDataSource>

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

        <!-- Modal Foto QR -->
        <div class="modal fade" id="Modal_QR" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center" data-backdrop="static" data-keyboard="false">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" id="MensajeModalLabel" style="text-align: center">FOTO QR</h5>
                    </div>
                    <div class="modal-body text-center">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <img id="Foto_QR" runat="server" class="img-fluid rounded" alt="Foto QR" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer d-flex">
                        <asp:Button ID="BtImprimir" runat="server" Text="IMPRIMIR" class="btn btn-primary" Font-Size="11pt" OnClientClick="imprimirImagen(); return false;" />
                        <asp:Button ID="BtCopiar" runat="server" Text="COPIAR" class="btn btn-primary" Font-Size="11pt" OnClientClick="copiarImagen(); return false;" />
                        <div class="ml-auto">
                            <asp:Button ID="BtGuardar" runat="server" Text="GUARDAR" class="btn btn-success" Font-Size="11pt" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>

    <script>
        // Función para imprimir la imagen
        function imprimirImagen() {
            const img = document.getElementById('<%= Foto_QR.ClientID %>');
            const nombreVisitante = document.getElementById('<%= TxNombres.ClientID %>').value;
            const fechaActivacion = document.getElementById('<%= TxFechaActivacion.ClientID %>').value;
            const Autoriza = document.getElementById('<%= TxNombrePersona.ClientID %>').selectedOptions[0].text;
            const cantidadAccesos = document.getElementById('<%= TxCantidadAccesos.ClientID %>').value;

            if (img && img.src) {

                // Formatear la fecha a formato "01/ENE/2025"
                const fecha = new Date(fechaActivacion + 'T00:00:00');  // Agregar hora para interpretación local
                const meses = ["ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC"];
                const mes = meses[fecha.getMonth()];  // Obtener el mes en español
                const fechaFormateada = `${fecha.getDate().toString().padStart(2, '0')}/${mes}/${fecha.getFullYear()}`;

                // Abrir una nueva ventana o pestaña
                const ventana = window.open('', '_blank');

                // Crear el contenido para impresión
                const contenidoHTML = `
            <!DOCTYPE html>
<html>
<head>
    <title>Imprimir QR</title>
    <style>
        /* Estilo general para el cuerpo de la página */
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background-color: #fff;
            padding: 0;
            height: 100vh;
            display: block;
        }
        /* Contenedor principal que ocupa toda la hoja */
        .hoja {
            width: 80mm; /* Ancho del papel */
            height: 297mm; /* Alto del papel */
            padding: 10mm; /* Margen interno */
            box-sizing: border-box;
            margin: 0 auto; /* Centrar el contenido en la página */
        }
        /* Cabecera de la hoja */
        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }
        .header img {
            width: 50mm; /* Ajustar el tamaño del logo */
        }
        /* Estilo para los detalles del servicio */
        .details {
            margin-bottom: 20px;
        }
        .details span {
            display: block;
            font-size: 12px; /* Reducir el tamaño de la fuente */
            margin-bottom: 5px;
        }
        /* Contenedor para la imagen QR */
        .qr-image {
            text-align: center;
            margin: 20px 0;
        }
        .qr-image img {
            width: 50mm; /* Ajustar el tamaño del QR */
            height: 50mm;
        }
        /* Pie de página de la hoja */
        .footer {
            margin-top: 20px;
            text-align: left;
            font-size: 12px; /* Reducir el tamaño de la fuente */
        }
    </style>
</head>
<body>
    <div class="hoja">
        <!-- Cabecera con nombre del visitante y logo -->
        <div class="header">
            <div>
                <span><strong>NOMBRE VISITANTE:</strong><br />
                ${nombreVisitante}</span>
            </div>
            <img src="https://www.clubcampestre.co/wp-content/uploads/2024/04/club-campestre-logo.png" alt="Logo">
        </div>
        <!-- Detalles del servicio -->
        <div class="details">
            <span><strong>SERVICIO VÁLIDO:</strong> ${fechaFormateada}</span>
            <span><strong>CANTIDAD DE ACCESOS:</strong> ${cantidadAccesos}</span>
        </div>
        <!-- Imagen QR centrada -->
        <div class="qr-image">
            <img src="${img.src}" alt="QR" />
        </div>
        <!-- Pie de página con autorización -->
        <div class="footer">
            <strong>AUTORIZADO POR:</strong> ${Autoriza}
        </div>
    </div>
</body>
</html>
        `;

                // Escribir el contenido en la nueva ventana
                ventana.document.write(contenidoHTML);
                ventana.document.close();

                // Esperar a que cargue la imagen antes de imprimir
                ventana.onload = function () {
                    ventana.focus();
                    ventana.print();
                    ventana.close();
                };
            } else {
                alert('No se encontró la imagen para imprimir.');
            }
        }

        async function copiarImagen() {
            const img = document.getElementById('<%= Foto_QR.ClientID %>');
            if (img && img.src) {
                try {
                    const response = await fetch(img.src);
                    const blob = await response.blob();
                    const data = [new ClipboardItem({ [blob.type]: blob })];
                    await navigator.clipboard.write(data);
                    alert('Imagen copiada al portapapeles.');
                } catch (error) {
                    console.error('Error al copiar la imagen:', error);
                    alert('No se pudo copiar la imagen. Verifica si tu navegador soporta esta funcionalidad.');
                }
            } else {
                alert('No se encontró la imagen para copiar.');
            }
        }

        function validarFormulario() {
            // Selecciona el formulario
            const formulario = document.forms[0]; // Cambia si usas múltiples formularios

            // Verifica si es válido
            if (formulario.checkValidity()) {
                return true; // Permite el postback
            } else {
                // Muestra los errores de validación nativos del navegador
                formulario.reportValidity();
                return false; // Bloquea el postback
            }
        }

    </script>

</asp:Content>




