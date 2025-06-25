using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Services.Sorting;
using System.Linq.Dynamic.Core;

namespace DevHabit.APi.Services.Sorting
{
    public sealed record SortMapping(
        string SortField,
        string PropertyName,
        bool Reverse = false
    );


}
