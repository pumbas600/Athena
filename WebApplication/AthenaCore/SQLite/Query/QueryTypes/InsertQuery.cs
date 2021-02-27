using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;
using WebApplication.AthenaCore.SQLite.Query.QueryStatements;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public class InsertQuery<TM> : Query<TM>,
        IFrom<SelectQuery<TM>, TM>,
        IWhere<SelectQuery<TM>, TM>
        where TM: BaseModel<TM>, new()
    {
        public InsertQuery(string tableName) : base(QueryType.Insert)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new IllegalTableException($"The table name {tableName} cannot be null or empty");

            QueryBuilder.Append("INSERT INTO ").Append(tableName);
        }

        public InsertQuery<TM> Model(TM model)
        {
            return this;
        }

        public SelectQuery<TM> Query { get; }
    }
}