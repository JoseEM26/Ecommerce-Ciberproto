using Grpc.Net.Client;
using GrpcService1;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly string grpcUrl = "https://localhost:7247";

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel();

            var canal = GrpcChannel.ForAddress(grpcUrl);

            try
            {
                var clientProd = new ServicioProductos.ServicioProductosClient(canal);
                var listaProd = await clientProd.GetAllAsync(new Empty());

                if (listaProd != null && listaProd.Items != null)
                {
                    model.ProductosBajoStock = listaProd.Items.Count(p => p.Stock < 5);
                }

                var clientUser = new ServicioUsuarios.ServicioUsuariosClient(canal);
                var listaUser = await clientUser.GetAllAsync(new EmptyUsuario());

                if (listaUser != null && listaUser.Items != null)
                {
                    model.TotalClientes = listaUser.Items.Count(u => u.Rol == "CLIENTE");
                }

                var clientVenta = new ServicioVentas.ServicioVentasClient(canal);
                var listaVentas = await clientVenta.GetAllAsync(new EmptyVenta());

                if (listaVentas != null && listaVentas.Items != null)
                {
                    model.VentasDelMes = listaVentas.Items.Count;

                    model.IngresosTotales = listaVentas.Items.Sum(v =>
                        decimal.TryParse(v.MontoTotal, out decimal monto) ? monto : 0);
                }
            }
            catch (Exception)
            {
                model.ProductosBajoStock = 0;
                model.TotalClientes = 0;
                model.VentasDelMes = 0;
                model.IngresosTotales = 0;
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
