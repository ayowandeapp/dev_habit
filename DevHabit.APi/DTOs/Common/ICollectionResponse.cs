using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.Common
{
    public interface ICollectionResponse<T>
    {
        List<T> Items { get; init; }
    }
}