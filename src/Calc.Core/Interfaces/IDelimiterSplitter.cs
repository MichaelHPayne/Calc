namespace Calc.Core.Interfaces
{
    public interface IDelimiterSplitter
    {
        string[] SplitWithAllDelimiters(string numbersString, string[] customDelimiters);
    }
}