using Grpc.Core;
using Microsoft.Data.SqlClient;
using System.Data;
namespace GrpcService1.Services
{
    public class VentaService:ServicioVentas.ServicioVentasBase
    {
        private readonly ILogger<VentaService> _logger;
        private readonly string cadena;


        public VentaService(ILogger<VentaService> logger, IConfiguration configuration)
        {
            _logger = logger;
            cadena = configuration.GetConnectionString("sql");
        }

        List<Venta> Lista()
        {
            List<Venta> temporal = new List<Venta>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListarVentagrpc", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    temporal.Add(new Venta()
                    {
                        IdVenta = dr.GetInt32(0),
                        IdUsuario = dr.GetInt32(1),
                        IdTarjeta = dr.IsDBNull(2) ? 0 : dr.GetInt32(2),
                        TotalProductos = dr.GetInt32(3),
                        MontoTotal = dr.GetString(4),
                        Contacto = dr.IsDBNull(5) ? "" : dr.GetString(5),
                        Telefono = dr.GetString(6),
                        Direccion = dr.GetString(7),
                        IdTransaccion = dr.GetString(8),
                        FechaVenta = dr.GetString(9),
                        NombreUsuario = dr.GetString(10),
                        NombreProducto = dr.GetString(11)
                    });
                }
                return temporal;
            }

        }

        public override Task<Ventas> GetAll(EmptyVenta request, ServerCallContext context)
        {
            Ventas response = new();
            response.Items.AddRange(Lista());
            return Task.FromResult(response);
        }

        public override Task<Ventas> GetByUsuario(ByUsuario request, ServerCallContext context)
        {
            Ventas response = new();
            response.Items.AddRange(Lista().Where(v => v.IdUsuario == request.IdUsuario));
            return Task.FromResult(response);
        }
        public override Task<Ventas> GetByProducto(ByProducto request, ServerCallContext context)
        {
            Ventas response = new();
            var texto = request.NombreProducto?.ToLower() ?? "";

            if (!String.IsNullOrEmpty(texto))
            {
                response.Items.AddRange(Lista().Where(v => v.NombreProducto.ToLower().Contains(texto)));

            }
            else
            {
                response.Items.AddRange(Lista());
            }
            return Task.FromResult(response);
        }
        public override Task<Ventas> GetBetweenFechas(Fechas request, ServerCallContext context)
        {
            DateTime inicio = DateTime.Parse(request.FechaInicio);
            DateTime fin = DateTime.Parse(request.FechaFin);

            Ventas response = new();
            response.Items.AddRange(
                Lista().Where(v =>
                    DateTime.Parse(v.FechaVenta) >= inicio &&
                    DateTime.Parse(v.FechaVenta) <= fin
                )
            );
            return Task.FromResult(response);
        }
    }
}
