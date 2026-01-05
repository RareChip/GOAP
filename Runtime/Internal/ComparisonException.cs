using System;

namespace GOAP.Runtime.Internal
{
    public class ComparisonException : Exception
    {
        public ComparisonException() 
            : base("Invalid comparison of type!") { }
    }
}