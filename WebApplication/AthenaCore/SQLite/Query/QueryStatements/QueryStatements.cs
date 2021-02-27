using System;
using System.Text;
using WebApplication.AthenaCore.Extensions;
using WebApplication.AthenaCore.SQLite.Query.QueryTypes;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.Condition;

namespace WebApplication.AthenaCore.SQLite.Query.QueryStatements
{
    public static class QueryStatements
    {
        public static StringBuilder Append<TQ, TM>(this IClause<TQ, TM> queryStatement, string text)
            where TQ: Query<TM>
            where TM: BaseModel<TM>, new()
        {
            return queryStatement.Query.QueryBuilder.Append(text);
        }
        
        public static TQ From<TQ, TM>(this IFrom<TQ, TM> queryStatement, string tableName)
            where TQ: Query<TM>
            where TM: BaseModel<TM>, new()
        {
            queryStatement.Append(" FROM ").Append(tableName);
            return queryStatement.Query;
        }

        public static TQ Where<TQ, TM>(this IWhere<TQ, TM> queryStatement, Condition<TM> condition)
            where TQ : Query<TM>
            where TM : BaseModel<TM>, new()
        {
            queryStatement.Append(" WHERE");
            queryStatement.Query.QueryBuilder.Append(condition.ConditionBuilder);
            queryStatement.Query.Values.AddAll(condition.Values,
                alreadyContainsKeyCallback: pair => Console.Write($"{pair.Key}: {pair.Value}"));
            
            return queryStatement.Query;
        }
    }
}