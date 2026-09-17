using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace INMOBILIARIA.Models
{
    public class Inmueble
    {
        [Key]
        public int Id { get; set; }

        [ValidateNever]
        public Propietario? Propietario { get; set; }

        [ValidateNever]
        public TipoInmueble? TipoInmueble { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255, ErrorMessage = "La dirección supera la longitud permitida")]
        public string Direccion { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
        public decimal? Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
        public decimal? Longitud { get; set; }

        [Required(ErrorMessage = "El cupo es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El cupo debe ser mayor a 0.")]
        public int Cupo { get; set; } = 1;

        [Required(ErrorMessage = "El precio por día es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio por día no puede ser negativo.")]
        public decimal PrecioDia { get; set; } = 0;

        [Required(ErrorMessage = "El porcentaje de reserva es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de reserva debe estar entre 0 y 100.")]
        public decimal PorcentajeReserva { get; set; } = 0;

        [Required]
        public bool Disponible { get; set; } = true;

        [Required]
        public bool Activo { get; set; } = true;
    }
}