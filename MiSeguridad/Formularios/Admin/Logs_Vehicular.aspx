<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Logs_Vehicular.aspx.vb" Inherits="MiSeguridad.Logs_Vehicular" %>

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
        /* Animación de entrada con rebote suave */
        @keyframes fade-in-up {
            0% {
                opacity: 0;
                transform: translateY(30px) scale(0.9);
            }

            60% {
                opacity: 1;
                transform: translateY(-5px) scale(1.03);
            }

            100% {
                transform: translateY(0) scale(1);
            }
        }

        h1 {
            color: #343a40;
            font-weight: bold;
            margin-bottom: 2rem;
        }

        /* Efecto de resalte para logs nuevos */
        .evento-card.nuevo {
            box-shadow: 0 0 20px rgba(4, 66, 166, 0.9);
            animation: fade-in-up 0.7s ease-out;
            transition: box-shadow 0.5s ease-in-out;
        }

            /* Suaviza la transición al quitar el resalte */
            .evento-card.nuevo.fade-out {
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
                transition: box-shadow 0.8s ease-out;
            }

        /* Estilo base de las tarjetas */
        /* Tarjeta de evento */
        .evento-card {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px;
            margin-bottom: 10px;
            background-color: #ffffff;
            border: 1px solid #9da8b3;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
            transition: transform 0.2s, box-shadow 0.2s;
        }

            /* Estilo para filas impares (gris) */
            .evento-card:nth-child(odd) {
                background-color: #e1e4e7; /* Color gris claro */
            }

            /* Estilo para filas pares (blanco) */
            .evento-card:nth-child(even) {
                background-color: #ffffff; /* Color blanco */
            }

            .evento-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            }

            /* Texto del evento */
            .evento-card h5 {
                margin: 0;
                font-size: 10.0rem;
                color: #495057;
                font-weight: 600;
            }

            .evento-card small {
                font-size: 9.5rem;
                color: #6c757d;
            }

        /* Personalización de scrollbar */
        #eventos-container-izquierda::-webkit-scrollbar,
        #eventos-container-derecha::-webkit-scrollbar {
            width: 6px;
        }

        #eventos-container-izquierda::-webkit-scrollbar-thumb,
        #eventos-container-derecha::-webkit-scrollbar-thumb {
            background: linear-gradient(180deg, #495057, #6c757d);
            border-radius: 10px;
        }

        /* Diseño responsivo para móviles */
        @media (max-width: 768px) {
            .row {
                flex-direction: column;
            }

            .col-md-6 {
                width: 100%;
            }
        }

        @media (min-width: 992px) {
            .container {
                max-width: 90%
            }
        }

        .custom-card {
            background: #ffffff;
            border-radius: 20px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
            padding: 20px 20px;
            max-width: 500px;
            margin: 60px auto;
            font-family: 'Segoe UI', sans-serif;
        }

            .custom-card h4 {
                color: #0442A6;
                font-weight: bold;
                margin-bottom: 20px;
            }

        .custom-dropdown {
            width: 100%;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 8px;
            background-color: #f9f9f9;
            font-size: 16px;
            color: #333;
            transition: all 0.3s ease;
        }

            .custom-dropdown:focus {
                border-color: #0442A6;
                outline: none;
                background-color: #fff;
            }

        .custom-button {
            background-color: #0442A6;
            border: none;
            color: #fff;
            padding: 10px 30px;
            border-radius: 25px;
            font-size: 16px;
            font-weight: 500;
            transition: background-color 0.3s ease;
        }

            .custom-button:hover {
                background-color: #033185;
            }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <asp:Panel ID="PanelSeleccion" runat="server">
            <div class="custom-card">
                <h4 class="text-center">Seleccione una Escuela</h4>
                <div class="mb-4 text-center">
                    <asp:DropDownList ID="TxEscuela" runat="server" CssClass="custom-dropdown" AppendDataBoundItems="true" DataSourceID="SqlEscuela" DataTextField="Escuela" DataValueField="Id_Escuela">
                        <asp:ListItem Value="">Escuela</asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlEscuela" runat="server" ConnectionString="<%$ ConnectionStrings:MiSeguridadConnectionString %>" SelectCommand="SELECT Id_Escuela, Escuela FROM Escuela ORDER BY Escuela"></asp:SqlDataSource>
                </div>
                <div class="text-center">
                    <asp:Button ID="btnContinuar" runat="server" CssClass="custom-button" Text="Continuar" OnClientClick="return validarSeleccionEscuela();" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="PanelContenido" runat="server" Visible="false">

            <nav class="navbar navbar-expand-lg navbar-light" style="background-color: #0442A6 !important;">
                <div class="container-fluid d-flex justify-content-between align-items-center">
                    <!-- Contenedor del título y logo -->
                    <div class="d-flex align-items-center text-center flex-grow-1 justify-content-center">
                        <span class="navbar-brand text-wrap m-0"
                            style="font-weight: bold; font-size: 16px; color: white; padding-right: 15px;">LLEGADA DE VEHICULOS
                            <asp:Label ID="EscuelaLabel" runat="server" Text="" />
                        </span>
                        <img src="/../../../Diseno/Imagenes/miro.png" class="img-fluid ms-2" alt="Logo" style="width: 35px;">
                    </div>
                </div>
            </nav>

            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="row">
                        <div class="container">
                            <div class="row">
                                <div class="col-md-12">
                                    <div id="eventos-container" class="list-group">
                                        <!-- Todos los eventos se mostrarán aquí -->
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                </ContentTemplate>
            </asp:UpdatePanel>

            <!-- Botón flotante para activar voz -->
            <div id="alerta-voz" onclick="activarVoz()" style="position: fixed; bottom: 20px; right: 20px; background-color: #007bff; color: white; padding: 12px 20px; border-radius: 30px; box-shadow: 0 4px 6px rgba(0,0,0,0.2); cursor: pointer; z-index: 9999; transition: all 0.3s ease; font-family: sans-serif; font-size: 14px;">
                🔊 Activar notificaciones por voz
            </div>

        </asp:Panel>

        <script src="Diseno/jquery-3.6.0.min.js"></script>

        <script>

            let vozActivada = false;

            function activarVoz() {
                vozActivada = true;
                localStorage.setItem("vozActivada", "true");
                hablarTexto("Notificaciones por voz activadas.");
                document.getElementById("alerta-voz").style.display = "none";
            }

            // Mostrar botón si la voz ya estaba activada antes, pero esta es una nueva sesión
            window.addEventListener("DOMContentLoaded", () => {
                if (localStorage.getItem("vozActivada") === "true") {
                    document.getElementById("alerta-voz").style.display = "block";

                    // Permitir que cualquier clic lo reactive automáticamente
                    document.addEventListener("click", () => {
                        if (!vozActivada) {
                            activarVoz();
                        }
                    }, { once: true });
                }
            });

            // Función para hablar texto
            function hablarTexto(texto) {
                if (!vozActivada) return;
                const u = new SpeechSynthesisUtterance(texto);
                u.lang = "es-ES";
                u.rate = 1;
                u.pitch = 1;
                speechSynthesis.speak(u);
            }

            // Variable para almacenar los logs ya mostrados
            let logsMostrados = [];

            // Función para agregar un evento a la lista
            function agregarEvento(placa, nombre, grado, esNuevo = false) {
                const eventosContainer = document.getElementById('eventos-container');
                const eventoCard = document.createElement('div');
                eventoCard.className = 'evento-card fade-in-up';

                // Solo si es nuevo, agregar la clase de resalte "nuevo"
                if (esNuevo) {
                    eventoCard.classList.add('nuevo');
                }

                eventoCard.innerHTML = `
                <div>
                    <h5>${nombre} - ${grado}</h5>
                    <small>Placa: ${placa}</small>
                </div>
            `;

                // Agregar el evento al principio del contenedor
                eventosContainer.insertBefore(eventoCard, eventosContainer.firstChild);

                // Eliminar la clase de animación después de 2 segundos solo si es nuevo
                if (esNuevo) {
                    setTimeout(() => {
                        eventoCard.classList.remove('fade-in-up');
                        eventoCard.classList.remove('nuevo');
                        eventoCard.classList.add('fade-out');
                    }, 2000);
                }
            }

            // Función para obtener logs desde el servidor
            function obtenerLogs(url, callback) {
                fetch(url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify({})
                })
                    .then(response => response.ok ? response.json() : Promise.reject('Error en la solicitud'))
                    .then(data => callback(data.d))
                    .catch(error => console.error('Error al obtener logs:', error));
            }

            // Función para cargar los logs iniciales SIN efectos visuales de nuevo
            function cargarLogsIniciales() {
                obtenerLogs('Logs_Vehicular.aspx/ObtenerUltimos14Logs', logs => {
                    logs.reverse().forEach(log => {
                        logsMostrados.unshift(log);
                        agregarEvento(log.Placa, log.Nombre_Asignado, log.Grado, false); // false para evitar el efecto
                    });
                });
            }

            // Función para obtener nuevos logs y aplicarle efectos solo a los nuevos
            function actualizarLogs() {
                obtenerLogs('Logs_Vehicular.aspx/ObtenerUltimosLogs', logs => {
                    let nuevosLogs = logs.filter(log => !logsMostrados.some(lm => lm.Id_Log_Vehicular === log.Id_Log_Vehicular));

                    nuevosLogs.reverse().forEach(log => {
                        logsMostrados.unshift(log);
                        agregarEvento(log.Placa, log.Nombre_Asignado, log.Grado, true); // true para los nuevos
                        // Reproducir el texto en voz
                        const texto = `${log.Nombre_Asignado}  ${log.Grado}.`;
                        hablarTexto(texto);
                    });

                    // Limitar la lista de logs a los últimos 20
                    if (logsMostrados.length > 20) logsMostrados = logsMostrados.slice(0, 20);

                });
            }

            // Iniciar la carga de logs al cargar la página
            document.addEventListener('DOMContentLoaded', () => {
                cargarLogsIniciales();
                setInterval(actualizarLogs, 4000);
            });

            // Funcion para hablar texto
            function hablarTexto(texto) {
                const utterance = new SpeechSynthesisUtterance(texto);
                utterance.lang = 'es-ES'; // Español
                utterance.rate = 0.8;       // Velocidad normal
                utterance.pitch = 1;      // Tono normal
                speechSynthesis.speak(utterance);
            }

            function validarSeleccionEscuela() {
                var ddl = document.getElementById('<%= TxEscuela.ClientID %>');
                if (ddl.value === "") {
                    alert("Debe seleccionar una escuela.");
                    return false; // Previene el postback
                }
                return true; // Permite el postback si la validación pasa
            }
        </script>
    </form>
</body>
</html>
