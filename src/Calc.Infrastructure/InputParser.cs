using System;
using System.Text.RegularExpressions;

namespace Calc.Infrastructure
{
    public class InputParser
    {
        public string[] Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Array.Empty<string>();
            }

            if (input.StartsWith("//"))
            {
                var match = Regex.Match(input, @"^//(.)\n(.*)$");
                if (match.Success)
                {
                    var delimiter = match.Groups[1].Value;
                    var numbers = match.Groups[2].Value;
                    return numbers.Split(new[] { delimiter }, StringSplitOptions.None);
                }
            }

            return input.Split(new[] { ',', '\n' }, StringSplitOptions.None);
        }
    }
}