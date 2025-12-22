using Microsoft.AspNetCore.Mvc;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class TiendaController : Controller
    {
        public IActionResult Index()
        {
            var productos = new List<ProductoModel>
            {
                new ProductoModel
                {
                    IdProducto = 1,
                    Nombre = "Mouse gamer",
                    Precio = 3500,
                    Stock = 5,
                    UrlImagen = "/img/mouse1.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 2,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse2.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 3,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse3.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 4,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse4.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 5,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse5.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 6,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse6.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 7,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse7.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 8,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse8.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 9,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse9.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 10,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/img/mouse10.jpg"
                },
            };

            return View(productos);
        }
    }
}