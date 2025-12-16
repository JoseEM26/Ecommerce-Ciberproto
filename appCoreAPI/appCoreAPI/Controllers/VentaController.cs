using appCoreAPI.Models;
using appCoreAPI.Services.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<Venta>>> Listar()
        {
            var lista = await Task.Run(() => new VentaDAO().Listar());
            return Ok(lista);
        }

        [HttpGet("ListarPorUsuario/{idUsuario}")]
        public async Task<ActionResult<List<Venta>>> ListarPorUsuario(int idUsuario)
        {
            var lista = await Task.Run(() => new VentaDAO().ListarPorUsuario(idUsuario));
            return Ok(lista);
        }

        [HttpGet("ListarPorFecha")]
        public async Task<ActionResult<List<Venta>>> ListarPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = await Task.Run(() => new VentaDAO().ListarPorFecha(fechaInicio, fechaFin));
            return Ok(lista);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<ActionResult<Venta>> Obtener(int id)
        {
            var obj = await Task.Run(() => new VentaDAO().Obtener(id));
            if (obj == null) return NotFound("No existe la venta");
            return Ok(obj);
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult<string>> Registrar(Venta venta)
        {
            var mensaje = await Task.Run(() => new VentaDAO().Registrar(venta));
            return Ok(mensaje);
        }

        [HttpPost("ProcesarVenta")]
        public async Task<ActionResult<object>> ProcesarVenta(
            [FromBody] ProcesarVentaRequest request)
        {
            var resultado = await Task.Run(() =>
                new VentaDAO().ProcesarVenta(
                    request.IdUsuario,
                    request.IdTarjeta,
                    request.Contacto,
                    request.Telefono,
                    request.Direccion,
                    request.IdTransaccion
                ));

            return Ok(new
            {
                Mensaje = resultado.Mensaje,
                IdVentaGenerado = resultado.IdVentaGenerado,
                TotalProductos = resultado.TotalProductos,
                MontoTotal = resultado.MontoTotal
            });
        }

        public class ProcesarVentaRequest
        {
            public int IdUsuario { get; set; }
            public int? IdTarjeta { get; set; }
            public string Contacto { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string IdTransaccion { get; set; }
        }

        [HttpGet("ObtenerDetallesVenta/{idVenta}")]
        public async Task<ActionResult<List<DetalleVenta>>> ObtenerDetallesVenta(int idVenta)
        {
            var detalles = await Task.Run(() => new VentaDAO().ObtenerDetallesVenta(idVenta));
            return Ok(detalles);
        }
    }
}
