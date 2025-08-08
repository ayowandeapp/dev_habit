using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.Entries;
using DevHabit.APi.Models;
using FluentValidation.Results;

namespace DevHabit.Api.Tests.Validators
{
    public class CreateEntryDtoValidatorTests
    {
        private readonly CreateEntryDtoValidator _validator = new();

        [Fact]
        public async Task Validate_Should_Succeed_When_InputDto_Is_Valid()
        {
            //Arrange
            var dto = new CreateEntryDto
            {
                HabitId = Habit.NewId(),
                Value = 1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            //Act
            ValidationResult result = await _validator.ValidateAsync(dto);

            //Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Validate_Should_Fail_When_InputDto_Is_Valid()
        {
            //Arrange
            var dto = new CreateEntryDto
            {
                HabitId = String.Empty,
                Value = 1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            //Act
            ValidationResult result = await _validator.ValidateAsync(dto);

            //Assert
            Assert.False(result.IsValid);
            ValidationFailure validationFailure = Assert.Single(result.Errors);
            Assert.Equal(nameof(CreateEntryDto.HabitId), validationFailure.PropertyName);
            
        }
    }
}