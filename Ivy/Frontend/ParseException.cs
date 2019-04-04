using System;
using System.Runtime.Serialization;

namespace Ivy.Frontend
{
    public class ParseException : Exception
    {
        public Token Token { get; }
        
        public ParseException()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParseException(string message, Token token) : base(message)
        {
            Token = token;
        }

        protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}