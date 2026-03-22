using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShoppingWebsite.Data.Seeding
{
    public static class AppSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            // ── Roles ──────────────────────────────────────────
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── Admin User ─────────────────────────────────────
            const string adminEmail = "admin@shopping.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Shop Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@1234");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
