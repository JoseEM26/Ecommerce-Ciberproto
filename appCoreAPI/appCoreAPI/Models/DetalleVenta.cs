namespace appCoreAPI.Models
{
    public class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Total { get; set; }

        /// JOINS
        public string? NombreProducto { get; set; }
        public string? DescripcionProducto { get; set; }
        public string? ImagenProducto { get; set; }
        public string? NombreMarca { get; set; }
        public string? NombreCategoria { get; set; }
    }
}
