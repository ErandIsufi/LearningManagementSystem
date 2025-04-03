
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace eStudentSystem
{
 

    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] { "Administrator", "Profesor", "Student" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = await userManager.FindByEmailAsync("admin@domain.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@domain.com", Email = "admin@domain.com" };
                await userManager.CreateAsync(adminUser, "Admin@123");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Administrator"))
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }

            var professorUser = await userManager.FindByEmailAsync("professor@domain.com");
            if (professorUser == null)
            {
                professorUser = new IdentityUser { UserName = "professor@domain.com", Email = "professor@domain.com" };
                await userManager.CreateAsync(professorUser, "Professor@123");
            }

            if (!await userManager.IsInRoleAsync(professorUser, "Profesor"))
            {
                await userManager.AddToRoleAsync(professorUser, "Profesor");
            }

            var studentUser = await userManager.FindByEmailAsync("student@domain.com");
            if (studentUser == null)
            {
                studentUser = new IdentityUser { UserName = "student@domain.com", Email = "student@domain.com" };
                await userManager.CreateAsync(studentUser, "Student@123");
            }

            if (!await userManager.IsInRoleAsync(studentUser, "Student"))
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }
    }
}
