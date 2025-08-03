using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.GitHub;
using DevHabit.APi.Models;
using Refit;

namespace DevHabit.APi.Services
{
    public class RefitGitHubService(
        IGitHubApi gitHubApi,
        ILogger<GitHubAccessToken> logger)
    {
        public async Task<GitHubUserProfileDto?> GetUserProfile(
            string accessToken,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrEmpty(accessToken);
            ApiResponse<GitHubUserProfileDto?> res = await gitHubApi.GetUserProfile(accessToken, cancellationToken);

            if (!res.IsSuccessStatusCode)
            {
                logger.LogWarning("Falied");
                return null;
            }
            return res.Content;
        }
/*
        public async Task<IReadOnlyList<GitHubEventDto>?> GetUserEventsAsync(
            string username,
            string accessToken,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrEmpty(username);

            using HttpClient client = CreateGitHubClient(accessToken);

            HttpResponseMessage res = await client.GetAsync(
                $"users/{username}/events?per_page=100",
                cancellationToken
            );
            
            if (!res.IsSuccessStatusCode)
            {
                logger.LogWarning("Falied");
                return null;
            }

            string content = await res.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<GitHubEventDto>(content);

        }
*/
    }
}