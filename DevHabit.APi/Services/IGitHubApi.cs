using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.GitHub;
using Refit;

namespace DevHabit.APi.Services
{
    [Headers("User-Agent: DevHabit/1.0", "Accept: application/vnd.github+json")]
    public interface IGitHubApi
    {
        [Get("/user")]
        Task<ApiResponse<GitHubUserProfileDto?>> GetUserProfile(
            [Authorize(scheme: "Bearer")] string accessToken,
            CancellationToken cancellationToken = default
        );

    }
}