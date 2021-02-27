using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.QueryStatements;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public class SelectQuery<TM> : Query<TM>,
        IFrom<SelectQuery<TM>, TM>,
        IWhere<SelectQuery<TM>, TM>
        where TM: BaseModel<TM>, new()
    {
        private SelectQuery(params Expression<Func<TM, object>>[] columns) : base(QueryType.Select)
        {
            QueryBuilder.Append("SELECT ");

            if (columns == null)
            {
                QueryBuilder.Append('*');
                return;
            }

            QueryBuilder.AppendJoin(", ", columns.Select(QueryHelper.GetColumnName).Where(n => n != null));
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
            
            return new(columns);
        }
        
        public SelectQuery<TM> Query => this;
    }
}