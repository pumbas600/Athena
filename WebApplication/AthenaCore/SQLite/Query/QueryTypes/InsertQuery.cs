using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication.AthenaCore.Extensions;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Model.Attributes;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;
using WebApplication.AthenaCore.SQLite.Query.QueryStatements;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public class InsertQuery<TM> : Query<TM>,
        IValues<InsertQuery<TM>, TM>
        where TM: BaseModel<TM>, new()
    {
        //TODO: INSERT INTO <table> DEFAULT VALUES
        //TODO: Allow Values to be a select statement
        
        public override string QueryFormat =>
@"INSERT INTO <tablename> <columns>
    VALUES <values>";

        public InsertQuery<TM> Query => this;

        private InsertQuery(string tableName) : base(QueryType.Insert)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new IllegalTableException($"The table name {tableName} cannot be null or empty");

            SetClauseValue("tablename", tableName);
        }

        public static InsertQuery<TM> Into(string tableName)
        {
            return new (tableName);
        }
    }
}