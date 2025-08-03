using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.APi.DTOs.EntryImports
{
    public class EntryImportJobsParameters
    {
        public string? Fields { get; init; }

        public int Page { get; init; } = 1;

        [FromQuery(Name = "page_size")]
        public int PageSize { get; init; } = 10;

        
        public void Deconstruct(out string? fields, out int page, out int pageSize)
        {
            fields = Fields;
            page = Page;
            pageSize = PageSize;
        }
        
    }
}