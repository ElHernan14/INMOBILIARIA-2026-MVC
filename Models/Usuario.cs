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

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(100)]
		public string Nombre { get; set; } = string.Empty;

		
		[Required(ErrorMessage = "El apellido es obligatorio.")]
		[StringLength(100)]
		public string Apellido { get; set; } = string.Empty;

		
		[Required(ErrorMessage = "El dni es obligatorio.")]
		[StringLength(10)]
		public string Dni { get; set; } = string.Empty;

		[Required(ErrorMessage = "El email es obligatorio.")]
    	[EmailAddress(ErrorMessage = "El formato del email no es valido")]
		[StringLength(254)]
		public string Email { get; set; } = string.Empty;

		[StringLength(255)]
		public string Avatar { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public RolUsuario Rol { get; set; } = RolUsuario.EMPLEADO;

        [Required]
		public bool Activo { get; set; } = true;

	}
}
