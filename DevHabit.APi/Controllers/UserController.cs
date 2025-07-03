using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Route("users")]
    public sealed class UserController(AppDbContext context) : ControllerBase
    {
        // [HttpGet("{id}")]
        // public async Task<ActionResult<UserDto>> GetUserById(string id)
        // {
        //     UserDto? user = await context.Users
        //         .Where(u => u.Id == id)
        //         .Select(UserQueries.ProjectToDto())
        //         .FirstOrDefaultAsync();

        //     if (user is null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(user);
        // }
    }
}