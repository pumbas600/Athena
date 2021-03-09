using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebApplication.AthenaCore.Extensions;
using WebApplication.AthenaCore.SQLite.Query.QueryTypes;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Model.Attributes;
using WebApplication.AthenaCore.SQLite.Query.Condition;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;

namespace WebApplication.AthenaCore.SQLite.Query.QueryStatements
{
    public static class QueryStatements
    {
        public static TQ From<TQ, TM>(this IFrom<TQ, TM> queryStatement, string tableName)
            where TQ: Query<TM>
            where TM: BaseModel<TM>, new()
        {
            if (string.IsNullOrEmpty(tableName))
                throw new IllegalTableException($"The table name {tableName} cannot be null or empty");

            queryStatement.Query.SetClauseValue("tablename", tableName);
            return queryStatement.Query;
        }

        public static TQ Where<TQ, TM>(this IWhere<TQ, TM> queryStatement, Condition<TM> condition)
            where TQ : Query<TM>
            where TM : BaseModel<TM>, new()
        {
            queryStatement.Query.SetClauseValue("condition", condition.BuildCondition());
            queryStatement.Query.Values.AddAll(condition.Values,
                alreadyContainsKeyCallback: pair => Console.Write($"Already contains the key: {pair.Key} [{pair.Value}]"));
            
            return queryStatement.Query;
        }

        public static TQ Limit<TQ, TM>(this ILimit<TQ, TM> queryStatement, int rowCount, int offset = 0)
            where TQ : Query<TM>
            where TM : BaseModel<TM>, new()
        {
            if (rowCount < 0)
                throw new ArgumentException($"The argument rowCount must be greater than 0, not {rowCount}");

            queryStatement.Query.SetClauseValue("limit", rowCount);
            
            if (offset > 0)
                queryStatement.Query.SetClauseValue("offset", offset);
            
            return queryStatement.Query;
        }

        public static TQ OrderBy<TQ, TM>(this IOrderBy<TQ, TM> queryStatement, Expression<Func<TM, object>> column,
            OrderDirection direction = OrderDirection.None)
            where TQ : Query<TM>
            where TM : BaseModel<TM>, new()
        {
            string clause = QueryHelper.GetColumnName(column);
            if (clause == null)
                throw new IllegalColumnException($"The column specified in {column} is not valid");

            if (direction != OrderDirection.None)
                clause += $" {QueryHelper.GetClauseName(direction)}";

            queryStatement.Query.SetClauseValue("order", clause);
            return queryStatement.Query;
        }

        public static TQ Model<TQ, TM>(this IValues<TQ, TM> queryStatement, TM model, bool asTemplate = false)
            where TQ : Query<TM>
            where TM : BaseModel<TM>, new()
        {
            InsertProperties(model.GetAllProperties(), queryStatement.Query, asTemplate);
            return queryStatement.Query;
        }

        public static TQ Model<TQ, TM>(this IValues<TQ, TM> queryStatement, TM model, ColumnFlags flag, bool asTemplate = false)
            where TQ : Query<TM>
            where TM : BaseModel<TM>, new()
        {
            InsertProperties(model.GetAllPropertiesWith(flag), queryStatement.Query, asTemplate);
            return queryStatement.Query;
        }

        private static void InsertProperties<TM>(IEnumerable<KeyValuePair<string, object>> properties, Query<TM> query, bool asTemplate)
            where TM : BaseModel<TM>, new()
        {
            var columns = new List<string>();
            var values = new List<string>();

            properties.ForEach(p =>
            {
                columns.Add(p.Key);
                values.Add(query.Values.AddQueryValue(p.Key, asTemplate ? null : p.Value));
            });
            
            query.SetClauseValue("columns", string.Join(", ", columns));
            query.SetClauseValue("values", string.Join(", ", values));
        }
    }
}