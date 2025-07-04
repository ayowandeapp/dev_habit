using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Auth;
using DevHabit.APi.DTOs.Users;
using DevHabit.APi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.APi.Controllers
{
    [ApiController]
    [Route("auth")]
    // [AllowAnonymous] //make controller publicly available
    public sealed class AuthenticationController(
        UserManager<IdentityUser> userManager,
        AppIdentityDbContext identityDbContext,
        AppDbContext appDbContext
    ) : ControllerBase
    {
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

            await transaction.CommitAsync();

            return Ok(user);
        }

        [HttpGet]
        public string Gets()
        {
            return "ok";
        }
    }
}