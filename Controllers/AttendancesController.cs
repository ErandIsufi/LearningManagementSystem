using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eStudentSystem.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendances
    
            public async Task<IActionResult> Index(string studentName, int? courseId, DateTime? date)
            {
                var attendances = _context.Attendances
                    .Include(a => a.Student)
                    .Include(a => a.Course)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(studentName))
                {
                    attendances = attendances.Where(a =>
                        a.Student.FirstName.Contains(studentName) || a.Student.LastName.Contains(studentName));
                }

                if (courseId.HasValue)
                {
                    attendances = attendances.Where(a => a.CourseId == courseId.Value);
                }

                if (date.HasValue)
                {
                attendances = attendances.Where(a => a.Date.Date == date.Value.Date);
               // attendances = attendances.Where(c => c.atte).Any(a => a.Date.Date == date.Value.Date));
            }
         
            var allCourses = await _context.Courses
                .Include(a=>a.Attendances)
                .ToListAsync();
            ViewBag.Courses = allCourses;
            //var courses = _context.Courses.Include(c => c.Attendances).ToList();

            //ViewBag.Courses = allCourses;
                return View(await attendances.ToListAsync());
            }


        // GET: Create Attendance
        public IActionResult Create()
        {
            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Title");
            return View();
        }

        // POST: Create Attendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int CourseId, DateTime Date, bool IsPresent)
        {
            if (CourseId == 0)
            {
                ModelState.AddModelError("CourseId", "Ju lutem zgjidhni një kurs.");
            }

            // Check if attendance already exists for the selected course on the same date
            bool attendanceExists = await _context.Attendances
                .AnyAsync(a => a.CourseId == CourseId && a.Date.Date == Date.Date);

            if (attendanceExists)
            {
                ModelState.AddModelError("", "Vijueshmëria për këtë kurs në këtë datë tashmë ekziston.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Title", CourseId);
                return View();
            }

            // Get all students enrolled in this course
            var studentsInCourse = _context.Students
                .Where(s => s.Courses.Any(c => c.Id == CourseId))
                .ToList();

            if (!studentsInCourse.Any())
            {
                ModelState.AddModelError("", "Nuk ka studentë të regjistruar në këtë kurs.");
                ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Title", CourseId);
                return View();
            }

            // Create attendance records for all students
            var attendanceRecords = studentsInCourse.Select(student => new Attendance
            {
                StudentId = student.Id,
                CourseId = CourseId,
                Date = Date,
                IsPresent = IsPresent
            }).ToList();

            try
            {
                _context.Attendances.AddRange(attendanceRecords);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError("", "Ndodhi një gabim gjatë ruajtjes së vijueshmërisë.");
            }

            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Title", CourseId);
            return View();
        }






        public async Task<IActionResult> ManageAttendance(int courseId, DateTime date)
        {
            // Get the course details
            var course = await _context.Courses
                .Include(c => c.Students)
                .Include(c => c.Professor)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Get the attendance records for this course on the selected date
            var attendances = await _context.Attendances
                .Where(a => a.CourseId == courseId && a.Date.Date == date.Date)
                .ToListAsync();

            // Prepare a list of students with their attendance status
            var attendanceViewModel = course.Students.Select(student => new AttendanceViewModel
            {
                StudentId = student.Id,
                StudentName = student.FirstName + " " + student.LastName,
                IsPresent = attendances.Any(a => a.StudentId == student.Id)
                    ? attendances.First(a => a.StudentId == student.Id).IsPresent
                    : false,
            }).ToList();

            // Pass the data to the view
            var model = new ManageAttendanceViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                Date = date,
                AttendanceRecords = attendanceViewModel
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAttendance(int courseId, DateTime date, List<AttendanceViewModel> attendanceRecords)
        {
            if (ModelState.IsValid)
            {

                //Remove existing attendance records for this date and course

               var existingAttendances = await _context.Attendances
                   .Where(a => a.CourseId == courseId && a.Date.Date == date.Date)
                   .ToListAsync();
               _context.Attendances.RemoveRange(existingAttendances);

                // Add updated attendance records
                foreach (var record in attendanceRecords)
                {
                    var attendance = new Attendance
                    {
                        CourseId = courseId,
                        StudentId = record.StudentId,
                        Date = date,
                        IsPresent = record.IsPresent
                    };
                    _context.Attendances.Add(attendance);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("ManageAttendance", new { courseId = courseId, date = date });
            }

            // If the model is not valid, return the view with the current data
            return View();
        }



        // GET: Attendances/Delete
        public async Task<IActionResult> Delete(int courseId, DateTime date)
        {
            var attendances = await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.CourseId == courseId && a.Date.Date == date.Date)
                .ToListAsync();

            if (!attendances.Any())
            {
                return NotFound();
            }

            ViewBag.CourseTitle = attendances.First().Course.Title;
            ViewBag.Date = date;

            return View();
        }

        // POST: Attendances/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseId, DateTime date)
        {
            var attendances = await _context.Attendances
                .Where(a => a.CourseId == courseId && a.Date.Date == date.Date)
                .ToListAsync();

            if (attendances.Any())
            {
                _context.Attendances.RemoveRange(attendances);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
