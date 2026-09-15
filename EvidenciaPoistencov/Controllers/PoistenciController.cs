
using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using EvidenciaPoistencov.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenciaPoistencov.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PoistenciController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PoistenciController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Poistenci.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenec = await _context.Poistenci.Include(p => p.Poistenia).FirstOrDefaultAsync(p => p.Id == id);

            if (poistenec == null)
            {
                return NotFound();
            }

            return View(poistenec);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoistenecCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Používateľ s týmto emailom už existuje.");

                return View(model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var poistenec = new Poistenec
            {
                Meno = model.Meno,
                Priezvisko = model.Priezvisko,
                Email = model.Email,
                Telefon = model.Telefon,
                Ulica = model.Ulica,
                Mesto = model.Mesto,
                PSC = model.PSC
            };

            _context.Poistenci.Add(poistenec);
            await _context.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                PoistenecId = poistenec.Id
            };

            var userResult = await _userManager.CreateAsync(user, model.Heslo);

            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await transaction.RollbackAsync();

                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Poistenec");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await transaction.RollbackAsync();

                return View(model);
            }

            await transaction.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenec = await _context.Poistenci.FindAsync(id);
            if (poistenec == null)
            {
                return NotFound();
            }
            return View(poistenec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Meno,Priezvisko,Email,Telefon,Ulica,Mesto,PSC")] Poistenec poistenec)
        {
            if (id != poistenec.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(poistenec);
            }

            var originalPoistenec = await _context.Poistenci.FindAsync(id);

            if (originalPoistenec == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PoistenecId == id);

            if (user != null)
            {
                var existingUserWithEmail = await _userManager.FindByEmailAsync(poistenec.Email);

                if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(poistenec.Email), "Používateľ s týmto emailom už existuje.");

                    return View(poistenec);
                }
            }

            await using var transaction =await _context.Database.BeginTransactionAsync();

            try
            {
                originalPoistenec.Meno = poistenec.Meno;
                originalPoistenec.Priezvisko = poistenec.Priezvisko;
                originalPoistenec.Email = poistenec.Email;
                originalPoistenec.Telefon = poistenec.Telefon;
                originalPoistenec.Ulica = poistenec.Ulica;
                originalPoistenec.Mesto = poistenec.Mesto;
                originalPoistenec.PSC = poistenec.PSC;

                if (user != null)
                {
                    var emailResult = await _userManager.SetEmailAsync(user, poistenec.Email);

                    if (!emailResult.Succeeded)
                    {
                        foreach (var error in emailResult.Errors)
                        {
                            ModelState.AddModelError(
                                nameof(poistenec.Email),
                                error.Description);
                        }

                        await transaction.RollbackAsync();
                        return View(poistenec);
                    }

                    var userNameResult = await _userManager.SetUserNameAsync(user, poistenec.Email);

                    if (!userNameResult.Succeeded)
                    {
                        foreach (var error in userNameResult.Errors)
                        {
                            ModelState.AddModelError(
                                nameof(poistenec.Email),
                                error.Description);
                        }

                        await transaction.RollbackAsync();
                        return View(poistenec);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PoistenecExists(poistenec.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenec = await _context.Poistenci.FirstOrDefaultAsync(m => m.Id == id);
            if (poistenec == null)
            {
                return NotFound();
            }

            return View(poistenec);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poistenec = await _context.Poistenci.Include(p => p.Poistenia).FirstOrDefaultAsync(p => p.Id == id);

            if (poistenec == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PoistenecId == id);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (user != null)
            {
                var userDeleteResult = await _userManager.DeleteAsync(user);

                if (!userDeleteResult.Succeeded)
                {
                    foreach (var error in userDeleteResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View("Delete", poistenec);
                }
            }

            _context.Poistenci.Remove(poistenec);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PoistenecExists(int? id)
        {
            return _context.Poistenci.Any(e => e.Id == id);
        }
    }
}
