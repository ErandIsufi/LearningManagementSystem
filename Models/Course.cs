using System.ComponentModel.DataAnnotations;

namespace eStudentSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Titulli i kursit është i detyrueshëm")]
        [StringLength(100, ErrorMessage = "Titulli duhet të ketë maksimumi 100 karaktere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Përshkrimi i kursit është i detyrueshëm")]
        [StringLength(500, ErrorMessage = "Përshkrimi duhet të ketë maksimumi 500 karaktere")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Numri i krediteve është i detyrueshëm")]
        [Range(3, 5, ErrorMessage = "Numri i krediteve duhet të jetë midis 3 dhe 5")]
        public int Credits { get; set; }

        // Relationships
        public Professor Professor { get; set; }
        public int ProfessorId { get; set; } // Foreign Key for Professor

        // Initialize collections to avoid null reference exceptions
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
