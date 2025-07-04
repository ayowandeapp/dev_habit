using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Auth
{
    public sealed record RegisterUserDto
    {
        public required string Email { get; init; }
        public required string Name { get; init; }
        public required string Password { get; init; }
        public required string ConfirmPassword { get; init; }
    }
}