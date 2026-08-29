using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class Reserva
	{
		[Key]
		public int Id { get; set; }

        public Inmueble? Inmueble { get; set; }

        public Inquilino? Inquilino { get; set; }

        public Usuario? UsuarioCreador { get; set; }

        public Usuario? UsuarioCancelador { get; set; } = null; //mirar esto

        [Required]
		public DateOnly FechaDesde { get; set; }

        [Required]
		public DateOnly FechaHasta { get; set; }

        [Required]
		public bool Activo { get; set; } = true;

        [Required]
		public DateTime FechaCreacion { get; set; } = DateTime.Now;

		public DateTime? FechaCancelacion { get; set; } //mirar esto
	}
}
