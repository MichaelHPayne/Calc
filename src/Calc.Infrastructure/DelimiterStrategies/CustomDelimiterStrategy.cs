using Calc.Core.Interfaces;
using System;

namespace Calc.Infrastructure.DelimiterStrategies
{
    public class CustomDelimiterStrategy : IDelimiterStrategy
    {
        public string[] Split(string input)
        {
            if (input.StartsWith("//") && input.Length > 3)
            {
                char delimiter = input[2];
                string actualInput = input.Substring(input.IndexOf('\n') + 1);
                return actualInput.Split(new[] { delimiter }, StringSplitOptions.None);
            }
            throw new ArgumentException("Invalid input format for custom delimiter", nameof(input));
        }
    }
}