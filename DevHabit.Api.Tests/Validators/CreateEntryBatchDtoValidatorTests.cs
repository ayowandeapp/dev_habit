using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.Entries;
using DevHabit.APi.Models;
using FluentValidation.TestHelper;

namespace DevHabit.Api.Tests.Validators
{
    public class CreateEntryBatchDtoValidatorTests
    {
        private readonly CreateEntryBatchDtoValidator _validator;
        private readonly CreateEntryDtoValidator _entryValidator = new();

        public CreateEntryBatchDtoValidatorTests()
        {
            _validator = new CreateEntryBatchDtoValidator(_entryValidator);
        }

        [Fact]
        public async Task Validate_Should_Not_Return_Error_When_All_Prperties_Are_Valid()
        {
            //Arrange
            var dto = new CreateEntryBatchDto
            {
                Entries =
                [
                    new CreateEntryDto
                    {
                        HabitId = Habit.NewId(),
                        Value = 1,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow)
                    }
                ]
            };
            //Act
            var result = await _validator.TestValidateAsync(dto);

            //Assert
            result.ShouldNotHaveAnyValidationErrors();

        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Prperties_Are_Invalid()
        {
            //Arrange
            var dto = new CreateEntryBatchDto
            {
                Entries = []
            };
            //Act
            var result = await _validator.TestValidateAsync(dto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Entries);

        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Entries_ExceeedMax_Batch_size()
        {
            //Arrange
            var dto = new CreateEntryBatchDto
            {
                Entries = Enumerable.Range(0, 21)
                    .Select(_ =>
                        new CreateEntryDto
                        {
                            HabitId = Habit.NewId(),
                            Value = 1,
                            Date = DateOnly.FromDateTime(DateTime.UtcNow)
                        }).ToList()
            };
            //Act
            var result = await _validator.TestValidateAsync(dto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Entries);

        }
        
        [Fact]
        public async Task Validate_ShouldReturnError_WhenAnyEntryIsInvalid()
        {
            // Arrange
            CreateEntryBatchDto dto = new()
            {
                Entries =
                [
                    new()
                    {
                        HabitId = string.Empty,
                        Value = 1,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                ],
            };

            // Act
            TestValidationResult<CreateEntryBatchDto> result = await _validator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor("Entries[0].HabitId");
        }
    }
}