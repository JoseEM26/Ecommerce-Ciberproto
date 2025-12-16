using appCoreAPI.Models;
using appCoreAPI.Services.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<Carrito>>> Listar()
        {
            var lista = await Task.Run(() => new CarritoDAO().Listar());
            return Ok(lista);
        }

        [HttpGet("ListarPorUsuario/{idUsuario}")]
        public async Task<ActionResult<List<Carrito>>> ListarPorUsuario(int idUsuario)
        {
            var lista = await Task.Run(() => new CarritoDAO().ListarPorUsuario(idUsuario));
            return Ok(lista);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<ActionResult<Carrito>> Obtener(int id)
        {
            var obj = await Task.Run(() => new CarritoDAO().Obtener(id));
            if (obj == null) return NotFound("No existe el item en el carrito");
            return Ok(obj);
        }

        [HttpGet("ObtenerPorUsuarioProducto")]
        public async Task<ActionResult<Carrito>> ObtenerPorUsuarioProducto(int idUsuario, int idProducto)
        {
            var obj = await Task.Run(() => new CarritoDAO().ObtenerPorUsuarioProducto(idUsuario, idProducto));
            if (obj == null) return NotFound("Producto no encontrado en el carrito");
            return Ok(obj);
        }

        [HttpPost("Agregar")]
        public async Task<ActionResult<string>> Agregar(Carrito obj)
        {
            var mensaje = await Task.Run(() => new CarritoDAO().Agregar(obj));
            return Ok(mensaje);
        }

        [HttpPut("Actualizar")]
        public async Task<ActionResult<string>> Actualizar(Carrito obj)
        {
            var mensaje = await Task.Run(() => new CarritoDAO().Actualizar(obj));
            return Ok(mensaje);
        }

        [HttpPut("ActualizarCantidad")]
        public async Task<ActionResult<string>> ActualizarCantidad(int idCarrito, int cantidad)
        {
            var carrito = await Task.Run(() => new CarritoDAO().Obtener(idCarrito));
            if (carrito == null) return NotFound("Item no encontrado en carrito");

            carrito.Cantidad = cantidad;
            var mensaje = await Task.Run(() => new CarritoDAO().Actualizar(carrito));
            return Ok(mensaje);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult<string>> Eliminar(int id)
        {
            var mensaje = await Task.Run(() => new CarritoDAO().Eliminar(id));
            return Ok(mensaje);
        }

        [HttpDelete("EliminarPorUsuarioProducto")]
        public async Task<ActionResult<string>> EliminarPorUsuarioProducto(int idUsuario, int idProducto)
        {
            var mensaje = await Task.Run(() => new CarritoDAO().EliminarPorUsuarioProducto(idUsuario, idProducto));
            return Ok(mensaje);
        }

        [HttpDelete("VaciarCarritoUsuario/{idUsuario}")]
        public async Task<ActionResult<string>> VaciarCarritoUsuario(int idUsuario)
        {
            var mensaje = await Task.Run(() => new CarritoDAO().VaciarCarritoUsuario(idUsuario));
            return Ok(mensaje);
        }

        [HttpGet("ContarItemsUsuario/{idUsuario}")]
        public async Task<ActionResult<int>> ContarItemsUsuario(int idUsuario)
        {
            var cantidad = await Task.Run(() => new CarritoDAO().ContarItemsUsuario(idUsuario));
            return Ok(cantidad);
        }

        [HttpGet("CalcularTotalUsuario/{idUsuario}")]
        public async Task<ActionResult<decimal>> CalcularTotalUsuario(int idUsuario)
        {
            var total = await Task.Run(() => new CarritoDAO().CalcularTotalUsuario(idUsuario));
            return Ok(total);
        }
    }
}
