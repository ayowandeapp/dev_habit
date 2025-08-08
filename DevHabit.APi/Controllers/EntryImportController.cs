using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Common;
using DevHabit.APi.DTOs.EntryImports;
using DevHabit.APi.Jobs.EntryImport;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Member)]
    [Route("entries/imports")]
    public class EntryImportController(
        AppDbContext context,
        LinkService linkService,
        UserContext userContext,
        ISchedulerFactory schedulerFactory
    // ISchedularFactory
    ) : ControllerBase
    {
        [HttpPost]
        [EndpointSummary("create import jobs")]
        [EndpointDescription("Create entry import jobs.")]
        public async Task<ActionResult<EntryImportJobDto>> CreateImportJob(
            CreateEntryImportJobDto createEntryImportJobDto,
            IValidator<CreateEntryImportJobDto> validator
        )
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            await validator.ValidateAsync(createEntryImportJobDto);

            //create job
            using var memoryStream = new MemoryStream();
            await createEntryImportJobDto.File.CopyToAsync(memoryStream);

            var importJob = createEntryImportJobDto.ToEntity(userId, memoryStream.ToArray());

            context.EntryImportJobs.Add(importJob);
            await context.SaveChangesAsync();

            // schedule processing job\
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            IJobDetail jobDetail = JobBuilder.Create<ProcessEntryImportJob>()
                .WithIdentity($"process-entry-import-{importJob.Id}")
                .UsingJobData("importJobId", importJob.Id)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"process-entry-import-trigger-{importJob.Id}")
                .StartNow()
                .Build();
            //start the backgroud process immediately
            await scheduler.ScheduleJob(jobDetail, trigger);

            EntryImportJobDto importJobDto = importJob.ToDto();

            return CreatedAtAction(nameof(GetImportJob), new { id = importJobDto.Id }, importJobDto);

        }

        [HttpGet("{id}")]
        [EndpointSummary("Get an import job by ID")]
        [EndpointDescription("Retrieves a specific entry import job by its unique identifier with optional field selection.")]
        public async Task<IActionResult> GetImportJob(
            string id,
            // EntryImportJobParameters entryImportJobParameters,
            CancellationToken cancellationToken)
        {
            string? userId = await userContext.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            // string? fields = entryImportJobParameters.Fields;

            EntryImportJobDto? result = await context.EntryImportJobs
                .Where(x => x.Id == id && x.UserId == userId)
                .Select(EntryImportQueries.ProjectToDto())
                .FirstOrDefaultAsync();

            return result is null ? NotFound() : Ok(result);
        }
        
         [HttpGet]
        [EndpointSummary("Get all import jobs")]
        [EndpointDescription("Retrieves a paginated list of entry import jobs with optional field selection.")]
    public async Task<IActionResult> GetImportJobs(
        EntryImportJobsParameters entryImportsParameters,
        CancellationToken cancellationToken)
    {
        string? userId = await userContext.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var (fields, page, pageSize) = entryImportsParameters;

        IQueryable<EntryImportJob> query = context.EntryImportJobs
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc);

        int totalCount = await query.CountAsync(cancellationToken: cancellationToken);

        List<EntryImportJobDto> importJobDtos = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(EntryImportQueries.ProjectToDto())
            .ToListAsync(cancellationToken: cancellationToken);

            var paginationResult = new PaginationResult<EntryImportJobDto>
            {
                Items = importJobDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };


        return Ok(paginationResult);
    }
    }
}