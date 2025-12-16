using appCoreAPI.Models;

namespace appCoreAPI.Services.Interface
{
    public interface IUsuario
    {
        IEnumerable<Usuario> GetAllUsuarios();

        string RegistrarUsuario(Usuario usuario);
        string ActualizarUsuario(Usuario usuario);
        Usuario GetUsuarioById(int id);
        string EliminarUsuario(int idUsuario);

        
        Usuario Login(string correo, string clave);

    }
}
