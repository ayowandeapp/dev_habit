using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Models
{
    public sealed class Habit
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public HabitType Type { get; set; }
        public Frequency Frequency { get; set; }
        public Target Target { get; set; }
        public HabitStatus Status { get; set; }
        public bool IsArchived { get; set; }
        public DateOnly? EndDate { get; set; }
        public Milestone? Milestone { get; set; }
        public AutomationSource? AutomationSource { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastCompletedAtUtc { get; set; }

        //collections
        public List<HabitTag> HashTags { get; set; } = [];
        // ef core allows you to skip the join entity table entirely in the many-many
        // and define a navigataton prop. directly to the other entity in
        // the many to many relationship. Hence, 
        public ICollection<Tag> Tags { get; set; } = [];

    }

    public enum AutomationSource
    {
        Github = 1,
        None = 0
    }

    public sealed class Milestone
    {
        public int Target { get; set; }
        public int Current { get; set; }
    }

    public enum HabitType
    {
        None = 0,
        Binary = 1,
        Measurable = 2
    }

    public enum HabitStatus
    {
        None = 0,
        Ongoing = 1,
        Completed = 2
    }

    public sealed class Frequency
    {
        public FrequencyType Type { get; set; }
        public int TimesPerPeriod { get; set; }
    }

    public enum FrequencyType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public sealed class Target
    {
        public int Value { get; set; }
        public string Unit { get; set; }
    }


}