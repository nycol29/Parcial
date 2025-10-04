using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catálogo de cursos (con filtros)
        public async Task<IActionResult> Index(
            string? nombre, 
            int? creditosMin, 
            int? creditosMax, 
            TimeSpan? horarioInicio, 
            TimeSpan? horarioFin)
        {
            // Traemos solo cursos activos con créditos positivos desde DB
            var cursos = await _context.Cursos
                .Where(c => c.Activo && c.Creditos > 0)
                .ToListAsync();

            // Filtramos horarios inválidos en memoria
            cursos = cursos
                .Where(c => c.HorarioFin >= c.HorarioInicio)
                .ToList();

            // Aplicamos filtros dinámicos
            if (!string.IsNullOrWhiteSpace(nombre))
                cursos = cursos.Where(c => c.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();

            if (creditosMin.HasValue)
            {
                if (creditosMin.Value < 0)
                    ModelState.AddModelError("CreditosMin", "Los créditos no pueden ser negativos.");
                else
                    cursos = cursos.Where(c => c.Creditos >= creditosMin.Value).ToList();
            }

            if (creditosMax.HasValue)
            {
                if (creditosMax.Value < 0)
                    ModelState.AddModelError("CreditosMax", "Los créditos no pueden ser negativos.");
                else
                    cursos = cursos.Where(c => c.Creditos <= creditosMax.Value).ToList();
            }

            if (horarioInicio.HasValue && horarioFin.HasValue && horarioFin <= horarioInicio)
                ModelState.AddModelError("Horario", "El Horario Fin debe ser mayor que el Horario Inicio.");

            if (horarioInicio.HasValue)
                cursos = cursos.Where(c => c.HorarioInicio >= horarioInicio.Value).ToList();

            if (horarioFin.HasValue)
                cursos = cursos.Where(c => c.HorarioFin <= horarioFin.Value).ToList();

            var vm = new CursoFiltroViewModel
            {
                Nombre = nombre,
                CreditosMin = creditosMin,
                CreditosMax = creditosMax,
                HorarioInicio = horarioInicio,
                HorarioFin = horarioFin,
                Cursos = cursos
            };

            return View(vm);
        }

        // GET: Detalle de curso
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
            if (curso == null)
                return NotFound();

            return View(curso);
        }
    }
}
