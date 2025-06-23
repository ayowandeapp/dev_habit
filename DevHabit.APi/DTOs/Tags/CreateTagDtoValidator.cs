using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DevHabit.APi.DTOs.Tags
{
    public sealed class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must be greater than 3 characters");
            RuleFor(x => x.Description).MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
        }
    }
}