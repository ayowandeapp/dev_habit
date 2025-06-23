using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using FluentValidation;

namespace DevHabit.APi.DTOs.Habits
{
    public sealed record CreateHabitDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required HabitType Type { get; init; }
        public required FrequencyDto Frequency { get; init; }
        public required TargetDto Target { get; init; }
        public DateOnly? EndDate { get; init; }
        public MilestoneDto? Milestone { get; init; }

    }
}