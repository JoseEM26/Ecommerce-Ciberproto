using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CarritoController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

            var carrito = new List<CarritoModel>();

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync($"api/Carrito/ListarPorUsuario/{idUsuario}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };

                    carrito = JsonSerializer.Deserialize<List<CarritoModel>>(content, options) ?? new List<CarritoModel>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar el carrito: {ex.Message}";
            }

            return View(carrito);
        }

        public async Task<IActionResult> Agregar(
            int idProducto,
            string nombre,
            decimal precio,
            int stock,
            string? descripcion = null,
            string? imagen = null,
            string? returnUrl = null)
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");

            Console.WriteLine($"=== DEBUG AGREGAR ===");
            Console.WriteLine($"IdUsuario: {idUsuario}");
            Console.WriteLine($"IdProducto: {idProducto}");
            Console.WriteLine($"Nombre: {nombre}");
            Console.WriteLine($"Precio: {precio}");

            if (idUsuario == null)
            {
                Console.WriteLine("ERROR: Usuario no logueado");
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Verificar si el producto ya está en el carrito
                var checkResponse = await client.GetAsync($"api/Carrito/ObtenerPorUsuarioProducto?idUsuario={idUsuario}&idProducto={idProducto}");

                if (checkResponse.IsSuccessStatusCode)
                {
                    // Producto ya existe, actualizar cantidad
                    var content = await checkResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Producto existe, contenido: {content}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };

                    var itemExistente = JsonSerializer.Deserialize<CarritoModel>(content, options);

                    if (itemExistente != null && itemExistente.TieneStock)
                    {
                        itemExistente.Cantidad++;
                        var jsonUpdate = JsonSerializer.Serialize(itemExistente);

                        var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");
                        await client.PutAsync("api/Carrito/Actualizar", contentUpdate);
                    }
                }
                else
                {
                    // Producto no existe, agregar nuevo
                    var nuevoItem = new CarritoModel
                    {
                        IdUsuario = idUsuario.Value,
                        IdProducto = idProducto,
                        Cantidad = 1,
                        NombreProducto = nombre,
                        DescripcionProducto = descripcion ?? "",
                        PrecioProducto = precio,
                        Stock = stock,
                        ImagenProducto = imagen
                    };

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };


                    var json = JsonSerializer.Serialize(nuevoItem, options);
                    var contentPost = new StringContent(json, Encoding.UTF8, "application/json");
                    var addResponse = await client.PostAsync("api/Carrito/Agregar", contentPost);

                    if (!addResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await addResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error al agregar: {errorContent}");
                    }
                }

                // Actualizar contador de sesión
                var countResponse = await client.GetAsync($"api/Carrito/ContarItemsUsuario/{idUsuario}");
                if (countResponse.IsSuccessStatusCode)
                {
                    var countContent = await countResponse.Content.ReadAsStringAsync();
                    var cantidad = JsonSerializer.Deserialize<int>(countContent);
                    HttpContext.Session.SetInt32("CarritoCantidad", cantidad);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"ERROR EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
            }

            // Redirigir a donde vino
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Tienda");
        }

        public async Task<IActionResult> ActualizarCantidad(int idProducto, int cambio, string? returnUrl = null)
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Obtener el item del carrito
                var response = await client.GetAsync($"api/Carrito/ObtenerPorUsuarioProducto?idUsuario={idUsuario}&idProducto={idProducto}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };
                    var item = JsonSerializer.Deserialize<CarritoModel>(content, options);

                    if (item != null)
                    {
                        int nuevaCantidad = item.Cantidad + cambio;

                        // Si la cantidad llega a 0 o menos, eliminar
                        if (nuevaCantidad <= 0)
                        {
                            await client.DeleteAsync($"api/Carrito/EliminarPorUsuarioProducto?idUsuario={idUsuario}&idProducto={idProducto}");
                        }
                        else
                        {
                            // No exceder el stock
                            if (nuevaCantidad > item.Stock)
                            {
                                nuevaCantidad = item.Stock ?? 0;
                            }

                            // Actualizar usando el endpoint correcto
                            var updateResponse = await client.PutAsync($"api/Carrito/ActualizarCantidad?idCarrito={item.IdCarrito}&cantidad={nuevaCantidad}", null);

                            if (!updateResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Error al actualizar: {updateResponse.StatusCode}");
                            }
                        }

                        // Actualizar contador de sesión
                        var countResponse = await client.GetAsync($"api/Carrito/ContarItemsUsuario/{idUsuario}");
                        if (countResponse.IsSuccessStatusCode)
                        {
                            var countContent = await countResponse.Content.ReadAsStringAsync();
                            var cantidad = JsonSerializer.Deserialize<int>(countContent);
                            HttpContext.Session.SetInt32("CarritoCantidad", cantidad);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarCantidad: {ex.Message}");
            }

            // Redirigir a donde vino
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Eliminar(int idProducto)
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                await client.DeleteAsync($"api/Carrito/EliminarPorUsuarioProducto?idUsuario={idUsuario}&idProducto={idProducto}");

                // Actualizar contador
                var countResponse = await client.GetAsync($"api/Carrito/ContarItemsUsuario/{idUsuario}");
                if (countResponse.IsSuccessStatusCode)
                {
                    var countContent = await countResponse.Content.ReadAsStringAsync();
                    var cantidad = JsonSerializer.Deserialize<int>(countContent);
                    HttpContext.Session.SetInt32("CarritoCantidad", cantidad);
                }
            }
            catch (Exception ex)
            {
                // Log error
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Vaciar()
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                await client.DeleteAsync($"api/Carrito/VaciarCarritoUsuario/{idUsuario}");

                HttpContext.Session.SetInt32("CarritoCantidad", 0);
            }
            catch (Exception ex)
            {
                // Log error
            }

            return RedirectToAction("Index");
        }
    }
}