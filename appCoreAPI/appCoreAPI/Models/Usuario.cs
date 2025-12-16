using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appCoreAPI.Models
{
     [Table("tb_usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "CLIENTE";

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

    }
}
