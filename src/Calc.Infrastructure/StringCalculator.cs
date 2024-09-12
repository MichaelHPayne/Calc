using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using System.Linq;

namespace Calc.Infrastructure
{
    public class StringCalculator : IStringCalculator
    {
        // Upper bound for valid numbers, as per requirement 5
        private const int MaxValidNumber = 1000;
        private readonly InputParser _inputParser;

        public StringCalculator(IDelimiterStrategyFactory delimiterStrategyFactory)
        {
            _inputParser = new InputParser(delimiterStrategyFactory);
        }

        public int Add(string numbers)
        {
            // Early return for empty input improves efficiency and readability
            if (string.IsNullOrWhiteSpace(numbers))
            {
                return 0;
            }

            var parsedNumbers = _inputParser.Parse(numbers);

            // Parse numbers, separating valid and negative numbers
            var (validNumbers, negativeNumbers) = ParseNumbers(parsedNumbers);

            // Checking for negative numbers before summing improves error handling
            if (negativeNumbers.Any())
            {
                throw new NegativeNumberException(negativeNumbers);
            }

            return validNumbers.Sum();
        }

        private static (List<int> Valid, List<int> Negative) ParseNumbers(IEnumerable<string> numbers)
        {
            // This pipeline handles parsing, filtering, and error checking in a single pass
            return numbers
                .Select(ParseNumber)
                .Aggregate(
                    (Valid: new List<int>(), Negative: new List<int>()),
                    (acc, num) =>
                    {
                        if (num < 0) acc.Negative.Add(num);
                        else if (num <= MaxValidNumber) acc.Valid.Add(num);
                        return acc;
                    });
        }

        private static int ParseNumber(string number) =>
            int.TryParse(number, out int result) ? result : 0;
    }
}