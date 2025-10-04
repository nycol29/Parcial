using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public class Curso : IValidatableObject
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Codigo { get; set; } = string.Empty; // único

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores que 0.")]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor que 0.")]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeSpan HorarioInicio { get; set; }

        [Required]
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        // Validación personalizada server-side
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioFin <= HorarioInicio)
            {
                yield return new ValidationResult(
                    "El horario de fin debe ser posterior al horario de inicio.",
                    new[] { nameof(HorarioFin), nameof(HorarioInicio) });
            }
        }
    }
}
