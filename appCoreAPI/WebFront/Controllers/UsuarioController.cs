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


        public async Task<IActionResult> ListarUsuarios(string nombre, int pagina = 1)
        {
            int PageSize = 10;

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

            int totalRegistros = temporal.Count;
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)PageSize);

            var registrosPaginados = temporal
                .Skip((pagina - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;

            return View(registrosPaginados);
        }


        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(UsuarioModel reg)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
                            Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("registrarUsuario", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["SweetAlert"] = JsonConvert.SerializeObject(new
                    {
                        icon = "success",
                        title = "¡Usuario Creado!",
                        text = "El usuario se registró correctamente."
                    });
                    return RedirectToAction("ListarUsuarios");
                }
                else
                {
                    ViewBag.mensaje = "Error: " + apiResponse;
                    return View(reg);
                }

            }
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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiServicio);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
                            Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync("actualizarUsuario", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SweetAlert"] = JsonConvert.SerializeObject(new
                    {
                        icon = "success",
                        title = "Perfil Actualizado",
                        text = "Los datos se guardaron correctamente."
                    });
                    return RedirectToAction("ListarUsuarios");
                }
                else
                {
                    ViewBag.mensaje = "Error al actualizar.";
                    return View(reg);
                }

            }
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

        public async Task<IActionResult> Delete(int id)
        {
            using (var api = new HttpClient())
            {
                api.BaseAddress = new Uri(ApiServicio);
                var response = await api.DeleteAsync($"EliminarUsuario/{id}");


            }
            return RedirectToAction("ListarUsuarios");
        }
    }
}
