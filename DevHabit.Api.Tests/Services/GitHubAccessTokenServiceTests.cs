using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.GitHub;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Tests.Services
{
    public class GitHubAccessTokenServiceTests : IDisposable
    {        
        private readonly GitHubAccessTokenService _gitHubAccessTokenService;
        private readonly AppDbContext _dbContext;
        private readonly EncryptionService _encryptionService;

        public GitHubAccessTokenServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(dbContextOptions);

            IOptions<EncryptionOptions> encryptionOptions = Options.Create(new EncryptionOptions()
            {
                Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            });

            _encryptionService = new EncryptionService(encryptionOptions);
            _gitHubAccessTokenService = new GitHubAccessTokenService(_dbContext, _encryptionService);

        }
        [Fact]
        public async Task StoreAsync_ShouldCreateNewToken_WhenUserDoesNotHaveOne()
        {
            // Arrange
            string userId = User.CreateNewId();

            StoreGitHubAccessTokenDto dto = new()
            {
                AccessToken = "github-token",
                ExpiresInDays = 30,
            };

            // Act
            await _gitHubAccessTokenService.Store(userId, dto);

            // Assert
            GitHubAccessToken? token = await _dbContext.GitHubAccessTokens.FirstOrDefaultAsync(x => x.UserId == userId);

            Assert.NotNull(token);
            Assert.Equal(userId, token.UserId);
            Assert.NotEqual(dto.AccessToken, token.Token);
            Assert.True(token.ExpiresAtUtc > DateTime.UtcNow);
        }
        
        [Fact]
        public async Task StoreAsync_ShouldUpdateExistingToken_WhenUserHaveOne()
        {
            // Arrange
            string userId = User.CreateNewId();

            GitHubAccessToken existingToken = new()
            {
                Id = GitHubAccessToken.CreateNewId(),
                UserId = userId,
                Token = "github-token",
                ExpiresAtUtc = DateTime.UtcNow.AddDays(29),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            };

            _dbContext.GitHubAccessTokens.Add(existingToken);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            StoreGitHubAccessTokenDto dto = new()
            {
                AccessToken = "new-github-token",
                ExpiresInDays = 30,
            };

            // Act
            await _gitHubAccessTokenService.Store(userId, dto);

            // Assert
            GitHubAccessToken? token = await _dbContext.GitHubAccessTokens.FirstOrDefaultAsync(x => x.UserId == userId);

            Assert.NotNull(token);
            Assert.Equal(existingToken.Id, token.Id);
            Assert.Equal(existingToken.UserId, token.UserId);
            Assert.NotEqual(existingToken.Token, token.Token);
            Assert.True(token.ExpiresAtUtc > existingToken.ExpiresAtUtc);
        }


        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenTokenDoesNotExist()
        {
            // Arrange
            string userId = User.CreateNewId();

            // Act
            string? token = await _gitHubAccessTokenService.GetAsync(userId);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task RevokeAsync_ShouldRemoveToken_WhenTokenExists()
        {
            // Arrange
            string userId = User.CreateNewId();

            GitHubAccessToken existingToken = new()
            {
                Id = GitHubAccessToken.CreateNewId(),
                UserId = userId,
                Token = "github-token",
                ExpiresAtUtc = DateTime.UtcNow.AddDays(29),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            };

            _dbContext.GitHubAccessTokens.Add(existingToken);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            // Act
            await _gitHubAccessTokenService.Revoke(userId);

            // Assert
            bool tokenExists = await _dbContext.GitHubAccessTokens.AnyAsync(x => x.UserId == userId);
            Assert.False(tokenExists);
        }

        [Fact]
        public async Task RevokeAsync_ShouldNotThrow_WhenTokenDoesNotExist()
        {
            // Arrange
            string userId = User.CreateNewId();

            // Act
            await _gitHubAccessTokenService.Revoke(userId);

            // Assert
            bool tokenExists = await _dbContext.GitHubAccessTokens.AnyAsync(x => x.UserId == userId);
            Assert.False(tokenExists);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}