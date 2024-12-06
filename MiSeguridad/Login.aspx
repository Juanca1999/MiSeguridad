<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="MiSeguridad.Login" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mi Seguridad</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.3/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ionicons/2.0.1/css/ionicons.min.css">
    <link rel="shortcut icon" href="/../../Diseno\Imagenes\ICONO1.ICO" />

    <style>
        .login-dark {
            height: 700px;
            background: #475d62 url('https://img.freepik.com/fotos-premium/edificio-azul-ciudad-fondo_1359-2595.jpg');
            background-size: cover;
            position: relative;
        }

        .placeholder-white::placeholder {
            color: #b0b5bb
        }

        .login-dark form {
            max-width: 320px;
            width: 90%;
            background: rgba(30, 40, 51, 0.8);
            padding: 40px;
            border-radius: 4px;
            transform: translate(-50%, -50%);
            position: absolute;
            top: 50%;
            left: 50%;
            color: #fff;
            box-shadow: 3px 3px 4px rgba(0,0,0,0.2);
        }

        .login-dark .illustration {
            text-align: center;
            padding: 15px 0 20px;
            font-size: 100px;
            color: #2980ef;
        }

        .login-dark form .form-control {
            background: none;
            border: none;
            border-bottom: 1px solid #434a52;
            border-radius: 0;
            box-shadow: none;
            outline: none;
            color: inherit;
        }

        .login-dark form .btn-primary {
            background: #214a80;
            border: none;
            border-radius: 4px;
            padding: 11px;
            box-shadow: none;
            margin-top: 26px;
            text-shadow: none;
            outline: none;
        }

            .login-dark form .btn-primary:hover, .login-dark form .btn-primary:active {
                background: #214a80;
                outline: none;
            }

        .login-dark form .forgot {
            display: block;
            text-align: center;
            font-size: 12px;
            color: #6f7a85;
            opacity: 0.9;
            text-decoration: none;
        }

            .login-dark form .forgot:hover, .login-dark form .forgot:active {
                opacity: 1;
                text-decoration: none;
            }

        .login-dark form .btn-primary:active {
            transform: translateY(1px);
        }
    </style>

</head>
<body>
    <div class="login-dark">
        <form runat="server">
            <h2 class="sr-only">Login Form</h2>
            <div class="illustration">
                <div style="text-align: center">
                    <img src="/../../../Diseno/Imagenes/miro.png" style="height: 20vh; margin-bottom: 10px;" class="icon ion-ios-locked-outline">
                </div>
                <h3>
                    <asp:Label ID="Label4" runat="server" Text="Inicio De Sesión" CssClass="fw-normal mb-3 pb-3" Style="letter-spacing: 1px; color: #a7a6b1;"></asp:Label>
                </h3>
            </div>
            <div class="form-group">
                <asp:TextBox ID="TxNombreUsuario" runat="server" Placeholder="Usuario" CssClass="form-control placeholder-white"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:TextBox ID="TxContraseña" runat="server" TextMode="Password" Placeholder="Contraseña" CssClass="form-control placeholder-white"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Button ID="BtIniciar" runat="server" Text="Iniciar Sesion" class="btn btn-primary btn-block" />
            </div>
            <asp:CheckBox ID="chkPersistCookie" runat="server" Style="display: none" />
        </form>
        <!-- Footer -->
        <div style="position: fixed; bottom: 0; color: white; left: 25%;">
            <asp:Label ID="Label1" runat="server" Style="font-size: 0.8rem; text-align: center; padding: 10px;" Text="Copyright © 2024 | MIRO Seguridad | Todos los Derechos Reservados - Medellín - Colombia by MIRO SEGURIDAD LTDA"></asp:Label>
        </div>
    </div>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.3/js/bootstrap.bundle.min.js"></script>
</body>
</html>


