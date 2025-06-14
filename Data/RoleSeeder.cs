using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudentPortalSystem.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using DotNetEnv;

public static class RoleSeeder
{
    public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
    {
        // Load environment variables from .env
        DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

        var userName = Environment.GetEnvironmentVariable("LOGIN_USER");
        var passWord = Environment.GetEnvironmentVariable("LOGIN_PASSWORD");

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
        {
            throw new Exception("Admin credentials not found in environment variables");
        }

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = new[] { "Admin", "User", "Student", "Tutor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminUser = await userManager.FindByEmailAsync(userName);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                FullName = "System Administrator"
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, passWord);
            if (!createAdminResult.Succeeded)
                throw new Exception("Failed to create admin user: " + string.Join(", ", createAdminResult.Errors));

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}