using appCoreAPI.Models;

namespace appCoreAPI.Services.Interface
{
    public interface IVentaService
    {
        IEnumerable<Venta> Listar();
        IEnumerable<Venta> ListarPorUsuario(int idUsuario);
        IEnumerable<Venta> ListarPorFecha(DateTime fechaInicio, DateTime fechaFin);
        Venta Obtener(int id);
        string Registrar(Venta venta);

        /// frontend
        IEnumerable<DetalleVenta> ObtenerDetallesVenta(int idVenta);
        (string Mensaje, int IdVentaGenerado, int TotalProductos, decimal MontoTotal) ProcesarVenta(
            int idUsuario,
            int? idTarjeta,
            string contacto,
            string telefono,
            string direccion,
            string idTransaccion = null);
    }
}
