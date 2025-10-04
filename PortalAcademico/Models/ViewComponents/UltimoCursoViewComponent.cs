using Microsoft.AspNetCore.Mvc;
using PortalAcademico.Data;
using Microsoft.EntityFrameworkCore;

namespace PortalAcademico.ViewComponents
{
    public class UltimoCursoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public UltimoCursoViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Revisar si hay un curso guardado en sesión
            var cursoId = HttpContext.Session.GetInt32("UltimoCursoId");

            if (cursoId == null)
                return Content(""); // No mostrar nada si no hay curso

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == cursoId.Value && c.Activo);

            if (curso == null)
                return Content("");

            return View(curso); // Enviar el curso a la vista
        }
    }
}
