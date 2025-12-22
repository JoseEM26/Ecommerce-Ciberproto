using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{
    public class CheckoutModel
    {
        [Required(ErrorMessage = "El nombre de contacto es requerido")]
        [StringLength(100)]
        public string Contacto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(500)]
        public string Direccion { get; set; } = string.Empty;

        public int? IdTarjeta { get; set; }
        public string? IdTransaccion { get; set; }

        // Para mostrar en la vista
        public List<CarritoModel> ItemsCarrito { get; set; } = new List<CarritoModel>();
        public decimal TotalCompra { get; set; }
    }
}
