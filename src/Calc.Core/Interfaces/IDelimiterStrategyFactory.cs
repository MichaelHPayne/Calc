using System;

namespace Calc.Core.Interfaces
{
    public interface IDelimiterStrategyFactory
    {
        IDelimiterStrategy CreateStrategy(string input);
    }
}