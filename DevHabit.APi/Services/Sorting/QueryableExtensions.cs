using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace DevHabit.APi.Services.Sorting
{
    
internal static class QueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sort,
        SortMapping[] mappings,
        string defaultOrderBy = "Id"
    )
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        var SortFields = sort.Split(",")
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();
        List<string> OrderByParts = [];
        foreach (var field in SortFields)
        {
            (string sortField, bool isDesc) = ParseSortField(field);

            SortMapping mapping = mappings.First(m =>
                m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase));

            string dir = (isDesc, mapping.Reverse) switch
            {
                (false, false) => "ASC",
                (false, true) => "DESC",
                (true, false) => "DESC",
                (true, true) => "ASC"
            };

            OrderByParts.Add($"{mapping.PropertyName} {dir} ");

        }
        string orderBy = string.Join(",", OrderByParts);

        return query.OrderBy(orderBy);

    }
    private static (string sortField, bool isDesc) ParseSortField(string field)
    {
        string[] parts = field.Split(' ');
        string sortField = parts[0];
        bool isDesc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (sortField, isDesc);
    }
}

}