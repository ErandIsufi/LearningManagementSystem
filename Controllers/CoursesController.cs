using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lista e kurseve
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Professor)  // Përfshi profesorin
            .Include(c => c.Students)  // Përfshi studentët
            .ToListAsync();
        return View(courses);
    }

    // Krijimi i një kursi të ri
    public IActionResult Create()
    {
        ViewBag.Professors = new SelectList(_context.Professors, "ProfessorId", "LastName", null);
        //ViewBag.Professors = new SelectList(_context.Professors, "Id", "LastName");
        //ViewData["ProfessorId"] = new SelectList(_context.Professors, "Id", "LastName");
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        ViewBag.Professors = new SelectList(_context.Professors, "ProfessorId", "LastName", course.ProfessorId);

             _context.Add(course);
            await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
        return View(course);
       
       

        // Repopulate the dropdown if validation fails
        //ViewBag.Professors = new SelectList(_context.Professors, "Id", "Id", course.Professor);
     
    }

    // Shtimi i studentëve në një kurs
    public async Task<IActionResult> AddStudents(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        var students = await _context.Students.ToListAsync();
        var model = new CourseStudentViewModel
        {
            Course = course,
            Students = students
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStudents(int courseId, List<int> studentIds)
    {
        var course = await _context.Courses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null) return NotFound();

        // Remove students that are not in the selected list
        var studentsToRemove = course.Students.Where(s => !studentIds.Contains(s.Id)).ToList();
        foreach (var student in studentsToRemove)
        {
            course.Students.Remove(student);
        }

        // Add new students that are checked but not in the course
        foreach (var studentId in studentIds)
        {
            if (!course.Students.Any(s => s.Id == studentId))
            {
                var student = await _context.Students.FindAsync(studentId);
                if (student != null)
                {
                    course.Students.Add(student);
                }
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }



    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

       ViewBag.Professors = new SelectList(_context.Professors, "ProfessorId", "LastName", course.ProfessorId);
        return View(course);
    }

    // POST: Courses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Professors = new SelectList(_context.Professors, "ProfessorId", "LastName", course.ProfessorId);

        //ViewData["ProfesoriId"] = new SelectList(_context.Professors, "Id", "FirstName", course.ProfessorId);
        return View(course);
    }

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Professor)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (course == null) return NotFound();

        return View(course);
    }

    // POST: Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }

    // GET: Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Professor)  //  Perfshij detajet e profesorit
            .Include(c => c.Students)   // Perfshij studentet qe kan caktuar kursin
            .Include(c=>c.Attendances) // perfshij vinjueshmerin
            .FirstOrDefaultAsync(m => m.Id == id);

        if (course == null) return NotFound();

        return View(course);
    }
}
