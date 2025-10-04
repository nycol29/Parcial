using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Código de curso único
            builder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            // Restricción: HorarioInicio < HorarioFin
            builder.Entity<Curso>()
                .HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");

            // Restricción: un usuario no puede matricularse más de una vez en el mismo curso
            builder.Entity<Matricula>()
                .HasIndex(m => new { m.CursoId, m.UsuarioId })
                .IsUnique();
        }
    }
}
