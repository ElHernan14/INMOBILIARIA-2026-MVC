using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class Pago
	{
		[Key]
		public int Id { get; set; }

        public Reserva? Reserva { get; set; }

        public Usuario? UsuarioCreador { get; set; }

        public Usuario? UsuarioCancelador { get; set; } = null; //mirar esto

		[StringLength(100)]
		public string Concepto { get; set; } = string.Empty;

        [Required]
		public DateOnly Fecha { get; set; }

        [Required]
		public decimal Importe { get; set; } = 0; // mirar esto

        [Required]
		public bool Anulado { get; set; } = false;

        [Required]
		public DateTime FechaCreacion { get; set; } = DateTime.Now;

		public DateTime FechaCancelacion { get; set; } //mirar esto
	}
}
