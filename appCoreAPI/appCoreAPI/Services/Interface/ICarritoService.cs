using appCoreAPI.Models;

namespace appCoreAPI.Services.Interface
{
    public interface ICarritoService
    {
        IEnumerable<Carrito> Listar();
        IEnumerable<Carrito> ListarPorUsuario(int idUsuario);
        Carrito Obtener(int id);
        Carrito ObtenerPorUsuarioProducto(int idUsuario, int idProducto);
        string Agregar(Carrito obj);
        string Actualizar(Carrito obj);
        string Eliminar(int id);
        string EliminarPorUsuarioProducto(int idUsuario, int idProducto);
        string VaciarCarritoUsuario(int idUsuario);
        int ContarItemsUsuario(int idUsuario);
        decimal CalcularTotalUsuario(int idUsuario);
    }
}
