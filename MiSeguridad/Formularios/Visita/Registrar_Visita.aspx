<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Registrar_Visita.aspx.vb" Inherits="MiSeguridad.Registrar_Visita" %>

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
            <button id="BtEntrada" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                REGISTRAR ENTRADA
            </button>

            <button id="BtSalida" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR SALIDA
            </button>

            <button id="BtEntradaContratista" runat="server" type="button" class="btn btn-primary" style="width: 200px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR ENTRADA CONTRATISTA
            </button>

            <button id="BtSalidaContratista" runat="server" type="button" class="btn btn-primary" style="width: 200px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                REGISTRAR SALIDA CONTRATISTA
            </button>

        </div>

        <div class="cd-form">

            <div class="row" style="padding-top: 0px; margin-top: 0px;">

                <div class="col-md-12 mb-3" style="margin-top: 0px;">
                    <div class="icon has-float-label" style="position: relative; margin-bottom: 10px; margin-top: 0px; text-align: center;">
                        <label class="cd-label" for="TxBuscarInmueble" style="font-size: 2rem; margin-left: 49%; transform: translateX(-50%); font-weight: bold;">Buscar Inmueble</label>
                        <i class="material-icons" style="position: absolute; top: 25%; left: 32%; font-size: 24px;">search</i>
                        <asp:DropDownList ID="TxBuscarInmueble" runat="server" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlInmueble" DataTextField="Id_inmueble" DataValueField="Id_inmueble" Style="width: 40%; display: inline-block; background-color: #f8f9fa; color: #007bff; border: 2px solid #007bff; font-size: 15pt; padding: 9px;">
                            <asp:ListItem Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlInmueble" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_inmueble FROM Adm_Inmueble ORDER BY Id_inmueble"></asp:SqlDataSource>
                    </div>
                </div>

                <div id="Tabla" runat="server" class="col-md-12 mb-3" visible="false">

                    <div class="col-md-12 mb-3" style="margin-top: 0px; text-align: center;">
                        <asp:Label ID="TituloLabel" runat="server" Text="" Font-Size="X-Large" ForeColor="Black" Font-Names="Trebuchet MS" Width="280px"></asp:Label>
                    </div>

                    <div class="col-md-12 mb-3" style="padding-right: 10%; padding-left: 10%;">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" style="margin-top: -15px;" UpdateMode="always">
                            <ContentTemplate>
                                <asp:ListView ID="LvResidentes" runat="server" DataSourceID="SqlResidentes">
                                    <EmptyDataTemplate>
                                        <table runat="server" style="">
                                            <tr>
                                                <td></td>
                                            </tr>
                                        </table>
                                    </EmptyDataTemplate>

                                    <ItemTemplate>
                                        <tr style="">
                                            <asp:Label ID="Id_InmuebleLabel" runat="server" Text='<%# Eval("Id_Inmueble") %>' Font-Size="0pt" />
                                            <asp:Label ID="Id_TerceroLabel" runat="server" Text='<%# Eval("Id_Tercero") %>' Font-Size="0pt" />
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="CedulaLabel" runat="server" Text='<%# Eval("Cedula") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="NombresLabel" runat="server" Text='<%# Eval("Nombres") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:Label ID="CelularLabel" runat="server" Text='<%# Eval("Celular") %>' Font-Size="8pt" />
                                            </td>
                                            <td style="width: 1%; text-align: center; vertical-align: middle; border: solid; border-width: 1px; border-color: lightgray; padding: 5px;">
                                                <asp:ImageButton ID="BtAutorizar" runat="server" CommandName="Autorizar" UseSubmitBehavior="false" ImageUrl="~/Diseno/Imagenes/bien.png" Width="25px" OnClientClick="autorizarClick(this, event);" CssClass="bi-autorizar" data-nombre='<%# Eval("Nombres") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>

                                    <LayoutTemplate>
                                        <table runat="server" style="width: 100%">
                                            <tr runat="server">
                                                <td runat="server">
                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="width: 100%">
                                                        <tr runat="server">
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">CEDULA</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">NOMBRES</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">TELEFONO</th>
                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 10%; background-color: #1e2833; color: white; font-size: 8pt; height: 30px">AUTORIZA</th>
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

                                <asp:SqlDataSource ID="SqlResidentes" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT IR.Id_Inmueble, TI.Tipo_Inmueble, I.Telefono, IR.Id_Tercero, T.Cedula, T.Nombres, T.Celular FROM Adm_Inmueble_Residentes IR LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IR.Id_Inmueble LEFT JOIN Terceros T ON T.Id_Tercero = IR.Id_Tercero LEFT JOIN Adm_Tipo_Inmueble TI ON TI.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble WHERE IR.Id_Inmueble = @Inmueble ORDER BY T.Nombres">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="TxBuscarInmueble" Name="Inmueble" PropertyName="Text" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>

                <div class="row">
                    <!-- Mover todos los campos a la izquierda -->
                    <div class="col-md-8">

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <div class="row">
                                    <div class="col-md-6 mb-3" style="margin-top: 10px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPersonaVisitada">Persona Visitada</label>
                                            <asp:TextBox ID="TxPersonaVisitada" runat="server" placeholder="Persona Visitada" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="margin-top: 10px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxQuienAutoriza">Quien Autoriza</label>
                                            <asp:TextBox ID="TxQuienAutoriza" runat="server" placeholder="Quien Autoriza" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-4 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCedulaVisitante">Cedula Visitante*</label>
                                            <asp:TextBox ID="TxCedulaVisitante" runat="server" TextMode="Number" placeholder="Cedula Visitante" CssClass="Cedula"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:TextBox ID="TxNombre1" runat="server" Style="opacity: 0; position: absolute; z-index: -1;"></asp:TextBox>

                                    <div class="col-md-8 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombreVisitante">Nombre Visitante*</label>
                                            <asp:TextBox ID="TxNombreVisitante" runat="server" placeholder="Nombre Visitante" CssClass="user"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:HiddenField ID="hfPlacaValida" runat="server" />

                                    <div class="col-md-4 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTipoVehiculo">Tipo Vehiculo</label>
                                            <asp:DropDownList ID="TxTipoVehiculo" runat="server" CssClass="Sedes" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlTipoVehiculo" DataTextField="Tipo_Vehiculo" DataValueField="Id_Tipo_Vehiculo">
                                                <asp:ListItem Value="">Tipo Vehiculo</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlTipoVehiculo" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Tipo_Vehiculo, Tipo_Vehiculo FROM Adm_Tipo_Vehiculo ORDER BY Tipo_Vehiculo"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-8 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPlaca">Placa</label>
                                            <asp:TextBox ID="TxPlaca" runat="server" placeholder="Placa" CssClass="Placa" AutoPostBack="true" oninput="handleInput()" Enabled="false"></asp:TextBox>
                                            <asp:Label ID="lblPlacaError" runat="server" ForeColor="Red" Style="margin-top: 60px;" Visible="False"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxObservacion">Observaciones</label>
                                            <asp:TextBox ID="TxObservacion" runat="server" class="des" TextMode="MultiLine" Height="130px" placeholder="Observaciones"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 0px;">
                                        <div class="icon" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <asp:CheckBox ID="CbDomiciliario" runat="server" Text="Domiciliario" EnableTheming="True" Style="font-weight: unset;" />
                                        </div>
                                    </div>

                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <!-- Campo para la foto -->
                    <div class="col-md-4 custom-border">

                        <h2 style="text-align: center">Seleccione una camara</h2>
                        <br />
                        <div style="display: flex; justify-content: center">
                            <select id="cameraSelect" style="width: 70%;"></select>
                        </div>
                        <br>

                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <div style="text-align: center">
                                    <img id="photo" runat="server" src="https://via.placeholder.com/170x170/FFFFFF/FFFFFF" width="170" height="170" style="border: 2px solid #6c757d;" />
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
                </div>

            </div>

            <div class="row">

                <div class="col-md-8">
                </div>

                <div class="col-md-4" style="display: flex; justify-content: center;">
                    <asp:Button ID="BtGuardar" runat="server" Text="REGISTRAR ENTRADA" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" OnClientClick="return ValidateForm();" />
                    <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                </div>

            </div>

        </div>

    </div>


    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>

    <script type="text/javascript">

        document.getElementById('<%= TxCedulaVisitante.ClientID %>').addEventListener('blur', function () {
            var cedula = this.value.trim();

            if (cedula !== "") {
                fetch('Registrar_Visita.aspx/ObtenerFoto', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ cedula: cedula })
                })
                    .then(response => {
                        return response.text(); // Lee la respuesta como texto primero
                    })
                    .then(text => {
                        try {
                            const data = JSON.parse(text); // Parsear el texto como JSON
                            const result = data.d; // Acceder al valor real dentro de "d"

                            // Actualizar la imagen si hay una
                            if (result.Foto !== "") {
                                document.getElementById('<%= photo.ClientID %>').src = result.Foto;
                                document.getElementById('<%= Fotico.ClientID %>').value = result.Foto;
                            } else {
                                document.getElementById('<%= photo.ClientID %>').src = "https://via.placeholder.com/170x170/FFFFFF/FFFFFF";
                            }

                            // Actualizar el nombre del visitante si está presente
                            if (result.Nombress !== "") {
                                document.getElementById('<%= TxNombreVisitante.ClientID %>').value = result.Nombress;
                                document.getElementById('<%= TxNombre1.ClientID %>').value = "";
                            }

                            // Actualizar los campos relacionados con la autorización
                            if (result.Autoriza !== "") {
                                document.getElementById('<%= TxPersonaVisitada.ClientID %>').value = result.Autoriza;
                                document.getElementById('<%= TxQuienAutoriza.ClientID %>').value = result.Autoriza;
                            } else {
                                if (document.getElementById('<%= TxPersonaVisitada.ClientID %>').value === "") {
                                    document.getElementById('<%= TxPersonaVisitada.ClientID %>').value = "";
                                }

                                if (document.getElementById('<%= TxQuienAutoriza.ClientID %>').value === "") {
                                    document.getElementById('<%= TxQuienAutoriza.ClientID %>').value = "";
                                }
                            }

                            // Actualizar los campos relacionados con la persona visitada
                            if (result.Persona_Visitada !== "") {
                                document.getElementById('<%= TxPersonaVisitada.ClientID %>').value = result.Persona_Visitada;
                            } else {
                                if (document.getElementById('<%= TxPersonaVisitada.ClientID %>').value === "") {
                                    document.getElementById('<%= TxPersonaVisitada.ClientID %>').value = "";
                                }
                            }

                            // Actualizar los campos relacionados con la Id_Inmueble
                            if (result.Id_Inmueble !== "") {
                                document.getElementById('<%= TxBuscarInmueble.ClientID %>').value = result.Id_Inmueble;
                            } else {
                                if (document.getElementById('<%= TxBuscarInmueble.ClientID %>').value === "") {
                                    document.getElementById('<%= TxBuscarInmueble.ClientID %>').value = "";
                                }
                            }

                        } catch (e) {
                            console.error('Error al parsear JSON:', e);
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                    });
            } else {
                // Si el campo está vacío, muestra la imagen por defecto y vacía los nombres
                document.getElementById('<%= photo.ClientID %>').src = "https://via.placeholder.com/170x170/FFFFFF/FFFFFF";
            }
        });

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

        function autorizarClick(button) {
            // Obtener el valor del atributo data-nombre
            var nombre = button.getAttribute('data-nombre');

            // Actualizar los campos de texto
            document.getElementById('<%= TxPersonaVisitada.ClientID %>').value = nombre;
            document.getElementById('<%= TxQuienAutoriza.ClientID %>').value = nombre;

            // Prevenir el comportamiento por defecto
            return false;
        }

    </script>

</asp:Content>




