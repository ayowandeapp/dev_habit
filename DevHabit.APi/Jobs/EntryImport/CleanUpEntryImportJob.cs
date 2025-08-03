using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DevHabit.APi.Jobs.EntryImport
{
    public class CleanUpEntryImportJob(
        AppDbContext dbContext,
        ILogger<ProcessEntryImportJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Delete completed jobs older than 7 days
                DateTime completedJobsCutoffDate = DateTime.UtcNow.AddDays(-7);

                int deletedCount = await dbContext.EntryImportJobs
                    .Where(x => x.Status == EntryImportStatus.Completed)
                    .Where(x => x.CompletedAtUtc < completedJobsCutoffDate)
                    .ExecuteDeleteAsync(context.CancellationToken);

                if (deletedCount > 0)
                {
                    logger.LogInformation("Deleted {Count} old import jobs", deletedCount);
                }

                // Delete failed jobs older than 30 days
                DateTime failedJobsCutoffDate = DateTime.UtcNow.AddDays(-30);

                deletedCount = await dbContext.EntryImportJobs
                    .Where(x => x.Status == EntryImportStatus.Failed)
                    .Where(x => x.CompletedAtUtc < failedJobsCutoffDate)
                    .ExecuteDeleteAsync(context.CancellationToken);

                if (deletedCount > 0)
                {
                    logger.LogInformation("Deleted {Count} old failed import jobs", deletedCount);
                }

                // Delete stuck jobs (Processing for more than 2 hours)
                DateTime processingJobsCutoffDate = DateTime.UtcNow.AddHours(-2);

                deletedCount = await dbContext.EntryImportJobs
                    .Where(x => x.Status == EntryImportStatus.Processing)
                    .Where(x => x.CompletedAtUtc < processingJobsCutoffDate)
                    .ExecuteDeleteAsync(context.CancellationToken);

                if (deletedCount > 0)
                {
                    logger.LogInformation("Deleted {Count} stuck import jobs", deletedCount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up old import jobs");
            }
            
        }
    }
}