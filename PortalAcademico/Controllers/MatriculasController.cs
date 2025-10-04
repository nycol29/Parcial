using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize] // Usuario debe estar autenticado
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatriculasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Inscribirse(int cursoId)
        {
            var usuarioId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value 
                            ?? User.Identity!.Name!;

            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);
            if (curso == null)
            {
                TempData["Error"] = "El curso no existe o está inactivo.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // Validación: no superar cupo máximo
            var inscritos = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
            if (inscritos >= curso.CupoMaximo)
            {
                TempData["Error"] = "El curso ha alcanzado el cupo máximo.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // Validación: no solaparse con otro curso
            var cursosUsuario = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == usuarioId && m.Estado != EstadoMatricula.Cancelada)
                .Select(m => m.Curso)
                .ToListAsync();

            bool solapado = cursosUsuario.Any(c =>
                (curso.HorarioInicio < c.HorarioFin) && (c.HorarioInicio < curso.HorarioFin)
            );

            if (solapado)
            {
                TempData["Error"] = "El curso se solapa con otro curso ya matriculado en tu horario.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // Crear matrícula en estado Pendiente
            var matricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = usuarioId,
                FechaRegistro = DateTime.Now,
                Estado = EstadoMatricula.Pendiente
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Te has inscrito correctamente en el curso (estado Pendiente).";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }
    }
}
