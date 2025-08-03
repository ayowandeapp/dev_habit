using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DevHabit.APi.DTOs.EntryImports
{
    public class CreateEntryImportJobDtoValidator : AbstractValidator<CreateEntryImportJobDto>
    {
        private const int MaxFileSizeInMegabytes = 10;
        private const int MaxFileSizeInBytes = MaxFileSizeInMegabytes * 1024 * 1024;
        public CreateEntryImportJobDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("Kindly specify a file");
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(MaxFileSizeInBytes)
                .WithMessage($"File size must be less than{MaxFileSizeInMegabytes}");

            RuleFor(x => x.File.FileName)
                .Must(fileName => fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("File must be a CSV file");
        }
        
    }
}