using appCoreAPI.Models;
using appCoreAPI.Services.Interface;
using Microsoft.Data.SqlClient;

namespace appCoreAPI.Services.DAO
{
    public class MarcaDAO : IMarcaService
    {

        private readonly string cadena;

        public MarcaDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("sql");
        }


        public string Editar(Marca obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarMarca", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdMarca", obj.IdMarca);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@Activo", obj.Activo);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0) mensaje = "Marca actualizada correctamente";
                    else mensaje = "No se pudo actualizar (Quizás el nombre ya existe)";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
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
                    SqlCommand cmd = new SqlCommand("sp_EliminarMarca", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdMarca", id);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Marca eliminada y productos asociados desactivados";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public string Guardar(Marca obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_GuardarMarca", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0) mensaje = "Marca registrada correctamente";
                    else mensaje = "La marca ya existe";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public IEnumerable<Marca> Listar()
        {
            List<Marca> temporal = new List<Marca>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarMarca", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Marca()
                    {
                        IdMarca = dr.GetInt32(0),
                        Descripcion = dr.GetString(1),
                        Activo = dr.GetBoolean(2)
                    });
                }
                dr.Close();

            }
            return temporal;
        }

        public Marca Obtener(int id)
        {
            return Listar().FirstOrDefault(m => m.IdMarca == id);
        }
    }
}
