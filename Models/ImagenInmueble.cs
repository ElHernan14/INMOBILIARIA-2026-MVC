using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class ImagenInmueble
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(255)]
		public string Path { get; set; } = string.Empty;

        [Required]
		public bool EsPortada { get; set; } = false;

        public Inmueble? Inmueble { get; set; }
	}
}
