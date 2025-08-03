using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.EntryImports
{
    public sealed class CreateEntryImportJobDto
    {
        public required IFormFile File { get; init; }
    }
}