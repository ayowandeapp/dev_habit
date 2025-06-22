using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.HabitTags
{
    public sealed record UpsertHabitTagsDto
    {
        public required List<string> TagIds { get; init; }
    }
}