using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Services.Sorting
{
    
    public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
    {
        public required SortMapping[] Mappings { get; set; }
    }
}