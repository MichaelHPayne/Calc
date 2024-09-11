using Calc.Core.Interfaces;
using Calc.Infrastructure.DelimiterStrategies;
using System.Text.RegularExpressions;

namespace Calc.Infrastructure
{
    public partial class InputParser
    {
        private readonly IDelimiterStrategyFactory _delimiterStrategyFactory;

        // Separate regexes are used to clearly distinguish between different delimiter formats
        // and to allow for easier modification of each pattern independently.

        // Matches input starting with '//', followed by one or more bracketed delimiters '[...]',
        // then a newline, and captures the remaining text. Treats the entire input as a single line.
        [GeneratedRegex(@"^//(\[.+?\])+\n(.*)$", RegexOptions.Singleline)]
        private static partial Regex MultipleCustomDelimiterRegex();

        // Matches input starting with '//', followed by either a single character or a bracketed delimiter '[...]',
        // then a newline, and captures the remaining text. Treats the entire input as a single line.
        [GeneratedRegex(@"^//(.|\[.+?\])\n(.*)$", RegexOptions.Singleline)]
        private static partial Regex SingleCustomDelimiterRegex();

        public InputParser(IDelimiterStrategyFactory delimiterStrategyFactory)
        {
            _delimiterStrategyFactory = delimiterStrategyFactory;
        }

        public string[] Parse(string input)
        {
            // Early return for empty input
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<string>();

            var strategy = _delimiterStrategyFactory.CreateStrategy(input);
            var (customDelimiters, numbersString) = ExtractCustomDelimiters(input);

            // Pattern matching to differentiate between custom and default strategies.
            // This approach allows for clear separation of concerns and easy extensibility for future delimiter types.
            return strategy is MultipleCustomDelimiterStrategy
                ? strategy.Split(numbersString)
                : SplitWithBackwardCompatibility(numbersString, customDelimiters);
        }

        private static (string[] customDelimiters, string numbersString) ExtractCustomDelimiters(string input)
        {
            // Check for multiple delimiters first because they're more specific.
            // This order prevents incorrectly parsing multiple delimiter inputs as single delimiter inputs.
            var multipleMatch = MultipleCustomDelimiterRegex().Match(input);
            if (multipleMatch.Success)
                return (ExtractMultipleDelimiters(multipleMatch.Groups[1].Value), multipleMatch.Groups[2].Value);

            var singleMatch = SingleCustomDelimiterRegex().Match(input);
            // Returning an empty array instead of null for custom delimiters
            // simplifies subsequent code by avoiding null checks
            return singleMatch.Success
                ? (new[] { singleMatch.Groups[1].Value.Trim('[', ']') }, singleMatch.Groups[2].Value)
                : (Array.Empty<string>(), input);
        }

        private static string[] ExtractMultipleDelimiters(string delimiterString)
        {
            // Extracts individual delimiters from the input string.
            // Crucial for handling complex inputs with multiple custom delimiters.
            // By isolating each delimiter, we ensure accurate string splitting later,
            // even when delimiters contain special characters or vary in length.
            return Regex.Matches(delimiterString, @"\[(.+?)\]")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();
        }

        private static string[] SplitWithBackwardCompatibility(string numbersString, string[] customDelimiters)
        {
            // Include comma and newline as default delimiters for backward compatibility
            // with earlier requirements that always allowed these as delimiters
            var allDelimiters = customDelimiters.Concat(new[] { ",", "\n" }).Distinct();
            
            // Use Regex.Escape to ensure special regex characters in custom delimiters are treated as literals
            var delimiterPattern = string.Join("|", allDelimiters.Select(Regex.Escape));

            return Regex.Split(numbersString, delimiterPattern)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }
    }
}