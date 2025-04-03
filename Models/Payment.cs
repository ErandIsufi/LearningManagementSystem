using System.ComponentModel.DataAnnotations;

namespace eStudentSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        [Required]        
        
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }


        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Vlera duhet te jete me e madhe se 0")]
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public Student? Student { get; set; }
        public Course? Course { get; set; }

    }
}
