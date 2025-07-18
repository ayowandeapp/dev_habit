using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.GitHub;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Services
{
    public sealed class GitHubAccessTokenService(AppDbContext context,
        EncryptionService encryptionService)
    {
        public async Task Store(
            string userId,
            StoreGitHubAccessTokenDto accessTokenDto,
            CancellationToken cancellationToken = default
        )
        {
            GitHubAccessToken? existingToken = await GetAcccessToken(userId, cancellationToken);

            string encryptedToken = encryptionService.Encrypt(accessTokenDto.AccessToken);

            if (existingToken is not null)
            {
                existingToken.Token = encryptedToken;
                existingToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(accessTokenDto.ExpiresInDays);

            }
            else
            {
                context.GitHubAccessTokens.Add(
                    new GitHubAccessToken
                    {
                        Id = $"gh_{Guid.CreateVersion7()}",
                        UserId = userId,
                        Token = encryptedToken,
                        ExpiresAtUtc = DateTime.UtcNow.AddDays(accessTokenDto.ExpiresInDays)
                    }
                );
            }

            await context.SaveChangesAsync(cancellationToken);

        }
        public async Task<string?> GetAsync(string userId, CancellationToken cancellationToken = default)
        {
            GitHubAccessToken? gitHubAccessToken = await GetAcccessToken(userId, cancellationToken);

            if (gitHubAccessToken is null)
            {
                return null;
            }

            string decryptedtoken = encryptionService.Decrypt(gitHubAccessToken.Token);

            return decryptedtoken;
        }

        public async Task Revoke(string userId, CancellationToken cancellationToken = default)
        {
            GitHubAccessToken? gitHubAccessToken = await GetAcccessToken(userId, cancellationToken);

            if (gitHubAccessToken is null)
            {
                return;
            }

            context.GitHubAccessTokens.Remove(gitHubAccessToken);

            await context.SaveChangesAsync(cancellationToken);

        }
        

        private async Task<GitHubAccessToken?> GetAcccessToken(string userId, CancellationToken cancellationToken)
        {
            return await context.GitHubAccessTokens.SingleOrDefaultAsync(g => g.UserId == userId, cancellationToken);
        }

    }
}