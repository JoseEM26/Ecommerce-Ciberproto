
using appCoreAPI.Models;
using appCoreAPI.Services.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        [HttpGet("getAllUsuarios")]
        public async Task<ActionResult<List<Venta>>> GetAllUsuarios()
        {
            var lista = await Task.Run(() => new usuarioDAO().GetAllUsuarios());
            return Ok(lista);
        }

        [HttpGet("getUsuarioById/{id}")]
        public async Task<ActionResult<List<Venta>>> GetUsuarioById(int id)
        {
            var lista = await Task.Run(() => new usuarioDAO().GetUsuarioById(id));
            return Ok(lista);
        }

        [HttpPost("registrarUsuario")]
        public async Task<ActionResult<string>> RegistrarUsuario(Usuario usuario)
        {
            var mensaje = await Task.Run(() => new usuarioDAO().RegistrarUsuario(usuario));
            return Ok(mensaje);
        }

        [HttpPut("actualizarUsuario")]
        public async Task<ActionResult<string>> ActualizarUsuario(Usuario usuario)
        {
            var mensaje = await Task.Run(() => new usuarioDAO().ActualizarUsuario(usuario));
            return Ok(mensaje);
        }
        [HttpDelete("EliminarUsuario")]
        public async Task<ActionResult<string>> EliminarUsuario(int id)
        {
            var mensaje = await Task.Run(() => new usuarioDAO().EliminarUsuario(id));
            return Ok(mensaje);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login(string correo, string clave)
        {
            var usuario = await Task.Run(() =>
                new usuarioDAO().Login(correo, clave));

            if (usuario == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(usuario);
        }

    }
}
