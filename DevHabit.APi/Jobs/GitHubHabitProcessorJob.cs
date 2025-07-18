using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.GitHub;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DevHabit.APi.Jobs
{
    public class GitHubHabitProcessorJob(
        AppDbContext _context,
        GitHubAccessTokenService gitHubAccessTokenService,
        GitHubService gitHubService,
        ILogger<GitHubHabitProcessorJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string habitId = context.JobDetail.JobDataMap.GetString("habitId")
                ?? throw new InvalidOperationException("HabitId not found in job data");

            try
            {
                logger.LogInformation("Processing GitHub events for habit {HabitId}", habitId);

                Habit? habit = await _context.Habits.FirstOrDefaultAsync(
                    x => x.Id == habitId &&
                    x.AutomationSource == AutomationSource.None &&
                    !x.IsArchived
                    );

                if (habit is null)
                {
                    logger.LogWarning("Habit {HabitId} not found or no longer configured for GitHub automation", habitId);
                    return;
                }

                string? accessToken = await gitHubAccessTokenService.GetAsync(habit.UserId, context.CancellationToken);

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    logger.LogWarning("No GitHub access token found for user {UserId}", habit.UserId);
                    return;
                }

            GitHubUserProfileDto? profile = await gitHubService.GetUserProfile(accessToken, context.CancellationToken);

            if (profile is null)
            {
                logger.LogWarning("Could not retrieve GitHub profile for user {UserId}", habit.UserId);
                return;
            }
            

            /*
                    List<GitHubEventDto> gitHubEvents = [];
                const int perPage = 100;
                const int pagesToFetch = 10;

                for (int page = 1; page <= pagesToFetch; page++)
                {
                    IReadOnlyList<GitHubEventDto> pageEvents = await gitHubService
                        .GetUserEventsAsync(profile.Login, accessToken, page, perPage, context.CancellationToken);

                    if (!pageEvents.Any())
                    {
                        break;
                    }

                    gitHubEvents.AddRange(pageEvents);
                }

                if (!gitHubEvents.Any())
                {
                    logger.LogWarning("Could not retrieve GitHub events for user {UserId}", habit.UserId);
                    return;
                }

                List<GitHubEventDto> pushEvents = gitHubEvents.Where(x => x.Type == PushEventType).ToList();

                logger.LogInformation("Found {Count} push events for habit {HabitId}", pushEvents.Count, habitId);

                foreach (var pushEvent in pushEvents)
                {
                    bool exists = await dbContext.Entries.AnyAsync(
                        x => x.HabitId == habitId &&
                        x.ExternalId == pushEvent.Id,
                        context.CancellationToken);

                    if (exists)
                    {
                        logger.LogDebug("Entry already exists for event {EventId}", pushEvent.Id);
                        continue;
                    }

                    Entry entry = new()
                    {
                        Id = Entry.CreateNewId(),
                        HabitId = habitId,
                        UserId = habit.UserId,
                        Value = 1, // each push counts as 1
                        Notes = $"""
                            {pushEvent.Actor.Login} pushed:

                            {string.Join(
                                Environment.NewLine,
                                pushEvent.Payload.Commits?.Select(x => $"- {x.Message}") ?? [])}
                        """,
                        Source = EntrySource.Automation,
                        ExternalId = pushEvent.Id,
                        Date = DateOnly.FromDateTime(pushEvent.CreatedAt.DateTime),
                        CreatedAtUtc = DateTime.UtcNow,
                    };

                    _context.Entries.Add(entry);
                    logger.LogInformation("Created entry for event {EventId} on habit {HabitId}", pushEvent.Id, habitId);
                }

                await _context.SaveChangesAsync(context.CancellationToken);

                */
                logger.LogInformation("Completed processing GitHub events for habit {HabitId}", habitId);

                
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing GitHub events for habit {HabitId}", habitId);
                throw;
            }
        }
    }
}