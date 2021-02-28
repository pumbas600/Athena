using WebApplication.AthenaCore.SQLite.Query.QueryTypes;
using WebApplication.AthenaCore.SQLite.Model;

namespace WebApplication.AthenaCore.SQLite.Query.QueryStatements
{
    public interface IOrderBy<out TQ, TM> : IClause<TQ, TM>
        where TQ : Query<TM>
        where TM : BaseModel<TM>, new()
    {
        
    }
}