using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Models
{
    public sealed class GitHubAccessToken
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public static string CreateNewId()
        {
            return $"gh_{Guid.CreateVersion7()}";
        }
    }
}