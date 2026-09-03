using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class TipoInmueble
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Nombre { get; set; } = string.Empty;

		[Required]
		[StringLength(255)]
		public string Descripcion { get; set; } = string.Empty;

		 [Required]
		public bool Activo { get; set; } = true;
	}
}
