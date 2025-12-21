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


        public IActionResult RegistroCliente()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistroCliente(UsuarioModel reg)
        {
            reg.Rol = "CLIENTE";
            reg.Activo = true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
                                        Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("registrarUsuario", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Cuenta creada con éxito. Ya puedes iniciar sesión.";
                    return RedirectToAction("LoginCliente","UsuarioLogin");
                }
                else
                {
                    ViewBag.mensaje = "Hubo un error al crear la cuenta. Intente con otro correo.";
                    return View(reg);
                }
            }
        }

    }
}
