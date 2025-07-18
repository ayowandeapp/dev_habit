using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Entries
{
    public class EntriesParameters
    {
        // [FromQuery(Name = "habit_id")]
        public string? HabitId { get; init; }

        // [FromQuery(Name = "from_date")]
        public DateOnly? FromDate { get; init; }

        // [FromQuery(Name = "to_date")]
        public DateOnly? ToDate { get; init; }

        public string? Sort { get; init; }

        public string? Fields { get; init; }

        public EntrySource? Source { get; init; }

        // [FromQuery(Name = "is_archive")]
        public bool? IsArchived { get; init; }

        public int Page { get; init; } = 1;

        // [FromQuery(Name = "page_size")]
        public int PageSize { get; init; } = 10;
        
    }
}