/// <summary>
/// This is the DataSeeder class that seeds the database with roles and an admin user.
/// It uses the ASP.NET Core Identity framework to manage user roles and authentication.
/// </summary>
/// reference = https://www.youtube.com/watch?app=desktop&v=Y6DCP-yH-9Q

using System;
using Programming3A.Constants;
using Microsoft.AspNetCore.Identity;

namespace Programming3A.Seed;

public static class DataSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Seed Roles
        string[] roles = { Roles.Admin, Roles.Farmer, Roles.Employee };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        // Seed Admin User (Basic Password and Email for Prototype) would enhance in real application then delete this
        // In a real application, you would want to use a more secure password and email management
        string adminEmail = "admin@farmhub.com";
        string adminPassword = "Admin123!";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            // Create the admin user 
            var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin); 
            }
        }
    }
}

// ------------------------------------------End of File-----------------------------------------------------