using appCoreAPI.Models;

namespace appCoreAPI.Services.Interface
{
    public interface IProductoService
    {
        IEnumerable<Producto> Listar();
        Producto Obtener(int id);
        string Guardar(Producto obj);
        string Editar(Producto obj);
        string Eliminar(int id);
    }
}
