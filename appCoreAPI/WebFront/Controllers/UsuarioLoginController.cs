using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class UsuarioLoginController : Controller
    {
        private readonly string ApiServicio = "https://localhost:7236/api/Usuario/";

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult LoginCliente()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave, string origen)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
            {
                ViewBag.mensaje = "Ingrese sus credenciales";
                return (origen == "TIENDA") ? View("LoginCliente") : View("Login");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                HttpResponseMessage response = await client.PostAsync($"login?correo={correo}&clave={clave}", null);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var usuario = JsonConvert.DeserializeObject<UsuarioModel>(apiResponse);

                    if (usuario != null)
                    {
                        if (origen == "ADMIN" && usuario.Rol != "ADMIN")
                        {
                            ViewBag.mensaje = "Acceso denegado: No es administrador";
                            return View("Login");
                        }

                        if (origen == "TIENDA" && usuario.Rol != "CLIENTE")
                        {
                            ViewBag.mensaje = "Acceso denegado: Esta cuenta no es de cliente";
                            return View("LoginCliente");
                        }

                        HttpContext.Session.SetString("UsuarioNombre", usuario.Nombres + " " + usuario.Apellidos);
                        HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
                        HttpContext.Session.SetInt32("UsuarioID", usuario.IdUsuario);

                        if (usuario.Rol == "ADMIN") return RedirectToAction("Index", "Home");
                        return RedirectToAction("Index", "Tienda");
                    }

                }
            }

            ViewBag.mensaje = "Correo o clave incorrectos";
            return (origen == "TIENDA") ? View("LoginCliente") : View("Login");
        }

        public async Task<IActionResult> Perfil()
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioID");

            if (userId == null) return RedirectToAction("Login", "Usuario");

            UsuarioModel usuario = new UsuarioModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);

                HttpResponseMessage response = await client.GetAsync("getUsuarioById/" + userId);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    usuario = JsonConvert.DeserializeObject<UsuarioModel>(json);
                }
            }

            return View(usuario);
        }

        public async Task<IActionResult> EditarPerfil()
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioID");
            if (userId == null) return RedirectToAction("Login");

            UsuarioModel usuario = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                HttpResponseMessage response = await client.GetAsync("getUsuarioById/" + userId);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    usuario = JsonConvert.DeserializeObject<UsuarioModel>(json);
                }
            }
            return View(usuario);
        }

        // POST: UsuarioLogin/EditarPerfil
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(UsuarioModel reg)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);

                // Enviamos el objeto a la misma API de actualizar
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
                                        Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync("actualizarUsuario", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SweetAlert"] = JsonConvert.SerializeObject(new
                    {
                        icon = "success",
                        title = "¡Perfil Actualizado!",
                        text = "Tus datos se han guardado correctamente."
                    });

                    HttpContext.Session.SetString("UsuarioNombre", reg.Nombres + " " + reg.Apellidos);
                    return RedirectToAction("Perfil");
                }

                ViewBag.mensaje = "No se pudieron guardar los cambios.";
                return View(reg);
            }
        }

        public IActionResult Logout()
        {
            string rol = HttpContext.Session.GetString("UsuarioRol");

            HttpContext.Session.Clear();

            if (rol == "CLIENTE")
            {
                return RedirectToAction("Index", "Tienda");
            }
            return RedirectToAction("Login");
        }

    }
}
