using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.APi.DTOs.Habits
{
    public sealed class HabitQueryParameters
    {
        [FromQuery(Name = "q")]
        public string? Search { get; set; }
        public HabitType? Type { get; init; }
        public HabitStatus? Status { get; init; }
        public string? Sort { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? Fields { get; init; }
        [FromHeader(Name ="Accept")]
        public string? Accept { get; init; }

    }
}