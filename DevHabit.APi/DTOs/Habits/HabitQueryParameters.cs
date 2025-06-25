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
        [FromQuery(Name="q")]
        public string? Search { get; set; }
        public HabitType? Type { get; set; }
        public HabitStatus? Status { get; set; }
        public string? Sort { get; set; }

    }
}