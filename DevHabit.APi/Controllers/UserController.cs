using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Users;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [Authorize(Roles = Roles.Member)]
    [ApiController]
    [Route("users")]
    public sealed class UserController(
        AppDbContext context,
        UserContext userContext
        ) : ControllerBase
    {
        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (id != userId)
            {
                return Forbid();
            }
            UserDto? user = await context.Users
                .Where(u => u.Id == id)
                .Select(UserQueries.ProjectToDto())
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            string? userId = await userContext.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            UserDto? user = await context.Users
                .Where(u => u.Id == userId)
                .Select(UserQueries.ProjectToDto())
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}