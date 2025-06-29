using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Common
{
    public class LinkDto
    {
        public required string Href { get; init; }
        public required string Rel { get; init; }
        public required string Method { get; set; }
    }
}