using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DevHabit.APi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace DevHabit.APi.Services
{
    public sealed record TokenRequest(string UserId, string Email);
    public sealed record AccessTokenDto(string AccessToken, string RefreshToken);
    public class TokenProvider(IOptions<JwtAuthOptions> options)
    {
        private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

        public AccessTokenDto Create(TokenRequest tokenRequest)
        {
            return new AccessTokenDto(GenerateAccessToken(tokenRequest), GenerateRefreshToken());
        }

        private string GenerateRefreshToken()
        {
            return string.Empty;
        }

        private string GenerateAccessToken(TokenRequest tokenRequest)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
                new Claim(JwtRegisteredClaimNames.Email, tokenRequest.Email)
            ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
                SigningCredentials = credentials,
                Issuer = _jwtAuthOptions.Issuer,
                Audience = _jwtAuthOptions.Audience
            };

            var handler = new JsonWebTokenHandler();
            string accessToken = handler.CreateToken(tokenDescriptor);
            return accessToken;


        }
    }
}