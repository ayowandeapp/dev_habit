using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Services.Sorting
{
    public sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
    {
        public SortMapping[] GetMappings<TSource, TDestination>()
        {
            SortMappingDefinition<TSource, TDestination>? sortMappingDefinition = sortMappingDefinitions
                .OfType<SortMappingDefinition<TSource, TDestination>>()
                .FirstOrDefault();

            if (sortMappingDefinition is null)
            {
                throw new InvalidOperationException("Error mapping");
            }

            return sortMappingDefinition.Mappings;
        }

        public bool ValidateMappings<TSource, TDestination>(string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return true;
            }
            var sortFields = sort.Split(',')
                .Select(f => f.Trim().Split(' ')[0])
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToList();

            SortMapping[] mapping = GetMappings<TSource, TDestination>();

            return sortFields.All(f => mapping.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));
        }

    }
}