using appCoreAPI.Models;
using appCoreAPI.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace appCoreAPI.Services.DAO
{
    public class CarritoDAO : ICarritoService
    {

        private readonly string cadena;

        public CarritoDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("sql");
        }

        public string Agregar(Carrito obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_GuardarCarrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);

                    cn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        mensaje = dr["Mensaje"].ToString();
                    }
                    dr.Close();
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public string Actualizar(Carrito obj)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarCarrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCarrito", obj.IdCarrito);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);

                    cn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        mensaje = dr["Mensaje"].ToString();
                    }
                    dr.Close();
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public decimal CalcularTotalUsuario(int idUsuario)
        {
            return ListarPorUsuario(idUsuario).Sum(c => c.Cantidad * (c.PrecioProducto ?? 0));
        }

        public int ContarItemsUsuario(int idUsuario)
        {
            return ListarPorUsuario(idUsuario).Sum(c => c.Cantidad);
        }

        public string Eliminar(int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarCarrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCarrito", id);

                    cn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        mensaje = dr["Mensaje"].ToString();
                    }
                    dr.Close();
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public string EliminarPorUsuarioProducto(int idUsuario, int idProducto)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarCarritoUsuarioProducto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                    cn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        mensaje = dr["Mensaje"].ToString();
                    }
                    dr.Close();
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public IEnumerable<Carrito> Listar()
        {
            List<Carrito> temporal = new List<Carrito>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarCarrito", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Carrito()
                    {
                        IdCarrito = dr.GetInt32(0),
                        IdUsuario = dr.GetInt32(1),
                        IdProducto = dr.GetInt32(2),
                        Cantidad = dr.GetInt32(3),
                        NombreProducto = dr.IsDBNull(4) ? "" : dr.GetString(4),
                        DescripcionProducto = dr.IsDBNull(5) ? "" : dr.GetString(5),
                        PrecioProducto = dr.IsDBNull(6) ? 0 : dr.GetDecimal(6),
                        Stock = dr.IsDBNull(7) ? 0 : dr.GetInt32(7),
                        ImagenProducto = dr.IsDBNull(8) ? "" : dr.GetString(8),
                        ActivoProducto = dr.IsDBNull(9) ? false : dr.GetBoolean(9),
                        NombreMarca = dr.IsDBNull(10) ? "" : dr.GetString(10),
                        NombreCategoria = dr.IsDBNull(11) ? "" : dr.GetString(11)
                    });
                }
                dr.Close();
            }
            return temporal;
        }
             
        public IEnumerable<Carrito> ListarPorUsuario(int idUsuario)
        {
            List<Carrito> temporal = new List<Carrito>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarCarritoPorUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Carrito()
                    {
                        IdCarrito = dr.GetInt32(0),
                        IdUsuario = dr.GetInt32(1),
                        IdProducto = dr.GetInt32(2),
                        Cantidad = dr.GetInt32(3),
                        NombreProducto = dr.IsDBNull(4) ? "" : dr.GetString(4),
                        DescripcionProducto = dr.IsDBNull(5) ? "" : dr.GetString(5),
                        PrecioProducto = dr.IsDBNull(6) ? 0 : dr.GetDecimal(6),
                        Stock = dr.IsDBNull(7) ? 0 : dr.GetInt32(7),
                        ImagenProducto = dr.IsDBNull(8) ? "" : dr.GetString(8),
                        ActivoProducto = dr.IsDBNull(9) ? false : dr.GetBoolean(9),
                        NombreMarca = dr.IsDBNull(10) ? "" : dr.GetString(10),
                        NombreCategoria = dr.IsDBNull(11) ? "" : dr.GetString(11)
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public Carrito Obtener(int id)
        {
            return Listar().FirstOrDefault(c => c.IdCarrito == id);
        }

        public Carrito ObtenerPorUsuarioProducto(int idUsuario, int idProducto)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerCarritoUsuarioProducto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new Carrito()
                    {
                        IdCarrito = dr.GetInt32(0),
                        IdUsuario = dr.GetInt32(1),
                        IdProducto = dr.GetInt32(2),
                        Cantidad = dr.GetInt32(3),
                        NombreProducto = dr.IsDBNull(4) ? "" : dr.GetString(4),
                        PrecioProducto = dr.IsDBNull(5) ? 0 : dr.GetDecimal(5),
                        ImagenProducto = dr.IsDBNull(6) ? "" : dr.GetString(6)
                    };
                }
                dr.Close();
            }
            return null;
        }

        public string VaciarCarritoUsuario(int idUsuario)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_VaciarCarritoUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    cn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        mensaje = dr["Mensaje"].ToString();
                    }
                    dr.Close();
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }
    }
}
