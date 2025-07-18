using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Common;
using DevHabit.APi.DTOs.Entries;
using DevHabit.APi.Extensions;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Route("entries")]
    [ApiVersion(1.0)]
    public class EntriesController(
        AppDbContext context,
        LinkService linkService,
        UserContext userContext
    ) : ControllerBase
    {

        [HttpGet]
        [EndpointSummary("Get all entries")]
        [EndpointDescription("Retrieves a paginated list of entries with optional filtering by habit, date range, source, archive status, sorting, and field selection.")]
        public async Task<IActionResult> GetEntries(
            [FromQuery] EntriesParameters query,
            SortMappingProvider sortMappingProvider,
            IDataShaperService<EntryDto> dataShaperService
        )
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (!sortMappingProvider.ValidateMappings<EntryDto, Entry>(query.Sort))
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

            SortMapping[] sortMapping = sortMappingProvider.GetMappings<EntryDto, Entry>();

            IQueryable<EntryDto> entryQuery = context.Entries
                .Where(x => x.UserId == userId)
                .Where(x => query.HabitId == null || x.HabitId == query.HabitId)
                .Where(x => query.FromDate == null || x.Date >= query.FromDate)
                .Where(x => query.ToDate == null || x.Date <= query.ToDate)
                .Where(x => query.Source == null || x.Source == query.Source)
                .Where(x => query.IsArchived == null || x.IsArchived == query.IsArchived)
                .ApplySort(query.Sort, sortMapping)
                .Select(EntryQueries.ProjectToDto())
                ;
            int totalCount = await entryQuery.CountAsync();

            List<EntryDto> items = await entryQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            List<ExpandoObject> newItems = dataShaperService.ShapeData(
                items,
                query.Fields,
                null
                );

            var paginationResult = new PaginationResult<ExpandoObject>
            {
                Items = newItems,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };


            return Ok(paginationResult);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntry(
            string id,
            string? fields,
           [FromHeader(Name = "Accept")] string accept,
            IDataShaperService<EntryDto> dataShaperService
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
            EntryDto? entry = await context.Entries
                .Where(h => h.UserId == userId)
                .Select(EntryQueries.ProjectToDto())
                .FirstOrDefaultAsync(h => h.Id == id);

            if (entry is null)
            {
                return NotFound();
            }

            ExpandoObject newItems = dataShaperService.ShapeData(entry, fields);

            bool includeLinks = accept == CustomMediaTypeNames.Application.HateoasJson;

            if (includeLinks)
            {
                // List<LinkDto> links = CreateLinkForHabit(id, fields);

                // newItems.TryAdd("links", links);

            }

            return Ok(newItems);
        }

        [HttpPost]
        public async Task<ActionResult<EntryDto>> CreateHabit(
            CreateEntryDto createEntryDto,
            IValidator<CreateEntryDto> validator
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
            await validator.ValidateAndThrowAsync(createEntryDto);

            Habit? habit = await context.Habits
                .FirstOrDefaultAsync(h => h.Id == createEntryDto.HabitId && h.UserId == userId);
            if (habit is null)
            {
                return ValidationProblem(
                    new ValidationProblemDetails()
                );
            }


            Entry e = createEntryDto.ToEntity(userId, habit);
            context.Entries.Add(e);
            await context.SaveChangesAsync();

            // var habitDto = e.ToDto();
            // habitDto.Links = CreateLinkForHabit(h.Id, null);

            return CreatedAtAction(nameof(GetEntry), new { id = e.Id }, e);
        }

        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveEntry(string id)
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            Entry? entry = await context.Entries
                .Where(h => h.UserId == userId)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (entry is null)
            {
                return NotFound();
            }

            entry.IsArchived = true;
            entry.UpdatedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/unarchive")]
        public async Task<IActionResult> UnArchiveEntry(string id)
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            Entry? entry = await context.Entries
                .Where(h => h.UserId == userId)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (entry is null)
            {
                return NotFound();
            }

            entry.IsArchived = false;
            entry.UpdatedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("batch")]
        public async Task<ActionResult<List<Entry>>> CreateEntryBatch(
            CreateEntryBatchDto createEntryBatchDto,
            IValidator<CreateEntryBatchDto> validator
        )
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            await validator.ValidateAndThrowAsync(createEntryBatchDto);

            var habitIds = createEntryBatchDto.Entries
                .Select(e => e.HabitId)
                .ToHashSet();
            List<Habit> existingHabits = await context.Habits
                .Where(h => habitIds.Contains(h.Id) && h.UserId == userId)
                .ToListAsync();

            if (existingHabits.Count != habitIds.Count)
            {
                return Problem(
                    detail: "One or more habit Ids is invalid",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            var entries = createEntryBatchDto.Entries
                .Select(e => e.ToEntity(userId: userId, existingHabits.First(h => h.Id == e.HabitId)))
                .ToList();

            context.Entries.AddRange(entries);
            await context.SaveChangesAsync();

            var entryDtos = entries.Select(e => e.ToDto()).ToList();
            
            return CreatedAtAction(nameof(GetEntries), entryDtos);


        }
    
        
        
    }
}