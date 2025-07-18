using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Entries
{
    public class CreateEntryBatchDto
    {
        public required List<CreateEntryDto> Entries { get; set; }
    }
}