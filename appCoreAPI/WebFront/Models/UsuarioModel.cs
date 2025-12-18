using System.ComponentModel.DataAnnotations;
using System;

namespace WebFront.Models
{
    public class UsuarioModel
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string Rol { get; set; } = "CLIENTE";

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
