using Grpc.Core;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GrpcService1.Services
{
    public class UsuarioService : ServicioUsuarios.ServicioUsuariosBase
    {
        private readonly ILogger<UsuarioService> _logger;
        private readonly string cadena;


        public UsuarioService(ILogger<UsuarioService> logger, IConfiguration configuration)
        {
            _logger = logger;
            cadena = configuration.GetConnectionString("sql");
        }

        List<Usuario> Lista()
        {
            List<Usuario> temporal = new List<Usuario>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListarUsuariogrpc", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    temporal.Add(new Usuario()
                    {
                        IdUsuario = dr.GetInt32(0),
                        Nombres = dr.GetString(1),
                        Apellidos = dr.GetString(2),
                        Correo = dr.GetString(3),
                        Telefono = dr.GetString(4),
                        Rol = dr.GetString(5),
                        Activo = dr.GetString(6),
                        FechaRegistro = dr.GetString(7)
                    });
                }
                return temporal;
            }

        }


        public override Task<Usuarios> GetAll(
            EmptyUsuario request,
            ServerCallContext context)
        {
            Usuarios response = new();
            response.Items.AddRange(Lista());
            return Task.FromResult(response);
        }

        public override Task<Usuarios> BuscarPorNombre(
            BuscarNombre request,
            ServerCallContext context)
        {
            Usuarios response = new();

            string texto = request.Texto?.ToLower() ?? "";
            if (!String.IsNullOrEmpty(texto))
            {
                response.Items.AddRange(Lista().Where(u => $"{u.Nombres} {u.Apellidos}".ToLower().Contains(texto)));

            }
            else
            {
                response.Items.AddRange(Lista());
            }

            return Task.FromResult(response);
        }




    }
}
