
using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenciaPoistencov.Controllers
{
    [Authorize(Roles = "Admin")]

    public class PoisteniaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PoisteniaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var poistenia = await _context.Poistenia.Include(p => p.Poistenec).ToListAsync();
            return View(poistenia);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenie = await _context.Poistenia.Include(p => p.Poistenec).FirstOrDefaultAsync(m => m.Id == id);
            if (poistenie == null)
            {
                return NotFound();
            }

            return View(poistenie);
        }

        public async Task<IActionResult> Create(int? poistenecId)
        {
            if (poistenecId.HasValue)
            {
                var poistenec = await _context.Poistenci
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == poistenecId.Value);

                if (poistenec != null)
                {
                    ViewData["SelectedPoistenecText"] =
                        $"{poistenec.Meno} {poistenec.Priezvisko} — " +
                        $"{poistenec.Email} — {poistenec.Mesto} — ID {poistenec.Id}";
                }
            }

            return View(new Poistenie
            {
                PoistenecId = poistenecId ?? 0,
                PlatnostOd = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nazov,PredmetPoistenia,Suma,PlatnostOd,PlatnostDo,PoistenecId")] Poistenie poistenie)
        {
            if (poistenie.PlatnostOd.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(poistenie.PlatnostOd), "Dátum začiatku platnosti nemôže byť v minulosti.");
            }
            if (poistenie.PlatnostDo.HasValue && poistenie.PlatnostDo.Value.Date < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError(nameof(poistenie.PlatnostDo), "Dátum konca platnosti musí byť najskôr zajtra.");
            }
            if (ModelState.IsValid)
            {
                _context.Poistenia.Add(poistenie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var selectedPoistenec = await _context.Poistenci.AsNoTracking().FirstOrDefaultAsync(p => p.Id == poistenie.PoistenecId);

            if (selectedPoistenec != null)
            {
                ViewData["SelectedPoistenecText"] = $"{selectedPoistenec.Meno} {selectedPoistenec.Priezvisko} — " + $"{selectedPoistenec.Email} — {selectedPoistenec.Mesto} — ID {selectedPoistenec.Id}";
            }

            return View(poistenie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenie = await _context.Poistenia.Include(p => p.Poistenec).FirstOrDefaultAsync(p => p.Id == id);

            if (poistenie == null)
            {
                return NotFound();
            }
            return View(poistenie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var originalPoistenie = await _context.Poistenia.Include(p => p.Poistenec).FirstOrDefaultAsync(p => p.Id == id);

            if (originalPoistenie == null)
            {
                return NotFound();
            }

            var updated = await TryUpdateModelAsync(originalPoistenie, "", p => p.Nazov, p => p.PredmetPoistenia, p => p.Suma, p => p.PlatnostOd, p => p.PlatnostDo);

            if (updated)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoistenieExists(id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return View(originalPoistenie);
        }

        [HttpGet]
        public async Task<IActionResult> SearchPoistenci(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(Array.Empty<object>());
            }

            term = term.Trim();

            var poistenci = await _context.Poistenci.AsNoTracking().Where(p => p.Meno.Contains(term) || p.Priezvisko.Contains(term) ||
            p.Email.Contains(term)).OrderBy(p => p.Priezvisko).ThenBy(p => p.Meno).Take(10).Select(p => new
            {
                p.Id,
                p.Meno,
                p.Priezvisko,
                p.Email,
                p.Mesto
            })
                .ToListAsync();

            var result = poistenci.Select(p => new
            {
                id = p.Id,
                text = $"{p.Meno} {p.Priezvisko} — {p.Email} — {p.Mesto} — ID {p.Id}"
            });

            return Json(result);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenie = await _context.Poistenia.Include(p => p.Poistenec).FirstOrDefaultAsync(m => m.Id == id);
            if (poistenie == null)
            {
                return NotFound();
            }

            return View(poistenie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poistenie = await _context.Poistenia.FindAsync(id);

            if (poistenie == null)
            {
                return NotFound();
            }

            _context.Poistenia.Remove(poistenie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PoistenieExists(int? id)
        {
            return _context.Poistenia.Any(e => e.Id == id);
        }
    }
}
