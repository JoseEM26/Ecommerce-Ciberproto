using appCoreAPI.Models;
using appCoreAPI.Services.Interface;
using Microsoft.Data.SqlClient;

namespace appCoreAPI.Services.DAO
{
    public class CategoriaDAO : ICategoriaService
    {

        private readonly string cadena;

        public CategoriaDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("sql");
        }

        public string Editar(Categoria obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarCategoria", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCategoria", obj.IdCategoria);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@Activo", obj.Activo);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0) mensaje = "Categoría actualizada correctamente";
                    else mensaje = "No se pudo actualizar (Quizás el nombre ya existe)";
                }
                catch (SqlException ex) { mensaje = "Error SQL: " + ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public string Eliminar(int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarCategoria", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCategoria", id);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Categoría eliminada correctamente";
                }
                catch (SqlException ex) { mensaje = "Error SQL: " + ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public string Guardar(Categoria obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_GuardarCategoria", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0) mensaje = "Categoría registrada correctamente";
                    else mensaje = "La categoría ya existe (Validado)";
                }
                catch (SqlException ex) { mensaje = "Error SQL: " + ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public IEnumerable<Categoria> Listar()
        {
            List<Categoria> temporal = new List<Categoria>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarCategoria", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Categoria()
                    {
                        IdCategoria = dr.GetInt32(0),
                        Descripcion = dr.GetString(1),
                        Activo = dr.GetBoolean(2)
                    });
                }
                dr.Close();

            }
            return temporal;
        }

        public Categoria Obtener(int id)
        {
            return Listar().FirstOrDefault(c => c.IdCategoria == id);
        }
    }
}
