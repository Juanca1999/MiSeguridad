<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Registrar_Visita.aspx.vb" Inherits="MiSeguridad.Registrar_Visita" %>

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

            .event-card.registrado-salida::before {
                background-color: #ff9800;
            }

            /* Rojo para no registrado (entrada o salida) */
            .event-card.no-registrado::before {
                background-color: #f44336;
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

        .cd-form .Buscar {
            background: url('/Diseno/2024/img/Buscar.svg') no-repeat 16px 6px;
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
                INGRESO VISITANTES
            </button>

            <button id="BtVisitaProveedor" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
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

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                <ContentTemplate>

                    <div class="row" style="padding-top: 0px; margin-top: 0px;">

                        <div class="col-md-8 mb-3" style="margin-top: 0px;">
                            <div class="icon has-float-label" style="position: relative; margin-bottom: 10px; margin-top: 0px; text-align: center;">
                                <label class="cd-label" for="TxBuscarInmueble" style="font-size: 2rem; margin-left: 49%; transform: translateX(-50%); font-weight: bold;">Buscar Área o Dependencia</label>
                                <i class="material-icons" style="position: absolute; top: 25%; left: 32%; font-size: 24px;">search</i>
                                <asp:DropDownList ID="TxBuscarInmueble" runat="server" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlInmueble" DataTextField="Id_inmueble" DataValueField="Id_inmueble" Style="width: 40%; display: inline-block; background-color: #f8f9fa; color: #007bff; border: 2px solid #007bff; font-size: 15pt; padding: 9px;">
                                    <asp:ListItem Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlInmueble" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_inmueble FROM Adm_Inmueble ORDER BY Id_inmueble"></asp:SqlDataSource>
                            </div>
                        </div>

                        <div id="Tabla" runat="server" class="col-md-8 mb-3" visible="false">

                            <div class="col-md-12 mb-3" style="margin-top: 0px; text-align: center;">
                                <asp:Label ID="TituloLabel" runat="server" Text="" Font-Size="X-Large" ForeColor="Black" Font-Names="Trebuchet MS" Width="280px"></asp:Label>
                            </div>

                            <div class="col-md-12 mb-3" style="margin-top: 0px; text-align: center; display: flex; justify-content: center;">
                                <div class="icon has-float-label" style="margin-bottom: -15px; margin-top: 0px;">
                                    <label class="cd-label" for="TxBuscarResidente">Buscar Persona</label>
                                    <asp:TextBox ID="TxBuscarResidente" runat="server" placeholder="Buscar Persona" AutoPostBack="true" CssClass="Buscar" Style="height: 36px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="padding-right: 10%; padding-left: 10%;">
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

                                <asp:SqlDataSource ID="SqlBuscarResidentes" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT IR.Id_Inmueble, TI.Tipo_Inmueble, I.Telefono, IR.Id_Tercero, T.Cedula, T.Nombres, T.Celular FROM Adm_Inmueble_Residentes IR LEFT JOIN Adm_Inmueble I ON I.Id_inmueble = IR.Id_Inmueble LEFT JOIN Terceros T ON T.Id_Tercero = IR.Id_Tercero LEFT JOIN Adm_Tipo_Inmueble TI ON TI.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble WHERE IR.Id_Inmueble = @Inmueble AND T.Nombres LIKE '%' + @Persona + '%' ORDER BY T.Nombres">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="TxBuscarInmueble" Name="Inmueble" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="TxBuscarResidente" Name="Persona" PropertyName="Text" />
                                    </SelectParameters>
                                </asp:SqlDataSource>

                            </div>

                        </div>

                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="cd-form">

                <div class="row">
                    <!-- Mover todos los campos a la izquierda -->
                    <div class="col-md-5">

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <div class="row" style="margin: 0px 0px 0px 35px;" onkeydown="return noSubmitOnEnter(event);">
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

                                    <div class="col-md-12 mb-3" style="margin-top: 0px;">
                                        <div class="icon" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <asp:CheckBox ID="CbDomiciliario" runat="server" Text="Domiciliario" EnableTheming="True" Style="font-weight: unset;" />
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="margin-top: 20px;">
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <asp:Button ID="BtGuardar" runat="server" Text="REGISTRAR ENTRADA" CssClass="btn btn-primary" OnClientClick="mostrarCargando()" Style="background-color: #1e2833; border-color: #1e2833;" />
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
                                    <img id="photo" runat="server" src="/Diseno/Imagenes/Blanco.jpg" width="170" height="170" style="border: 2px solid #6c757d;" />
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
                            <asp:Button ID="BtCapturar" runat="server" Text="Tomar Foto" CssClass="btn btn-primary" UseSubmitBehavior="false" OnClientClick="return false;" Style="color: white" />
                        </div>

                        <br>

                        <!-- Campo oculto para enviar la imagen en Base64 -->
                        <asp:HiddenField ID="base64image" runat="server" />

                    </div>

                    <div class="col-md-4 custom-border2" style="padding-left: 5px; margin-top: -80px">

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

                                    <div class="container mt-12" style="padding-left: 5px; padding-right: 5px;">
                                        <h1 class="text-center mb-12" style="margin-bottom: 15px;">Eventos Vehiculos</h1>
                                        <div class="row">
                                            <div class="col-md-12" style="padding-left: 0px;">
                                                <div id="eventos-container-vehiculos">
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
                                document.getElementById('<%= photo.ClientID %>').src = "/Diseno/Imagenes/Blanco.jpg";
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

                            // Actualizar los campos relacionados con la Tipo_Vehiculo
                            if (result.Tipo_Vehiculo !== "") {
                                document.getElementById('<%= TxTipoVehiculo.ClientID %>').value = result.Tipo_Vehiculo;
                            } else {
                                if (document.getElementById('<%= TxTipoVehiculo.ClientID %>').value === "") {
                                    document.getElementById('<%= TxTipoVehiculo.ClientID %>').value = "";
                                }
                            }

                            // Actualizar los campos relacionados con la placa
                            if (result.Placa !== "") {
                                document.getElementById('<%= TxPlaca.ClientID %>').value = result.Placa;
                                document.getElementById('<%= TxPlaca.ClientID %>').disabled = false;
                            } else {
                                if (document.getElementById('<%= TxPlaca.ClientID %>').value === "") {
                                    document.getElementById('<%= TxPlaca.ClientID %>').value = "";
                                }
                            }

                            // Actualizar los campos relacionados con la placa
                            if (result.Observacion !== "") {
                                document.getElementById('<%= TxObservacion.ClientID %>').value = result.Observacion;
                            } else {
                                if (document.getElementById('<%= TxObservacion.ClientID %>').value === "") {
                                    document.getElementById('<%= TxObservacion.ClientID %>').value = "";
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
                document.getElementById('<%= photo.ClientID %>').src = "/Diseno/Imagenes/Blanco.jpg";
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
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');
            var placa = txPlaca.value.trim();
            var isValid = false;

            // Convertir el valor a mayúsculas
            txPlaca.value = txPlaca.value.toUpperCase();

            // Realizar validación
            if (placa) {
                if (tipoVehiculo === "1" || tipoVehiculo === "3") { // Carro
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
            if (tipoVehiculo === "1" || tipoVehiculo === "3") { // Carro
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

        function autorizarClick(button) {
            // Obtener el valor del atributo data-nombre
            var nombre = button.getAttribute('data-nombre');

            // Actualizar los campos de texto
            document.getElementById('<%= TxPersonaVisitada.ClientID %>').value = nombre;
            document.getElementById('<%= TxPersonaVisitada.ClientID %>').disabled = true;
            document.getElementById('<%= TxQuienAutoriza.ClientID %>').value = nombre;

            // Prevenir el comportamiento por defecto
            return false;
        }

        function updatePlacaState() {
            var tipoVehiculo = document.getElementById('<%= TxTipoVehiculo.ClientID %>').value;
            var btGuardar = document.getElementById('<%= btGuardar.ClientID %>');
            var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

            if (tipoVehiculo === "1" || tipoVehiculo === "2" || tipoVehiculo === "3") { // Carro o Moto
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

        // Función para obtener los últimos 4 logs desde el servidor
        function obtenerUltimos4Logs() {

            fetch('Registrar_Visita.aspx/ObtenerUltimos4Logs', {
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

                    // Agregar los últimos 4 logs a la lista
                    logs.reverse().forEach(log => {
                        logsMostrados.unshift(log); // Agregar al principio del array
                        agregarEvento(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.Nombre_Acceso, log.FotoUrl);
                    });
                })
                .catch(error => {
                    console.error('Error al obtener los últimos 4 logs:', error); // Depuración
                });
        }

        // Función para obtener el último log desde el servidor
        function obtenerUltimoLog() {

            fetch('Registrar_Visita.aspx/ObtenerUltimoLog', {
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

                        if (logsMostrados.length > 4) {
                            logsMostrados.pop(); // Eliminar el último elemento si hay más de 4
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

            // Verificar si hay más de 4 elementos en el contenedor visual y eliminar los sobrantes
            while (eventosContainer.childElementCount > 4) {
                eventosContainer.removeChild(eventosContainer.lastChild);
            }
        }

        // Obtener los últimos 4 logs al cargar la página
        document.addEventListener('DOMContentLoaded', function () {
            obtenerUltimos4Logs(); // Cargar los últimos 4 logs
            obtenerUltimos4LogsContratista();
            obtenerUltimos4LogsVehiculos();
            setInterval(obtenerUltimoLog, 5000);
            setInterval(ObtenerUltimoLogVehiculo, 5000);
            setInterval(ObtenerUltimoLogContratista, 5000);          
        });

        // Variable para almacenar los logs ya mostrados
        let logsMostradosContratisas = [];

        // Función para obtener los últimos 4 logs desde el servidor
        function obtenerUltimos4LogsContratista() {

            fetch('Registrar_Visita.aspx/ObtenerUltimos4LogsContratista', {
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
                    logsMostradosContratisas = [];

                    // Agregar los últimos 4 logs a la lista
                    logs.reverse().forEach(log => {
                        logsMostradosContratisas.unshift(log); // Agregar al principio del array
                        agregarEventoContratista(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.FotoUrl);
                    });
                })
                .catch(error => {
                    console.error('Error al obtener los últimos 4 logs:', error); // Depuración
                });
        }

        // Función para obtener el último log desde el servidor
        function ObtenerUltimoLogContratista() {

            fetch('Registrar_Visita.aspx/ObtenerUltimoLogContratista', {
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

                    // Verificar si el log ya ha sido mostrado (en memoria y en localStorage)
                    const logYaMostrado = logsMostradosContratisas.some(logMostrado => logMostrado.Id_Log === log.Id_Log);
                    const logEnStorage = localStorage.getItem('alerta_contratista_' + log.Id_Log);

                    if (!logYaMostrado && !logEnStorage) {
                        logsMostradosContratisas.unshift(log);

                        if (logsMostradosContratisas.length > 4) {
                            logsMostradosContratisas.pop();
                        }

                        if (log && log.Id_Log && log.Nombres) {
                            // Validación pasada, mostrar alerta
                            if (!localStorage.getItem('alerta_contratista_' + log.Id_Log)) {
                                // Mostrar alerta con SweetAlert
                                swal({
                                    title: '¡ATENCIÓN!',
                                    text: 'El contratista: ' + log.Nombres + 'tiene vencido el documento ' + log.Tipo_Evento + '',
                                    type: 'warning',
                                    confirmButtonText: 'OK',
                                    allowOutsideClick: false,
                                    allowEscapeKey: false,
                                    customClass: 'alerta-vencimiento'
                                }, function () {
                                    // Guardar el log en localStorage como ya mostrado
                                    localStorage.setItem('alerta_contratista_' + log.Id_Log, 'mostrado');
                                });
                            }
                        }

                        // agregarEventoContratista(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.FotoUrl);
                    }
                })
                .catch(error => {
                    console.error('Error al obtener el último log:', error); // Depuración
                });
        }

        // Función para agregar un evento a la lista
        function agregarEventoContratista(tipo, nombre, hora, fotoUrl) {
            const registrosContainer = document.getElementById('registros-container');
            const eventoCard = document.createElement('div');
            eventoCard.className = `novedad-card`;

            eventoCard.innerHTML = `
            <div class="d-flex align-items-center">
                <div class="event-details">
                    <h5 class="mb-0">${nombre}</h5>
                    <small>${tipo.toUpperCase()} - ${hora}</small><br>
                </div>
            </div>
            <div class="event-photo">
                <img src="${fotoUrl}" alt="Foto de ${nombre}">
            </div>
        `;

            // Agregar el evento al principio del contenedor
            registrosContainer.insertBefore(eventoCard, registrosContainer.firstChild);
        }

        let logsMostradosVehiculos = [];

        // Función para obtener los últimos 4 logs desde el servidor
        function obtenerUltimos4LogsVehiculos() {

            fetch('Registrar_Visita.aspx/ObtenerUltimos4LogsVehiculos', {
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
                    logsMostradosVehiculos = [];

                    // Agregar los últimos 4 logs a la lista
                    logs.reverse().forEach(log => {
                        logsMostradosVehiculos.unshift(log); // Agregar al principio del array
                        agregarEventoVehiculo(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.Nombre_Acceso, log.FotoUrl, log.Registrado);
                    });
                })
                .catch(error => {
                    console.error('Error al obtener los últimos 4 logs:', error); // Depuración
                });
        }

        // Función para obtener el último log desde el servidor
        function ObtenerUltimoLogVehiculo() {

            fetch('Registrar_Visita.aspx/ObtenerUltimoLogVehiculo', {
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
                    const logYaMostrado = logsMostradosVehiculos.some(logMostrado =>
                        logMostrado.Id_Log === log.Id_Log
                    );

                    if (!logYaMostrado) {
                        logsMostradosVehiculos.unshift(log); // Agregar al principio del array

                        if (logsMostradosVehiculos.length > 4) {
                            logsMostradosVehiculos.pop(); // Eliminar el último elemento si hay más de 4
                        }

                        agregarEventoVehiculo(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento, log.Nombre_Acceso, log.FotoUrl, log.Registrado);
                    }
                })
                .catch(error => {
                    console.error('Error al obtener el último log:', error); // Depuración
                });
        }

        // Función para agregar un evento a la lista
        function agregarEventoVehiculo(tipo, nombre, hora, nombreAcceso, fotoUrl, registrado) {
            const eventosContainer = document.getElementById('eventos-container-vehiculos');
            const eventoCard = document.createElement('div');

            let claseColor = "";
            if (registrado) {
                claseColor = tipo === "entrada" ? "registrado-entrada" : "registrado-salida";
            } else {
                claseColor = "no-registrado";
            }

            const icono = tipo === 'entrada' ? 'fa-door-open' : 'fa-door-closed';
            eventoCard.className = `event-card ${claseColor}`;

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

            eventosContainer.insertBefore(eventoCard, eventosContainer.firstChild);

            while (eventosContainer.childElementCount > 4) {
                eventosContainer.removeChild(eventosContainer.lastChild);
            }
        }

        function noSubmitOnEnter(e) {
            // Si la tecla presionada es Enter (keyCode 13), cancela la acción
            if (e.key === "Enter" || e.keyCode === 13) {
                return false;
            }
            return true;
        }

        function mostrarCargando() {
            swal({
                title: 'Cargando...',
                text: 'Por favor espera mientras procesamos los datos...',
                type: 'info',
                allowEscapeKey: false,
                allowOutsideClick: false,
                showCancelButton: false,
                showConfirmButton: false,
            });
        }

    </script>

</asp:Content>




