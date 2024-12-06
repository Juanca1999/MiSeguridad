<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Asignar_Parqueadero.aspx.vb" Inherits="MiSeguridad.Asignar_Parqueadero" %>

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
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>


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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - ASIGNAR PARQUEADERO</span>
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
                                    <asp:LinkButton ID="Refrescar" runat="server" Style="color: white; margin-left: 20px;"><i class="material-icons"></i>ASIGNAR PARQUEADERO A INMUEBLE</asp:LinkButton>
                                </h4>
                                <p class="card-category">
                                </p>
                            </div>
                        </div>

                        <asp:Panel ID="PUsuario" runat="server" Visible="false" Style="margin: 0px; padding: 0px;" Width="100%">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate>

                                    <div class="row" style="padding-top: 0px; margin-top: 0px;">

                                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                                <label class="cd-label" for="TxIdInmueble">ID Inmueble</label>
                                                <i class="material-icons" style="margin-top: 10px; margin-left: 10px; position: absolute;">home</i>
                                                <asp:TextBox ID="TxIdInmueble" runat="server" placeholder="ID Inmueble" Enabled="false" Style="height: 44px; padding-left: 40px !important;"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-10 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <div class="icon has-float-label" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 10px; margin-top: 0px;">
                                                <label class="cd-label" for="TxIdParqueadero">Numero Parqueadero</label>
                                                <asp:TextBox ID="TxIdParqueadero" runat="server" placeholder="Numero Parqueadero" CssClass="Sedes" AutoPostBack="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-2 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: -1px;">
                                            <asp:Button ID="BtnAgregarParqueadero" runat="server" Text="OK" CssClass="btn btn-outline-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                                        </div>

                                        <div class="col-md-12 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
                                            <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered table-hover table-responsive-md table-condensed text-center" Style="font-size: 8pt;">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Image" CommandName="Actualizar" ImageUrl="~/Diseno/Imagenes/Eliminar_Material.svg" />
                                                </Columns>
                                                <FooterStyle />
                                                <HeaderStyle CssClass="bg-info border text-center" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <RowStyle CssClass="border" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                            </asp:GridView>
                                        </div>

                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <div class="col-md-12 mb-3">
                                <asp:Button ID="BtGuardar" runat="server" Text="ASIGNAR" CssClass="btn btn-primary" Style="background-color: #1e2833; border-color: #1e2833;" />
                                <asp:Timer ID="Timer1" runat="server" Interval="999999999"></asp:Timer>
                            </div>
                        </asp:Panel>

                    </div>

                </div>

            </div>

        </div>

        <div class="col-md-7 mb-3" style="padding-top: 0px; padding-bottom: 0px; margin-bottom: 0px; margin-top: 0px;">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="row" style="text-align: center; padding: 0px; margin: 0px">

                        <div class="col-lg-12 col-md-12">
                            <div class="content">
                                <div class="container-fluid">
                                    <div class="card">
                                        <div class="card-header card-header-primary" style="background: #1e2833;">
                                            <ul class="nav nav-tabs md-tabs" id="myTabEx" role="tablist">
                                                <li class="nav-item" style="margin-bottom: 10px; display: flex; align-items: center;">
                                                    <a style="font-size: 11pt; color: white; display: flex; align-items: center;">
                                                        <i class="material-icons">apartment</i>INMUEBLES
                                                    </a>
                                                </li>
                                            </ul>
                                            <p class="card-category" style="text-align: right">
                                                <i class="material-icons" style="color: white; position: absolute; margin-top: 2px; margin-left: -25px;">search</i>
                                                <asp:TextBox ID="TxBuscar" runat="server" placeholder="Buscar Inmueble" AutoPostBack="true" Width="30%" Font-Size="11pt" BackColor="White"></asp:TextBox>
                                            </p>
                                        </div>
                                        <div class="card-body table-responsive">

                                            <div class="col-md-12 mb-3 scrollbar" style="padding-right: 0px; padding-left: 0px;">

                                                <asp:ListView ID="Lvinmuebles" runat="server" DataSourceID="SqlInmuebles">

                                                    <EmptyDataTemplate>
                                                        <table runat="server" style="">
                                                            <tr>
                                                                <td>No se han devuelto datos.</td>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>

                                                    <ItemTemplate>
                                                        <tr style="">
                                                            <asp:Label ID="Id_SedeLabel" runat="server" Text='<%# Eval("Id_Sede") %>' />
                                                            <td style="vertical-align: middle;" class="border">
                                                                <asp:LinkButton ID="Editar" runat="server" CommandName="Editar"><i class="material-icons" style="color: #1e2833;">create</i></asp:LinkButton>
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Id_inmuebleLabel" runat="server" Text='<%# Eval("Id_inmueble") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Tipo_InmuebleLabel" runat="server" Text='<%# Eval("Tipo_Inmueble") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="TelefonoLabel" runat="server" Text='<%# Eval("Telefono") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Nombre_SedeLabel" runat="server" Text='<%# Eval("Nombre_Sede") %>' />
                                                            </td>
                                                            <td style="vertical-align: middle" class="border">
                                                                <asp:Label ID="Id_ParqueaderoLabel" runat="server" Text='<%# Eval("Id_Parqueadero") %>' />
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
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">ID INMUEBLE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">TIPO INMUEBLE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">TELEFONO</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 30%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">SEDE</th>
                                                                            <th runat="server" style="border: thin solid #FFFFFF; padding: 5px; vertical-align: middle; text-align: center; width: 20%; background-color: #3c60af; color: white; font-size: 8pt; height: 20px">PARQUEADERO</th>
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


                                                <asp:SqlDataSource ID="SqlInmuebles" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT I.Id_inmueble, I.Id_Tipo_Inmueble, T.Tipo_Inmueble, I.Telefono, I.Id_Sede, S.Nombre_Sede, IP.Id_Parqueadero FROM Adm_Inmueble I LEFT JOIN Adm_Tipo_Inmueble T ON T.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble LEFT JOIN Adm_Inmueble_Parqueadero IP ON IP.Id_Inmueble = I.Id_inmueble LEFT JOIN Adm_Sedes S ON S.Id_Sede = I.Id_Sede ORDER BY Id_inmueble ASC"></asp:SqlDataSource>

                                                <asp:SqlDataSource ID="Sql_Buscar_Inmueble" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT I.Id_inmueble, I.Id_Tipo_Inmueble, T.Tipo_Inmueble, I.Telefono, I.Id_Sede, S.Nombre_Sede, IP.Id_Parqueadero FROM Adm_Inmueble I LEFT JOIN Adm_Tipo_Inmueble T ON T.Id_Tipo_Inmueble = I.Id_Tipo_Inmueble LEFT JOIN Adm_Inmueble_Parqueadero IP ON IP.Id_Inmueble = I.Id_inmueble LEFT JOIN Adm_Sedes S ON S.Id_Sede = I.Id_Sede WHERE I.Id_inmueble LIKE '%' + @Inmueble + '%' ORDER BY Id_inmueble ASC">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="TxBuscar" Name="Inmueble" PropertyName="Text" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
    
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div class="modal fade" id="Confirmacion" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="text-align: center">
            <div class="modal-dialog" role="document" style="text-align: center">
                <div class="modal-content" style="text-align: center">
                    <div class="modal-header" style="text-align: center">
                        <h5 class="modal-title" id="MensajeModalLabel" style="text-align: center">BORRAR PARQUEADERO DE INMUEBLE</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body" style="text-align: center">
                        <asp:Label ID="Label1" runat="server" Text="OJO: ¿DESEA BORRAR EL PARQUEADERO DEL INMUEBLE?"></asp:Label>
                    </div>
                    <div class="modal-footer" style="text-align: center">
                        <asp:Button ID="BtAceptar_Borrar" runat="server" Text="Aceptar" class="btn btn-danger" BackColor="IndianRed" ForeColor="White" Width="160px" Font-Size="11pt" />
                        <asp:Button ID="BtCancelar_Borrar" runat="server" Text="Cancelar" class="btn btn-primary" ForeColor="White" Width="160px" Font-Size="11pt" />
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
</asp:Content>




