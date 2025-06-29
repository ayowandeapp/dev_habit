using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.Common;

namespace DevHabit.APi.Services
{
    public class LinkService(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor
    )
    {
        public LinkDto Create(
            string endpoint,
            string rel,
            string method,
            object? values = null,
            string? controller = null
        )
        {
            string? href = linkGenerator.GetUriByAction(
                httpContextAccessor.HttpContext!,
                endpoint,
                controller,
                values
            );

            return new LinkDto
            {
                Href = href ?? throw new Exception("Url invalid"),
                Rel = rel,
                Method = method
            };
        }
        
        
    }
}