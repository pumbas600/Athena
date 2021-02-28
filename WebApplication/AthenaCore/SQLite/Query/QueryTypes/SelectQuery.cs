using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;
using WebApplication.AthenaCore.SQLite.Query.QueryStatements;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public class SelectQuery<TM> : Query<TM>,
        IFrom<SelectQuery<TM>, TM>,
        IWhere<SelectQuery<TM>, TM>,
        IOrderBy<SelectQuery<TM>, TM>,
        ILimit<SelectQuery<TM>, TM>
        where TM: BaseModel<TM>, new()
    {

        public override string QueryFormat =>
@"SELECT
    <columns>
FROM
    <tablename>
[WHERE
    <condition>
ORDER BY
    <order>
LIMIT <limit> OFFSET <offset>]";

        public SelectQuery<TM> Query => this;
        
        private SelectQuery(IEnumerable<string> columns) : base(QueryType.Select)
        {
            if (columns == null)
            {
                SetClauseValue("columns", "*");
                return;
            }

            if (!columns.Any())
                throw new IllegalColumnException("There must be atleast one column in the select statment");
            
            SetClauseValue("columns", string.Join(", ", columns));
        }

        public static SelectQuery<TM> All()
        {
            return new (null);
        }

        public static SelectQuery<TM> Of(params Expression<Func<TM, object>>[] columns)
        {
            //Check that the columns aren't null here, as in the constructor it means select all ('*')
            if (columns == null)
                throw new NullReferenceException();
            
            return new(columns.Select(QueryHelper.GetColumnName));
        }
        
        public static SelectQuery<TM> Of(Expression<Func<TM, List<object>>> columns)
        {
            //Check that the columns aren't null here, as in the constructor it means select all ('*')
            if (columns == null)
                throw new NullReferenceException();
            
            return new(QueryHelper.GetColumnNames(columns));
        }
    }
}