using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenciaPoistencov.Controllers
{
    [Authorize(Roles = "Poistenec")]
    public class UdajePoistencaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UdajePoistencaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.PoistenecId == null)
            {
                return Forbid();
            }

            var poistenec = await _context.Poistenci.Include(p => p.Poistenia).FirstOrDefaultAsync(p => p.Id == user.PoistenecId);

            if (poistenec == null)
            {
                return NotFound();
            }

            return View(poistenec);
        }
    }
}
