using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.Common;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Habits
{
    public sealed record HabitDto: ILinksResponse
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required HabitType Type { get; init; }
        public required FrequencyDto Frequency { get; init; }
        public required TargetDto Target { get; init; }
        public required HabitStatus Status { get; init; }
        public required bool IsArchived { get; init; }
        public DateOnly? EndDate { get; init; }
        public MilestoneDto? Milestone { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public required DateTime? UpdatedAtUtc { get; init; }
        public required DateTime? LastCompletedAtUtc { get; init; }

        public List<LinkDto> Links { get; set; } //comment this
    }

    public sealed record MilestoneDto
    {
        public required int Target { get; init; }
        public required int Current { get; init; }
    }

    public sealed record FrequencyDto
    {
        public required FrequencyType Type { get; init; }
        public required int TimesPerPeriod { get; init; }
    }

    public sealed record TargetDto
    {
        public required int Value { get; init; }
        public required string Unit { get; init; }
    }

}