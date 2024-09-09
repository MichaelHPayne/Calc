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
            throw new NotImplementedException();
        }
    }
}