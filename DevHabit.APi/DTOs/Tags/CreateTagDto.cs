using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DevHabit.APi.DTOs.Tags
{
    public sealed record CreateTagDto
    {
        public required string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
    }

}