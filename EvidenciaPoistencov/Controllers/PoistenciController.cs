
using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EvidenciaPoistencov.ViewModels;

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

        // GET: POISTENECS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Poistenci.ToListAsync());
        }

        // GET: POISTENECS/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: POISTENECS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: POISTENECS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoistenecCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Používateľ s týmto emailom už existuje.");
                    return View(model);
                }

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

                _context.Add(poistenec);
                await _context.SaveChangesAsync();

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    PoistenecId = poistenec.Id
                };

                var result = await _userManager.CreateAsync(user, model.Heslo);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Poistenec");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                _context.Poistenci.Remove(poistenec);
                await _context.SaveChangesAsync();
            }

            return View(model);
        }

        // GET: POISTENECS/Edit/5
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

        // POST: POISTENECS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Meno,Priezvisko,Email,Telefon,Ulica,Mesto,PSC,Poistenia")] Poistenec poistenec)
        {
            if (id != poistenec.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poistenec);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoistenecExists(poistenec.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(poistenec);
        }

        // GET: POISTENECS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenec = await _context.Poistenci
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poistenec == null)
            {
                return NotFound();
            }

            return View(poistenec);
        }

        // POST: POISTENECS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var poistenec = await _context.Poistenci.FindAsync(id);
            if (poistenec != null)
            {
                _context.Poistenci.Remove(poistenec);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PoistenecExists(int? id)
        {
            return _context.Poistenci.Any(e => e.Id == id);
        }
    }
}
