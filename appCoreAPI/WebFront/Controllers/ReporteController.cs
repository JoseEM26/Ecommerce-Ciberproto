using Grpc.Net.Client;
using GrpcService1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class ReporteController : Controller
    {
        private readonly string grpcUrl = "https://localhost:7247";

        public async Task<IActionResult> Index()
        {
            await CargarComboClientes();

            var canal = GrpcChannel.ForAddress(grpcUrl);
            var client = new ServicioVentas.ServicioVentasClient(canal);
            List<VentaModel> listaReporte = new List<VentaModel>();

            try
            {
                var respuestaGrpc = await client.GetAllAsync(new EmptyVenta());

                if (respuestaGrpc != null && respuestaGrpc.Items != null)
                {
                    foreach (var item in respuestaGrpc.Items)
                    {
                        listaReporte.Add(new VentaModel
                        {
                            IdVenta = item.IdVenta,
                            Cliente = item.NombreUsuario,
                            Producto = item.NombreProducto,
                            Cantidad = item.TotalProductos,
                            MontoTotal = decimal.TryParse(item.MontoTotal, out decimal monto) ? monto : 0,
                            Fecha = item.FechaVenta,
                            IdTransaccion = item.IdTransaccion,
                            Contacto = item.Contacto,
                            Telefono = item.Telefono,
                            Direccion = item.Direccion
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error inicial: " + ex.Message;
            }

            return View(listaReporte); 
        }


        [HttpGet] 
        public async Task<IActionResult> Filtrar(string tipoFiltro, string fechaInicio, string fechaFin, int? idUsuario, string nombreProducto)
        {
            var canal = GrpcChannel.ForAddress(grpcUrl);
            var client = new ServicioVentas.ServicioVentasClient(canal);

            Ventas respuestaGrpc = new Ventas();

            try
            {
                switch (tipoFiltro)
                {
                    case "fechas":
                        if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
                        {
                            var request = new Fechas { FechaInicio = fechaInicio, FechaFin = fechaFin };
                            respuestaGrpc = await client.GetBetweenFechasAsync(request);
                        }
                        break;

                    case "cliente":
                        if (idUsuario.HasValue)
                        {
                            var request = new ByUsuario { IdUsuario = idUsuario.Value };
                            respuestaGrpc = await client.GetByUsuarioAsync(request);
                        }
                        break;

                    case "producto":
                        if (!string.IsNullOrEmpty(nombreProducto))
                        {
                            var request = new ByProducto { NombreProducto = nombreProducto };
                            respuestaGrpc = await client.GetByProductoAsync(request);
                        }
                        break;

                    default:
                        respuestaGrpc = await client.GetAllAsync(new EmptyVenta());
                        break;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo conectar con el servicio de reportes: " + ex.Message;
            }

            
            List<VentaModel> listaReporte = new List<VentaModel>();

            if (respuestaGrpc != null && respuestaGrpc.Items != null)
            {
                foreach (var item in respuestaGrpc.Items)
                {
                    listaReporte.Add(new VentaModel
                    {
                        IdVenta = item.IdVenta,
                        Cliente = item.NombreUsuario,
                        Producto = item.NombreProducto,
                        Cantidad = item.TotalProductos,
                        MontoTotal = decimal.TryParse(item.MontoTotal, out decimal monto) ? monto : 0,
                        Fecha = item.FechaVenta, 
                        IdTransaccion = item.IdTransaccion,
                        Contacto = item.Contacto,
                        Telefono = item.Telefono,
                        Direccion = item.Direccion
                    });
                }
            }

            await CargarComboClientes(); 

            ViewBag.TipoFiltro = tipoFiltro;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.IdUsuario = idUsuario;
            ViewBag.NombreProducto = nombreProducto;

            return View("Index", listaReporte);
        }

        private async Task CargarComboClientes()
        {
            try
            {
                var canal = GrpcChannel.ForAddress(grpcUrl);
                var clientUsuarios = new ServicioUsuarios.ServicioUsuariosClient(canal);

                var respuesta = await clientUsuarios.GetAllAsync(new EmptyUsuario());

                var clientes = respuesta.Items
                    .Where(u => u.Rol == "CLIENTE")
                    .Select(u => new
                    {
                        Id = u.IdUsuario,
                        NombreCompleto = $"{u.Nombres} {u.Apellidos}"
                    })
                    .ToList();

                ViewBag.Clientes = new SelectList(clientes, "Id", "NombreCompleto");
            }
            catch
            {
                ViewBag.Clientes = new SelectList(new List<string>()); 
            }
        }

        public async Task<IActionResult> Inventario(string busqueda)
        {
            var canal = GrpcChannel.ForAddress(grpcUrl);
            var client = new ServicioProductos.ServicioProductosClient(canal);

            // Usamos la clase generada por el proto "Empty" o "EmptyProducto" 
            // (Revisa cómo se llama en tu productos.proto, usualmente es Empty)
            var respuesta = await client.GetAllAsync(new GrpcService1.Empty());

            List<ProductoModel> lista = new List<ProductoModel>();

            if (respuesta != null && respuesta.Items != null)
            {
                // Mapeamos de gRPC a nuestro Modelo
                foreach (var item in respuesta.Items)
                {
                    lista.Add(new ProductoModel
                    {
                        IdProducto = item.IdProducto,
                        Nombre = item.Nombre,
                        NombreMarca = item.Marca,
                        NombreCategoria = item.Categoria,
                        Precio = decimal.Parse(item.Precio),
                        Stock = item.Stock,
                        // Si agregaste Activo al proto, úsalo, si no, asume true
                        // Activo = item.Activo == "1" 
                    });
                }
            }

            // Filtro simple en memoria (opcional)
            if (!string.IsNullOrEmpty(busqueda))
            {
                lista = lista.Where(p => p.Nombre.ToLower().Contains(busqueda.ToLower()) ||
                                         p.NombreMarca.ToLower().Contains(busqueda.ToLower())).ToList();
                ViewBag.Busqueda = busqueda;
            }

            return View(lista);
        }


    }
}
