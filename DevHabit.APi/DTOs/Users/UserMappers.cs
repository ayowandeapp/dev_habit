using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.Auth;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Users
{
    public static class UserMappers
    {
        public static User ToEntity(this RegisterUserDto registerUserDto)
        {
            return new User
            {
                Id = $"u_{Guid.CreateVersion7()}",
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
            };
        }
        
    }
}