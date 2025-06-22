using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.HabitTags;
using DevHabit.APi.Migrations.Application;
using DevHabit.APi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Route("habits/{habitId}/tags")]
    public class HabitTagsController(AppDbContext context) : ControllerBase
    {
        [HttpPut]
        public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagsDto upsertHabitTagsDto)
        {
            var habit = await context.Habits
                .Include(h => h.HashTags)
                .FirstOrDefaultAsync(h => h.Id == habitId);
            if (habit is null)
            {
                return NotFound("habit does not exist");
            }
            //verify if the existing hashtags is the same as what is being sent
            var currentTagIds = habit.HashTags.Select(ht => ht.TagId).ToHashSet();
            if (currentTagIds.SetEquals(upsertHabitTagsDto.TagIds))
            {
                return NoContent();
            }
            //verify if all the ids exit on the db
            var existingTagIds = await context
                .Tags
                .Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();
            if (existingTagIds.Count != upsertHabitTagsDto.TagIds.Count)
            {
                return BadRequest("One or more tag ids is invalid");
            }
            //remove all ids in the db not present in the dto
            habit.HashTags.RemoveAll(ht => upsertHabitTagsDto.TagIds.Contains(ht.TagId));
            //get all ids not present in db but present in dto
            string[] tagIdsToAdd = upsertHabitTagsDto.TagIds.Except(currentTagIds).ToArray();
            habit.HashTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
            {
                HabitId = habitId,
                TagId = tagId
            }));

            await context.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{tagId}")]
        public async Task<ActionResult> DeleteHabitTag(string habitId, string tagId)
        {
            var habitTag = await context.HabitTags
                .SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);

            if (habitTag is null)
            {
                return NotFound();
            }

            context.HabitTags.Remove(habitTag);
            await context.SaveChangesAsync();
            
            return NoContent();

        }
    }
}