using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;
using System.Text.Json;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CursosController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: Catálogo de cursos (con filtros y cache 60s)
        public async Task<IActionResult> Index(
            string? nombre, 
            int? creditosMin, 
            int? creditosMax, 
            TimeSpan? horarioInicio, 
            TimeSpan? horarioFin)
        {
            string cacheKey = "CursosActivos";
            List<Curso> cursos;

            // Intentamos obtener cursos desde cache
            var cachedCursos = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCursos))
            {
                cursos = JsonSerializer.Deserialize<List<Curso>>(cachedCursos)!;
            }
            else
            {
                cursos = await _context.Cursos
                    .Where(c => c.Activo && c.Creditos > 0)
                    .ToListAsync();

                // Guardar en cache por 60 segundos
                var serialized = JsonSerializer.Serialize(cursos);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                });
            }

            // Filtramos horarios inválidos
            cursos = cursos.Where(c => c.HorarioFin >= c.HorarioInicio).ToList();

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

        // GET: Detalle de curso y guardar último curso visitado en Redis
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
            if (curso == null)
                return NotFound();

            // Guardar en Redis como "UltimoCurso" por 30 minutos
            await _cache.SetStringAsync("UltimoCurso", curso.Nombre, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return View(curso);
        }

        // Método auxiliar para invalidar cache (cuando se cree/edite curso)
        private async Task InvalidateCache()
        {
            await _cache.RemoveAsync("CursosActivos");
        }
    }
}
