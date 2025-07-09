using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Auth
{
    public sealed record LoginUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}