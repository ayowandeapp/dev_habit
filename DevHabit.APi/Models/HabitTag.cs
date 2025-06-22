using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Models
{
    public sealed class HabitTag
    {
        [MaxLength(191)]
        public string HabitId { get; set; }
        [MaxLength(191)]
        public string TagId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}