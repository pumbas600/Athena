using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.Exceptions;

namespace WebApplication.AthenaCore.SQLite.Query.Condition
{
    public class Condition<TM>
        where TM : BaseModel<TM>, new()
    {
        //TODO: Condition Groups (brackets)
        //TODO: In collection Filter
        //TODO: Multiple Values with same ValueName -> Add an incrementing number to end: @id, @id1, @id2, etc
        
        
        public static readonly Dictionary<ExpressionType, string> ConditionalOperators = new ()
        {
            {ExpressionType.Equal, "="},
            {ExpressionType.NotEqual, "!="},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="}
        };

        public static readonly Dictionary<ExpressionType, string> BooleanOperators = new()
        {
            {ExpressionType.OrElse, "OR"},
            {ExpressionType.AndAlso, "AND"}
        };

        public StringBuilder ConditionBuilder { get; }
        public Dictionary<string, object> Values { get; }

        private bool isFirstCondition = true;

        private Condition()
        {
            ConditionBuilder = new StringBuilder();
            Values = new Dictionary<string, object>();
        }

        public static Condition<TM> Equals(TM model)
        {
            var conditon = new Condition<TM>();
            conditon.AddConjuction(" AND ");

            var properties = model.GetAllColumnProperties().ToList();
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties.ElementAt(i);
                var name = property.Name;
                var value = model.GetValue(name);

                conditon.Values["@" + name] = value;
                
                conditon.ParsePredicate(name, value, ExpressionType.Equal);
                
                if (i != properties.Count - 1)
                {
                    conditon.ConditionBuilder.Append(" AND ");
                }
            }

            return conditon;
        }
        
        public static Condition<TM> Of(Expression<Predicate<TM>> predicate)
        {
            var condition = new Condition<TM> { isFirstCondition = false };

            condition.ParsePredicate(predicate);
            return condition;
        }

        // public Condition<TM> And(Expression<Predicate<TM>> condition)
        // {
        //     AddConjuction(" AND");
        //     ParseCondition(condition);
        //     
        //     return this;
        // }
        //
        // public Condition<TM> Or(Expression<Predicate<TM>> condition)
        // {
        //     AddConjuction(" OR");
        //     ParseCondition(condition);
        //
        //     return this;
        // }

        public string BuildCondition()
        {
            return ConditionBuilder.ToString();
        }

        private void ParsePredicate(Expression<Predicate<TM>> condition)
        {

            ParseBinaryExpression(condition.Body as BinaryExpression);
        }

        private void ParseBinaryExpression(BinaryExpression expression, bool isEnclosed = false)
        {
            if (IsBooleanOperator(expression.NodeType))
            {
                bool isAnd = expression.NodeType == ExpressionType.AndAlso;
                if (expression.Left is BinaryExpression left)
                {
                    ParseBranch(left, isAnd, isEnclosed);
                }

                ConditionBuilder.Append(' ').Append(BooleanOperators[expression.NodeType]);
                
                if (expression.Right is BinaryExpression right)
                {
                    ParseBranch(right, isAnd);
                }
            }
            else
            {
                if (expression.Left is not MemberExpression)
                {
                    throw new IllegalColumnException(
                        $"The expression {expression} is not valid. You cannot compare the model itself");
                }
                
                var column = ((MemberExpression) expression.Left).Member.Name;
                var value = GetRightValue(expression);

                ParsePredicate(column, value, expression.NodeType, isEnclosed);
                
            }
        }

        private void ParseBranch(BinaryExpression branch, bool isAnd, bool isEnclosed = false)
        {
            if (isAnd && branch.NodeType == ExpressionType.OrElse)
                ParseEnclosedBinaryExpression(branch);
            else
                ParseBinaryExpression(branch, isEnclosed);
        }

        private void ParseEnclosedBinaryExpression(BinaryExpression expression)
        {
            ConditionBuilder.Append(" (");
            ParseBinaryExpression(expression, true);
            ConditionBuilder.Append(')');
        }

        private void ParsePredicate(string column, object value, ExpressionType conditionType, bool isEnclosed = false)
        {
            string conditionOperator = null;
            if (value.Equals("NULL"))
            {
                if (conditionType == ExpressionType.Equal)
                    conditionOperator = "IS";
                else if (conditionType == ExpressionType.NotEqual)
                    conditionOperator = "IS NOT";
            }
            else conditionOperator = ConditionalOperators[conditionType];

            if (!isEnclosed) ConditionBuilder.Append(' ');
            
            ConditionBuilder.AppendJoin(' ', column, conditionOperator, value);
        }

        private object GetRightValue(BinaryExpression expression)
        {
            switch (expression.Right)
            {
                case ConstantExpression constantExpression:
                    return QueryHelper.FormatValue(constantExpression.Value);

                case MemberExpression memberExpression:
                {
                    var valueName = "@" + memberExpression.Member.Name;

                    while (memberExpression.Expression is MemberExpression childMemberExpression)
                    {
                        memberExpression = childMemberExpression;
                    }

                    var value = QueryHelper.FormatValue((memberExpression.Expression as ConstantExpression)?.Value);

                    Values[valueName] = value;
                    return valueName;
                }

                default:
                    return "NULL";
            }
        }

        private void AddConjuction(string conjuction)
        {
            if (!isFirstCondition)
            {
                ConditionBuilder.Append(conjuction);
            }
            else
            {
                isFirstCondition = false;
            }
        }

        private static bool IsBooleanOperator(ExpressionType type)
        {
            return BooleanOperators.ContainsKey(type);
        }
    }
    
    
}