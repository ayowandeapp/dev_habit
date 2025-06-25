using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Habits;
using DevHabit.APi.Models;
using DevHabit.APi.Services.Sorting;
using FluentValidation;
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
        public async Task<ActionResult<HabitCollectionDto>> GetHabits(
            [FromQuery] HabitQueryParameters query,
            SortMappingProvider sortMappingProvider
        )
        {
            if (!sortMappingProvider.ValidateMappings<HabitDto, Habit>(query.Sort))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"The provided sort parameter isnt valid"
                );
            }
            query.Search = query.Search?.Trim().ToLower();

            SortMapping[] sortMapping = sortMappingProvider.GetMappings<HabitDto, Habit>();
            
            List<HabitDto> habits = await context
                .Habits
                .Where(h => query.Search == null ||
                    h.Name.ToLower().Contains(query.Search) ||
                    (h.Description != null && h.Description.ToLower().Contains(query.Search))
                )
                .Where(h => query.Type == null || h.Type == query.Type)
                .Where(h => query.Status == null || h.Status == query.Status)
                .ApplySort(query.Sort, sortMapping)
                .Select(HabitQueries.ProjectToDto())
                .ToListAsync();

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
                .Include(h => h.Tags)
                .Select(HabitQueries.ProjectToHabitWithTagsDto())
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habit is null)
            {
                return NotFound();
            }

            return Ok(habit);
        }

        [HttpPost]
        public async Task<ActionResult<HabitDto>> CreateHabit(
            CreateHabitDto createHabitDto,
            IValidator<CreateHabitDto> validator
        )
        {
            // var validationResult = await validator.ValidateAsync(createHabitDto);
            // if (!validationResult.IsValid)
            // {
            //     return ValidationProblem(
            //         new ValidationProblemDetails(validationResult.ToDictionary())
            //     );
            // }
             await validator.ValidateAndThrowAsync(createHabitDto);
            
            Habit h = createHabitDto.ToEntity();
            context.Habits.Add(h);
            await context.SaveChangesAsync();

            var habitDto = h.ToDto();

            return CreatedAtAction(nameof(GetHabit), new { id = h.Id }, habitDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
        {
            Habit? habit = await context.Habits.FirstOrDefaultAsync(h => h.Id == id);
            if (habit is null)
            {
                return NotFound();
            }
            habit.UpdateFromDto(updateHabitDto);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHabit(string id)
        {
            Habit? habit = await context.Habits.FirstOrDefaultAsync(h => h.Id == id);

            if (habit is null)
            {
                return NotFound();
            }

            context.Habits.Remove(habit);
            await context.SaveChangesAsync();

            return NoContent();
        }


        
    }
}