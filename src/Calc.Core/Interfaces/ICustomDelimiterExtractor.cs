namespace Calc.Core.Interfaces
{
    public interface ICustomDelimiterExtractor
    {
        (string[] customDelimiters, string numbersString) ExtractCustomDelimiters(string input);
    }
}
