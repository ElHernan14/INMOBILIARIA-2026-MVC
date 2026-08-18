using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class Inmueble
	{
		[Key]
		public int Id { get; set; }

        public Propietario? Propietario { get; set; }

        public TipoInmueble? TipoInmueble { get; set; }

		[Required]
		[StringLength(255)]
		public string Direccion { get; set; } = string.Empty;

		[StringLength(100)]
		public string Cordenadas { get; set; } = string.Empty;

        [Required]
		public int Cupo { get; set; } = 1; // min 1 ver esto

        [Required]
		public decimal PrecioDia { get; set; } = 0; // ver esto

        [Required]
		public decimal PorcentajeReserva { get; set; } = 0; // ver esto

        [Required]
		public bool Disponible { get; set; } = true;

        [Required]
		public bool Activo { get; set; } = true;
	}
}
