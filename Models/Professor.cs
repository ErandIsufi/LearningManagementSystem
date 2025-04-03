using System.ComponentModel.DataAnnotations;

namespace eStudentSystem.Models
{
    public class Professor
    {
        [Key]
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        [StringLength(50, ErrorMessage = "Emri duhet të ketë maksimumi 50 karaktere")]
        [RegularExpression(@"^[a-zA-ZëËçÇüÜöÖäÄšŠžŽ]+$", ErrorMessage = "Emri duhet të përmbajë vetëm shkronja")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Mbiemri është i detyrueshëm")]
        [StringLength(50, ErrorMessage = "Mbiemri duhet të ketë maksimumi 50 karaktere")]
        [RegularExpression(@"^[a-zA-ZëËçÇüÜöÖäÄšŠžŽ]+$", ErrorMessage = "Mbiemri duhet të përmbajë vetëm shkronja")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Titulli është i detyrueshëm")]
        [StringLength(100, ErrorMessage = "Titulli duhet të ketë maksimumi 100 karaktere")]
        [RegularExpression(@"^[a-zA-Z\s.\-]+$", ErrorMessage = "Titulli duhet të përmbajë vetëm shkronja, hapësira, pika dhe viza")]
        public string Title { get; set; }

        // Relationship
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
