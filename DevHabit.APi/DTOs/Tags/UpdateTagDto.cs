using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Tags
{
    public sealed class UpdateTagDto
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        
    }
}