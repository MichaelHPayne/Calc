using Calc.Core.Utilities;
using Calc.Core.Exceptions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calc.Infrastructure.Parsing
{
    internal class CustomDelimiterExtractor
    {
        public (string[] customDelimiters, string numbersString) ExtractCustomDelimiters(string input)
        {
            var multipleMatch = RegexPatterns.MultipleCustomDelimiter().Match(input);
            if (multipleMatch.Success)
            {
                var delimiters = ExtractMultipleDelimiters(multipleMatch.Groups[1].Value);
                ValidateDelimiters(delimiters);
                return (delimiters, multipleMatch.Groups[2].Value);
            }

            var singleMatch = RegexPatterns.SingleCustomDelimiter().Match(input);
            if (singleMatch.Success)
            {
                var delimiter = singleMatch.Groups[1].Value;
                ValidateDelimiters(new[] { delimiter });
                return (new[] { delimiter }, singleMatch.Groups[2].Value);
            }

            return (Array.Empty<string>(), input);
        }

        private string[] ExtractMultipleDelimiters(string delimiterString)
        {
            return RegexPatterns.ExtractMultipleDelimiters().Matches(delimiterString)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();
        }

        private void ValidateDelimiters(string[] delimiters)
        {
            if (delimiters.Any(string.IsNullOrWhiteSpace))
            {
                throw new InvalidDelimiterException("Delimiter cannot be empty or whitespace.", string.Empty);
            }
        }
    }
}