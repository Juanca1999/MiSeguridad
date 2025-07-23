<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Maestras/Maestra.Master" CodeBehind="Logs_Facial.aspx.vb" Inherits="MiSeguridad.Logs_Facial" %>

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
    <link rel="stylesheet" type="text/css" href="/Diseno/2019/sweetalert/sweetalert2.min.css">
    <script type="text/javascript" src="/Diseno/2019/sweetalert/sweetalert2.min.js"></script>

    <link rel="stylesheet" href="/Diseno/2019/SubMenu/css/reset.css">
    <!-- CSS reset -->
    <link rel="stylesheet" href="/Diseno/2019/SubMenu/css/style.css">
    <!-- Resource style -->
    <link rel="stylesheet" href="/Diseno/2019/Menu/css/style.css" />

    <style>
        .event-card {
        border: 1px solid #e0e0e0; /* Borde sutil */
        border-radius: 10px;
        background-color: white; /* Fondo blanco */
        margin-bottom: 15px;
        padding: 15px;
        position: relative;
        animation: fadeIn 0.5s ease-in-out;
    }

    .event-card::before {
        content: '';
        position: absolute;
        left: 34px;
        top: 50%;
        transform: translateY(-50%);
        width: 12px;
        height: 12px;
        border-radius: 50%;
        background-color: #4caf50; /* Color verde por defecto */
    }

    .event-card.salida::before {
        background-color: #f44336; /* Color rojo para salida */
    }

    .event-details {
        margin-left: 25px; /* Espacio para el círculo */
        color: #333; /* Color de texto más oscuro */
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
                <span class="navbar-brand" style="font-weight: bold; font-size: 24px; color: white;">ADMINISTRACION - LOGS DE ACCESOS</span>
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

    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="row" style="padding-top: 0px; margin-top: 0px;">

                <div class="container mt-5">
                    <h1 class="text-center mb-4">Eventos de Cámara Facial</h1>
                    <div class="row">
                        <div class="col-md-6 offset-md-3">
                            <div id="eventos-container">
                                <!-- Los eventos se mostrarán aquí -->
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="Diseno/jquery-3.6.0.min.js"></script>

    <script>
        // Variable para almacenar los logs ya mostrados
        let logsMostrados = [];

        // Función para obtener los últimos 10 logs desde el servidor
        function obtenerUltimos10Logs() {

            fetch('Logs_Facial.aspx/ObtenerUltimos10Logs', {
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
                        agregarEvento(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento);
                    });
                })
                .catch(error => {
                    console.error('Error al obtener los últimos 10 logs:', error); // Depuración
                });
        }

        // Función para obtener el último log desde el servidor
        function obtenerUltimoLog() {

            fetch('Logs_Facial.aspx/ObtenerUltimoLog', {
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

                        if (logsMostrados.length > 20) {
                            logsMostrados.pop(); // Eliminar el último elemento si hay más de 20
                        }

                        agregarEvento(log.Tipo_Evento.toLowerCase(), log.Nombres, log.Fecha_Hora_Evento);
                    }
                })
                .catch(error => {
                    console.error('Error al obtener el último log:', error); // Depuración
                });
        }

        // Función para agregar un evento a la lista
        function agregarEvento(tipo, nombre, hora) {
            const eventosContainer = document.getElementById('eventos-container');
            const eventoCard = document.createElement('div');
            eventoCard.className = `event-card p-3 ${tipo}`;

            const icono = tipo === 'entrada' ? 'fa-door-open' : 'fa-door-closed';
            eventoCard.innerHTML = `
            <div class="d-flex align-items-center">
                <i class="event-icon fas ${icono}"></i>
                <div class="event-details">
                    <h5 class="mb-0">${nombre}</h5>
                    <small>${tipo.toUpperCase()} - ${hora}</small>
                </div>
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
