using appCoreAPI.Models;
using appCoreAPI.Services.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<Categoria>>> Listar()
        {
            var lista = await Task.Run(() => new CategoriaDAO().Listar());
            return Ok(lista);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<ActionResult<Categoria>> Obtener(int id)
        {
            var obj = await Task.Run(() => new CategoriaDAO().Obtener(id));
            if (obj == null) return NotFound("No existe la categoría");
            return Ok(obj);
        }

        [HttpPost("Guardar")]
        public async Task<ActionResult<string>> Guardar(Categoria obj)
        {
            var mensaje = await Task.Run(() => new CategoriaDAO().Guardar(obj));
            return Ok(mensaje);
        }

        [HttpPut("Editar")]
        public async Task<ActionResult<string>> Editar(Categoria obj)
        {
            var mensaje = await Task.Run(() => new CategoriaDAO().Editar(obj));
            return Ok(mensaje);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult<string>> Eliminar(int id)
        {
            var mensaje = await Task.Run(() => new CategoriaDAO().Eliminar(id));
            return Ok(mensaje);
        }
    }
}
