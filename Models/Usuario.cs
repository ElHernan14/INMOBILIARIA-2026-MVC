using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
    public enum RolUsuario
    {
        ADMINISTRADOR = 1,
        EMPLEADO = 2
    }

	public class Usuario
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Nombre { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Apellido { get; set; } = string.Empty;

		[Required]
		[StringLength(10)]
		public string Dni { get; set; } = string.Empty;

		[Required]
		[StringLength(254)]
		public string Email { get; set; } = string.Empty;

        [Required]
		[StringLength(255)]
		public string Avatar { get; set; } = string.Empty;

        [Required]
        public RolUsuario Rol { get; set; } = RolUsuario.EMPLEADO;

        [Required]
		public bool Activo { get; set; } = true;

	}
}
