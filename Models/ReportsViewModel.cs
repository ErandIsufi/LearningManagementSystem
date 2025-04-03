using System.Collections.Generic;
using eStudentSystem.Models;

namespace eStudentSystem.ViewModels
{
    public class ReportsViewModel
    {
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<DateTime> AttendanceDates { get; set; } = new List<DateTime>();
    }
}
