using Calc.Core.Interfaces;
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
            return numbers.Split(delimiters)
                          .Select(n => int.TryParse(n, out int num) ? num : 0)
                          .Sum();
        }
    }
}