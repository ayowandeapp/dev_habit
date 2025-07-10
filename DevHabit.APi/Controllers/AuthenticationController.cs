using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Auth;
using DevHabit.APi.DTOs.Users;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Route("auth")]
    // [AllowAnonymous] //make controller publicly available
    public sealed class AuthenticationController(
        UserManager<IdentityUser> userManager,
        AppIdentityDbContext identityDbContext,
        AppDbContext appDbContext,
        TokenProvider tokenProvider,
        IOptions<JwtAuthOptions> options
    ) : ControllerBase
    {
        private readonly JwtAuthOptions _jwtAuthOptions = options.Value;


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            //To prevent data inconsistency, share the identity transaction with appcontext
            using var transaction = await identityDbContext.Database.BeginTransactionAsync();
            appDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
            await appDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

            var identityUser = new IdentityUser
            {
                Email = registerUserDto.Email,
                UserName = registerUserDto.Email
            };

            IdentityResult result = await userManager.CreateAsync(identityUser, registerUserDto.Password);

            if (!result.Succeeded)
            {
                var extensions = new Dictionary<string, object?>
                {
                    {
                        "errors",
                        result.Errors.ToDictionary(e => e.Code, e => e.Description)
                    }
                };
                return Problem(
                    "Unable to register user, please try again",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: extensions
                );
            }

            User user = registerUserDto.ToEntity();
            user.identityId = identityUser.Id;

            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();

            var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email);
            AccessTokenDto tokens = tokenProvider.Create(tokenRequest);

            var refreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Token = tokens.RefreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
            };
            identityDbContext.RefreshTokens.Add(refreshToken);
            await identityDbContext.SaveChangesAsync();


            await transaction.CommitAsync();

            return Ok(tokens);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, loginUserDto.Password))
            {
                return Unauthorized();
            }

            var tokenRequest = new TokenRequest(user.Id, user.Email!);
            var tokens = tokenProvider.Create(tokenRequest);

            var refreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Token = tokens.RefreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
            };
            identityDbContext.RefreshTokens.Add(refreshToken);
            await identityDbContext.SaveChangesAsync();

            return Ok(tokens);
        }
        
        
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto refreshTokenDto)
        {
            RefreshToken? refreshToken = await identityDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

            if (refreshToken is null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!);
            AccessTokenDto tokens = tokenProvider.Create(tokenRequest);

            refreshToken.Token = tokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
           
            await identityDbContext.SaveChangesAsync();

            return Ok(tokens);
        }
    }
}