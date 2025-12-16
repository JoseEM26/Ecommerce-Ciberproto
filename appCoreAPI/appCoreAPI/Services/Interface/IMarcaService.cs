using appCoreAPI.Models;

namespace appCoreAPI.Services.Interface
{
    public interface IMarcaService
    {
        IEnumerable<Marca> Listar();
        Marca Obtener(int id);
        string Guardar(Marca obj);
        string Editar(Marca obj);
        string Eliminar(int id);
    }
}
