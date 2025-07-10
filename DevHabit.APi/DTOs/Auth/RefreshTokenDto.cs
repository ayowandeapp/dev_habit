using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Auth
{
    public class RefreshTokenDto
    {
        public required string RefreshToken { get; init; }
    }
}