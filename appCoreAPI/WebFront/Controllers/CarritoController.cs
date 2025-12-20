using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class CarritoController : Controller
    {

        private const string SessionKey = "CARRITO";

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        public IActionResult Agregar(
            int idProducto,
            string nombre,
            decimal precio,
            int stock,
            string? imagen = null,
            string? returnUrl = null)
        {
            var carrito = ObtenerCarrito();

            var item = carrito.FirstOrDefault(x => x.IdProducto == idProducto);

            if (item == null)
            {
                carrito.Add(new CarritoModel
                {
                    IdProducto = idProducto,
                    NombreProducto = nombre,
                    PrecioProducto = precio,
                    Stock = stock,
                    Cantidad = 1,
                    ImagenProducto = imagen
                });
            }
            else
            {
                if (item.TieneStock)
                {
                    item.Cantidad++;
                }
            }

            GuardarCarrito(carrito);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Tienda");
        }

        public IActionResult ActualizarCantidad(int idProducto, int cambio, string? returnUrl = null)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(x => x.IdProducto == idProducto);

            if (item != null)
            {
                item.Cantidad += cambio;

                // Si la cantidad llega a 0 o menos, eliminar el producto
                if (item.Cantidad <= 0)
                {
                    carrito.Remove(item);
                }
                // Verificar que no exceda el stock               
            }

            GuardarCarrito(carrito);

            // Redirigir a donde vino
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int idProducto)
        {
            var carrito = ObtenerCarrito();

            var item = carrito.FirstOrDefault(x => x.IdProducto == idProducto);
            if (item != null)
            {
                carrito.Remove(item);
            }

            GuardarCarrito(carrito);
            return RedirectToAction("Index");
        }



        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove(SessionKey);
            HttpContext.Session.SetInt32("CarritoCantidad", 0);

            return RedirectToAction("Index");
        }

        // ===============================
        // MÉTODOS PRIVADOS
        // ===============================

        private List<CarritoModel> ObtenerCarrito()
        {
            var data = HttpContext.Session.GetString(SessionKey);
            return data == null
                ? new List<CarritoModel>()
                : JsonSerializer.Deserialize<List<CarritoModel>>(data)!;
        }

        private void GuardarCarrito(List<CarritoModel> carrito)
        {
            HttpContext.Session.SetString(SessionKey,
                JsonSerializer.Serialize(carrito));

            HttpContext.Session.SetInt32(
                "CarritoCantidad",
                carrito.Sum(x => x.Cantidad));
        }

    }
}
