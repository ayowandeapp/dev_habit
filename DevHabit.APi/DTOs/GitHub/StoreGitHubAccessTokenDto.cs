using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.GitHub
{
    public sealed record StoreGitHubAccessTokenDto
    {
        public string AccessToken { get; init; }
        public double ExpiresInDays { get; init; } = 30;
    }
}