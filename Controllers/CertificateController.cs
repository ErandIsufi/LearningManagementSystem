using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace eStudentSystem.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CertificatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Students"] = _context.Students.ToList();
            ViewData["Courses"] = _context.Courses.Include(c => c.Professor).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult GenerateCertificate(int studentId, int courseId)
        {
            var payment = _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Course)
                .ThenInclude(c => c.Professor)
                .FirstOrDefault(p => p.StudentId == studentId && p.CourseId == courseId && p.IsPaid);

            if (payment == null)
            {
                TempData["Error"] = "Studenti nuk ka paguar për këtë kurs ose nuk ekziston!";
                return RedirectToAction("Index");
            }

            return View("Certificate", payment);
        }
    }
}
