using System.Text.RegularExpressions;

namespace Calc.Core.Utilities
{
    public static partial class RegexPatterns
    {
        [GeneratedRegex(@"^//((?:\[.+?\])+)\n(.*)$", RegexOptions.Singleline)]
        public static partial Regex MultipleCustomDelimiter();

        [GeneratedRegex(@"^//(.).?\n(.*)$", RegexOptions.Singleline)]
        public static partial Regex SingleCustomDelimiter();

        [GeneratedRegex(@"^//(\[.*?\])+\n")]
        public static partial Regex MultipleCustomDelimiterFormat();

        // This regex is used to extract individual delimiters from the bracketed format
        [GeneratedRegex(@"\[([^]]*)\]")]
        public static partial Regex ExtractMultipleDelimiters();

        // This regex is used to check if a string is fully numeric
        [GeneratedRegex(@"^\d+$")]
        public static partial Regex FullyNumeric();

        // New regex for validating multiple custom delimiter format
        [GeneratedRegex(@"^//(\[.+?\])+\n.+$", RegexOptions.Singleline)]
        public static partial Regex ValidMultipleCustomDelimiterFormat();
    }
}