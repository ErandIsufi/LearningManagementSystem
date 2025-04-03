namespace eStudentSystem.Models
{
    public class ManageAttendanceViewModel
    {
  
            public int CourseId { get; set; }
            public string CourseTitle { get; set; }
            public DateTime Date { get; set; }
            public List<AttendanceViewModel> AttendanceRecords { get; set; } = new List<AttendanceViewModel>();
        
    }
}
