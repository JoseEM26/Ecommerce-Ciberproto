using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{
    public class VentaModel
    {
        [Display(Name = "ID Venta")]
        public int IdVenta { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty; 

        [Display(Name = "Producto Principal")]
        public string Producto { get; set; } = string.Empty; 
        [Display(Name = "Cant.")]
        public int Cantidad { get; set; } 

        [Display(Name = "Total")]
        public decimal MontoTotal { get; set; } 

        [Display(Name = "Fecha Venta")]
        public string Fecha { get; set; } = string.Empty; 

        [Display(Name = "Cód. Transacción")]
        public string IdTransaccion { get; set; } = string.Empty;

        public string Contacto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}
