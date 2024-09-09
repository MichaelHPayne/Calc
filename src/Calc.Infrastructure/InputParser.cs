using System;
using System.Text.RegularExpressions;
using Calc.Core.Interfaces;

namespace Calc.Infrastructure
{
    public class InputParser
    {
        private readonly IDelimiterStrategyFactory _delimiterStrategyFactory;
        private readonly IDefaultDelimiterStrategy _defaultDelimiterStrategy;

        public InputParser(IDelimiterStrategyFactory delimiterStrategyFactory, IDefaultDelimiterStrategy defaultDelimiterStrategy)
        {
            _delimiterStrategyFactory = delimiterStrategyFactory ?? throw new ArgumentNullException(nameof(delimiterStrategyFactory));
            _defaultDelimiterStrategy = defaultDelimiterStrategy ?? throw new ArgumentNullException(nameof(defaultDelimiterStrategy));
        }

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
                    var strategy = _delimiterStrategyFactory.CreateStrategy(delimiter);
                    return strategy.Split(numbers);
                }
            }

            return _defaultDelimiterStrategy.Split(input);
        }
    }
}
