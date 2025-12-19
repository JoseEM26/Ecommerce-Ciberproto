using Grpc.Net.Client;
using GrpcService1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class ProductoController : Controller
    {
        private readonly string apiUrl = "https://localhost:7236/api/";

        public async Task<IActionResult> Index(string nombre, int? idMarca, int? idCategoria)
        {
            await CargarCombos();

            List<ProductoModel> lista = new List<ProductoModel>();

            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(apiUrl);

                var response = await api.GetAsync("Producto/Listar");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    
                    lista = JsonConvert.DeserializeObject<List<ProductoModel>>(json);
                }
            }

            
            if (!string.IsNullOrEmpty(nombre))
            {
                lista = lista.Where(p => p.Nombre.ToLower().Contains(nombre.ToLower())).ToList();
            }
            if (idMarca.HasValue)
            {
                lista = lista.Where(p => p.IdMarca == idMarca.Value).ToList();
            }
            if (idCategoria.HasValue)
            {
                lista = lista.Where(p => p.IdCategoria == idCategoria.Value).ToList();
            }

            ViewBag.NombreActual = nombre;
            ViewBag.MarcaActual = idMarca;
            ViewBag.CategoriaActual = idCategoria;

            return View(lista);
        }

        
        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoModel model)
        {
            if (ModelState.IsValid)
            {
                using (var api = new HttpClient())
                {
                    api.BaseAddress = new Uri(apiUrl);
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response = await api.PostAsync("Producto/Guardar", content);

                    if (response.IsSuccessStatusCode) return RedirectToAction("Index");
                    else ViewBag.Mensaje = "Error al conectar con la API";
                }
            }
            await CargarCombos();
            return View(model);
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            ProductoModel model = new ProductoModel();
            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(apiUrl);
                // Llamamos al GET de la API para obtener los datos actuales
                var response = await api.GetAsync($"Producto/Obtener/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProductoModel>(json);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            await CargarCombos();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductoModel model)
        {
            if (ModelState.IsValid)
            {
                using (var api = new HttpClient())
                {
                    api.BaseAddress = new Uri(apiUrl);
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                    // PUT a la API
                    var response = await api.PutAsync("Producto/Editar", content);

                    if (response.IsSuccessStatusCode) return RedirectToAction("Index");
                    else ViewBag.Mensaje = "No se pudo actualizar el producto.";
                }
            }
            await CargarCombos();
            return View(model);
        }

       
        public async Task<IActionResult> Delete(int id)
        {
            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(apiUrl);
                var response = await api.DeleteAsync($"Producto/Eliminar/{id}");

                
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Activar(int id)
        {
            ProductoModel model = new ProductoModel();
            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(apiUrl); 
                var response = await api.GetAsync($"Producto/Obtener/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProductoModel>(json);
                }
            }

            model.Activo = true;

            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(apiUrl);
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                await api.PutAsync("Producto/Editar", content);
            }

            return RedirectToAction("Index");
        }

        private async Task CargarCombos()
        {
            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(apiUrl);

                // Marcas
                var resMarca = await api.GetAsync("Marca/Listar");
                if (resMarca.IsSuccessStatusCode)
                {
                    var lista = JsonConvert.DeserializeObject<List<MarcaCombo>>(await resMarca.Content.ReadAsStringAsync());
                    ViewBag.Marcas = new SelectList(lista, "IdMarca", "Descripcion");
                }

                // Categorías
                var resCat = await api.GetAsync("Categoria/Listar");
                if (resCat.IsSuccessStatusCode)
                {
                    var lista = JsonConvert.DeserializeObject<List<CategoriaCombo>>(await resCat.Content.ReadAsStringAsync());
                    ViewBag.Categorias = new SelectList(lista, "IdCategoria", "Descripcion");
                }
            }
        }
    }
}
