using Microsoft.AspNetCore.Mvc;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult Index(int? idCategoria, int? idMarca)
        {

            var categorias = new List<CategoriaModel>
            {
                new CategoriaModel { IdCategoria = 1, Descripcion = "Laptops", Activo = true },
                new CategoriaModel { IdCategoria = 2, Descripcion = "Periféricos", Activo = true },
                new CategoriaModel { IdCategoria = 3, Descripcion = "Componentes", Activo = true },
                new CategoriaModel { IdCategoria = 4, Descripcion = "Audio", Activo = true }
            };

            // Datos mock de marcas
            var marcas = new List<MarcaModel>
            {
                new MarcaModel { IdMarca = 1, Descripcion = "Logitech", Activo = true },
                new MarcaModel { IdMarca = 2, Descripcion = "Razer", Activo = true },
                new MarcaModel { IdMarca = 3, Descripcion = "ASUS", Activo = true },
                new MarcaModel { IdMarca = 4, Descripcion = "HyperX", Activo = true },
                new MarcaModel { IdMarca = 5, Descripcion = "Corsair", Activo = true }
            };

            // por ahora mock o datos que ya consumes
            var productos = new List<ProductoModel>
            {
                new ProductoModel
                {
                    IdProducto = 1,
                    Nombre = "Laptop Gamer ROG",
                    Precio = 3500,
                    Stock = 10,
                    UrlImagen = "/img/laptop.png",
                    IdCategoria = 1,
                    IdMarca = 3,
                    NombreCategoria = "Laptops",
                    NombreMarca = "ASUS"
                },
                new ProductoModel
                {
                    IdProducto = 2,
                    Nombre = "Mouse Gamer G502",
                    Precio = 150,
                    Stock = 25,
                    UrlImagen = "/img/mouse.png",
                    IdCategoria = 2,
                    IdMarca = 1,
                    NombreCategoria = "Periféricos",
                    NombreMarca = "Logitech"
                },
                new ProductoModel
                {
                    IdProducto = 3,
                    Nombre = "Mouse Razer DeathAdder",
                    Precio = 180,
                    Stock = 15,
                    UrlImagen = "/img/mouse.png",
                    IdCategoria = 2,
                    IdMarca = 2,
                    NombreCategoria = "Periféricos",
                    NombreMarca = "Razer"
                },
                new ProductoModel
                {
                    IdProducto = 4,
                    Nombre = "Teclado Mecánico K70",
                    Precio = 250,
                    Stock = 20,
                    UrlImagen = "/img/keyboard.png",
                    IdCategoria = 2,
                    IdMarca = 5,
                    NombreCategoria = "Periféricos",
                    NombreMarca = "Corsair"
                },
                new ProductoModel
                {
                    IdProducto = 5,
                    Nombre = "Audífonos Cloud II",
                    Precio = 200,
                    Stock = 30,
                    UrlImagen = "/img/headset.png",
                    IdCategoria = 4,
                    IdMarca = 4,
                    NombreCategoria = "Audio",
                    NombreMarca = "HyperX"
                },
                new ProductoModel
                {
                    IdProducto = 6,
                    Nombre = "Laptop TUF Gaming",
                    Precio = 2800,
                    Stock = 8,
                    UrlImagen = "/img/laptop.png",
                    IdCategoria = 1,
                    IdMarca = 3,
                    NombreCategoria = "Laptops",
                    NombreMarca = "ASUS"
                },
                new ProductoModel
                {
                    IdProducto = 7,
                    Nombre = "Tarjeta Gráfica RTX 4070",
                    Precio = 1500,
                    Stock = 5,
                    UrlImagen = "/img/gpu.png",
                    IdCategoria = 3,
                    IdMarca = 3,
                    NombreCategoria = "Componentes",
                    NombreMarca = "ASUS"
                },
                new ProductoModel
                {
                    IdProducto = 8,
                    Nombre = "Mouse Inalámbrico G305",
                    Precio = 120,
                    Stock = 40,
                    UrlImagen = "/img/mouse.png",
                    IdCategoria = 2,
                    IdMarca = 1,
                    NombreCategoria = "Periféricos",
                    NombreMarca = "Logitech"
                }
            };

            var productosFiltrados = productos.AsQueryable();

            if (idCategoria.HasValue)
            {
                productosFiltrados = productosFiltrados.Where(p => p.IdCategoria == idCategoria.Value);
            }

            if (idMarca.HasValue)
            {
                productosFiltrados = productosFiltrados.Where(p => p.IdMarca == idMarca.Value);
            }

            // Pasar datos a la vista
            ViewBag.Categorias = categorias;
            ViewBag.Marcas = marcas;
            ViewBag.CategoriaSeleccionada = idCategoria;
            ViewBag.MarcaSeleccionada = idMarca;

            return View(productosFiltrados.ToList());
        }
    }
}
