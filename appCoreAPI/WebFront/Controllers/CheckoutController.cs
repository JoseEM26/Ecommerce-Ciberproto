using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CheckoutController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var model = new CheckoutModel();

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Obtener items del carrito
                var carritoResponse = await client.GetAsync($"api/Carrito/ListarPorUsuario/{idUsuario}");
                if (carritoResponse.IsSuccessStatusCode)
                {
                    var content = await carritoResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };
                    model.ItemsCarrito = JsonSerializer.Deserialize<List<CarritoModel>>(content, options) ?? new List<CarritoModel>();
                }

                // Si el carrito está vacío, redirigir
                if (!model.ItemsCarrito.Any())
                {
                    TempData["Error"] = "Tu carrito está vacío";
                    return RedirectToAction("Index", "Carrito");
                }

                // Calcular total
                model.TotalCompra = model.ItemsCarrito.Sum(item => item.Subtotal);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar datos: {ex.Message}";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarPago(CheckoutModel model)
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!ModelState.IsValid)
            {
                // Recargar items del carrito si hay errores
                try
                {
                    var client = _httpClientFactory.CreateClient("ApiClient");
                    var carritoResponse = await client.GetAsync($"api/Carrito/ListarPorUsuario/{idUsuario}");
                    if (carritoResponse.IsSuccessStatusCode)
                    {
                        var content = await carritoResponse.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = null
                        };
                        model.ItemsCarrito = JsonSerializer.Deserialize<List<CarritoModel>>(content, options) ?? new List<CarritoModel>();
                        model.TotalCompra = model.ItemsCarrito.Sum(item => item.Subtotal);
                    }
                }
                catch { }

                return View("Index", model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Generar ID de transacción único
                var idTransaccion = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{idUsuario}";

                // Preparar request
                var request = new ProcesarVentaRequest
                {
                    IdUsuario = idUsuario.Value,
                    IdTarjeta = null, // Puedes agregar selección de tarjeta después
                    Contacto = model.Contacto,
                    Telefono = model.Telefono,
                    Direccion = model.Direccion,
                    IdTransaccion = idTransaccion
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = null
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Llamar al endpoint ProcesarVenta
                var response = await client.PostAsync("api/Venta/ProcesarVenta", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ProcesarVentaResponse>(responseContent, options);

                    // Actualizar contador del carrito a 0
                    HttpContext.Session.SetInt32("CarritoCantidad", 0);

                    // Guardar datos de la venta en TempData para la vista de confirmación
                    TempData["IdVenta"] = resultado?.IdVentaGenerado.ToString();
                    TempData["MontoTotal"] = resultado?.MontoTotal.ToString("F2");
                    TempData["TotalProductos"] = resultado?.TotalProductos.ToString();
                    TempData["IdTransaccion"] = idTransaccion;

                    return RedirectToAction("Confirmacion");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al procesar la venta: {errorContent}";

                    // Recargar carrito
                    var carritoResponse = await client.GetAsync($"api/Carrito/ListarPorUsuario/{idUsuario}");
                    if (carritoResponse.IsSuccessStatusCode)
                    {
                        var carritoContent = await carritoResponse.Content.ReadAsStringAsync();
                        model.ItemsCarrito = JsonSerializer.Deserialize<List<CarritoModel>>(carritoContent, options) ?? new List<CarritoModel>();
                        model.TotalCompra = model.ItemsCarrito.Sum(item => item.Subtotal);
                    }

                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View("Index", model);
            }
        }

        public IActionResult Confirmacion()
        {
            // Verificar que venimos de un pago exitoso
            if (TempData["IdVenta"] == null)
            {
                return RedirectToAction("Index", "Tienda");
            }

            return View();
        }
    }
}