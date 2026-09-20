using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
	public class Reserva
	{
		[Key]
		public int Id { get; set; }

        public Inmueble? Inmueble { get; set; }

		public int InmuebleId { get; set; }

        public Inquilino? Inquilino { get; set; }
		public int InquilinoId { get; set; }

        public Usuario? UsuarioCreador { get; set; }

        public Usuario? UsuarioCancelador { get; set; } = null; //mirar esto

        [Required]
		public DateOnly FechaDesde { get; set; }

        [Required]
		public DateOnly FechaHasta { get; set; }

		[Required(ErrorMessage = "El precio por día es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio por día no puede ser negativo.")]
        public decimal PrecioDia { get; set; } = 0;

        [Required]
		public bool Activo { get; set; } = true;

        [Required]
		public DateTime FechaCreacion { get; set; } = DateTime.Now;

		public DateTime? FechaCancelacion { get; set; } //mirar esto

		public decimal precioReserva()
		{
			if (Inmueble == null)
			{
				return 0m; 
			}

			int dias = FechaHasta.DayNumber - FechaDesde.DayNumber;
			if (dias <= 0) dias = 0;

			decimal totalAlquiler = dias * PrecioDia;

			return (totalAlquiler * Inmueble.PorcentajeReserva) / 100m;
		}

		public decimal CalcularMontoCancelacion()
		{
			DateOnly fechaCancelacion = FechaCancelacion.HasValue ? DateOnly.FromDateTime(FechaCancelacion.Value) : DateOnly.FromDateTime(DateTime.Today);

			int diasTotales = FechaHasta.DayNumber - FechaDesde.DayNumber;
			if (diasTotales <= 0) diasTotales = 1; 

			int diasTranscurridos = fechaCancelacion.DayNumber - FechaDesde.DayNumber;
			
			if (diasTranscurridos < 0) diasTranscurridos = 0;
			if (diasTranscurridos > diasTotales) diasTranscurridos = diasTotales;

			decimal porcentajeCumplido = ((decimal)diasTranscurridos / diasTotales) * 100m;
			
			// Días que restaban por transcurrir
			int diasRestantes = diasTotales - diasTranscurridos;
			if (diasRestantes < 0) diasRestantes = 0;
			decimal montoRestante = diasRestantes * PrecioDia;

			if (porcentajeCumplido < 50m)
			{
				return montoRestante * 0.50m; // 50% del restante
			}
			else
			{
				return montoRestante * 0.25m; // 25% del restante
			}
		}
	}
}
