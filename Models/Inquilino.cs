using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class Inquilino
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(100, ErrorMessage = "El nombre supera la longitud permitida")]
		public string Nombre { get; set; } = string.Empty;

		[Required(ErrorMessage = "El apellido es obligatorio.")]
		[StringLength(100, ErrorMessage = "El apellido supera la longitud permitida")]
		public string Apellido { get; set; } = string.Empty;

		[Required(ErrorMessage = "El dni es obligatorio.")]
		[MinLength(8, ErrorMessage = "El dni debe tener al menos 8 caracteres")]
		[StringLength(10, ErrorMessage = "El dni supera la longitud permitida")]
		public string Dni { get; set; } = string.Empty;

		[Required(ErrorMessage = "El email es obligatorio.")]
    	[EmailAddress(ErrorMessage = "El formato del email no es valido")]
		[StringLength(254)]
		public string Email { get; set; } = string.Empty;

        [Required]
		public bool Activo { get; set; } = true;

        // USAR CUANDO SE NECESITE
		// public override string ToString()
		// {
		// 	return $"{Nombre} {Apellido} ({Dni})";
		// }
	}
}
