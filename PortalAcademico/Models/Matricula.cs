using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public enum EstadoMatricula { Pendiente, Confirmada, Cancelada }

    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
    }
}
