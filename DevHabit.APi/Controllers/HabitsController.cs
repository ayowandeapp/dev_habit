using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Asp.Versioning;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Common;
using DevHabit.APi.DTOs.Habits;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [Authorize(Roles = Roles.Member)]
    [ApiController]
    [Route("habits")]
    [ApiVersion(1.0)]
    public class HabitsController(
        AppDbContext context,
        LinkService linkService,
        UserContext userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetHabits(
            [FromQuery] HabitQueryParameters query,
            SortMappingProvider sortMappingProvider,
            IDataShaperService<HabitDto> dataShaperService
        )
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            
            if (!sortMappingProvider.ValidateMappings<HabitDto, Habit>(query.Sort))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"The provided sort parameter isnt valid"
                );
            }
            if (!dataShaperService.ValidateFields(query.Fields))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"The provided data shaping fields are not valid {query.Fields}"
                );
            }
            query.Search = query.Search?.Trim().ToLower();

            SortMapping[] sortMapping = sortMappingProvider.GetMappings<HabitDto, Habit>();

            IQueryable<HabitDto> habitsQuery = context
                .Habits
                .Where(h => h.UserId == userId)
                .Where(h => query.Search == null ||
                    h.Name.ToLower().Contains(query.Search) ||
                    (h.Description != null && h.Description.ToLower().Contains(query.Search))
                )
                .Where(h => query.Type == null || h.Type == query.Type)
                .Where(h => query.Status == null || h.Status == query.Status)
                .ApplySort(query.Sort, sortMapping)
                .Select(HabitQueries.ProjectToDto());

            bool includeLinks = query.Accept == CustomMediaTypeNames.Application.HateoasJson;

            int totalCount = await habitsQuery.CountAsync();

            List<HabitDto> items = await habitsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            List<ExpandoObject> newItems = dataShaperService.ShapeData(
                items,
                query.Fields,
                includeLinks ? h => CreateLinkForHabit(h.Id, query.Fields) : null
                );

            var data = new PaginationResult<ExpandoObject>
            {
                Items = newItems,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };

            if (includeLinks)
            {
                data.Links = CreateLinkForHabits(query);
            }

            // var data = await PaginationResult<HabitDto>.CreateAsync(
            //     habitsQuery,
            //     query.Page,
            //     query.PageSize
            // );
            return Ok(data);
        }

        [HttpGet("{id}")]
        [MapToApiVersion(1.0)]
        public async Task<IActionResult> GetHabit(
            string id,
            string? fields,
           [FromHeader(Name = "Accept")] string accept,
            IDataShaperService<HabitWithTagsDto> dataShaperService
        )
        {
            
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            
            if (!dataShaperService.ValidateFields(fields))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"The provided data shaping fields are not valid {fields}"
                );
            }
            HabitWithTagsDto? habit = await context.Habits
            .Where(h => h.UserId == userId)
                .Include(h => h.Tags)
                .Select(HabitQueries.ProjectToHabitWithTagsDto())
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habit is null)
            {
                return NotFound();
            }

            ExpandoObject newItems = dataShaperService.ShapeData(habit, fields);

            bool includeLinks = accept == CustomMediaTypeNames.Application.HateoasJson;

            if (includeLinks)
            {
                List<LinkDto> links = CreateLinkForHabit(id, fields);

                newItems.TryAdd("links", links);

            }

            return Ok(newItems);
        }
/*
        [HttpGet("{id}")]
        [ApiVersion(2.0)]
        public async Task<IActionResult> GetHabitv2(
            string id,
            string? fields,
           [FromHeader(Name = "Accept")] string accept,
            IDataShaperService<HabitWithTagsDtoV2> dataShaperService
        )
        {
            
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (!dataShaperService.ValidateFields(fields))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"The provided data shaping fields are not valid {fields}"
                );
            }
            HabitWithTagsDtoV2? habit = await context.Habits
                .Where(h => h.UserId == userId)
                .Include(h => h.Tags)
                .Select(HabitQueries.ProjectToHabitWithTagsDtoV2())
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habit is null)
            {
                return NotFound();
            }

            ExpandoObject newItems = dataShaperService.ShapeData(habit, fields);

            bool includeLinks = accept == CustomMediaTypeNames.Application.HateoasJson;

            if (includeLinks)
            {
                List<LinkDto> links = CreateLinkForHabit(id, fields);

                newItems.TryAdd("links", links);

            }

            return Ok(newItems);
        }
*/
        [HttpPost]
        public async Task<ActionResult<HabitDto>> CreateHabit(
            CreateHabitDto createHabitDto,
            IValidator<CreateHabitDto> validator
        )
        {
            
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            // var validationResult = await validator.ValidateAsync(createHabitDto);
            // if (!validationResult.IsValid)
            // {
            //     return ValidationProblem(
            //         new ValidationProblemDetails(validationResult.ToDictionary())
            //     );
            // }
            await validator.ValidateAndThrowAsync(createHabitDto);

            Habit h = createHabitDto.ToEntity(userId);
            context.Habits.Add(h);
            await context.SaveChangesAsync();

            var habitDto = h.ToDto();
            habitDto.Links = CreateLinkForHabit(h.Id, null);

            return CreatedAtAction(nameof(GetHabit), new { id = h.Id }, habitDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
        {
            
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            Habit? habit = await context.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
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
            
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            Habit? habit = await context.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (habit is null)
            {
                return NotFound();
            }

            context.Habits.Remove(habit);
            await context.SaveChangesAsync();

            return NoContent();
        }
        private List<LinkDto> CreateLinkForHabits(HabitQueryParameters habitQueryParameters)
        {
            List<LinkDto> links =
            [
                linkService.Create(nameof(GetHabits), "self", HttpMethods.Get, new {
                    page = habitQueryParameters.Page,
                    pageSize = habitQueryParameters.PageSize,
                    fields = habitQueryParameters.Fields,
                    q = habitQueryParameters.Search,
                    sort = habitQueryParameters.Sort,
                    type = habitQueryParameters.Type,
                    status = habitQueryParameters.Status
                }),
                linkService.Create(nameof(CreateHabit), "create", HttpMethods.Post)

            ];

            return links;
        }
        private List<LinkDto> CreateLinkForHabit(string id, string? fields)
        {
            List<LinkDto> links =
            [
                linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new { id, fields }),
                linkService.Create(nameof(UpdateHabit), "update", HttpMethods.Put, new { id }),
                linkService.Create(nameof(DeleteHabit), "delete", HttpMethods.Delete, new { id }),
                linkService.Create(
                    nameof(HabitTagsController.UpsertHabitTags),
                    "upsert-tags",
                    HttpMethods.Put,
                    new { habitId = id },
                    "HabitTags"
                    )

            ];

            return links;
        }


    }
}