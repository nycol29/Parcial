using System;
using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }

        [Required]
        public string UsuarioId { get; set; } // Solo guardamos Id del usuario

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public EstadoMatricula Estado { get; set; }

        // Navegación opcional
        public Curso? Curso { get; set; }
    }
}
