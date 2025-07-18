using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Models
{
    public class Entry
    {
        public string Id { get; set; }
        public string HabitId { get; set; }
        public string UserId { get; set; }
        public int Value { get; set; }
        public string? Notes { get; set; }
        public EntrySource Source { get; set; }
        public string? ExternalId { get; set; }
        public bool IsArchived { get; set; }
        public DateOnly Date { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public Habit Habit { get; set; }
    }

    public enum EntrySource
    {
        Manuel = 0,
        Automation = 1
    }
}