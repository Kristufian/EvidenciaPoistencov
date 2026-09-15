using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenciaPoistencov.ViewComponents
{
    public class UserGreetingViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserGreetingViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Content(string.Empty);
            }

            var displayName = "Používateľ";

            if (User.IsInRole("Admin"))
            {
                displayName = "Administrátor";
            }
            else if (User.IsInRole("Poistenec"))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (user?.PoistenecId != null)
                {
                    var poistenec = await _context.Poistenci.AsNoTracking().FirstOrDefaultAsync(p => p.Id == user.PoistenecId);

                    if (poistenec != null)
                    {
                        displayName = $"{poistenec.Meno} {poistenec.Priezvisko}";
                    }
                }
            }

            return View("Default", displayName);
        }
    }
}