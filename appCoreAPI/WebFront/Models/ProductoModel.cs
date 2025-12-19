using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{
    public class ProductoModel
    {
        [Key]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione una marca")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "Seleccione una categoría")]
        public int IdCategoria { get; set; }

        [Required]
        [Range(0.1, 99999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Display(Name = "Imagen URL")]
        public string? UrlImagen { get; set; }

        public bool Activo { get; set; } = true;

        public string? NombreMarca { get; set; }
        public string? NombreCategoria { get; set; }
    }

    public class MarcaCombo { public int IdMarca { get; set; } public string Descripcion { get; set; } }
    public class CategoriaCombo { public int IdCategoria { get; set; } public string Descripcion { get; set; } }
}

