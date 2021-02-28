using System.Collections.Generic;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public class ClauseData
    {
        public string Clause { get; }
        
        //The key for this clause
        public string ClauseKey { get; }

        //If this clause is optional
        public bool IsOptional { get; }
        
        //List of the clause keys, from which there can only be one with a value
        //NOTE: This is a shallow copy of the list, which is shared by all the clauses in the list
        public List<string> ConditionalClauses { get; set; }
        
        //The value which will replace the clause key
        public object ClauseValue { get; set; }
        
        public ClauseData(string clause, string clauseKey, bool isOptional)
        {
            Clause = clause;
            ClauseKey = clauseKey;
            IsOptional = isOptional;
        }

        public override string ToString()
        {
            string conditionalClauses = "No Conditional Clauses";
            if (ConditionalClauses != null)
            {
                conditionalClauses = GetFormattedConditionalClauses();
            }

            return $"{Clause} <{ClauseKey}> [{IsOptional},({conditionalClauses})]";
        }

        public string GetFormattedConditionalClauses()
        {
            return $"<{string.Join(">, <", ConditionalClauses)}>";
        }

        public string GetRegexClauseReplace()
        {
            return $@"{Clause}\s*<{ClauseKey}>\s*";
        }
        
        public string GetFormattedKey()
        {
            return $"<{ClauseKey}>";
        }
    }
}