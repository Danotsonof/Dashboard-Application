using DashboardApplication.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.Data
{
    /// <summary>
    /// This handles the preseeding of the data
    /// </summary>
    public class AdminUserSeeder
    {
        public static async Task Seeder(AppDbContext ctx, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager) 
        {
            //ctx.Database.EnsureCreated();
            await ctx.Database.EnsureCreatedAsync();
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        /// <summary>
        /// This seeds the roles
        /// </summary>
        /// <param name="roleManager"></param>
        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {

            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole("User");
                roleManager.CreateAsync(role).Wait();
            }

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole("Admin");
                roleManager.CreateAsync(role).Wait();
            }
        }
        /// <summary>
        /// This seeds the user and admin
        /// </summary>
        /// <param name="userManager"></param>
        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("user1@me.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "user1@me.com",
                    Email = "user1@me.com",
                    FirstName = "User",
                    LastName = "One"
                };

                var result = userManager.CreateAsync(user, "P@ssword1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                }
            }

            if (userManager.FindByEmailAsync("alex@localhost").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "admin1@me.com",
                    Email = "admin1@me.com",
                    FirstName = "Admin",
                    LastName = "One"
                };

                var result = userManager.CreateAsync(user, "P@ssword1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
