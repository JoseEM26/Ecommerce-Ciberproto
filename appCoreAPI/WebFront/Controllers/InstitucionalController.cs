using Microsoft.AspNetCore.Mvc;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class InstitucionalController : Controller
    {
        public IActionResult Nosotros()
        {
            return View();
        }

        public IActionResult Contacto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnviarContacto(ContactoModel modelo)
        {
            if (ModelState.IsValid)
            {

                TempData["MensajeExito"] = "¡Gracias por contactarnos! Te responderemos pronto.";
                return RedirectToAction("Contacto");
            }

            return View("Contacto", modelo);
        }
    }
}
