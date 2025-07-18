using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using DevHabit.APi.Services.Sorting;

namespace DevHabit.APi.DTOs.Entries
{
    public static class EntryMappers
    {
        
        //Add sorting definitions for entry entity
        public static readonly SortMappingDefinition<EntryDto, Entry> sortMapping = new()
        {
            Mappings = [
                new SortMapping(nameof(EntryDto.Value), nameof(Entry.Value)),
                new SortMapping(nameof(EntryDto.Notes), nameof(Entry.Notes)),
                new SortMapping(nameof(EntryDto.IsArchived), nameof(Entry.IsArchived)),
                
                new SortMapping(nameof(EntryDto.Date), nameof(Entry.Date)),
                new SortMapping(nameof(EntryDto.CreatedAtUtc), nameof(Entry.CreatedAtUtc)),
                new SortMapping(nameof(EntryDto.UpdatedAtUtc), nameof(Entry.UpdatedAtUtc))
            ]

        };

        public static Entry ToEntity(this CreateEntryDto dto, string userId, Habit habit)
        {
            Entry entry = new()
            {
                Id = $"e_{Guid.CreateVersion7()}",
                UserId = userId,
                HabitId = habit.Id,
                Value = dto.Value,
                Notes = dto.Notes,
                Source = EntrySource.Automation,
                ExternalId = null,
                IsArchived = false,
                Date = dto.Date
            };
            return entry;
        }
    }
}