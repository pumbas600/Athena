using System;
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
    }
}