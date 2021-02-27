using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WebApplication.AthenaCore.SQLite.Query
{
    public static class QueryHelper
    {
        public static object FormatValue(object value)
        {
            if (value == null) return "NULL";
            return value is string ? $"'{value}'" : value;
        }

        public static string GetColumnName<T>(Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.Body is UnaryExpression unaryExpression
                ? unaryExpression.Operand as MemberExpression
                : expression.Body as MemberExpression;

            return memberExpression?.Member.Name;
        }

        public static string AddQueryValue(this Dictionary<string, object> dictionary, string key, object value)
        {
            string valueName = "@" + key;
            dictionary[valueName] = value;

            return valueName;
        }
        
        public static string AddQueryValue(this Dictionary<string, object> dictionary, KeyValuePair<string, object> pair)
        {
            return AddQueryValue(dictionary, pair.Key, pair.Value);
        }
        
        
    }
}