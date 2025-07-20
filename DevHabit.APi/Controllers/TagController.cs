using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Tags;
using DevHabit.APi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [ResponseCache(Duration = 120)]
    [Authorize(Roles = Roles.Member)]
    [ApiController]
    [Route("tags")]
    public class TagController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<TagsCollectionDto>> GetTags()
        {
            List<TagDto> tags = await context
                .Tags
                .Select(TagQueries.ProjectToDto())
                .ToListAsync();

            var dataDto = new TagsCollectionDto
            {
                Items = tags
            };
            return Ok(dataDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(string id)
        {
            TagDto? tag = await context
                .Tags
                .Select(TagQueries.ProjectToDto())
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag is null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag(
            CreateTagDto createTagDto,
            IValidator<CreateTagDto> validator
        )
        {
            var validationResult = await validator.ValidateAsync(createTagDto);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(
                    new ValidationProblemDetails(validationResult.ToDictionary())
                );
            }
            Tag tag = createTagDto.ToEntity();
            if (await context.Tags.AnyAsync(t => t.Name == tag.Name))
            {
                return Problem(
                    $"The tag {tag.Name} already exist",
                    statusCode: StatusCodes.Status409Conflict
                );
                // return Conflict($"The tag {tag.Name} already exist");
            }
            context.Tags.Add(tag);
            await context.SaveChangesAsync();

            TagDto tagDto = tag.ToTagDto();

            return CreatedAtAction(nameof(GetTag), new { id = tagDto.Id }, tagDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTag(string id, UpdateTagDto updateTagDto)
        {
            Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null)
            {
                return NotFound();
            }
            tag.UpdateFromDto(updateTagDto);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(string id)
        {
            Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null)
            {
                return NotFound();
            }
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();

            return NoContent();

        }
        
    }

}