using System.Collections.Generic;
using System.Text;
using WebApplication.AthenaCore.SQLite.Model;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public abstract class Query<T> where T: BaseModel<T>, new()
    {
        public QueryType QueryType { get; }
        public StringBuilder QueryBuilder { get; }
        public Dictionary<string, object> Values { get; }

        public Query(QueryType queryType)
        {
            QueryType = queryType;
            QueryBuilder = new StringBuilder();
            Values = new Dictionary<string, object>();
        }

        public string BuildQuery()
        {
            return QueryBuilder.ToString();
        }
    }
}