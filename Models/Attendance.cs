using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eStudentSystem.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        public bool IsPresent { get; set; }

        // Prevent validation of navigation properties
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
