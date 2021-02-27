using System;

namespace WebApplication.AthenaCore.SQLite.Query.Exceptions
{
    public class IllegalTableException : Exception
    {
        public IllegalTableException(string message) : base(message)
        {
            
        }
    }
}