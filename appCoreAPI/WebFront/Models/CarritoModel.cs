using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{
    public class CarritoModel
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El producto es requerido")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        // Datos del producto (JOIN)
        public string? NombreProducto { get; set; }
        public string? DescripcionProducto { get; set; }

        public decimal PrecioProducto { get; set; }

        public int? Stock { get; set; }
        public string? ImagenProducto { get; set; }
        public string? NombreMarca { get; set; }
        public string? NombreCategoria { get; set; }
        public bool? ActivoProducto { get; set; } = true;

        // Propiedades calculadas
        public decimal Subtotal => PrecioProducto * Cantidad;

        public bool TieneStock => (Stock ?? 0) >= Cantidad;

        public string ImagenUrl =>
            string.IsNullOrWhiteSpace(ImagenProducto)
                ? "/images/no-image.png"
                : ImagenProducto;
    }
}
