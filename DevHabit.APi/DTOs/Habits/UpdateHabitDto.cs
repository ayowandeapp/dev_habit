using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Habits
{
    public sealed class UpdateHabitDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required HabitType Type { get; init; }
        public required FrequencyDto Frequency { get; init; }
        public required TargetDto Target { get; init; }
        public DateOnly? EndDate { get; init; }
        public UpdateMilestoneDto? Milestone { get; init; }

    }

    public sealed record UpdateMilestoneDto
    {
        public required int Target { get; init; }
    }
}