using Microsoft.AspNetCore.Identity;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Data;

public static class DataSeeder
{
    
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "ADMIN", "USER" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        
        
        var adminEmail = "admin@test.pl";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if(adminUser == null)
        {
            var newAdmin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var createAdmin = await userManager.CreateAsync(newAdmin, "zaq1@WSX");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "ADMIN");
            }
        }
    }
}