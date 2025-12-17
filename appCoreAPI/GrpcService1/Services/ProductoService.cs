using Grpc.Core;
using System.Data;
using Microsoft.Data.SqlClient;
namespace GrpcService1.Services
{
    public class ProductoService:ServicioProductos.ServicioProductosBase
    {

        private readonly ILogger<ProductoService> _logger;

        public ProductoService(ILogger<ProductoService> logger)
        {
            _logger = logger;
        }

        string cadena = "server=.;database=CiberProtoDatabase; trusted_Connection=true;" +
       "MultipleActiveResultSets=true; TrustServerCertificate=false; Encrypt=false";


        List<Producto> Lista()
        {
            List<Producto> temporal = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListarProductogrpc", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    temporal.Add(new Producto()
                    {
                        IdProducto = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Descripcion = dr.GetString(2),
                        IdMarca = dr.GetInt32(3),
                        Marca = dr.GetString(4),
                        IdCategoria = dr.GetInt32(5),
                        Categoria = dr.GetString(6),
                        Precio = dr.GetString(7),
                        Stock = dr.GetInt32(8),
                        UrlImagen = dr.GetString(9)
                    });
                }
                return temporal;
            }
        }

        public override Task<Productos> GetAll(Empty request, ServerCallContext context)
        {
            Productos response = new Productos();
            response.Items.AddRange(Lista());
            return Task.FromResult(response);
        }

        public override Task<Productos> GetByMarca(Marca request, ServerCallContext context)
        {
            Productos response = new Productos();

            var lista = Lista().Where(p => p.IdMarca == request.IdMarca).ToList();

            response.Items.AddRange(lista);
            return Task.FromResult(response);

        }


    }
}
