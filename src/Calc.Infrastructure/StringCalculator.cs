using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using System.Linq;
using System.Diagnostics;

namespace Calc.Infrastructure
{
    public class StringCalculator : IStringCalculator
    {
        private const int MaxValidNumber = 1000;
        private readonly InputParser _inputParser;

        public StringCalculator(IDelimiterStrategyFactory delimiterStrategyFactory)
        {
            _inputParser = new InputParser(delimiterStrategyFactory);
        }

        public int Add(string numbers)
        {
            Debug.WriteLine($"StringCalculator.Add called with input: {numbers}");

            if (string.IsNullOrWhiteSpace(numbers))
            {
                Debug.WriteLine("Input is null or empty, returning 0");
                return 0;
            }

            var parsedNumbers = _inputParser.Parse(numbers);
            Debug.WriteLine($"Parsed numbers: {string.Join(", ", parsedNumbers)}");

            var processedNumbers = parsedNumbers
                .Select(ParseNumber)
                .Where(n => n <= MaxValidNumber)
                .ToList();

            Debug.WriteLine($"Processed number list: {string.Join(", ", processedNumbers)}");

            var negativeNumbers = processedNumbers.Where(n => n < 0).ToList();
            if (negativeNumbers.Any())
            {
                Debug.WriteLine($"Negative numbers found: {string.Join(", ", negativeNumbers)}");
                throw new NegativeNumberException(negativeNumbers);
            }

            var result = processedNumbers.Sum();
            Debug.WriteLine($"Final result: {result}");
            return result;
        }

        private int ParseNumber(string number)
        {
            return int.TryParse(number, out int result) ? result : 0;
        }
    }
}