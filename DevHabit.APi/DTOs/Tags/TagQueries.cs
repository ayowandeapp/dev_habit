using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Tags
{
    internal static class TagQueries
    {
        public static Expression<Func<Tag, TagDto>> ProjectToDto()
        {
            return t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CreatedAtUtc = t.CreatedAtUtc,
                UpdatedAtUtc = t.UpdatedAtUtc
            };
        }
    }
}