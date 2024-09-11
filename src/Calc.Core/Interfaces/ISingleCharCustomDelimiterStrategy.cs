namespace Calc.Core.Interfaces
{
    public interface ISingleCharCustomDelimiterStrategy : IDelimiterStrategy
    {
        ISingleCharCustomDelimiterStrategy WithDelimiter(string delimiter);
    }
}