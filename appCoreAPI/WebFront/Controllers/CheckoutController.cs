using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            var carrito = HttpContext.Session
            .GetString("CARRITO");

            if (carrito == null)
                return RedirectToAction("Index", "Carrito");

            var items = JsonSerializer
                .Deserialize<List<CarritoModel>>(carrito);

            return View(items);
        }

        [HttpPost]
        public IActionResult Confirmar()
        {
            HttpContext.Session.Remove("CARRITO");
            HttpContext.Session.SetInt32("CarritoCantidad", 0);

            TempData["Mensaje"] = "Compra realizada con éxito 🎉";

            return RedirectToAction("Index", "Home");
        }
    }
}
