using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eStudentSystem.Controllers
{
    public class ProfessorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfessorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Professors
        public async Task<IActionResult> Index()
        {
            var professors = await _context.Professors.Include(p=>p.Courses).ToListAsync();
            return View(professors);
        }

        // GET: Professors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Professors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Professor professor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(professor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(professor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var professor = await _context.Professors.FirstOrDefaultAsync(p => p.ProfessorId == id);
            if (professor == null) return NotFound();

            return View(professor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Professor professor)
        {
            if (id != professor.ProfessorId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessorExists(id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(professor);
        }
        private bool ProfessorExists(int id)
        {
            return _context.Professors.Any(e => e.ProfessorId == id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(m => m.ProfessorId == id);
            //if (professor == null)
            //{
            //    return NotFound();
            //}

            return View(professor);
        }

        // POST: Professors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor != null)
            {
                _context.Professors.Remove(professor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
