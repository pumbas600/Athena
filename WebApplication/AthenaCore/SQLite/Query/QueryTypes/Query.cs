using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebApplication.AthenaCore.Extensions;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;

namespace WebApplication.AthenaCore.SQLite.Query.QueryTypes
{
    public abstract class Query<T> where T: BaseModel<T>, new()
    {
        //TODO: ClauseData with no key, e.g: DEFAULT VALUES for an insert statement
        //TODO: ClauseData with no clause e.g: <columns> for an insert statement
        
        private static Regex ClauseRegex { get; } =
            new (@"(?:(?<clause>[A-Z]+ ?)+(?:\s*)<(?<clausekey>[a-z]+)+>)+", RegexOptions.Compiled);

        private static Regex ClauseSpecialCharactersRegex { get; } = new(@"[\[\]]|\| ", RegexOptions.Compiled);
        
        public QueryType QueryType { get; }
        public Dictionary<string, object> Values { get; }
        protected Dictionary<string, ClauseData> ClauseData { get; }
        
        /// <summary>
        /// The format of the query, utilising the following syntax rules:
        /// <list type="bullet">
        ///     <item><description>Clauses must be in all capitals.</description></item>
        ///     <item><description>Clause keys should be in lowercase, surrounded by  &lt;...&gt;.</description></item>
        ///     <item><description>Optional clauses should be surrounded by [...].</description></item>
        ///     <item><description>Clauses where there can be one OR the other, should be separated by '|'.</description></item>
        /// </list>
        /// <code>
        /// E.g:
        ///     SELECT &lt;columns&gt;
        ///     FROM &lt;tablename&gt;
        ///     [WHERE &lt;condition&gt;
        ///     ORDER BY &lt;order&gt;
        ///     LIMIT &lt;limit&gt;]
        /// </code>
        /// </summary>
        public abstract string QueryFormat { get; }

        public Query(QueryType queryType)
        {
            QueryType = queryType;
            Values = new Dictionary<string, object>();
            ClauseData = new Dictionary<string, ClauseData>();
            
            ParseQuery();
            ClauseData.Values.ForEach(Console.WriteLine);
        }

        public void SetClauseValue(string clauseKey, object clauseValue)
        {
            if (!ClauseData.ContainsKey(clauseKey))
            {
                throw new IllegalQueryException(
                    $"The clause key, {clauseKey} is not valid for the query type {QueryType}");
            }

            var clauseData = ClauseData[clauseKey];
            if (clauseData.ClauseValue == null && clauseData.ConditionalClauses != null
                && clauseData.ConditionalClauses.Any(k => ClauseData[k].ClauseValue != null))
            {
                throw new IllegalQueryException(
                    $"Only one of the conditional clauses ({clauseData.GetFormattedConditionalClauses()}) can have a value.");
            }

            clauseData.ClauseValue = clauseValue;
        }

        protected void ParseQuery()
        {
            if (string.IsNullOrEmpty(QueryFormat))
                throw new IllegalQueryException("The query format cannot be null or empty.");

            var tempQueryFormat = QueryFormat; //Note this creates a deep copy.
            int openBracketIndex;
            while ((openBracketIndex = tempQueryFormat.IndexOf('[')) != -1)
            {
                int length = tempQueryFormat.IndexOf(']') - openBracketIndex - 1;
                string optionalClauses = tempQueryFormat.Substring(openBracketIndex + 1, length);
                tempQueryFormat = tempQueryFormat.Replace($"[{optionalClauses}]", "");
                
                ParseQuery(optionalClauses, true);
            }
            ParseQuery(tempQueryFormat, false);
        }

        protected void ParseQuery(string clauses, bool isOptional)
        {
            int endOfLastGroupIndex = -1;
            string previousClauseKey = null;
            var conditionalClauses = new List<string>();
            
            foreach (Match match in ClauseRegex.Matches(clauses))
            {
                var clauseKey = match.Groups["clausekey"].Value;
                var clause = string.Join(" ", match.Groups["clause"].Captures
                    .ToList()
                    .Select(c => c.Value.TrimEnd()));

                ClauseData[clauseKey] = new ClauseData(clause, clauseKey, isOptional);

                HandleConditionalClauses(clauses, endOfLastGroupIndex, match.Index, 
                    previousClauseKey, clauseKey, ref conditionalClauses);
                
                endOfLastGroupIndex = match.Index + match.Length;
                previousClauseKey = clauseKey;
            }
        }

        protected void HandleConditionalClauses(string clauses, int endGroupIndex, int startGroupIndex,
            string previousClauseKey, string currentClauseKey, ref List<string> conditionalClauses)
        {
            if (endGroupIndex == -1) return;
            
            int length = startGroupIndex - endGroupIndex - 1;
            if (clauses.Substring(endGroupIndex + 1, length).Contains('|'))
            {
                //This is the first conditional operator found, so you'll have to add the previous clause
                if (conditionalClauses.Count == 0)
                    conditionalClauses.Add(previousClauseKey);
                  
                conditionalClauses.Add(currentClauseKey);
                    
            }
            //End of conditionalClauses Group
            else if (conditionalClauses.Count != 0)
            {
                foreach (var clauseKey in conditionalClauses)
                {
                    var clauseData = ClauseData[clauseKey];
                    //NOTE: The clauseData share a shallow copy of the list.
                    clauseData.ConditionalClauses = conditionalClauses;
                }

                //Remove references to existing list
                conditionalClauses = new List<string>();
            }
        }

        public string BuildQuery()
        {
            //Remove '[', ']' and '| '.
            string query = ClauseSpecialCharactersRegex.Replace(QueryFormat, "");

            foreach (var clauseData in ClauseData.Values)
            {
                if (clauseData.ClauseValue == null)
                {
                    //Note: You don't need to check if there's already another conditional clause value as this will throw
                    //      an exception when you try and set the clauseValue.
                    //If none of the other conditional clauses have a value.
                    if (!clauseData.IsOptional && clauseData.ConditionalClauses != null && clauseData.ConditionalClauses
                        .All(k => ClauseData[k].ClauseValue != null))
                    {
                        throw new IllegalQueryException(
                            $"There is no value assigned to the required clause: {clauseData.Clause}");
                    }

                    query = Regex.Replace(query, clauseData.GetRegexClauseReplace(), "");
                }
                else
                {
                    query = query.Replace(clauseData.GetFormattedKey(), clauseData.ClauseValue.ToString());
                }
            }

            return query;
        }
    }
}