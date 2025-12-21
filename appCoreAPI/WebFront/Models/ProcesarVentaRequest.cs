namespace WebFront.Models
{
    public class ProcesarVentaRequest
    {
        public int IdUsuario { get; set; }
        public int? IdTarjeta { get; set; }
        public string Contacto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string IdTransaccion { get; set; } = string.Empty;
    }
}
