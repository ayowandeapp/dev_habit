using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DevHabit.APi.DTOs.Entries
{
    public class CreateEntryBatchDtoValidator : AbstractValidator<CreateEntryBatchDto>
    {
        public CreateEntryBatchDtoValidator(CreateEntryDtoValidator validationRules)
        {
            RuleFor(x => x.Entries)
                .NotEmpty()
                .WithMessage("At least one entry is required")
                .Must(entries => entries.Count <= 20)
                .WithMessage("Maximum of 20 entries per batch");

            RuleForEach(x => x.Entries)
                .SetValidator(validationRules);
        }
        
    }
}