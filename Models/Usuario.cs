using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA.Models
{
    public enum RolUsuario
    {
        ADMINISTRADOR,
        EMPLEADO
    }

    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(
            10,
            MinimumLength = 7,
            ErrorMessage = "El DNI debe tener entre 7 y 10 caracteres.")]
        [RegularExpression(
            @"^\d{7,10}$",
            ErrorMessage = "El DNI debe contener solamente números.")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [StringLength(
            255,
            ErrorMessage = "El avatar no puede superar los 255 caracteres.")]
        public string Avatar { get; set; } = string.Empty;

        // Este campo almacena el hash de la contraseña.
        // No debe mostrarse directamente en las vistas.
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public RolUsuario Rol { get; set; } = RolUsuario.EMPLEADO;

        public bool Activo { get; set; } = true;

        public string RolNombre => Rol.ToString();

        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}