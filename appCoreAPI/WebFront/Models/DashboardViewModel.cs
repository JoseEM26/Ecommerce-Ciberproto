namespace WebFront.Models
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; } = 0;
        public int ProductosBajoStock { get; set; } = 0;
        public int VentasDelMes { get; set; } = 0;
        public decimal IngresosTotales { get; set; } = 0;
    }
}
