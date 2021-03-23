using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;

namespace BathDream
{
    public class UserAndRolesInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "root@admin.com";
            string adminPassword = "Qq1945!";

            if (await roleManager.FindByNameAsync("admin") == null)
                await roleManager.CreateAsync(new IdentityRole("admin"));

            if (await roleManager.FindByNameAsync("customer") == null)
                await roleManager.CreateAsync(new IdentityRole("customer"));

            if (await roleManager.FindByNameAsync("executor") == null)
                await roleManager.CreateAsync(new IdentityRole("executor"));

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new()
                {
                    Email = adminEmail,
                    UserName = adminEmail
                };

                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    admin.EmailConfirmed = true;
                    await userManager.UpdateAsync(admin);
                }
            }
        }
    }
}
