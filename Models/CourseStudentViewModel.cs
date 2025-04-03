namespace eStudentSystem.Models
{
    public class CourseStudentViewModel
    {
        public Course Course { get; set; } = new Course();
        public List<Student> Students { get; set; } = new List<Student>();
    }
}
