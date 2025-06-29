using System.Dynamic;
using DevHabit.APi.DTOs.Common;

namespace DevHabit.APi.Services
{
    public interface IDataShaperService<T>
    {
        List<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString, Func<T, List<LinkDto>>? linksFactory);
        ExpandoObject ShapeData(T entity, string fieldsString);
        bool ValidateFields(string? fields);
    }
}