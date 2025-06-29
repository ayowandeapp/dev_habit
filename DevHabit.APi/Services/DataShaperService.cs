using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DevHabit.APi.DTOs.Common;

namespace DevHabit.APi.Services
{
    public sealed class DataShaperService<T> : IDataShaperService<T>
    {
        public PropertyInfo[] Properties { get; set; }

        public DataShaperService()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        public List<ExpandoObject> ShapeData(
            IEnumerable<T> entities,
            string fieldsString,
            Func<T, List<LinkDto>>? linksFactory = null
            )
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredProperties, linksFactory);
        }
        
        public ExpandoObject ShapeData(T entity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return DataShaperService<T>.FetchDataForEntity(entity, requiredProperties);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();

            bool isFieldsSet = string.IsNullOrWhiteSpace(fieldsString);
            if (isFieldsSet)
            {
                requiredProperties = Properties.ToList();
            }
            else
            {
                var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var field in fields)
                {
                    var property = Properties.FirstOrDefault(pi =>
                        pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                    if (property != null)
                    {
                        requiredProperties.Add(property);
                    }
                }
            }

            return requiredProperties;
        }

        private List<ExpandoObject> FetchData(
            IEnumerable<T> entities,
            IEnumerable<PropertyInfo> requiredProperties,
            Func<T, List<LinkDto>>? linksFactory
            )
        {
            List<ExpandoObject> shapedObjects = [];
            foreach (var entity in entities)
            {
                ExpandoObject shapedObject = DataShaperService<T>.FetchDataForEntity(entity, requiredProperties);

                if (linksFactory is not null)
                {
                    shapedObject.TryAdd("links", linksFactory(entity)); 
                }
                shapedObjects.Add(shapedObject);
            }
            return shapedObjects;
        }

        private static ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ExpandoObject();

            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.TryAdd(property.Name, objectPropertyValue);
            }
            return shapedObject;
        }


        public bool ValidateFields(string? fieldsString)
        {
            if (string.IsNullOrWhiteSpace(fieldsString)) return true;

            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var field in fields)
            {
                var property = Properties.FirstOrDefault(pi =>
                    pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (property == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}