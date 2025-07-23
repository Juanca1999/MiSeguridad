<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Registrar_Visita_Proveedor.aspx.vb" Inherits="MiSeguridad.Registrar_Visita_Proveedor" %>

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
        .novedad-card {
            border: 1px solid #383d41;
            border-radius: 10px;
            background-color: #f8f9fa;
            margin-bottom: 15px !important;
            padding: 5px !important;
            position: relative !important;
            animation: fadeIn 0.5s ease-in-out;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .event-card {
            border: 1px solid #383d41;
            border-radius: 10px;
            background-color: #f8f9fa;
            margin-bottom: 15px !important;
            padding: 5px !important;
            position: relative !important;
            animation: fadeIn 0.5s ease-in-out;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

            .event-card::before {
                content: '';
                position: absolute;
                left: 30px;
                top: 50%;
                transform: translateY(-50%);
                width: 20px;
                height: 20px;
                border-radius: 50%;
                background-color: #4caf50; /* Color verde por defecto */
            }

            .event-card.salida::before {
                background-color: #f44336; /* Color rojo para salida */
            }

        .event-details {
            margin-left: 35px !important; /* Espacio para el círculo */
            color: #333; /* Color de texto más oscuro */
            flex: 1; /* Ocupa el espacio restante */
        }

        small, .small {
            font-size: 80% !important;
            font-weight: 400 !important;
        }

        .h5, h5 {
            font-size: 1.25rem !important;
            font-weight: 600 !important;
        }

        .event-photo {
            width: 60px;
            height: 50px;
            border: 2px solid #ccc;
            overflow: hidden;
            margin-left: 15px; /* Espacio entre el texto y la foto */
        }

            .event-photo img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(20px);
            }

            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .cd-form {
            max-width: 100%;
            width: 100%;
            margin: 0px;
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

        .custom-border2 {
            position: relative;
        }

            .custom-border2::before {
                content: '';
                position: absolute;
                left: -5%;
                height: 91%; /* Ajusta esto para hacer la línea más corta o más larga */
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

        <div class="col-md-12 mb-3" style="padding-bottom: 25px; margin-top: 0px; text-align: center">
            <button id="BtEntrada" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                INGRESO VISITANTES
            </button>

            <button id="BtVisitaProveedor" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                INGRESO PROVEEDOR
            </button>

            <button id="BtEntradaContratista" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
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

            <div class="cd-form">

                <div class="row">
                    <!-- Mover todos los campos a la izquierda -->
                    <div class="col-md-5">

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <div class="row" style="margin: 0px 0px 0px 35px;">
                                    <div class="col-md-5 mb-3" style="margin-top: 10px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxBuscarPersona">Buecar Persona Visitada</label>
                                            <asp:TextBox ID="TxBuscarPersona" runat="server" AutoPostBack="true" placeholder="Buecar Persona Visitada" CssClass="Buscar"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-7 mb-3" style="margin-top: 10px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombrePersona">Nombre Persona</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">person</i>
                                            <asp:DropDownList ID="TxNombrePersona" runat="server" Required="1" Style="height: 44px; padding: 12px; padding-left: 40px !important;">
                                                <asp:ListItem Value="">Nombre Persona</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-5 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCedulaVisitante">Cedula Proveedor*</label>
                                            <asp:TextBox ID="TxCedulaVisitante" runat="server" TextMode="Number" placeholder="Cedula Proveedor" CssClass="Cedula"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:TextBox ID="TxNombre1" runat="server" Style="opacity: 0; position: absolute; z-index: -1;"></asp:TextBox>

                                    <div class="col-md-7 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreVisitante">Nombre Proveedor*</label>
                                            <asp:TextBox ID="TxNombreVisitante" runat="server" placeholder="Nombre Proveedor" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxEmpresa">Empresa</label>
                                            <asp:DropDownList ID="TxEmpresa" runat="server" CssClass="ciudad" AppendDataBoundItems="true" DataSourceID="SqlEmpresas" DataTextField="Empresa" DataValueField="Id_Empresa">
                                                <asp:ListItem Value="">Empresa</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlEmpresas" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Empresa, Empresa FROM Empresas ORDER BY Id_Empresa"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-4 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTipoVehiculo">Tipo Vehiculo</label>
                                            <asp:DropDownList ID="TxTipoVehiculo" runat="server" CssClass="Sedes" AutoPostBack="true" OnChange="updatePlacaState()" AppendDataBoundItems="true" DataSourceID="SqlTipoVehiculo" DataTextField="Tipo_Vehiculo" DataValueField="Id_Tipo_Vehiculo">
                                                <asp:ListItem Value="">Tipo Vehiculo</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlTipoVehiculo" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Tipo_Vehiculo, Tipo_Vehiculo FROM Adm_Tipo_Vehiculo ORDER BY Tipo_Vehiculo"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-8 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPlaca">Placa</label>
                                            <asp:TextBox ID="TxPlaca" runat="server" placeholder="Placa" CssClass="Placa" AutoPostBack="true" oninput="handleInput()" Enabled="false"></asp:TextBox>
                                            <asp:Label ID="lblPlacaError" runat="server" ForeColor="Red" Style="display: none; margin-top: 60px;"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxObservacion">Observaciones</label>
                                            <asp:TextBox ID="TxObservacion" runat="server" class="des" TextMode="MultiLine" Height="130px" placeholder="Observaciones"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 20px;">
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <asp:Button ID="BtGuardar" runat="server" Text="REGISTRAR ENTRADA" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" OnClientClick="return ValidateForm();" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>

                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <!-- Campo para la foto -->
                    <div class="col-md-3 custom-border">

                        <h2 style="text-align: center">Seleccione una camara</h2>
                        <br />
                        <div style="display: flex; justify-content: center">
                            <select id="cameraSelect" style="width: 70%;"></select>
                        </div>
                        <br>

                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <div style="text-align: center">
                                    <img id="photo" runat="server" src="~/Diseno/Imagenes/Blanco.jpg" width="170" height="170" style="border: 2px solid #6c757d;" />
                                </div>
                                <asp:HiddenField ID="Fotico" runat="server" />
                                <br>

                                <div style="text-align: center">
                                    <canvas id="canvas" width="170" height="170" style="display: none;"></canvas>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <hr style="border: 1px solid #6c757d; width: 80%" />

                        <h2 style="text-align: center">Capturar Foto</h2>
                        <br>

                        <div style="text-align: center">
                            <video id="video" width="170" height="170" style="border: 2px solid #6c757d;" autoplay></video>
                        </div>

                        <div style="text-align: center">
                            <asp:Button ID="BtCapturar" runat="server" Text="Tomar Foto" CssClass="btn btn-primary" UseSubmitBehavior="false" Style="color: white" OnClientClick="return ValidateForm();" />
                        </div>

                        <br>

                        <!-- Campo oculto para enviar la imagen en Base64 -->
                        <asp:HiddenField ID="base64image" runat="server" />

                    </div>

                    <div class="col-md-4 custom-border2" style="padding-left: 5px;">

                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">

                                    <div class="container mt-12" style="padding-left: 5px; padding-right: 5px; margin-bottom: 40px;">
                                        <h1 class="text-center mb-12" style="margin-bottom: 15px;">Eventos</h1>
                                        <div class="row">
                                            <div class="col-md-12" style="padding-left: 0px;">
                                                <div id="eventos-container">
                                                    <!-- Los eventos se mostrarán aquí -->
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>

            </div>

        </div>

    </div>

    <script src="Diseno/jquery-3.6.0.min.js"></script>

    <script type="text/javascript">

        function initCedulaBlurEvent() {
            var txtCedula = document.getElementById('<%= TxCedulaVisitante.ClientID %>');
            if (txtCedula) {
                txtCedula.addEventListener('blur', function () {
                    var cedula = this.value.trim();

                    if (cedula !== "") {
                        fetch('Registrar_Visita_Proveedor.aspx/ObtenerFoto', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ cedula: cedula })
                        })
                            .then(response => response.text())
                            .then(text => {
                                try {
                                    const data = JSON.parse(text);
                                    const result = data.d;

                                    if (result.Foto !== "") {
                                        document.getElementById('<%= photo.ClientID %>').src = result.Foto;
                                document.getElementById('<%= Fotico.ClientID %>').value = result.Foto;
                            } else {
                                document.getElementById('<%= photo.ClientID %>').src = "~/Diseno/Imagenes/Blanco.jpg";
                            }

                            if (result.Nombress !== "") {
                                document.getElementById('<%= TxNombreVisitante.ClientID %>').value = result.Nombress;
                                document.getElementById('<%= TxNombre1.ClientID %>').value = "";
                            }

                            if (result.Tipo_Vehiculo !== "") {
                                document.getElementById('<%= TxTipoVehiculo.ClientID %>').value = result.Tipo_Vehiculo;
                            }

                            if (result.Placa !== "") {
                                document.getElementById('<%= TxPlaca.ClientID %>').value = result.Placa;
                                document.getElementById('<%= TxPlaca.ClientID %>').disabled = false;
                            }

                            if (result.Observacion !== "") {
                                document.getElementById('<%= TxObservacion.ClientID %>').value = result.Observacion;
                                }

                            } catch (e) {
                                console.error('Error al parsear JSON:', e);
                            }
                        })
                        .catch(error => {
                            console.error('Error:', error);
                        });
                } else {
                    document.getElementById('<%= photo.ClientID %>').src = "~/Diseno/Imagenes/Blanco.jpg";
                    }
                });
            }
        }

        Sys.Application.add_load(initCedulaBlurEvent);

        let typingTimer;
        const typingDelay = 100;

        // Agregar evento para detectar cuando el usuario escribe en TxNombre1
        document.getElementById('<%= TxNombre1.ClientID %>').addEventListener('input', function () {
            clearTimeout(typingTimer); // Reiniciar el temporizador en cada tecla
            var nombre1 = this.value.trim();
            var nombreVisitante = document.getElementById('<%= TxNombreVisitante.ClientID %>');

            // Iniciar temporizador para verificar si el usuario ha dejado de escribir
            typingTimer = setTimeout(function () {
                // Solo actualiza TxNombreVisitante si está vacío
                if (nombreVisitante.value.trim() === "" && nombre1 !== "") {
                    nombreVisitante.value = nombre1;
                }
            }, typingDelay);
        });

        function handleInput() {
            var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
            var btGuardar = document.getElementById('<%= btGuardar.ClientID %>');
            var tipoVehiculo = document.getElementById('<%= TxTipoVehiculo.ClientID %>').value;
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');
            var placa = txPlaca.value.trim();
            var isValid = false;

            // Convertir el valor a mayúsculas
            txPlaca.value = txPlaca.value.toUpperCase();

            // Realizar validación
            if (placa) {
                if (tipoVehiculo === "1") { // Carro
                    isValid = /^[A-Z]{3}[0-9]{3}$/.test(placa);
                } else if (tipoVehiculo === "2") { // Moto
                    isValid = /^[A-Z]{3}[0-9]{2}[A-Z]{1}$/.test(placa);
                }
            } else {
                isValid = true; // Permitir que el campo esté vacío
            }

            // Habilitar o deshabilitar el botón basado en la validación
            btGuardar.disabled = !isValid;

            // Mostrar u ocultar el mensaje de error
            if (isValid) {
                lblPlacaError.style.display = 'none';  // Ocultar el mensaje de error
            } else {
                lblPlacaError.style.display = 'block';  // Mostrar el mensaje de error
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

        const video = document.getElementById('video');
        const canvas = document.getElementById('canvas');
        const photo = document.getElementById('<%= photo.ClientID %>');
        const base64image = document.getElementById('<%= base64image.ClientID %>');
        const cameraSelect = document.getElementById('cameraSelect');

        // Función para acceder a la cámara seleccionada
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

        // Obtener lista de dispositivos (cámaras)
        navigator.mediaDevices.enumerateDevices()
            .then(devices => {
                devices.forEach(device => {
                    if (device.kind === 'videoinput') {
                        const option = document.createElement('option');

                        // Eliminar el texto entre paréntesis del nombre de la cámara
                        const cameraName = device.label.replace(/\s*\(.*?\)\s*/g, '');

                        option.value = device.deviceId;
                        option.text = cameraName || `Sin cámaras disponibles`;
                        cameraSelect.appendChild(option);
                    }
                });

                // Empezar con la primera cámara si está disponible
                if (cameraSelect.length > 0) {
                    startCamera(cameraSelect.value);
                }
            })
            .catch(error => {
                console.error('Error al enumerar dispositivos:', error);
            });

        // Cambiar cámara cuando el usuario selecciona otra
        cameraSelect.onchange = () => {
            startCamera(cameraSelect.value);
        };

        // Capturar la imagen al hacer clic en "Tomar Foto"
        document.getElementById('<%= BtCapturar.ClientID %>').addEventListener('click', () => {
            const context = canvas.getContext('2d');
            // Dibujar el video en el canvas con las dimensiones deseadas (ajustado a 170x170)
            context.drawImage(video, 0, 0, 170, 170);

            // Obtener la imagen en formato base64
            const dataURL = canvas.toDataURL('image/png');

            // Asignar la imagen al campo base64 y mostrarla en el <img>
            base64image.value = dataURL;
            photo.src = dataURL;  // Mostrar la imagen capturada
        });

        function updatePlacaState() {
            var tipoVehiculo = document.getElementById('<%= TxTipoVehiculo.ClientID %>').value;
            var btGuardar = document.getElementById('<%= btGuardar.ClientID %>');
            var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

            if (tipoVehiculo === "1" || tipoVehiculo === "2") { // Carro o Moto
                txPlaca.disabled = false; // Habilitar
                txPlaca.setAttribute("required", "1"); // Agregar el atributo required
            } else {
                txPlaca.disabled = true; // Deshabilitar
                txPlaca.value = ""; // Vaciar
                lblPlacaError.style.display = 'none';
                txPlaca.removeAttribute("required"); // Eliminar el atributo required
                btGuardar.disabled = false;
            }
        }

        // Variable para almacenar los logs ya mostrados
        let logsMostrados = [];

        // Función para obtener los últimos 10 logs desde el servidor
        function obtenerUltimos10Logs() {

            fetch('Registrar_Visita_Proveedor.aspx/ObtenerUltimos10Logs', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({})
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Error en la solicitud');
                    }
                    return response.json();
                })
                .then(data => {
                    const logs = data.d;

                    // Limpiar la lista de logs mostrados
                    logsMostrados = [];

                    // Agregar los últimos 10 logs a la lista
                    logs.reverse().forEach(log => {
                        logsMostrados.unshift(log); // Agregar al principio del array
                        agregarEvento(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.Nombre_Acceso, log.FotoUrl);
                    });
                })
                .catch(error => {
                    console.error('Error al obtener los últimos 10 logs:', error); // Depuración
                });
        }

        // Función para obtener el último log desde el servidor
        function obtenerUltimoLog() {

            fetch('Registrar_Visita_Proveedor.aspx/ObtenerUltimoLog', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({})
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Error en la solicitud');
                    }
                    return response.json();
                })
                .then(data => {
                    const log = data.d;

                    // Verificar si el log ya ha sido mostrado usando Id_Log
                    const logYaMostrado = logsMostrados.some(logMostrado =>
                        logMostrado.Id_Log === log.Id_Log
                    );

                    if (!logYaMostrado) {
                        logsMostrados.unshift(log); // Agregar al principio del array

                        if (logsMostrados.length > 10) {
                            logsMostrados.pop(); // Eliminar el último elemento si hay más de 10
                        }

                        agregarEvento(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.Nombre_Acceso, log.FotoUrl);
                    }
                })
                .catch(error => {
                    console.error('Error al obtener el último log:', error); // Depuración
                });
        }

        // Función para agregar un evento a la lista
        function agregarEvento(tipo, nombre, hora, nombreAcceso, fotoUrl) {
            const eventosContainer = document.getElementById('eventos-container');
            const eventoCard = document.createElement('div');
            eventoCard.className = `event-card ${tipo}`;

            const icono = tipo === 'entrada' ? 'fa-door-open' : 'fa-door-closed';
            eventoCard.innerHTML = `
            <div class="d-flex align-items-center">
                <i class="event-icon fas ${icono}"></i>
                <div class="event-details">
                    <h5 class="mb-0">${nombre}</h5>
                    <small>${tipo.toUpperCase()} - ${hora}</small><br>
                    <small>${nombreAcceso.toUpperCase()}</small>
                </div>
            </div>
            <div class="event-photo">
                <img src="${fotoUrl}" alt="Foto de ${nombre}">
            </div>
        `;

            // Agregar el evento al principio del contenedor
            eventosContainer.insertBefore(eventoCard, eventosContainer.firstChild);
        }

        // Obtener los últimos 10 logs al cargar la página
        document.addEventListener('DOMContentLoaded', function () {
            obtenerUltimos10Logs(); // Cargar los últimos 10 logs
            setInterval(obtenerUltimoLog, 5000); // Iniciar el intervalo para obtener el último log
        });

    </script>

</asp:Content>




