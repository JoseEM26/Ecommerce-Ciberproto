using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{
    public class CategoriaModel
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
