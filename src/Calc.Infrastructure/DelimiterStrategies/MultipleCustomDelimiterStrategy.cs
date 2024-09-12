using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Calc.Core.Utilities;

namespace Calc.Infrastructure.DelimiterStrategies
{
    public class MultipleCustomDelimiterStrategy : IDelimiterStrategy
    {
        private readonly string[] _delimiters;

        public MultipleCustomDelimiterStrategy(string input)
        {
            ValidateInputFormat(input);
            _delimiters = ExtractDelimiters(input);
            ValidateDelimiters(_delimiters);
        }

        public string[] Split(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Array.Empty<string>();
            }

            // Escape special regex characters to treat them as literals
            // This prevents unintended regex behavior with custom delimiters
            var escapedDelimiters = _delimiters.Select(Regex.Escape);
            var pattern = string.Join("|", escapedDelimiters);
            return Regex.Split(input, pattern)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        private static void ValidateInputFormat(string input)
        {
            // Ensure the input follows the specified format for multiple custom delimiters
            // This check prevents processing invalid inputs early in the pipeline
            if (!RegexPatterns.MultipleCustomDelimiterFormat().IsMatch(input))
            {
                throw new InvalidDelimiterException("Invalid format for multiple custom delimiters", input);
            }
        }

        private static string[] ExtractDelimiters(string input)
        {
            // Extract delimiters from the input string using regex
            // This approach allows for flexible delimiter definitions within square brackets
            return RegexPatterns.ExtractMultipleDelimiters().Matches(input)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();
        }

        private static void ValidateDelimiters(string[] delimiters)
        {
            // Perform various checks on the extracted delimiters to ensure they meet requirements
            // These validations maintain the integrity of the delimiter set
            if (delimiters.Length == 0)
            {
                throw new InvalidDelimiterException("No delimiters specified", string.Empty);
            }

            var uniqueDelimiters = new HashSet<string>(StringComparer.Ordinal);

            foreach (var delimiter in delimiters)
            {
                ValidateDelimiter(delimiter, uniqueDelimiters);
            }
        }

        private static void ValidateDelimiter(string delimiter, HashSet<string> uniqueDelimiters)
        {
            // Empty delimiters would lead to incorrect splitting behavior
            if (string.IsNullOrWhiteSpace(delimiter))
            {
                throw new InvalidDelimiterException("Delimiter cannot be empty or whitespace.", delimiter);
            }

            // Fully numeric delimiters could be confused with actual numbers in the input
            if (RegexPatterns.FullyNumeric().IsMatch(delimiter))
            {
                throw new InvalidDelimiterException("Fully numeric delimiters are not allowed.", delimiter);
            }

            // Duplicate delimiters could cause ambiguity in splitting
            if (!uniqueDelimiters.Add(delimiter))
            {
                throw new InvalidDelimiterException("Duplicate delimiters are not allowed.", delimiter);
            }
        }
    }
}