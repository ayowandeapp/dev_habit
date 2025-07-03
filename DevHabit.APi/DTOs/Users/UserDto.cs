using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Users
{
    public sealed class UserDto
    {        
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}