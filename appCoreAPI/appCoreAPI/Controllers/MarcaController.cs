using appCoreAPI.Models;
using appCoreAPI.Services.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<Marca>>> Listar()
        {
            var lista = await Task.Run(() => new MarcaDAO().Listar());
            return Ok(lista);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<ActionResult<Marca>> Obtener(int id)
        {
            var obj = await Task.Run(() => new MarcaDAO().Obtener(id));
            if (obj == null) return NotFound("No existe la marca");
            return Ok(obj);
        }

        [HttpPost("Guardar")]
        public async Task<ActionResult<string>> Guardar(Marca obj)
        {
            var mensaje = await Task.Run(() => new MarcaDAO().Guardar(obj));
            return Ok(mensaje);
        }

        [HttpPut("Editar")]
        public async Task<ActionResult<string>> Editar(Marca obj)
        {
            var mensaje = await Task.Run(() => new MarcaDAO().Editar(obj));
            return Ok(mensaje);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult<string>> Eliminar(int id)
        {
            var mensaje = await Task.Run(() => new MarcaDAO().Eliminar(id));
            return Ok(mensaje);
        }
    }
}
