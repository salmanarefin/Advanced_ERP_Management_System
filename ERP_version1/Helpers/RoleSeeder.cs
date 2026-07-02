using Microsoft.AspNetCore.Identity;

namespace ERP_version1.Helpers
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles =
            {
                "Admin",
                "HR",
                "Manager",
                "Employee",
                "PayrollOfficer"
            };

            foreach (var role in roles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminEmail = "admin@gmail.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                bool isAdmin = await userManager.IsInRoleAsync(adminUser, "Admin");

                if (!isAdmin)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}