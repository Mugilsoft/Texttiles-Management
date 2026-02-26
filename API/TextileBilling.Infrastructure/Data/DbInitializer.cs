using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TextileBilling.Core.Entities;
using System.Linq;

namespace TextileBilling.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            string[] roles = { "Admin", "BranchManager", "Cashier", "StockKeeper", "Accounts", "Purchase" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@textile.com";
            System.Console.WriteLine($"[Seeder] Checking for admin user: {adminEmail}");
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                System.Console.WriteLine("[Seeder] Admin user not found. Creating...");
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    System.Console.WriteLine("[Seeder] Admin user created successfully. Assigning role...");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    System.Console.WriteLine($"[Seeder] Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                System.Console.WriteLine("[Seeder] Admin user already exists.");
            }
        }
    }
}
