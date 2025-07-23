<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Firma.aspx.vb" Inherits="MiroOptimus.Firma" %>

<!DOCTYPE html>

<html xmlns="https://www.w3.org/1999/xhtml">
<head runat="server">
<meta https-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>1</title>
      <link rel="stylesheet" href="Firma.css" />
</head>
<body>
    <form id="form1" runat="server">

	<div class="contenedor">
		<div class="row">
			<div class="col-md-12">
		 		<canvas id="draw-canvas" width="620" height="360">
		 			No tienes un buen navegador.
		 		</canvas>
		 	</div>
		</div>
	<div class="row">
		<div class="col-md-12">
				<input type="button" class="button" id="draw-submitBtn" value="Guardar"/>
				<input type="button" class="button" id="draw-clearBtn" value="Borrar Firma"/>
						<input type="color" id="color" style="visibility:hidden"/>
						<input type="range" id="puntero" min="1" default="1" max="5" width="10%" style="visibility:hidden" />
			</div>
		</div>
	</div>
    </form>
  <script src="Firma.js"></script>
</body>
</html>
