using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using Calc.Infrastructure.Parsing;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Core.Utilities;

namespace Calc.Infrastructure
{
    public class InputParser
    {
        private readonly IDelimiterStrategyFactory _delimiterStrategyFactory;
        private readonly CustomDelimiterExtractor _customDelimiterExtractor;
        private readonly DelimiterSplitter _delimiterSplitter;

        public InputParser(IDelimiterStrategyFactory delimiterStrategyFactory)
        {
            _delimiterStrategyFactory = delimiterStrategyFactory;
            _customDelimiterExtractor = new CustomDelimiterExtractor();
            _delimiterSplitter = new DelimiterSplitter();
        }

        public string[] Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<string>();

            ValidateMultipleCustomDelimiterFormat(input);

            var strategy = _delimiterStrategyFactory.CreateStrategy(input);
            var (customDelimiters, numbersString) = _customDelimiterExtractor.ExtractCustomDelimiters(input);

            return strategy switch
            {
                MultipleCustomDelimiterStrategy multipleStrategy => multipleStrategy.Split(numbersString),
                SingleCharCustomDelimiterStrategy singleCharStrategy => singleCharStrategy.Split(numbersString),
                _ => _delimiterSplitter.SplitWithAllDelimiters(numbersString, customDelimiters)
            };
        }

        private void ValidateMultipleCustomDelimiterFormat(string input)
        {
            if (input.StartsWith("//[") && !RegexPatterns.MultipleCustomDelimiter().IsMatch(input))
            {
                throw new InvalidDelimiterException("Invalid format for multiple custom delimiters", input);
            }
        }
    }
}