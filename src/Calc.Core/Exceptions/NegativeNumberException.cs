using System;
using System.Collections.Generic;

namespace Calc.Core.Exceptions
{
    public class NegativeNumberException : Exception
    {
        public IEnumerable<int> NegativeNumbers { get; }

        public NegativeNumberException(IEnumerable<int> negativeNumbers)
            : base($"Negatives not allowed: {string.Join(", ", negativeNumbers)}")
        {
            NegativeNumbers = negativeNumbers;
        }
    }
}