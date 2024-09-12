using Calc.Core.Interfaces;
using Calc.Infrastructure.DelimiterStrategies;
using System.Text.RegularExpressions;
using System.Text;
using Calc.Core.Utilities;

namespace Calc.Infrastructure.Factories
{
    public class DelimiterStrategyFactory : IDelimiterStrategyFactory
    {
        private readonly IDefaultDelimiterStrategy _defaultStrategy;
        private readonly ISingleCharCustomDelimiterStrategy _singleCharStrategy;
        private readonly StringBuilder _log = new StringBuilder();

        public DelimiterStrategyFactory(IDefaultDelimiterStrategy defaultStrategy, ISingleCharCustomDelimiterStrategy singleCharStrategy)
        {
            _defaultStrategy = defaultStrategy;
            _singleCharStrategy = singleCharStrategy;
        }

        private void Log(string message)
        {
            _log.AppendLine(message);
            Console.WriteLine(message);
        }

        public IDelimiterStrategy CreateStrategy(string input)
        {
            Log($"Creating strategy for input: {input}");

            if (string.IsNullOrEmpty(input))
            {
                Log("Using DefaultDelimiterStrategy for empty input");
                return _defaultStrategy;
            }

            // Check for multiple custom delimiters first
            if (RegexPatterns.MultipleCustomDelimiterFormat().IsMatch(input))
            {
                Log($"Using MultipleCustomDelimiterStrategy with pattern: {RegexPatterns.MultipleCustomDelimiterFormat()}");
                return new MultipleCustomDelimiterStrategy(input);
            }

            // Check for single char custom delimiter
            var match = RegexPatterns.SingleCustomDelimiter().Match(input);
            if (match.Success)
            {
                var delimiter = match.Groups[1].Value;
                Log($"Using SingleCharCustomDelimiterStrategy with delimiter '{delimiter}'");
                return _singleCharStrategy.WithDelimiter(delimiter);
            }

            // Fallback to default strategy if no custom delimiter format matches
            Log("Falling back to DefaultDelimiterStrategy");
            return _defaultStrategy;
        }
    }
}