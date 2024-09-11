using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Calc.Infrastructure.DelimiterStrategies
{
    public class MultipleCustomDelimiterStrategy : IDelimiterStrategy
    {
        private readonly string[] _delimiters;

        public MultipleCustomDelimiterStrategy(string input)
        {
            var match = Regex.Match(input, @"^//(\[.*?\])+\n");
            if (!match.Success)
            {
                throw new InvalidDelimiterException("Invalid format for multiple custom delimiters", input);
            }

            _delimiters = Regex.Matches(match.Value, @"\[(.*?)\]")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

            if (_delimiters.Length == 0)
            {
                throw new InvalidDelimiterException("No delimiters specified", input);
            }

            for (int i = 0; i < _delimiters.Length; i++)
            {
                if (string.IsNullOrEmpty(_delimiters[i]))
                {
                    throw new InvalidDelimiterException("Delimiter cannot be empty or whitespace.", string.Empty);
                }
                if (string.IsNullOrWhiteSpace(_delimiters[i]))
                {
                    throw new InvalidDelimiterException("Delimiter cannot be empty or whitespace.", _delimiters[i]);
                }
            }

            ValidateDelimiters(_delimiters);
        }

        public string[] Split(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Array.Empty<string>();
            }

            var escapedDelimiters = _delimiters.Select(d => Regex.Escape(d)).ToArray();
            var pattern = string.Join("|", escapedDelimiters);
            return Regex.Split(input, pattern)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        private void ValidateDelimiters(string[] delimiters)
        {
            var delimiterSet = new HashSet<string>(StringComparer.Ordinal);
            foreach (var delimiter in delimiters)
            {
                if (Regex.IsMatch(delimiter, @"^\d+$"))
                {
                    throw new InvalidDelimiterException("Fully numeric delimiters are not allowed.", delimiter);
                }
            }
        }
    }
}