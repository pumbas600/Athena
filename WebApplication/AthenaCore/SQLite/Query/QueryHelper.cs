using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;
using WebApplication.AthenaCore.SQLite.Query.QueryStatements;

namespace WebApplication.AthenaCore.SQLite.Query
{
    public static class QueryHelper
    {
        private static readonly Dictionary<Type, Func<object, object>> ValueFormatters = new()
        {
            { typeof(bool), v => (bool) v ? 1 : 0 },
            { typeof(char), v => $"'{v}'" },
            { typeof(string), v => $"'{v}'" }
        };
        
        public static object FormatValue(object value)
        {
            if (value == null) return "NULL";
            return ValueFormatters
                .GetValueOrDefault(value.GetType(), v => v)
                .Invoke(value);
        }

        public static string GetColumnName<T>(Expression<Func<T, object>> expression)
        {
            return ParseColumnName(expression.Body);
        }

        public static string ParseColumnName(Expression expression)
        {
            var memberExpression = expression is UnaryExpression unaryExpression
                ? unaryExpression.Operand as MemberExpression
                : expression as MemberExpression;

            return memberExpression?.Member.Name;
        }

        public static IEnumerable<string> GetColumnNames<T>(Expression<Func<T, List<object>>> expression)
        {
            if (!(expression.Body is ListInitExpression listExpression))
                throw new IllegalColumnException($"The columns specified in {expression} should be a list.");
            
            var columnNames = listExpression.Initializers
                .Select(element => ParseColumnName(element.Arguments[0]))
                .ToList();

            return columnNames;
        }

        public static string GetClauseName(OrderDirection direction)
        {
            if (direction == OrderDirection.None) return string.Empty;

            return direction == OrderDirection.Ascending ? "ASC" : "DESC";
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