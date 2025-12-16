using appCoreAPI.Models;
using appCoreAPI.Services.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<Producto>>> Listar()
        {
            var lista = await Task.Run(() => new ProductoDAO().Listar());
            return Ok(lista);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<ActionResult<Producto>> Obtener(int id)
        {
            var obj = await Task.Run(() => new ProductoDAO().Obtener(id));
            if (obj == null) return NotFound("No existe el producto");
            return Ok(obj);
        }

        [HttpPost("Guardar")]
        public async Task<ActionResult<string>> Guardar(Producto obj)
        {
            var mensaje = await Task.Run(() => new ProductoDAO().Guardar(obj));
            return Ok(mensaje);
        }

        [HttpPut("Editar")]
        public async Task<ActionResult<string>> Editar(Producto obj)
        {
            var mensaje = await Task.Run(() => new ProductoDAO().Editar(obj));
            return Ok(mensaje);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult<string>> Eliminar(int id)
        {
            var mensaje = await Task.Run(() => new ProductoDAO().Eliminar(id));
            return Ok(mensaje);
        }
    }
}
