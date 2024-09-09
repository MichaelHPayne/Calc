using Calc.Core.Interfaces;

public class InputParser
{
    private readonly IDelimiterStrategyFactory _delimiterStrategyFactory;
    private readonly IDefaultDelimiterStrategy _defaultStrategy;

    public InputParser(IDelimiterStrategyFactory delimiterStrategyFactory, IDefaultDelimiterStrategy defaultStrategy)
    {
        _delimiterStrategyFactory = delimiterStrategyFactory;
        _defaultStrategy = defaultStrategy;
    }

    public string[] Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new string[0];
        }

        if (input.StartsWith("//") && input.Length > 3)
        {
            int newLineIndex = input.IndexOf('\n');
            if (newLineIndex > 2 && newLineIndex < input.Length - 1)
            {
                string customDelimiter = input.Substring(2, newLineIndex - 2);
                string numberString = input.Substring(newLineIndex + 1);
                var strategy = _delimiterStrategyFactory.CreateStrategy(customDelimiter);
                return strategy.Split(numberString.Replace("\n", customDelimiter));
            }
        }

        // Use default strategy for all other cases, including malformed custom delimiter inputs
        return _defaultStrategy.Split(input);
    }
}
