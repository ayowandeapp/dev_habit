using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.GitHub;
using DevHabit.APi.Models;

namespace DevHabit.APi.Services
{
    public class GitHubService(
        IHttpClientFactory httpClientFactory,
        ILogger<GitHubAccessToken> logger)
    {
        public async Task<GitHubUserProfileDto?> GetUserProfile(
            string accessToken,
            CancellationToken cancellationToken = default
        )
        {
            using HttpClient client = CreateGitHubClient(accessToken);

            HttpResponseMessage res = await client.GetAsync("user", cancellationToken);

            if (!res.IsSuccessStatusCode)
            {
                logger.LogWarning("Falied");
                return null;
            }

            string content = await res.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var deserialize = JsonSerializer.Deserialize<GitHubUserProfileDto>(content, options);
            return deserialize;
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
        private HttpClient CreateGitHubClient(string accessToken)
        {
            HttpClient client = httpClientFactory.CreateClient("github");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return client;
        }
    }
}