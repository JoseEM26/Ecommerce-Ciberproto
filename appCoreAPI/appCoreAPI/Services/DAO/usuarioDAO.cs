using appCoreAPI.Models;
using appCoreAPI.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace appCoreAPI.Services.DAO
{
    public class usuarioDAO : IUsuario
    {
        private readonly string cadena;
        public usuarioDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("sql");
        }
        public IEnumerable<Usuario> GetAllUsuarios()
        {
            List<Usuario> temporal = new List<Usuario>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_listarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Usuario
                    {
                        IdUsuario = dr.GetInt32(0),
                        Nombres = dr.GetString(1),
                        Apellidos = dr.GetString(2),
                        Correo = dr.GetString(3),
                        Telefono = dr.GetString(4),
                        Rol = dr.GetString(5),
                        Activo = dr.GetBoolean(6),
                        FechaRegistro = dr.GetDateTime(7)
                    });
                }
                dr.Close();
            }
            ;
            return temporal;
        }
        public Usuario GetUsuarioById(int id)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new("usp_obtenerUsuarioById", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", id);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read()) return null;

                return new Usuario
                {
                    IdUsuario = dr.GetInt32(0),
                    Nombres = dr.GetString(1),
                    Apellidos = dr.GetString(2),
                    Correo = dr.GetString(3),
                    Telefono = dr.GetString(4),
                    Rol = dr.GetString(5),
                    Activo = dr.GetBoolean(6)
                };
            }
        }
        public string RegistrarUsuario(Usuario usuario)
        {
            string mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new("usp_registrarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Clave", usuario.Clave);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se inserto {1} registros";

                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }

                return mensaje;

            }
        }

        public string ActualizarUsuario(Usuario usuario)
        {
            string mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new("usp_actualizarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    cmd.Parameters.AddWithValue("@Activo", usuario.Activo);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizaron  {1} registros";

                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }

                return mensaje;

            }
        }

        public string EliminarUsuario(int idUsuario)
        {
            string mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new("usp_usuario_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se eliminaron  {1} registros";

                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }

                return mensaje;

            }
        }

        public Usuario Login(string correo, string clave)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new("usp_usuario_login", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Clave", clave);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                    return null;

                return new Usuario
                {
                    IdUsuario = dr.GetInt32(0),
                    Nombres = dr.GetString(1),
                    Apellidos = dr.GetString(2),
                    Correo = dr.GetString(3),
                    Telefono = dr.GetString(4),
                    Rol = dr.GetString(5),
                    Activo = dr.GetBoolean(6)
                };
            }
        }

    }
}
