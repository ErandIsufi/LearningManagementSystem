using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Linq;
using eStudentSystem.ViewModels;

namespace eStudentSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new ReportsViewModel
            {
                Courses = _context.Courses.Include(c => c.Professor).ToList(),
                AttendanceDates = _context.Attendances
                    .Select(a => a.Date.Date) // Qikjo esht per me i marr veq datat qe jan te Vijueshmeria mos harro
                    .Distinct()
                    .OrderByDescending(d => d)
                    .ToList()
            };

            return View(model);
        }


        public IActionResult DownloadStudents()
        {
            var students = _context.Students.ToList();
            var csv = new StringBuilder();
            csv.AppendLine("Id,First Name,Last Name,Email");

            foreach (var student in students)
            {
                csv.AppendLine($"{student.Id},{student.FirstName},{student.LastName},{student.Email}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "StudentsReport.csv");
        }

        public IActionResult DownloadProfessors()
        {
            var professors = _context.Professors.ToList();
            var csv = new StringBuilder();
            csv.AppendLine("Id,First Name,Last Name,Title");

            foreach (var professor in professors)
            {
                csv.AppendLine($"{professor.ProfessorId},{professor.FirstName},{professor.LastName},{professor.Title}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "ProfessorsReport.csv");
        }

        public IActionResult DownloadCourses()
        {
            var courses = _context.Courses.Include(c => c.Professor).ToList();
            var csv = new StringBuilder();
            csv.AppendLine("Id,Title,Description,Credits,Professor");

            foreach (var course in courses)
            {
                csv.AppendLine($"{course.Id},{course.Title},{course.Description},{course.Credits},{course.Professor?.FirstName} {course.Professor?.LastName}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "CoursesReport.csv");
        }

        public IActionResult CourseReport(int courseId)
        {
            var course = _context.Courses
                .Include(c => c.Students)
                .Include(c => c.Professor)
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null) return NotFound();

            return View("CourseReport", course);
        }

        public IActionResult AttendanceReport(int courseId, DateTime date)
        {
            var attendanceRecords = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.CourseId == courseId && a.Date.Date == date.Date)
                .ToList();

            return View(attendanceRecords);
        }

        public JsonResult GetAttendanceDates(int courseId)
        {
            var dates = _context.Attendances
                .Where(a => a.CourseId == courseId) // Filter by course
                .Select(a => a.Date.Date) // Get only distinct dates
                .Distinct()
                .OrderByDescending(d => d) // Order by latest first
                .ToList();

            return Json(dates);
        }


    }
}
