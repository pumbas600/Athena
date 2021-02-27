using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApplication.AthenaCore.SQLite.Model.Attributes;
using WebApplication.AthenaCore.SQLite.Query;

namespace WebApplication.AthenaCore.SQLite.Model
{
    public class BaseModel<T> where T: BaseModel<T>, new()
    {
        public object GetPrimaryKey()
        {
            var property = GetAllColumnProperties()
                .FirstOrDefault(p => CreateFilter(p, c => c.HasFlag(ColumnFlags.PrimaryKey)));
            return property?.GetValue(this);
            
        }

        public IEnumerable<KeyValuePair<string, object>> GetAllPropertiesWith(ColumnFlags flag)
        {
            return GetAllColumnProperties()
                .Where(p => CreateFilter(p, c => c.HasFlag(flag)))
                .Select(p => new KeyValuePair<string, object>(p.Name, QueryHelper.FormatValue(p.GetValue(this))));
        }
        
        public IEnumerable<KeyValuePair<string, object>> GetAllProperties()
        {
            return GetAllColumnProperties()
                .Select(p => new KeyValuePair<string, object>(p.Name, QueryHelper.FormatValue(p.GetValue(this))));
        }

        public IEnumerable<PropertyInfo> GetAllColumnProperties()
        {
            return typeof(T).GetProperties()
                .Where(p => p.CustomAttributes.FirstOrDefault(
                    x => x.AttributeType == typeof(ColumnAttribute)) != null)
                .ToList();
        }

        public object GetValue(string propertyName)
        {
            return QueryHelper.FormatValue(
                typeof(T).GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(propertyName))
                    ?.GetValue(this));
        }

        private static bool CreateFilter(PropertyInfo p, Func<ColumnAttribute, bool> filter)
        {
            var columnAttribute = (ColumnAttribute)p.GetCustomAttribute(typeof(ColumnAttribute));
            
            return filter.Invoke(columnAttribute);
        }
    }
}