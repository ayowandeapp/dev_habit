using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Habits;
using DevHabit.APi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Route("habits")]
    public class HabitsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<HabitCollectionDto>> GetHabits()
        {
            List<HabitDto> habits = await context.Habits.
                Select(h => new HabitDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description,
                    Type = h.Type,
                    Frequency = new FrequencyDto
                    {
                        Type = h.Frequency.Type,
                        TimesPerPeriod = h.Frequency.TimesPerPeriod
                    },
                    Target = new TargetDto
                    {
                        Value = h.Target.Value,
                        Unit = h.Target.Unit
                    },
                    Status = h.Status,
                    IsArchived = h.IsArchived,
                    EndDate = h.EndDate,
                    Milestone = h.Milestone == null ? null : new MilestoneDto
                    {
                        Target = h.Milestone.Target,
                        Current = h.Milestone.Current
                    },
                    CreatedAtUtc = h.CreatedAtUtc,
                    UpdatedAtUtc = h.UpdatedAtUtc,
                    LastCompletedAtUtc = h.LastCompletedAtUtc,

                }).ToListAsync();

            var data = new HabitCollectionDto
            {
                Data = habits
            };
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HabitDto>> GetHabit(string id)
        {
            var habit = await context.Habits
                .Select(h => new HabitDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description,
                    Type = h.Type,
                    Frequency = new FrequencyDto
                    {
                        Type = h.Frequency.Type,
                        TimesPerPeriod = h.Frequency.TimesPerPeriod
                    },
                    Target = new TargetDto
                    {
                        Value = h.Target.Value,
                        Unit = h.Target.Unit
                    },
                    Status = h.Status,
                    IsArchived = h.IsArchived,
                    EndDate = h.EndDate,
                    Milestone = h.Milestone == null ? null : new MilestoneDto
                    {
                        Target = h.Milestone.Target,
                        Current = h.Milestone.Current
                    },
                    CreatedAtUtc = h.CreatedAtUtc,
                    UpdatedAtUtc = h.UpdatedAtUtc,
                    LastCompletedAtUtc = h.LastCompletedAtUtc,

                })
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habit is null)
            {
                return NotFound();
            }
            
            return Ok(habit);
        }
        
        
    }
}