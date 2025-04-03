using eStudentSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eStudentSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payment> Payments { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProfessorId);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Students)
                .WithMany(s => s.Courses);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students);

            modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId);

            //modelBuilder.Entity<Course>()
            //    .HasOne(c => c.Professor)
            //    .WithMany(p => p.Courses)
            //    .HasForeignKey(c => c.ProfessorId);

            modelBuilder.Entity<Payment>()
           .Property(p => p.Amount)
           .HasColumnType("decimal(18,2)") 
           .HasPrecision(18, 2);  


        }
    }
}
