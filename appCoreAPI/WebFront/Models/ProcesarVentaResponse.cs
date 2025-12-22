namespace WebFront.Models
{
    public class ProcesarVentaResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public int IdVentaGenerado { get; set; }
        public int TotalProductos { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
