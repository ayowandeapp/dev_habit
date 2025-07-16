using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.Settings
{
    public sealed record EncryptionOptions
    {
        public required string Key { get; init; }
    }
}