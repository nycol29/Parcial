
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Panel de cursos
        public async Task<IActionResult> Cursos()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }

        // Crear Curso
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                curso.Activo = true;
                _context.Add(curso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Curso creado correctamente.";
                return RedirectToAction(nameof(Cursos));
            }
            return View(curso);
        }

        // Editar Curso
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Curso editado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Cursos));
            }
            return View(curso);
        }

        // Desactivar Curso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = false;
                _context.Update(curso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Curso {curso.Nombre} desactivado.";
            }
            return RedirectToAction(nameof(Cursos));
        }

        // Listado de matrículas por curso
        public async Task<IActionResult> Matriculas(int cursoId)
        {
            var curso = await _context.Cursos
                .Include(c => c.Matriculas)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null) return NotFound();
            return View(curso);
        }

        // Confirmar matrícula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarMatricula(int matriculaId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Confirmada;
                _context.Update(matricula);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Matrícula confirmada.";
            }
            return RedirectToAction(nameof(Matriculas), new { cursoId = matricula?.CursoId });
        }

        // Cancelar matrícula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarMatricula(int matriculaId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Cancelada;
                _context.Update(matricula);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Matrícula cancelada.";
            }
            return RedirectToAction(nameof(Matriculas), new { cursoId = matricula?.CursoId });
        }

        private bool CursoExists(int id) => _context.Cursos.Any(e => e.Id == id);
    }
}
