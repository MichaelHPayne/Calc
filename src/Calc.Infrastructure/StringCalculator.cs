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

        public StringCalculator(IDelimiterStrategyFactory delimiterStrategyFactory, IDefaultDelimiterStrategy defaultDelimiterStrategy)
        {
            _inputParser = new InputParser(delimiterStrategyFactory, defaultDelimiterStrategy);
        }

        public int Add(string numbers)
        {
            Debug.WriteLine($"StringCalculator.Add called with input: {numbers}");

            if (string.IsNullOrEmpty(numbers))
            {
                Debug.WriteLine("Input is null or empty, returning 0");
                return 0;
            }

            var parsedNumbers = _inputParser.Parse(numbers);
            Debug.WriteLine($"Parsed numbers: {string.Join(", ", parsedNumbers)}");

            var numberList = parsedNumbers
                .Select(n => int.TryParse(n, out int num) ? num : 0)
                .Where(n => n <= MaxValidNumber)
                .ToList();

            Debug.WriteLine($"Processed number list: {string.Join(", ", numberList)}");

            var negativeNumbers = numberList.Where(n => n < 0).ToList();
            if (negativeNumbers.Any())
            {
                Debug.WriteLine($"Negative numbers found: {string.Join(", ", negativeNumbers)}");
                throw new NegativeNumberException(negativeNumbers);
            }

            var result = numberList.Sum();
            Debug.WriteLine($"Final result: {result}");
            return result;
        }
    }
}
