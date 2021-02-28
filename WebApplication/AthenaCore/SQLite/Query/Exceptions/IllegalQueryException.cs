using System;

namespace WebApplication.AthenaCore.SQLite.Query.Exceptions
{
    public class IllegalQueryException : Exception
    {
        public IllegalQueryException(string message) : base(message)
        {
            
        }        
    }
}