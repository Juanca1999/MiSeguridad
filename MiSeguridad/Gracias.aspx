<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Gracias.aspx.vb" Inherits="MiSeguridad.Gracias" %>

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

        <div class="container mt-5">
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <br />
                    <br />
                    <br />
                    <h5 style="text-align: center; color: black; font-size: 20pt">Gracias por su preregistro</h5>
                    <br />
                    <br />
                    <div style="display: flex; justify-content: center; margin-top: -10px;">
                        <img id="Logo" runat="server" src="/../../../Adjunto/Logo/Logo.png" style="width: 200px;" />
                    </div>
                </div>
            </div>
        </div>

        <script src="/Diseno/jquery.min.js-2.js"></script>
    </form>
</body>
</html>




