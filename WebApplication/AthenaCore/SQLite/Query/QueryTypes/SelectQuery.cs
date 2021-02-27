using System;
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
            QueryBuilder.Append("SELECT");

            if (columns == null)
            {
                QueryBuilder.Append(" *");
                return;
            }

            for (int i = 0; i < columns.Length; i++)
            {
                string columnName = QueryHelper.GetColumnName(columns[i]);
                if (columnName == null) continue;
                
                QueryBuilder.Append(' ').Append(columnName);

                if (i != columns.Length - 1)
                {
                    QueryBuilder.Append(',');
                }
            }
        }

        public static SelectQuery<TM> All()
        {
            return new (null);
        }

        public static SelectQuery<TM> Of(params Expression<Func<TM, object>>[] columns)
        {
            if (columns == null)
                throw new NullReferenceException();
            
            return new(columns);
        }
        
        public SelectQuery<TM> Query => this;
    }
}