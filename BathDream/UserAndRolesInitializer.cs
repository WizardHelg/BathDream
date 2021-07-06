﻿using System;
using System.Collections.Generic;
using System.Linq;
using BathDream.Data;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BathDream
{
    public class UserAndRolesInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, DBApplicationaContext db)
        {
            //string adminEmail = "root@admin.com";
            //string adminPassword = "Qq1945!";

            string architectPhone = "70000000000";
            string adminPhone = "79030000735";

            if (await roleManager.FindByNameAsync("admin") == null)
                await roleManager.CreateAsync(new IdentityRole("admin"));

            if (await roleManager.FindByNameAsync("architect") == null)
                await roleManager.CreateAsync(new IdentityRole("architect"));

            if (await roleManager.FindByNameAsync("customer") == null)
                await roleManager.CreateAsync(new IdentityRole("customer"));

            if (await roleManager.FindByNameAsync("executor") == null)
                await roleManager.CreateAsync(new IdentityRole("executor"));

            if (await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == architectPhone) == null)
            {
                User architector = new()
                {
                    PhoneNumber = architectPhone
                };

                architector.UserName = architector.Id;

                IdentityResult result = await userManager.CreateAsync(architector);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(architector, "architect");
                    await userManager.UpdateAsync(architector);
                }

                UserProfile userProfile = new()
                {
                    User = architector
                };
                await db.UserProfiles.AddAsync(userProfile);
                await db.SaveChangesAsync();
            }

            if (await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == adminPhone) == null)
            {
                User admin = new()
                {
                    PhoneNumber = adminPhone
                };

                admin.UserName = admin.Id;

                IdentityResult result = await userManager.CreateAsync(admin);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    await userManager.UpdateAsync(admin);
                }
                await db.SaveChangesAsync();
            }


            //if (await userManager.FindByNameAsync(adminEmail) == null)
            //{
            //    User admin = new()
            //    {
            //        Email = adminEmail,
            //        UserName = adminEmail
            //    };

            //    IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

            //    if (result.Succeeded)
            //    {
            //        await userManager.AddToRoleAsync(admin, "admin");
            //        admin.EmailConfirmed = true;
            //        await userManager.UpdateAsync(admin);
            //    }
            //}
        }
    }
}
