using Microsoft.AspNetCore.Identity;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            // Crear un scope para obtener servicios
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
                    new Curso { Codigo = "CS101", Nombre = "Programación I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
                    new Curso { Codigo = "CS102", Nombre = "Base de Datos", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
                    new Curso { Codigo = "CS103", Nombre = "Redes", Creditos = 3, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
                );
                await context.SaveChangesAsync();
            }

            // Rol Coordinador
            const string coordinatorRole = "Coordinador";
            if (!await roleManager.RoleExistsAsync(coordinatorRole))
                await roleManager.CreateAsync(new IdentityRole(coordinatorRole));

            // Usuario Coordinador inicial
            const string coordinatorEmail = "coordinador@uni.edu";
            const string coordinatorPassword = "Coordinador123!";
            var coordinatorUser = await userManager.FindByEmailAsync(coordinatorEmail);
            if (coordinatorUser == null)
            {
                coordinatorUser = new IdentityUser
                {
                    UserName = coordinatorEmail,
                    Email = coordinatorEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(coordinatorUser, coordinatorPassword);
                if (result.Succeeded)
                {
                    // Asignar rol si el usuario se creó correctamente
                    if (!await userManager.IsInRoleAsync(coordinatorUser, coordinatorRole))
                        await userManager.AddToRoleAsync(coordinatorUser, coordinatorRole);
                }
                else
                {
                    throw new Exception($"Error creando usuario Coordinador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
