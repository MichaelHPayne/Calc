using Calc.Core.Interfaces;
using System.Text.RegularExpressions;

namespace Calc.Infrastructure
{
    public class DelimiterSplitter : IDelimiterSplitter
    {
        public string[] SplitWithAllDelimiters(string numbersString, string[] customDelimiters)
        {
            var allDelimiters = customDelimiters.Concat(new[] { ",", "\n" }).Distinct();
            var delimiterPattern = string.Join("|", allDelimiters.Select(Regex.Escape));

            return Regex.Split(numbersString, delimiterPattern)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }
    }
}