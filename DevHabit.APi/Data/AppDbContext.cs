using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data.Configurations;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Data
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Habit> Habits { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasDefaultSchema(Schemas.App);

            //Apply configurations
            // modelBuilder.ApplyConfiguration(new HabitConfiguration()); //OR
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

    }

    public static class Schemas
    {
        public const string App = "dev_habit";
    }
}