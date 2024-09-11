using System;

namespace Calc.Core.Exceptions
{
    public class InvalidDelimiterException : Exception
    {
        public string InvalidDelimiter { get; }

        public InvalidDelimiterException(string message, string invalidDelimiter) 
            : base(message)
        {
            InvalidDelimiter = invalidDelimiter;
        }

        public InvalidDelimiterException(string message, string invalidDelimiter, Exception innerException) 
            : base(message, innerException)
        {
            InvalidDelimiter = invalidDelimiter;
        }
    }
}