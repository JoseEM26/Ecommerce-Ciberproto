using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{
    public class ContactoModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El asunto es requerido")]
        [StringLength(200)]
        public string Asunto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es requerido")]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Teléfono inválido")]
        public string? Telefono { get; set; }
    }
}
