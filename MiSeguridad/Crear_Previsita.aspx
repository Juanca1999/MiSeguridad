<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Crear_Previsita.aspx.vb" Inherits="MiSeguridad.Crear_Previsita" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

    <link href="/Diseno/poppins.css" rel="stylesheet">

    <link rel="stylesheet" href="/Diseno/font-awesome.min.css">
    <link rel="stylesheet" href="/../../Diseno/2024/css/style.css">
    <link rel="shortcut icon" href="/../../Diseno\Imagenes\ICONO1.ICO" />
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
            margin: auto;
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
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <nav class="navbar navbar-expand-lg navbar-light" style="background-color: #0442A6 !important;">
            <div class="container-fluid d-flex justify-content-between align-items-center">
                <!-- Contenedor del título y logo -->
                <div class="d-flex align-items-center text-center flex-grow-1 justify-content-center">
                    <span class="navbar-brand text-wrap m-0"
                        style="font-weight: bold; font-size: 16px; color: white; white-space: normal;">FO-13-006 REGISTRO DE VISITANTES Y PROVEEDORES
                    </span>
                    <img src="/../../../Diseno/Imagenes/miro.png" class="img-fluid ms-2" alt="Logo" style="width: 35px;">
                </div>
            </div>
        </nav>

        <div class="row" style="margin: 5px 5%;">

            <div class="cd-form">

                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                    <ContentTemplate>

                        <div class="row" style="display: flex; justify-content: center;">

                            <div class="col-md-12 mb-3" style="display: flex; justify-content: center;">
                                <div class="icon has-float-label" style="margin-bottom: 10px;">
                                    <div>
                                        <img id="Logo" runat="server" src="/../../../Adjunto/Logo/Logo.png" class="img-fluid" alt="Logo" style="width: 140px; background-color: white;">
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxCedula">Cedula<span style="color: red;">*</span></label>
                                    <asp:TextBox ID="TxCedula" runat="server" placeholder="Cedula" CssClass="Cedula" TextMode="Number" AutoPostBack="true" Required="1"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-8 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxNombres">Nombre Completo<span style="color: red;">*</span></label>
                                    <asp:TextBox ID="TxNombres" runat="server" placeholder="Nombre Completo" CssClass="Nombre_Usuario" Required="1"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxEmpresa">Empresa a la que pertenece <span style="color: red;">*</span></label>
                                    <asp:TextBox ID="TxEmpresa" runat="server" placeholder="Empresa a la que pertenece" CssClass="ciudad" Required="1"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxSede">Sede que visita<span style="color: red;">*</span></label>
                                    <asp:DropDownList ID="TxSede" runat="server" CssClass="ciudad" Required="1" AppendDataBoundItems="true" DataSourceID="SqlSedes" DataTextField="Nombre_Sede" DataValueField="Id_Sede">
                                        <asp:ListItem Value="">Sede</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlSedes" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Sede, Nombre_Sede FROM Adm_Sedes ORDER BY Id_Sede"></asp:SqlDataSource>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxFechaIngreso">Fecha Ingreso<span style="color: red;">*</span></label>
                                    <asp:TextBox ID="TxFechaIngreso" runat="server" Required="1" TextMode="Date" placeholder="Fecha Ingreso" CssClass="Fecha"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon" style="display: flex; align-items: center; padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="CbSi" style="margin-right: 10px;">¿Ingresa equipo de computo?</label>
                                    <div>
                                        <asp:RadioButton ID="CbSi" runat="server" GroupName="Computo" Text="Si" AutoPostBack="true" />
                                    </div>
                                    <div style="margin-left: 30px;">
                                        <asp:RadioButton ID="CbNo" runat="server" GroupName="Computo" Text="No" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>

                            <div id="Serial" runat="server" visible="false" class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxSerialEquipo">Serial del equipo de computo</label>
                                    <asp:TextBox ID="TxSerialEquipo" runat="server" placeholder="Serial del equipo de computo" CssClass="Cod"></asp:TextBox>
                                </div>
                            </div>

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
                                <div id="Placa" runat="server" class="icon has-float-label" style="display: none; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxPlaca">Placa</label>
                                    <asp:TextBox ID="TxPlaca" runat="server" placeholder="Placa" CssClass="Placa" AutoPostBack="true" oninput="handleInput()" Enabled="false"></asp:TextBox>
                                    <asp:Label ID="lblPlacaError" runat="server" ForeColor="Red" Style="display: none; margin-top: 60px;"></asp:Label>
                                </div>
                            </div>

                            <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxARL">ARL<span style="color: red;">*</span></label>
                                    <asp:DropDownList ID="TxARL" runat="server" CssClass="Cargo_Cliente" AppendDataBoundItems="true" DataSourceID="SqlARL" DataTextField="ARL" DataValueField="Id_ARL">
                                        <asp:ListItem Value="">ARL</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlARL" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_ARL, ARL FROM Sys_ARL ORDER BY Id_ARL"></asp:SqlDataSource>
                                </div>
                            </div>

                            <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxEPS">EPS<span style="color: red;">*</span></label>
                                    <asp:DropDownList ID="TxEPS" runat="server" CssClass="Vendedor" AppendDataBoundItems="true" DataSourceID="SqlEPS" DataTextField="EPS" DataValueField="Id_EPS">
                                        <asp:ListItem Value="">EPS</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlEPS" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_EPS, EPS FROM Sys_EPS ORDER BY Id_EPS"></asp:SqlDataSource>
                                </div>
                            </div>

                            <div class="col-md-4 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxBuscarPersona">Buscar Quien Autoriza<span style="color: red;">*</span></label>
                                    <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                    <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Quien Autoriza" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-8 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="TxNombrePersona">Nombre Persona<span style="color: red;">*</span></label>
                                    <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">person</i>
                                    <asp:DropDownList ID="TxNombrePersona" runat="server" Required="1" Style="height: 44px; padding: 12px; padding-left: 40px !important;">
                                        <asp:ListItem Value="">Nombre Persona</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon" style="display: flex; align-items: center; padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <label class="cd-label" for="CbSi" style="margin-right: 10px; color: black; text-align: justify; display: block;">
                                        Mediante la presente autorizo expresamente a AUTECO MOBILITY S.A.S y a AUTOTÉCNICA COLOMBIANA S.A.S. (AUTECO S.A.S) para que le dé tratamiento a mis datos personales conforme a la Política de tratamiento de Datos Personales que se encuentra publicada en las páginas web: 
                                        <br />
                                        <a href="https://www.autecomobility.com/manual-de-politicas-y-procedimientos-de-datos" style="color: blue; text-decoration: underline;" target="_blank">https://www.autecomobility.com/manual-de-politicas-y-procedimientos-de-datos</a><br />
                                        <a href="https://www.auteco.com.co/nosotros/manual-de-politicas-y-procedimientos-de-datos/" style="color: blue; text-decoration: underline;" target="_blank">https://www.auteco.com.co/nosotros/manual-de-politicas-y-procedimientos-de-datos/</a>
                                        <span style="color: red;">*</span>
                                    </label>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                <div class="icon" style="display: flex; align-items: center; padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                    <div>
                                        <asp:RadioButton ID="RbAcepto" runat="server" GroupName="Privacidad" Text="Acepto" Checked="True" />
                                    </div>
                                    <div style="margin-left: 20px;">
                                        <asp:RadioButton ID="RbNoAcepto" runat="server" GroupName="Privacidad" Text="No Acepto" />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="col-md-12">
                    <asp:Button ID="BtGuardar" runat="server" Text="ENVIAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                    <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                </div>

            </div>
        </div>

        <script src="/Diseno/jquery.min.js-2.js"></script>
        <script>

            function handleInput() {
                var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
                var btGuardar = document.getElementById('<%= btGuardar.ClientID %>');
                var tipoVehiculoElement = document.getElementById('<%= TxTipoVehiculo.ClientID %>');
                var tipoVehiculo = tipoVehiculoElement ? tipoVehiculoElement.value.trim() : null;
                var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

                // Convertir el valor del input a mayúsculas antes de obtenerlo
                txPlaca.value = txPlaca.value.toUpperCase();
                var placa = txPlaca.value.trim();  // Ahora sí se obtiene en mayúsculas

                var isValid = false;

                // Depuración: mostrar valores en la consola
                console.log("Valor de tipoVehiculo:", tipoVehiculo);
                console.log("Valor de placa ingresada:", placa);

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
                var btGuardar = document.getElementById('<%= btGuardar.ClientID %>');
                var CampoPlaca = document.getElementById('<%= Placa.ClientID %>');
                var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
                var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

                if (tipoVehiculo === "1" || tipoVehiculo === "2") { // Carro o Moto
                    CampoPlaca.style.display = "block";
                    txPlaca.disabled = false; // Habilitar
                    txPlaca.setAttribute("required", "1"); // Agregar el atributo required
                    // Si ya hay una placa ingresada, validarla al cambiar el tipo de vehículo
                    if (txPlaca.value.trim() !== "") {
                        handleInput(); // Llamar a la validación de la placa
                    }
                } else {
                    CampoPlaca.style.display = "none";
                    txPlaca.disabled = true; // Deshabilitar
                    txPlaca.value = ""; // Vaciar
                    lblPlacaError.style.display = 'none';
                    txPlaca.removeAttribute("required"); // Eliminar el atributo required
                    btGuardar.disabled = false;
                }
            }

        </script>
    </form>
</body>
</html>




