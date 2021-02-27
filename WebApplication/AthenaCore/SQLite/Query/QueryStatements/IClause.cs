using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.QueryTypes;

namespace WebApplication.AthenaCore.SQLite.Query.QueryStatements
{
    public interface IClause<out TQ, TM>
        where TQ : Query<TM>
        where TM : BaseModel<TM>, new()
    {
        public TQ Query { get; }
    }
}