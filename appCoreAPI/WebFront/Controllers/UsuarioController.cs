using Grpc.Net.Client;
using GrpcService1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class UsuarioController : Controller
    {
        private ServicioUsuarios.ServicioUsuariosClient _client;
        private readonly string grpcServicio = "https://localhost:7247";
        private readonly string ApiServicio = "https://localhost:7236/api/Usuario/";


        public async Task<IActionResult> ListarUsuarios(string nombre)
        {
            var canal = GrpcChannel.ForAddress(grpcServicio);
            _client = new ServicioUsuarios.ServicioUsuariosClient(canal);

            var request = new BuscarNombre
            {
                Texto = nombre ?? ""
            };
            var mensaje = await _client.BuscarPorNombreAsync(request);

            List<UsuarioModel> temporal = new List<UsuarioModel>();
            foreach (var reg in mensaje.Items)
            {
                temporal.Add(new UsuarioModel()
                {
                    IdUsuario = reg.IdUsuario,
                    Nombres = reg.Nombres,
                    Apellidos = reg.Apellidos,
                    Correo = reg.Correo,
                    Telefono = reg.Telefono,
                    Rol = reg.Rol,
                    Activo = reg.Activo == "1",
                    FechaRegistro =Convert.ToDateTime(reg.FechaRegistro)
                });

            }

            return View(temporal);
        }


        //==================CRUD=========================

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(UsuarioModel reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
                            Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("registrarUsuario", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;

            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }



        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("ListarUsuarios");
            UsuarioModel reg = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);

                HttpResponseMessage response = await client.GetAsync("getUsuarioById/" + id);
                string apiresponse = await response.Content.ReadAsStringAsync();
                reg = JsonConvert.DeserializeObject<UsuarioModel>(apiresponse);

            }
            return View(await Task.Run(() => reg));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UsuarioModel reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
                            Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync("actualizarUsuario", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;

            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        //==================================================
        // GET: Usuario/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
            {
                ViewBag.mensaje = "Ingrese sus credenciales";
                return View();
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
                        HttpContext.Session.SetString("UsuarioNombre", usuario.Nombres + " " + usuario.Apellidos);
                        HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
                        HttpContext.Session.SetInt32("UsuarioID", usuario.IdUsuario);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.mensaje = "Correo o clave incorrectos";
            return View();
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}
