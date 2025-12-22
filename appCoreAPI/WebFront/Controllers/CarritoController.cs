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
                return RedirectToAction("LoginCliente", "UsuarioLogin");
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
                return RedirectToAction("LoginCliente", "UsuarioLogin");
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
            Console.WriteLine($"=== ACTUALIZAR CANTIDAD ===");
            Console.WriteLine($"IdProducto: {idProducto}, Cambio: {cambio}");

            var idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            Console.WriteLine($"IdUsuario: {idUsuario}");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Obtener el item del carrito
                var url = $"api/Carrito/ObtenerPorUsuarioProducto?idUsuario={idUsuario}&idProducto={idProducto}";
                Console.WriteLine($"URL GET: {url}");

                var response = await client.GetAsync(url);
                Console.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };
                    var item = JsonSerializer.Deserialize<CarritoModel>(content, options);

                    if (item != null)
                    {
                        Console.WriteLine($"Item.Cantidad actual: {item.Cantidad}");
                        int nuevaCantidad = item.Cantidad + cambio;
                        Console.WriteLine($"Nueva cantidad: {nuevaCantidad}");

                        // Si la cantidad llega a 0 o menos, eliminar
                        if (nuevaCantidad <= 0)
                        {
                            Console.WriteLine("Eliminando...");
                            await client.DeleteAsync($"api/Carrito/EliminarPorUsuarioProducto?idUsuario={idUsuario}&idProducto={idProducto}");
                        }
                        else
                        {
                            var updateUrl = $"api/Carrito/ActualizarCantidad?idCarrito={item.IdCarrito}&cantidad={nuevaCantidad}";
                            Console.WriteLine($"URL PUT: {updateUrl}");

                            var updateResponse = await client.PutAsync(updateUrl, null);
                            Console.WriteLine($"Update Status: {updateResponse.StatusCode}");

                            if (!updateResponse.IsSuccessStatusCode)
                            {
                                var error = await updateResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"Error: {error}");
                            }
                        }

                        // Actualizar contador de sesión
                        var countResponse = await client.GetAsync($"api/Carrito/ContarItemsUsuario/{idUsuario}");
                        if (countResponse.IsSuccessStatusCode)
                        {
                            var countContent = await countResponse.Content.ReadAsStringAsync();
                            var cantidad = JsonSerializer.Deserialize<int>(countContent);
                            HttpContext.Session.SetInt32("CarritoCantidad", cantidad);
                            Console.WriteLine($"Contador actualizado: {cantidad}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }

            Console.WriteLine($"=== FIN ===");

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