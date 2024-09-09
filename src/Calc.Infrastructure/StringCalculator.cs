using Calc.Core.Interfaces;

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

            return numbers.Split(',')
                          .Select(n => int.TryParse(n, out int num) ? num : 0)
                          .Sum();
        }
    }
}