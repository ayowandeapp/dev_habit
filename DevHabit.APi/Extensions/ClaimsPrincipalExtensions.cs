using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevHabit.APi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetIdentityId(this ClaimsPrincipal? principal)
        {
            if (principal == null) return null;

            if (!principal.Claims.Any())
            {
                Console.WriteLine($"no claims exist");
            }

            // Debug: Log all claims
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            string? identityId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return identityId;
        }
    }
}