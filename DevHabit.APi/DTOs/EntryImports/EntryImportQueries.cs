using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevHabit.APi.Models;

namespace DevHabit.APi.DTOs.EntryImports
{
    public class EntryImportQueries
    {
    public static Expression<Func<EntryImportJob, EntryImportJobDto>> ProjectToDto()
    {
        return entry => new()
        {
            Id = entry.Id,
            UserId = entry.UserId,
            Status = entry.Status,
            FileName = entry.FileName,
            TotalRecords = entry.TotalRecords,
            ProcessedRecords = entry.ProcessedRecords,
            SuccessfulRecords = entry.SuccessfulRecords,
            FailedRecords = entry.FailedRecords,
            CreatedAtUtc = entry.CreatedAtUtc,
        };
    }
        
    }
}