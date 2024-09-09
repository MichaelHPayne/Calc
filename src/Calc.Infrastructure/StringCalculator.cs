using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using System.Linq;

namespace Calc.Infrastructure
{
    public class StringCalculator : IStringCalculator
    {
        public int Add(string numbers)
        {
            if (string.IsNullOrEmpty(numbers))
            {
                return 0;
            }

            var delimiters = new[] { ',', '\n' };
            var numberList = numbers.Split(delimiters)
                                    .Select(n => int.TryParse(n, out int num) ? num : 0)
                                    .ToList();

            var negativeNumbers = numberList.Where(n => n < 0).ToList();
            if (negativeNumbers.Any())
            {
                throw new NegativeNumberException(negativeNumbers);
            }

            return numberList.Sum();
        }
    }
}