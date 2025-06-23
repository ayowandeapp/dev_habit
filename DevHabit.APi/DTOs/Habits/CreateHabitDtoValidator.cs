using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using FluentValidation;

namespace DevHabit.APi.DTOs.Habits
{
    public sealed class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
    {
        private static readonly string[] AllowedUnits =
        [
            "minutes", "hours", "steps", "km", "cal",
            "pages", "books", "tasks", "sessions"
        ];
        private static readonly string[] AllowedUnitsForBinaryHabits = ["sessions", "tasks"];

        public CreateHabitDtoValidator()
        {
            RuleFor(x => x.Name) //Select the property to validate
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .WithMessage("Habit name must be between 3 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description is not null) // Condition
                .WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid habit type");

            RuleFor(x => x.Frequency.Type)
                .IsInEnum()
                .WithMessage("invalid frequency period");

            RuleFor(x => x.Frequency.TimesPerPeriod)
                .GreaterThan(0)
                .WithMessage("Frequency must be greater than 0");

            RuleFor(x => x.Target.Value)
                .GreaterThan(0)
                .WithMessage("Target value must be greater than 0");

            RuleFor(x => x.Target.Unit)
                .NotEmpty()
                .Must(unit => AllowedUnits.Contains(unit.ToLowerInvariant())) // Validation
                .WithMessage($"Unit must be one of {string.Join(", ", AllowedUnits)}");

            RuleFor(x => x.EndDate)
                .Must(date => date is null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow)) //validation must be true to pass
                .WithMessage("End date must be in the future");

            When(x => x.Milestone is not null, () =>
            {
                RuleFor(x => x.Milestone!.Target)
                    .GreaterThan(0)
                    .WithMessage("Milestone target must be greater than 0");
            });

            RuleFor(x => x.Target.Unit)
                .Must((dto, unit) => IsTargetCompatibleWithType(dto.Type, unit)) //dto give access to the entire object being validated
                .WithMessage("Target unit is not compatible wih the habit type");
        }

        private static bool IsTargetCompatibleWithType(HabitType type, string unit)
        {
            string normalizedUnit = unit.ToLowerInvariant();
            return type switch
            {
                HabitType.Binary => AllowedUnitsForBinaryHabits.Contains(normalizedUnit),
                HabitType.Measurable => AllowedUnits.Contains(normalizedUnit),
                _ => false
            };
        }
    }

}