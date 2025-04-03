using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

public class StudentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var students = await _context.Students
                               .Include(s => s.Courses).ThenInclude(c=>c.Professor)// Include Courses for each student
                               .ToListAsync();
    
        return View(students);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
{
    if (ModelState.IsValid)
    {
        try
        {
            // Add the student to the context
            _context.Add(student);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Index view
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log or display the error
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            // Optionally, display the exception on the view for debugging
            ModelState.AddModelError(string.Empty, "An error occurred while saving the student.");
        }
    }

    // If we reach here, the operation failed or ModelState was invalid
    return View(student);
}

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var student = await _context.Students
            .Include(s => s.Courses)
                .ThenInclude(c => c.Professor)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return NotFound();

        return View(student);
    }

}

