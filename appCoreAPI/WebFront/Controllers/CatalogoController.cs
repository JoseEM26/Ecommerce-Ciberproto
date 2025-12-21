using Microsoft.AspNetCore.Mvc;
using WebFront.Models;
using System.Text.Json;

namespace WebFront.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CatalogoController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int? idCategoria, int? idMarca, int page = 1)
        {
            const int pageSize = 12;

            var productos = new List<ProductoModel>();
            var categorias = new List<CategoriaModel>();
            var marcas = new List<MarcaModel>();
            var carrito = new List<CarritoModel>();



            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var idUsuario = HttpContext.Session.GetInt32("UsuarioID");

                if (idUsuario.HasValue)
                {
                    var carritoResponse = await client.GetAsync($"api/Carrito/ListarPorUsuario/{idUsuario}");
                    if (carritoResponse.IsSuccessStatusCode)
                    {
                        var content = await carritoResponse.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = null
                        };
                        carrito = JsonSerializer.Deserialize<List<CarritoModel>>(content, options) ?? new List<CarritoModel>();
                    }
                }

                var productosResponse = await client.GetAsync("api/Producto/Listar");
                if (productosResponse.IsSuccessStatusCode)
                {
                    var content = await productosResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };
                    productos = JsonSerializer.Deserialize<List<ProductoModel>>(content, options) ?? new List<ProductoModel>();

                    // Filtrar solo activos
                    productos = productos.Where(p => p.Activo).ToList();
                }
                else
                {
                    ViewBag.Error = $"Error API: {productosResponse.StatusCode}";
                }

                // Obtener categorías
                var categoriasResponse = await client.GetAsync("api/Categoria/Listar");
                if (categoriasResponse.IsSuccessStatusCode)
                {
                    var content = await categoriasResponse.Content.ReadAsStringAsync();
                    categorias = JsonSerializer.Deserialize<List<CategoriaModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<CategoriaModel>();

                    categorias = categorias.Where(c => c.Activo).ToList();
                }

                // Obtener marcas
                var marcasResponse = await client.GetAsync("api/Marca/Listar");
                if (marcasResponse.IsSuccessStatusCode)
                {
                    var content = await marcasResponse.Content.ReadAsStringAsync();
                    marcas = JsonSerializer.Deserialize<List<MarcaModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<MarcaModel>();

                    marcas = marcas.Where(m => m.Activo).ToList();
                }

                // Aplicar filtros
                if (idCategoria.HasValue)
                {
                    productos = productos.Where(p => p.IdCategoria == idCategoria.Value).ToList();
                }

                if (idMarca.HasValue)
                {
                    productos = productos.Where(p => p.IdMarca == idMarca.Value).ToList();
                }
            }
            catch (Exception ex)
            {
                // Log del error
                ViewBag.Error = "Error al cargar los datos";
            }

            int totalProductos = productos.Count;
            int totalPaginas = (int)Math.Ceiling(totalProductos / (double)pageSize);

            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;

            productos = productos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pasar datos a la vista
            ViewBag.Categorias = categorias;
            ViewBag.Marcas = marcas;
            ViewBag.CategoriaSeleccionada = idCategoria;
            ViewBag.MarcaSeleccionada = idMarca;
            ViewBag.Carrito = carrito;
            return View(productos);
        }
    }
}