using Calc.Core.Interfaces;

namespace Calc.Infrastructure.DelimiterStrategies
{
    public class DefaultDelimiterStrategy : IDelimiterStrategy, IDefaultDelimiterStrategy
    {
        public string[] Split(string input)
        {
            return input.Split(',', '\n');
        }
    }
}