using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DevHabit.APi.Jobs
{
    public sealed class GitHubAutomationSchedularJob(
        AppDbContext _context,
        ILogger<GitHubAutomationSchedularJob> logger
    ) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                logger.LogInformation("starting github automation schedular");
                
                List<Habit> habitsToProcess = await _context.Habits
                    .Where(x => x.AutomationSource == AutomationSource.Github && !x.IsArchived)
                    .ToListAsync();
                logger.LogInformation("Found {Count} habits with GitHub automation", habitsToProcess.Count);

                foreach (var habit in habitsToProcess)
                {
                    // Create trigger for immediate execution
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity($"github-habit-{habit.Id}", "github-habits")
                        .StartNow()
                        .Build();

                    // Create the job with habit data
                    IJobDetail jobDetail = JobBuilder.Create<GitHubHabitProcessorJob>()
                        .WithIdentity($"github-habit-{habit.Id}", "github-habits")
                        .UsingJobData("habitId", habit.Id)
                        .Build();
                        
                    // Schedule the job
                    await context.Scheduler.ScheduleJob(jobDetail, trigger);
                    logger.LogInformation("Scheduled processor job for habit {HabitId}", habit.Id);
                
                }

                logger.LogInformation("Completed GitHub automation scheduler job");

                
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error scheduling GitHub automation scheduler job");
                throw;
            }
        }
    }
}