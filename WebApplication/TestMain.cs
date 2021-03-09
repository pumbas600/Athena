using System;
using System.Collections.Generic;
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
            var user = new UserModel
            {
                Id = 1,
                Username = "pumbas600",
                Name = "pumbas"
            };
            
            //Test(u => new() { u.Id, u.Name });
            //InsertQuery<UserModel>.Into("Users");
            SelectUser(user);
        }

        public static void SelectUser(UserModel user)
        {
            Console.WriteLine(
                SelectQuery<UserModel>.All()
                    .From("Users")
                    .Where(Condition<UserModel>.Of(u =>
                        u.Id == user.Id || u.Username == "pumbas600"))
                    .OrderBy(u => u.Name)
                    .Limit(10)
                    .BuildQuery()
            );
            
            Console.WriteLine(
                SelectQuery<UserModel>.Of(u => u.Id, u => u.Username)
                    .From("Users")
                    .OrderBy(u => u.Id, OrderDirection.Descending)
                    .Limit(10)
                    .BuildQuery()
            );

            Console.WriteLine(
                InsertQuery<UserModel>.Into("Users")
                    .Model(user, true)
                    .BuildQuery()
            );
        }

        public static void Test(Expression<Func<UserModel, List<object>>> exp)
        {
            var test = exp.Body;
        }

    }
}