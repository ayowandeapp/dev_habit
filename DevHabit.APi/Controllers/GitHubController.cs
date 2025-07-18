using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.GitHub;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.APi.Controllers
{
    [Authorize(Roles = Roles.Member)]
    [ApiController]
    [Route("github")]
    public sealed class GitHubController(
        GitHubAccessTokenService gitHubAccessTokenService,
        GitHubService gitHubService,
        UserContext userContext,
        LinkService linkService
    ) : ControllerBase
    {
        [HttpPut("personal-access-token")]
        public async Task<IActionResult> StoreAccessToken(
            StoreGitHubAccessTokenDto storeGitHubAccessTokenDto
        )
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            await gitHubAccessTokenService.Store(userId, storeGitHubAccessTokenDto);
            return NoContent();
        }

        [HttpDelete("personal-access-token")]
        public async Task<IActionResult> RevokeAccessToken()
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            await gitHubAccessTokenService.Revoke(userId);

            return NoContent();
        }

        [HttpGet("profile")]
        public async Task<ActionResult<GitHubUserProfileDto>> GetUserProfile()
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            string? accessToken = await gitHubAccessTokenService.GetAsync(userId);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Unauthorized();
            }

            GitHubUserProfileDto? userProfile = await gitHubService.GetUserProfile(accessToken);
            if (userProfile is null)
            {
                return NotFound();
            }

            return Ok(userProfile);

        }
    }
}