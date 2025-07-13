using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Extensions
{
    public static class DbExtensions
    {
        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            //using: Ensures IDisposable objects (like file streams, DB connections) are automatically disposed (frees resources) when they go out of scope.
            using var scope = app.Services.CreateScope();
            await using var appDbContext =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await using var identityDbContext =
                scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

            try
            {
                await appDbContext.Database.MigrateAsync();
                app.Logger.LogInformation("App Database migrations applied successfully");
                await identityDbContext.Database.MigrateAsync();
                app.Logger.LogInformation("identity Database migrations applied successfully");
            }
            catch (Exception e)
            {
                app.Logger.LogInformation(e,"An error occured while applying db migratons");

                throw;
            }

        }

        public static async Task SeedRolesDataAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            RoleManager<IdentityRole> roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                if (!await roleManager.RoleExistsAsync(Roles.Member))
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.Member));
                }
                if (!await roleManager.RoleExistsAsync(Roles.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                }

                app.Logger.LogInformation("successfully created roles");
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "An error occured creating roles");

                throw;
            }
        }
    }
}