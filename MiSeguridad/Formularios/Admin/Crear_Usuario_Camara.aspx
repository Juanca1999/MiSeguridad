<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Usuario_Camara.aspx.vb" Inherits="MiSeguridad.Crear_Usuario_Camara" %>

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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR USUARIO CAMARA</span>
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
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR USUARIO CAMARA</asp:LinkButton>
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
                                            <asp:TextBox ID="TxCedula" runat="server" placeholder="Cedula" CssClass="Cedula" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombres">Nombres</label>
                                            <asp:TextBox ID="TxNombres" runat="server" placeholder="Nombres" CssClass="Nombre_Usuario" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCorreo">Correo</label>
                                            <asp:TextBox ID="TxCorreo" runat="server" placeholder="Correo" TextMode="Email" CssClass="email"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxUsuario">Usuario</label>
                                            <asp:TextBox ID="TxUsuario" runat="server" placeholder="Usuario" CssClass="Nombre_Usuario"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPassword">Contraseña</label>
                                            <asp:TextBox ID="TxPassword" runat="server" placeholder="Contraseña" CssClass="Cod"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCargo">Cargo</label>
                                            <asp:TextBox ID="TxCargo" runat="server" placeholder="Cargo" CssClass="Nombre_Usuario"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxGenero">Genero</label>
                                            <asp:DropDownList ID="TxGenero" runat="server" CssClass="tipo" AppendDataBoundItems="true" Required="1">
                                                <asp:ListItem Value="">Genero</asp:ListItem>
                                                <asp:ListItem Value="male">MASCULINO</asp:ListItem>
                                                <asp:ListItem Value="female">FEMENINO</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTipoUsuario">Tipo Usuario</label>
                                            <asp:DropDownList ID="TxTipoUsuario" runat="server" CssClass="tipo" AppendDataBoundItems="true" Required="1">
                                                <asp:ListItem Value="">Tipo Usuario</asp:ListItem>
                                                <asp:ListItem Value="normal">NORMAL</asp:ListItem>
                                                <asp:ListItem Value="visitor">VISITANTE</asp:ListItem>
                                                <asp:ListItem Value="blackList">LISTA NEGRA</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxIdRol">Rol</label>
                                            <asp:DropDownList ID="TxIdRol" runat="server" CssClass="tipo" AppendDataBoundItems="true" DataSourceID="SqlRol" DataTextField="Rol" DataValueField="Id_Rol">
                                                <asp:ListItem Value="">Rol</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlRol" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Rol, Rol FROM Adm_Rol ORDER BY Rol"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxCelular">Celular</label>
                                            <asp:TextBox ID="TxCelular" runat="server" placeholder="Celular" TextMode="Number" CssClass="Telefono"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 15px; margin-top: 0px;">
                                            <label class="cd-label" for="TxEstado">Estado</label>
                                            <asp:DropDownList ID="TxEstado" runat="server" AppendDataBoundItems="true" CssClass="Calificacion">
                                                <asp:ListItem Value="">Estado</asp:ListItem>
                                                <asp:ListItem Value="1">ACTIVO</asp:ListItem>
                                                <asp:ListItem Value="0">INACTIVO</asp:ListItem>
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

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 15px; margin-top: -15px;">
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
                                        <img id="photo" runat="server" src="https://via.placeholder.com/170x170/FFFFFF/FFFFFF" width="700" height="700" style="border: 2px solid #6c757d;" />
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
                                                <i class="material-icons" style="margin-right: 5px;">apartment</i> USUARIOS
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                                <div class="card-body table-responsive">

                                    <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvUsuarios" runat="server">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se encuentran usuarios</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Editar" runat="server" CommandName="Editar"><i class="material-icons" style="color: #1e2833;">edit</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="employeeNoLabel" runat="server" Text='<%# Eval("employeeNo") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="nameLabel" runat="server" Text='<%# Eval("name") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="userTypelabel" runat="server" Text='<%# Eval("userType") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Eliminar" runat="server" CommandName="Eliminar"><i class="material-icons" style="color: #1e2833;">delete</i></asp:LinkButton>
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
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">ID EMPLEADO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">NOMBRE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 40%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">TIPO USUARIO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 1%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px"><i class="material-icons">delete</i></th>
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




