using System;
using System.Linq.Expressions;
using WebApplication.AthenaCore.SQLite.Model;
using WebApplication.AthenaCore.SQLite.Query.Condition;
using WebApplication.AthenaCore.SQLite.Query.QueryStatements;
using WebApplication.AthenaCore.SQLite.Query.QueryTypes;

namespace WebApplication
{
    public class TestMain
    {
        public static void Main(string[] args)
        {
            var user = new UserModel();
            user.Id = 1;
            var id = 2;
            //Test(u => (u.Id == id || u.Username == user.Username) && (u != null || u == user) && u.Name == user.Name);
            SelectUser(user);
        }

        public static void SelectUser(UserModel user)
        {
            Console.WriteLine(
                SelectQuery<UserModel>.All()
                    .From("Users")
                    .Where(Condition<UserModel>.Of(u =>
                        (u.Id == user.Id || u.Username == user.Username) &&
                        (u.Name != null  || u.Name != "pumbas600")))
                    .BuildQuery()
            );
        }

        public static void Test(Expression<Predicate<UserModel>> exp)
        {
            var test = exp.Body;

            if (!(exp.Body is MemberExpression body))  
            {  
                UnaryExpression ubody = (UnaryExpression)exp.Body;  
                body = ubody.Operand as MemberExpression;  
            }

            Console.WriteLine(body.Member.Name);  
        }

    }
}