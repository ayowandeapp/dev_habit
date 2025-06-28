using System.Dynamic;

namespace DevHabit.APi.Services
{
    public interface IDataShaperService<T>
    {
        List<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString);
        ExpandoObject ShapeData(T entity, string fieldsString);
        bool ValidateFields(string? fields);
    }
}