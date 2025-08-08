using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevHabit.Api.Tests.Services
{
    public class TokenProviderTests
    {
        private readonly TokenProvider _tokenProvider;
        private readonly JwtAuthOptions _jwtAuthOptions;

        public TokenProviderTests()
        {
            IOptions<JwtAuthOptions> options = Options.Create(new JwtAuthOptions()
            {
                Key = "your-secret-key-here-that-should-also-be-fairly-long",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpirationInMinutes = 30,
                RefreshTokenExpirationDays = 7,
            });

            _jwtAuthOptions = options.Value;
            _tokenProvider = new TokenProvider(options);
        }

        [Fact]
        public void Create_ShouldReturnBothAccessTokens()
        {
            // Arrange
            var tokenRequest = new TokenRequest(User.CreateNewId(), "test@example.com", [Roles.Member]);

            // Act
            var accessTokensDto = _tokenProvider.Create(tokenRequest);

            // Assert
            Assert.NotNull(accessTokensDto.AccessToken);
            Assert.NotNull(accessTokensDto.RefreshToken);
        }
        
        
        [Fact]
        public void Create_ShouldGenerateValidAccessToken()
        {
            // Arrange
            var tokenRequest = new TokenRequest(User.CreateNewId(), "test@example.com", [Roles.Member]);

            // Act
            var accessTokensDto = _tokenProvider.Create(tokenRequest);

            // Assert
            var handler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new()
            {
                ValidIssuer = _jwtAuthOptions.Issuer,
                ValidAudience = _jwtAuthOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal? claimsPrincipal = handler.ValidateToken(
                accessTokensDto.AccessToken,
                validationParameters,
                out SecurityToken? validatedToken);

            Assert.NotNull(validatedToken);
            Assert.Equal(tokenRequest.UserId, claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
            Assert.Equal(tokenRequest.Email, claimsPrincipal.FindFirstValue(ClaimTypes.Email));
            Assert.Contains(claimsPrincipal.FindAll(ClaimTypes.Role), claim => claim.Value == Roles.Member);
        }

        [Fact]
        public void Create_ShouldGenerateUniqueRefreshTokens()
        {
            // Arrange
            var tokenRequest = new TokenRequest(User.CreateNewId(), "test@example.com", [Roles.Member]);

            // Act
            var accessTokensDto1 = _tokenProvider.Create(tokenRequest);
            var accessTokensDto2 = _tokenProvider.Create(tokenRequest);

            // Assert
            Assert.NotEqual(accessTokensDto1.RefreshToken, accessTokensDto2.RefreshToken);
        }
        
        [Fact]
        public void Create_ShouldGenerateAccessTokenWithCorrectExpiration()
        {
            // Arrange
            var tokenRequest = new TokenRequest(User.CreateNewId(), "test@example.com", [Roles.Member]);

            // Act
            var accessTokensDto = _tokenProvider.Create(tokenRequest);

            // Assert
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(accessTokensDto.AccessToken);

            DateTime expectedExpiration = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes);
            DateTime actualExpiration = jwtSecurityToken.ValidTo;

            // Allow for a small time difference due to test execution
            Assert.True(Math.Abs((expectedExpiration - actualExpiration).TotalSeconds) < 3);
        }

        [Fact]
        public void Create_ShouldGenerateBase64RefreshToken()
        {
            // Arrange
            var tokenRequest = new TokenRequest(User.CreateNewId(), "test@example.com", [Roles.Member]);

            // Act
            var accessTokensDto = _tokenProvider.Create(tokenRequest);

            // Assert
            Assert.True(IsBase64String(accessTokensDto.RefreshToken));
        }

        private static bool IsBase64String(string base64)
            {
                Span<byte> buffer = new byte[base64.Length];
                return Convert.TryFromBase64String(base64, buffer, out _);
            }
        }
}