using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class Inquilino
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
		public bool Activo { get; set; } = true;

        // USAR CUANDO SE NECESITE
		// public override string ToString()
		// {
		// 	return $"{Nombre} {Apellido} ({Dni})";
		// }
	}
}
