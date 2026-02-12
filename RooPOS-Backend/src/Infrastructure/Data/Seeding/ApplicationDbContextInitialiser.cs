using Domain.Constants;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Seeding;

public static class InitialiserExtensions
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        var roles = new[]
        {
            new ApplicationRole { Name = Roles.Admin },
            new ApplicationRole { Name = Roles.Manager },
            new ApplicationRole { Name = Roles.Cashier },
            new ApplicationRole { Name = Roles.StoreKeeper }
        };

        if (!roleManager.Roles.AsNoTracking().Any())
        {
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }
         
        // Default users
        var admin = new ApplicationUser { UserName = "admin@roopos", Email = "admin@roopos.com", EmailConfirmed = true };

        if (userManager.Users.AsNoTracking().All(u => u.UserName != admin.UserName))
        {
            var result =  await userManager.CreateAsync(admin, "123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
        // defualt data
    }
}
