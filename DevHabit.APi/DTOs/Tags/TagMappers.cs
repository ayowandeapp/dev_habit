using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Tags
{
    internal static class TagMappers
    {
        public static Tag ToEntity(this CreateTagDto createTagDto)
        {
            return new Tag
            {
                Id = $"t_{Guid.CreateVersion7()}",
                Name = createTagDto.Name,
                Description = createTagDto.Description
            };
        }

        public static TagDto ToTagDto(this Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                CreatedAtUtc = tag.CreatedAtUtc,
                UpdatedAtUtc = tag.UpdatedAtUtc
            };
        }

        public static void UpdateFromDto(this Tag tag, UpdateTagDto updateTagDto)
        {
            tag.Name = updateTagDto.Name;
            tag.Description = updateTagDto.Description ?? tag.Description ;
            tag.UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}