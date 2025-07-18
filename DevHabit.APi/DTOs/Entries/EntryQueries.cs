using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.Entries
{
    public static class EntryQueries
    {
        
        public static Expression<Func<Entry, EntryDto>> ProjectToDto()
        {
            return h => new EntryDto
            {
                Id = h.Id,
                Value = h.Value,
                Notes = h.Notes,
                Source = h.Source,
                ExternalId = h.ExternalId,
                IsArchived = h.IsArchived,
                Date = h.Date,
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,

            };
        }
    }
}