using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Entries
{
    public sealed class CreateEntryDto
    {
        
        public required string HabitId { get; init; }
        public required int Value { get; init; }
        public string? Notes { get; init; }
        public required DateOnly Date { get; init; }
        
    }
}