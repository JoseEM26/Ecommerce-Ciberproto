using appCoreAPI.Models;
using appCoreAPI.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace appCoreAPI.Services.DAO
{
    public class VentaDAO : IVentaService
    {

        private readonly string cadena;

        public VentaDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("sql");
        }

        public IEnumerable<Venta> Listar()
        {
            List<Venta> temporal = new List<Venta>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarVenta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Venta()
                    {
                        IdVenta = dr.GetInt32(0),
                        IdUsuario = dr.GetInt32(1),
                        IdTarjeta = dr.IsDBNull(2) ? (int?)null : dr.GetInt32(2),
                        TotalProductos = dr.GetInt32(3),
                        MontoTotal = dr.GetDecimal(4),
                        Contacto = dr.GetString(5),
                        Telefono = dr.GetString(6),
                        Direccion = dr.GetString(7),
                        IdTransaccion = dr.IsDBNull(8) ? "" : dr.GetString(8),
                        FechaVenta = dr.GetDateTime(9),
                        NombreUsuario = dr.IsDBNull(10) ? "" : dr.GetString(10),
                        CorreoUsuario = dr.IsDBNull(11) ? "" : dr.GetString(11),
                        TipoTarjeta = dr.IsDBNull(12) ? "" : dr.GetString(12),
                        NumeroTarjeta = dr.IsDBNull(13) ? "" : dr.GetString(13)
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public IEnumerable<Venta> ListarPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Venta> ListarPorUsuario(int idUsuario)
        {
            List<Venta> temporal = new List<Venta>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarVentaPorUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Venta()
                    {
                        IdVenta = dr.GetInt32(0),
                        IdUsuario = dr.GetInt32(1),
                        IdTarjeta = dr.IsDBNull(2) ? (int?)null : dr.GetInt32(2),
                        TotalProductos = dr.GetInt32(3),
                        MontoTotal = dr.GetDecimal(4),
                        Contacto = dr.GetString(5),
                        Telefono = dr.GetString(6),
                        Direccion = dr.GetString(7),
                        IdTransaccion = dr.IsDBNull(8) ? "" : dr.GetString(8),
                        FechaVenta = dr.GetDateTime(9),
                        NombreUsuario = dr.IsDBNull(10) ? "" : dr.GetString(10),
                        CorreoUsuario = dr.IsDBNull(11) ? "" : dr.GetString(11),
                        TipoTarjeta = dr.IsDBNull(12) ? "" : dr.GetString(12),
                        NumeroTarjeta = dr.IsDBNull(13) ? "" : dr.GetString(13)
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public Venta Obtener(int id)
        {
            return Listar().FirstOrDefault(v => v.IdVenta == id);
        }

        public IEnumerable<DetalleVenta> ObtenerDetallesVenta(int idVenta)
        {
            List<DetalleVenta> temporal = new List<DetalleVenta>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarDetalleVenta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new DetalleVenta()
                    {
                        IdDetalleVenta = dr.GetInt32(0),
                        IdVenta = dr.GetInt32(1),
                        IdProducto = dr.GetInt32(2),
                        Cantidad = dr.GetInt32(3),
                        PrecioUnitario = dr.GetDecimal(4),
                        Total = dr.GetDecimal(5),
                        NombreProducto = dr.IsDBNull(6) ? "" : dr.GetString(6),
                        DescripcionProducto = dr.IsDBNull(7) ? "" : dr.GetString(7),
                        ImagenProducto = dr.IsDBNull(8) ? "" : dr.GetString(8),
                        NombreMarca = dr.IsDBNull(9) ? "" : dr.GetString(9),
                        NombreCategoria = dr.IsDBNull(10) ? "" : dr.GetString(10)
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public (string Mensaje, int IdVentaGenerado, int TotalProductos, decimal MontoTotal) ProcesarVenta(int idUsuario, int? idTarjeta, string contacto, string telefono, string direccion, string idTransaccion = null)
        {
        {
                string mensaje = "";
                int idVentaGenerado = 0;
                int totalProductos = 0;
                decimal montoTotal = 0;

                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("sp_RegistrarVenta", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@IdTarjeta", (object)idTarjeta ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Contacto", contacto);
                        cmd.Parameters.AddWithValue("@Telefono", telefono);
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                        cmd.Parameters.AddWithValue("@IdTransaccion", (object)idTransaccion ?? DBNull.Value);

                        cn.Open();

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            mensaje = dr["Mensaje"].ToString();
                            idVentaGenerado = dr.IsDBNull(dr.GetOrdinal("IdVentaGenerado")) ? 0 : dr.GetInt32(dr.GetOrdinal("IdVentaGenerado"));
                            totalProductos = dr.IsDBNull(dr.GetOrdinal("TotalProductos")) ? 0 : dr.GetInt32(dr.GetOrdinal("TotalProductos"));
                            montoTotal = dr.IsDBNull(dr.GetOrdinal("MontoTotal")) ? 0 : dr.GetDecimal(dr.GetOrdinal("MontoTotal"));
                        }
                        dr.Close();
                    }
                    catch (SqlException ex)
                    {
                        mensaje = ex.Message;
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

                return (mensaje, idVentaGenerado, totalProductos, montoTotal);
            }
        }

        public string Registrar(Venta venta)
        {
            var resultado = ProcesarVenta(
               venta.IdUsuario,
               venta.IdTarjeta,
               venta.Contacto,
               venta.Telefono,
               venta.Direccion,
               venta.IdTransaccion
           );

            return resultado.Mensaje;
        }


    }
}
