using System;

namespace WebApplication.AthenaCore.SQLite.Query.Exceptions
{
    public class IllegalColumnException : Exception
    {
        public IllegalColumnException(string message) : base(message)
        {
            
        }
    }
}