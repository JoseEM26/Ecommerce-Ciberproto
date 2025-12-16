using appCoreAPI.Models;
using appCoreAPI.Services.Interface;
using Microsoft.Data.SqlClient;

namespace appCoreAPI.Services.DAO
{
    public class ProductoDAO : IProductoService
    {

        private readonly string cadena;

        public ProductoDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("sql");
        }

        public string Editar(Producto obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarProducto", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@IdMarca", obj.IdMarca);
                    cmd.Parameters.AddWithValue("@IdCategoria", obj.IdCategoria);
                    cmd.Parameters.AddWithValue("@Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("@Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("@UrlImagen", obj.UrlImagen);
                    cmd.Parameters.AddWithValue("@Activo", obj.Activo);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0) mensaje = "Producto actualizado correctamente";
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
                    SqlCommand cmd = new SqlCommand("sp_EliminarProducto", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProducto", id);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Producto eliminado correctamente";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public string Guardar(Producto obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_GuardarProducto", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@IdMarca", obj.IdMarca);
                    cmd.Parameters.AddWithValue("@IdCategoria", obj.IdCategoria);
                    cmd.Parameters.AddWithValue("@Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("@Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("@UrlImagen", obj.UrlImagen);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0) mensaje = "Producto registrado correctamente";
                    else mensaje = "El producto ya existe";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }

            }
            return mensaje;
        }

        public IEnumerable<Producto> Listar()
        {
            List<Producto> temporal = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarProducto", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Producto()
                    {
                        IdProducto = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Descripcion = dr.GetString(2),
                        IdMarca = dr.GetInt32(3),
                        NombreMarca = dr.GetString(4),
                        IdCategoria = dr.GetInt32(5),
                        NombreCategoria = dr.GetString(6),
                        Precio = dr.GetDecimal(7),
                        Stock = dr.GetInt32(8),
                        UrlImagen = dr.IsDBNull(9) ? "" : dr.GetString(9),
                        Activo = dr.GetBoolean(10)
                    });
                }
                dr.Close();

            }
            return temporal;
        }

        public Producto Obtener(int id)
        {
            return Listar().FirstOrDefault(p => p.IdProducto == id);
        }
    }
}
