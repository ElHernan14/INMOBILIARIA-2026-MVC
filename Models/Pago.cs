using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace INMOBILIARIA.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una reserva.")]
        public Reserva? Reserva { get; set; }

        [ValidateNever]
        public Usuario? UsuarioCreador { get; set; }

        [ValidateNever]
        public Usuario? UsuarioCancelador { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El concepto no puede superar los 100 caracteres.")]
        public string Concepto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "El importe es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0.")]
        public decimal Importe { get; set; }

        [Required]
        public bool Anulado { get; set; } = false;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaCancelacion { get; set; }
    }
}