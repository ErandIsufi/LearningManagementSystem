using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace eStudentSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        [StringLength(50, ErrorMessage = "Emri duhet të ketë maksimumi 50 karaktere")]
        [RegularExpression(@"^[a-zA-ZëËçÇüÜöÖäÄšŠžŽ]+$", ErrorMessage = "Emri duhet të përmbajë vetëm shkronja")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Mbiemri është i detyrueshëm")]
        [StringLength(50, ErrorMessage = "Mbiemri duhet të ketë maksimumi 50 karaktere")]
        [RegularExpression(@"^[a-zA-ZëËçÇüÜöÖäÄšŠžŽ]+$", ErrorMessage = "Mbiemri duhet të përmbajë vetëm shkronja")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Data e lindjes është e detyrueshme")]
        [DataType(DataType.Date, ErrorMessage = "Data e lindjes nuk është valide")]
        [BirthDateValidation]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Numri i ID-së është i detyrueshëm")]
        [RegularExpression(@"^\d{7}$", ErrorMessage = "Numri i ID-së duhet të ketë saktësisht 7 shifra")]
        public string idkarteles { get; set; }

        [Required(ErrorMessage = "Email adresa është e detyrueshme")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Emaili duhet të jetë në formatin korrekt, p.sh. name@example.com")]
        public string Email { get; set; }

        public string NgjyraHunes { get; set; }


        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    // Custom validation attribute for birth date
    public class BirthDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                if (birthDate > DateTime.Today)
                {
                    return new ValidationResult("Data e lindjes nuk mund të jetë në të ardhmen");
                }
            }
            return ValidationResult.Success;
        }
    }
}
