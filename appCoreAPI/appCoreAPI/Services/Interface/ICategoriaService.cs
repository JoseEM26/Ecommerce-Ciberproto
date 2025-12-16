using appCoreAPI.Models;

namespace appCoreAPI.Services.Interface
{
    public interface ICategoriaService
    {
        IEnumerable<Categoria> Listar();
        Categoria Obtener(int id); 
        string Guardar(Categoria obj);
        string Editar(Categoria obj);
        string Eliminar(int id);
    }
}
