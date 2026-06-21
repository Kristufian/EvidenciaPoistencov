
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvidenciaPoistencov.Models;
using EvidenciaPoistencov.Data;
using Microsoft.AspNetCore.Authorization;

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

        // GET: POISTENIES
        public async Task<IActionResult> Index()
        {
        var poistenia = await _context.Poistenia.Include(p => p.Poistenec).ToListAsync();
            return View(poistenia);
        }

        // GET: POISTENIES/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenie = await _context.Poistenia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poistenie == null)
            {
                return NotFound();
            }

            return View(poistenie);
        }

        // GET: POISTENIES/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: POISTENIES/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nazov,PredmetPoistenia,Suma,PlatnostOd,PlatnostDo,PoistenecId,Poistenec")] Poistenie poistenie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poistenie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(poistenie);
        }

        // GET: POISTENIES/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenie = await _context.Poistenia.FindAsync(id);
            if (poistenie == null)
            {
                return NotFound();
            }
            return View(poistenie);
        }

        // POST: POISTENIES/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Nazov,PredmetPoistenia,Suma,PlatnostOd,PlatnostDo,PoistenecId,Poistenec")] Poistenie poistenie)
        {
            if (id != poistenie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poistenie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoistenieExists(poistenie.Id))
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
            return View(poistenie);
        }

        // GET: POISTENIES/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poistenie = await _context.Poistenia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poistenie == null)
            {
                return NotFound();
            }

            return View(poistenie);
        }

        // POST: POISTENIES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var poistenie = await _context.Poistenia.FindAsync(id);
            if (poistenie != null)
            {
                _context.Poistenia.Remove(poistenie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PoistenieExists(int? id)
        {
            return _context.Poistenia.Any(e => e.Id == id);
        }
    }
}
