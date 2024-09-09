using Xunit;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Infrastructure.Factories;

namespace Calc.Infrastructure.Tests
{
    public class InputParserTests
    {
        private readonly InputParser _parser;

        public InputParserTests()
        {
            var delimiterStrategyFactory = new DefaultDelimiterStrategyFactory();
            var defaultDelimiterStrategy = new DefaultDelimiterStrategy();
            _parser = new InputParser(delimiterStrategyFactory, defaultDelimiterStrategy);
        }

        [Theory]
        [InlineData("1,2,3", new[] { "1", "2", "3" })]
        [InlineData("1\n2,3", new[] { "1", "2", "3" })]
        [InlineData("//;\n1;2;3", new[] { "1", "2", "3" })]
        [InlineData("1,2\n3", new[] { "1", "2", "3" })]
        [InlineData("//,\n1,2,3", new[] { "1", "2", "3" })]
        public void Parse_VariousInputFormats_ReturnsCorrectlySplitArray(string input, string[] expected)
        {
            var result = _parser.Parse(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Parse_EmptyString_ReturnsEmptyArray()
        {
            var result = _parser.Parse("");
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_NullString_ReturnsEmptyArray()
        {
            var result = _parser.Parse("");
            Assert.Empty(result);
        }
    }
}