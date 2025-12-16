namespace appCoreAPI.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public int? IdTarjeta { get; set; }
        public int TotalProductos { get; set; }
        public decimal MontoTotal { get; set; }
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? IdTransaccion { get; set; }
        public DateTime FechaVenta { get; set; }

        /// JOINS
       
        public string? NombreUsuario { get; set; }
        public string? TipoTarjeta { get; set; }
        public string? NumeroTarjeta { get; set; }
        public string? CorreoUsuario { get; set; }
    }
}
