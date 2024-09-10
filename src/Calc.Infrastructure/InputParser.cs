using Calc.Core.Interfaces;
using System.Text.RegularExpressions;
using System.Linq;

namespace Calc.Infrastructure
{
    public class InputParser
    {
        private readonly IDelimiterStrategyFactory _delimiterStrategyFactory;

        public InputParser(IDelimiterStrategyFactory delimiterStrategyFactory)
        {
            _delimiterStrategyFactory = delimiterStrategyFactory;
        }

        public string[] Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Array.Empty<string>();
            }

            var (customDelimiter, numbersString) = ExtractCustomDelimiter(input);

            // Split by both the custom delimiter and newline
            var delimiters = new[] { customDelimiter, "\n" }.Distinct().ToArray();
            var numbers = Regex.Split(numbersString, string.Join("|", delimiters.Select(Regex.Escape)))
                              .Where(s => !string.IsNullOrWhiteSpace(s));

            return numbers.ToArray();
        }

        private (string customDelimiter, string numbersString) ExtractCustomDelimiter(string input)
        {
            var customDelimiterRegex = new Regex(@"^//(.|\[.+\])\n(.*)$", RegexOptions.Singleline);
            var match = customDelimiterRegex.Match(input);

            if (match.Success)
            {
                var delimiter = match.Groups[1].Value;
                var numbers = match.Groups[2].Value;
                return (delimiter, numbers);
            }

            return (",", input);
        }
    }
}