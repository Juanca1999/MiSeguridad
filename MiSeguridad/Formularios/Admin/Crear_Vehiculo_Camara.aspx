<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Crear_Vehiculo_Camara.aspx.vb" Inherits="MiSeguridad.Crear_Vehiculo_Camara" %>

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

        .bg-info {
            background-color: #3c60af !important;
        }

        .card-body {
            padding: 0.65rem;
        }

        .table td, .table th {
            vertical-align: middle;
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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - CREAR VEHICULO CAMARA</span>
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

        <div class="col-md-12 mb-3" style="text-align: center">

            <button id="BtCrearVehiculo" runat="server" type="button" class="btn btn-outline-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #1e2833; background-color: transparent; border-color: #1e2833;">
                CREAR VEHICULO
            </button>

            <button id="BtAsignarTag" runat="server" type="button" class="btn btn-primary" style="width: 140px; height: 30px; font-size: 8pt; border-radius: 5px; margin-left: 10px; margin-right: 10px; margin-bottom: 10px; color: #fff; background-color: #1e2833; border-color: #1e2833;">
                ASIGNAR TAG
            </button>

        </div>

        <div class="col-md-5 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px; padding-right: 0pt;">

            <div class="cd-form" style="padding-top: 5px; margin-top: 0px">
                <div class="content" style="padding-top: 0px; margin-top: 0px">
                    <div class="container-fluid" style="padding-top: 0px; margin-top: 0px">
                        <div class="card" style="border: none; border-color: transparent; padding-top: 0px; margin-top: -6px; background: #1e2833;">
                            <div class="card-header card-header-primary" style="width: 100%; margin: 0px; padding: 5px">
                                <h4 style="margin-bottom: 14px; margin-top: 14px;">
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>CREAR VEHICULO CONTROL</asp:LinkButton>
                                </h4>
                                <p class="card-category">
                                </p>
                            </div>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="row" style="padding-top: 0px; margin-top: 0px;">

                                    <div class="col-md-6 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxTipoVehiculo">Tipo Vehiculo</label>
                                            <asp:DropDownList ID="TxTipoVehiculo" runat="server" CssClass="Sedes" Required="1" AutoPostBack="true" OnChange="updatePlacaState()" AppendDataBoundItems="true" DataSourceID="SqlTipoVehiculo" DataTextField="Tipo_Vehiculo" DataValueField="Id_Tipo_Vehiculo">
                                                <asp:ListItem Value="">NINGUNO</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlTipoVehiculo" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Tipo_Vehiculo, Tipo_Vehiculo FROM Adm_Tipo_Vehiculo ORDER BY Tipo_Vehiculo"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-6 mb-3" style="margin-top: 0px;">
                                        <div id="Placa" runat="server" class="icon has-float-label" style="margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxPlaca">Placa</label>
                                            <asp:TextBox ID="TxPlaca" runat="server" placeholder="Placa" CssClass="Placa" AutoPostBack="true" Required="1" oninput="handleInput()"></asp:TextBox>
                                            <asp:Label ID="lblPlacaError" runat="server" ForeColor="Red" Style="display: none; margin-top: 60px;"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxMarca">Marca</label>
                                            <asp:TextBox ID="TxMarca" runat="server" placeholder="Marca" CssClass="tipo" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxModelo">Modelo</label>
                                            <asp:TextBox ID="TxModelo" runat="server" placeholder="Modelo" CssClass="tipo" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxColor">Color</label>
                                            <asp:TextBox ID="TxColor" runat="server" placeholder="Color" CssClass="Color" Required="1"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxBuscarPersona">Buscar Propietario</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">search</i>
                                            <asp:TextBox ID="TxBuscarPersona" runat="server" placeholder="Buscar Propietario" AutoPostBack="true" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-10 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxNombrePersona">Nombre Propietario</label>
                                            <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">person</i>
                                            <asp:DropDownList ID="TxNombrePersona" runat="server" Required="1" Style="height: 44px; padding: 12px; padding-left: 40px !important;">
                                                <asp:ListItem Value="">Nombre Propietario</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-2 mb-3" style="margin-top: -10px; align-content: center;">
                                        <asp:ImageButton ID="BtnAgregarEmpresa" UseSubmitBehavior="false" runat="server" ImageUrl="https://cdn-icons-png.flaticon.com/512/6325/6325028.png" Width="35px" data-toggle="modal" data-target="#Crear_Persona" />
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxInmueble">Dependencia</label>
                                            <asp:DropDownList ID="TxInmueble" runat="server" CssClass="ciudad" Required="1" AppendDataBoundItems="true" DataSourceID="SqlInmueble" DataTextField="Id_inmueble" DataValueField="Id_inmueble">
                                                <asp:ListItem Value="">Dependencia</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlInmueble" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_inmueble FROM Adm_Inmueble ORDER BY Id_inmueble"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                        <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                            <label class="cd-label" for="TxSede">Sede</label>
                                            <asp:DropDownList ID="TxSede" runat="server" CssClass="ciudad" Required="1" AppendDataBoundItems="true" DataSourceID="SqlSedes" DataTextField="Nombre_Sede" DataValueField="Id_Sede">
                                                <asp:ListItem Value="">Sede</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlSedes" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Sede, Nombre_Sede FROM Adm_Sedes ORDER BY Id_Sede"></asp:SqlDataSource>
                                        </div>
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

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
                                                <i class="material-icons" style="margin-right: 5px;">directions_car</i> VEHICULOS
                                            </a>
                                        </li>
                                    </ul>
                                    <p class="card-category" style="text-align: right">
                                        <i class="material-icons" style="color: white; position: absolute; margin-top: 2px; margin-left: -25px;">search</i>
                                        <asp:TextBox ID="TxBuscar" runat="server" placeholder="Buscar vehiculo" AutoPostBack="true" Width="30%" Font-Size="11pt" BackColor="White"></asp:TextBox>
                                    </p>
                                </div>

                                <div class="card-body table-responsive">

                                    <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:ListView ID="LvVehiculos" runat="server" DataSourceID="SqlVehiculos">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se han devuelto datos.</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <asp:Label ID="EstadoLabel" runat="server" Text='<%# Eval("Estado") %>' Font-Size="0pt" />
                                                            <asp:Label ID="Id_InmuebleLabel" runat="server" Text='<%# Eval("Id_Inmueble") %>' Font-Size="0pt" />
                                                            <asp:Label ID="Id_TerceroLabel" runat="server" Text='<%# Eval("Id_Tercero") %>' Font-Size="0pt" />
                                                            <asp:Label ID="Id_SedeLabel" runat="server" Text='<%# Eval("Id_Sede") %>' Font-Size="0pt" />
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Editar" runat="server" CommandName="Editar"><i class="material-icons" style="color: #1e2833;">edit</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Tipo_VehiculoLabel" runat="server" Text='<%# Eval("Tipo_Vehiculo") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="MarcaLabel" runat="server" Text='<%# Eval("Marca") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="ModeloLabel" runat="server" Text='<%# Eval("Modelo") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="ColorLabel" runat="server" Text='<%# Eval("Color") %>' />
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
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">PLACA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">TIPO VEHICULO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">MARCA</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">MODELO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">COLOR</th>
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


                                                <asp:SqlDataSource ID="SqlVehiculos" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT V.Placa, V.Id_Tipo_Vehiculo, T.Tipo_Vehiculo, V.Marca, V.Modelo, V.Color, V.Estado, IV.Id_Inmueble, IP.Id_Tercero, IP.Id_Sede FROM Adm_Vehiculo V LEFT JOIN Adm_Tipo_Vehiculo T ON T.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo LEFT JOIN Adm_Inmueble_Vehiculo IV ON IV.Placa = V.Placa LEFT JOIN Adm_Vehiculo_Persona IP ON IP.Placa = V.Placa LEFT JOIN Terceros TE ON TE.Id_Tercero = IP.Id_Tercero WHERE V.Estado = 1 ORDER BY V.Placa"></asp:SqlDataSource>

                                                <asp:SqlDataSource ID="Sql_Buscar_Placa" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT V.Placa, V.Id_Tipo_Vehiculo, T.Tipo_Vehiculo, V.Marca, V.Modelo, V.Color, V.Estado, IV.Id_Inmueble, IP.Id_Tercero, IP.Id_Sede FROM Adm_Vehiculo V LEFT JOIN Adm_Tipo_Vehiculo T ON T.Id_Tipo_Vehiculo = V.Id_Tipo_Vehiculo LEFT JOIN Adm_Inmueble_Vehiculo IV ON IV.Placa = V.Placa LEFT JOIN Adm_Vehiculo_Persona IP ON IP.Placa = V.Placa LEFT JOIN Terceros TE ON TE.Id_Tercero = IP.Id_Tercero WHERE V.Estado = 1 AND V.Placa LIKE '%' + @Placa + '%' ORDER BY V.Placa">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="TxBuscar" Name="Placa" PropertyName="Text" />
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

        <div class="modal fade" id="Confirmacion" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" id="MensajeModalLabel" style="text-align: center">BORRAR ASIGNADO DEL VEHICULO</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body" style="text-align: center">
                        <asp:Label ID="Label1" runat="server" Text="OJO: ¿DESEA BORRAR EL ASIGNADO DEL VEHICULO?"></asp:Label>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtAceptar_Borrar" runat="server" Text="Aceptar" class="btn btn-danger" BackColor="IndianRed" ForeColor="White" Width="160px" Font-Size="11pt" />
                        <asp:Button ID="BtCancelar_Borrar" runat="server" Text="Cancelar" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Crear Persona -->
        <div class="modal fade" id="Crear_Persona" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" style="text-align: center">CREAR PERSONA</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body cd-form" style="text-align: center; margin: 0em auto; width: 100%; margin-top: 0px;">
                        <div class="row" style="margin: 0px 0px;">
                            <div class="col-md-12 mb-3" style="margin: 0px 0px;">
                                <div class="icon has-float-label" style="margin: 0px 0px;">
                                    <label class="cd-label" for="TxCedula">Cedula Persona</label>
                                    <asp:TextBox ID="TxCedula" runat="server" placeholder="Cedula Persona" CssClass="Cedula" TextMode="Number"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-12 mb-3" style="margin: 0px 0px;">
                                <div class="icon has-float-label" style="margin: 0px 0px;">
                                    <label class="cd-label" for="TxNombre">Nombre Persona</label>
                                    <asp:TextBox ID="TxNombre" runat="server" placeholder="Nombre Persona" CssClass="Nombre_Usuario"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtCrearPersona" runat="server" Text="CREAR" UseSubmitBehavior="false" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" />
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script src="/Diseno/jquery.min.js-2.js"></script>

    <script type="text/javascript">

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

            // Validación de placa
            if (placa) {
                if (tipoVehiculo === "1" || tipoVehiculo === "3") { // Carro
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
            if (tipoVehiculo === "1" || tipoVehiculo === "3") { // Carro
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
            var txPlaca = document.getElementById('<%= TxPlaca.ClientID %>');
            var lblPlacaError = document.getElementById('<%= lblPlacaError.ClientID %>');

            if (tipoVehiculo === "1" || tipoVehiculo === "2" || tipoVehiculo === "3") { // Carro o Moto
                txPlaca.disabled = false; // Habilitar
                txPlaca.setAttribute("required", "1"); // Agregar el atributo required
                // Si ya hay una placa ingresada, validarla al cambiar el tipo de vehículo
                if (txPlaca.value.trim() !== "") {
                    handleInput(); // Llamar a la validación de la placa
                }
            } else {
                txPlaca.value = ""; // Vaciar
                lblPlacaError.style.display = 'none';
            }
        }

    </script>

</asp:Content>




