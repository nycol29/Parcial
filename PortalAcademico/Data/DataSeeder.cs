using Microsoft.AspNetCore.Identity;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Migraciones pendientes
            await context.Database.EnsureCreatedAsync();

            // Cursos iniciales
            if (!context.Cursos.Any())
            {
                context.Cursos.AddRange(
                    new Curso { Codigo = "CS101", Nombre = "Programación I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8,0,0), HorarioFin = new TimeSpan(10,0,0), Activo = true },
                    new Curso { Codigo = "CS102", Nombre = "Base de Datos", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10,0,0), HorarioFin = new TimeSpan(12,0,0), Activo = true },
                    new Curso { Codigo = "CS103", Nombre = "Redes", Creditos = 3, CupoMaximo = 20, HorarioInicio = new TimeSpan(14,0,0), HorarioFin = new TimeSpan(16,0,0), Activo = true }
                );
                await context.SaveChangesAsync();
            }

            // Rol Coordinador
            if (!await roleManager.RoleExistsAsync("Coordinador"))
                await roleManager.CreateAsync(new IdentityRole("Coordinador"));

            // Usuario Coordinador
            var user = await userManager.FindByEmailAsync("coordinador@uni.edu");
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "coordinador@uni.edu",
                    Email = "coordinador@uni.edu",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Coordinador123!");
                await userManager.AddToRoleAsync(user, "Coordinador");
            }
        }
    }
}
