using System;
using Calc.Core.Interfaces;

namespace Calc.Infrastructure.DelimiterStrategies
{
    public class SingleCharCustomDelimiterStrategy : ISingleCharCustomDelimiterStrategy
    {
        private string _delimiter;

        public ISingleCharCustomDelimiterStrategy WithDelimiter(string delimiter)
        {
            if (delimiter.Length != 1)
            {
                throw new ArgumentException("Delimiter must be a single character", nameof(delimiter));
            }
            _delimiter = delimiter;
            return this;
        }

        public string[] Split(string input)
        {
            return input.Split(new[] { _delimiter }, StringSplitOptions.None);
        }
    }
}