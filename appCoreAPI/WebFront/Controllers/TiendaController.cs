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
                    UrlImagen = "/imgs/mouse1.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 2,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse2.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 3,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse3.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 4,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse4.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 5,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse5.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 6,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse6.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 7,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse7.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 8,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse8.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 9,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse9.jpg"
                },
                new ProductoModel
                {
                    IdProducto = 10,
                    Nombre = "Mouse Gamer",
                    Precio = 150,
                    Stock = 20,
                    UrlImagen = "/imgs/mouse10.jpg"
                },
            };

            return View(productos);
        }
    }
}