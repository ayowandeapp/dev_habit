using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Extensions
{
    public static class DbExtensions
    {
        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            //using: Ensures IDisposable objects (like file streams, DB connections) are automatically disposed (frees resources) when they go out of scope.
            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                await dbContext.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied successfully");
            }
            catch (Exception)
            {
                app.Logger.LogInformation("An error occured while applying db migratons");

                throw;
            }


        }
    }
}