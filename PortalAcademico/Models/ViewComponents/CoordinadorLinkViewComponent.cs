using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PortalAcademico.ViewComponents
{
    public class CoordinadorLinkViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CoordinadorLinkViewComponent(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
                if (user != null && await _userManager.IsInRoleAsync(user, "Coordinador"))
                {
                    return Content(@"<li class=""nav-item"">
                                        <a class=""nav-link text-dark"" asp-controller=""Coordinador"" asp-action=""Cursos"">Panel Coordinador</a>
                                     </li>");
                }
            }
            return Content(string.Empty);
        }
    }
}
