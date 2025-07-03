using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Users
{
    internal static class UserQueries
    {
        public static Expression<Func<User, UserDto>> ProjectToDto()
        {
            return u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                CreatedAtUtc = u.CreatedAtUtc,
                UpdatedAtUtc = u.UpdatedAtUtc
            };

        }
    }
}