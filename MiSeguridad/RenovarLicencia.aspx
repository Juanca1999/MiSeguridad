﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RenovarLicencia.aspx.vb" Inherits="MiSeguridad.RenovarLicencia" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Renovar Licencia</title>
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap" rel="stylesheet">
    <link rel="shortcut icon" href="/../../Diseno\Imagenes\ICONO1.ICO" />
    <style>
        body {
            font-family: 'Roboto', sans-serif;
            background-color: #f4f4f9;
            color: #333;
            margin: 0;
            padding: 0;
        }
        .container {
            width: 100%;
            max-width: 500px;
            margin: 50px auto;
            padding: 20px;
            background-color: white;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            border-radius: 10px;
        }
        h1 {
            text-align: center;
            color: #4c54af;
            margin-bottom: 20px;
        }
        p {
            text-align: center;
            font-size: 18px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Licencia Expirada</h1>
            <p>Tu licencia ha expirado. Para continuar utilizando la aplicación, debes renovarla.</p>
        </div>
    </form>
</body>
</html>