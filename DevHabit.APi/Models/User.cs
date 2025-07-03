using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Models
{
    public sealed class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        //we will use this to store the identityId from the identity provider.
        //provider like Auth0, Okta,...
        public string identityId { get; set; }
        
    }
}