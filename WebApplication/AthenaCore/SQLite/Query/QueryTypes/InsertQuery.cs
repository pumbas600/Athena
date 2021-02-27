using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication.AthenaCore.Extensions;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Model.Attributes;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public class InsertQuery<TM> : Query<TM>
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
            var properties = model.GetAllProperties();
            InsertProperties(properties);
            
            return this;
        }
        
        public InsertQuery<TM> Model(TM model, ColumnFlags flag)
        {
            var properties = model.GetAllPropertiesWith(flag);
            InsertProperties(properties);
            
            return this;
        }

        private void InsertProperties(IEnumerable<KeyValuePair<string, object>> properties) 
        {
            QueryBuilder.Append(" (");
            
            var valueKeys = new List<string>();
            properties.ForEachNotLast((p, notLast) =>
            {
                //Add the values to the Values dictionary, so they can be inserted in an injection-safe way later.
                valueKeys.Add(Values.AddQueryValue(p));

                QueryBuilder.Append(p.Key);
                if (notLast)
                    QueryBuilder.Append(", ");
            });
            
            QueryBuilder.Append(") VALUES (").AppendJoin(", ", valueKeys).Append(')');
        }

        public static InsertQuery<TM> Into(string tableName)
        {
            return new (tableName);
        }

        public InsertQuery<TM> Query => this;
    }
}