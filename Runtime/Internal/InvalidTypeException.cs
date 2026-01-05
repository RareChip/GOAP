using System;

namespace GOAP.Runtime.Internal
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(object value, Type correctType) 
            : base($"Value [{value}] is not of type [{correctType}]!") { }
    }
}