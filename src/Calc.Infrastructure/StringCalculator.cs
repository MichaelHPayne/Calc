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

            string[] numberArray = numbers.Split(',');

            if (numberArray.Length > 2)
            {
                throw new ArgumentException("More than 2 numbers are not allowed.");
            }

            int sum = 0;
            foreach (string number in numberArray)
            {
                if (int.TryParse(number, out int parsedNumber))
                {
                    sum += parsedNumber;
                }
            }

            return sum;
        }
    }
}