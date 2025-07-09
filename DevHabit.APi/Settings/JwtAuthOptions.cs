using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Settings
{
    public sealed record JwtAuthOptions
    {
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public string Key { get; init; }
        public int ExpirationInMinutes { get; init; }
        public int RefreshTokenExpirationDays { get; init; }
    }
}