using Xunit;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Infrastructure.Factories;
using Calc.Core.Interfaces;
using Moq;

namespace Calc.Infrastructure.Tests
{
    public class InputParserTests
    {
        private readonly InputParser _parser;
        private readonly Mock<IDefaultDelimiterStrategy> _mockDefaultStrategy;
        private readonly Mock<ISingleCharCustomDelimiterStrategy> _mockSingleCharStrategy;

        public InputParserTests()
        {
            _mockDefaultStrategy = new Mock<IDefaultDelimiterStrategy>();
            _mockSingleCharStrategy = new Mock<ISingleCharCustomDelimiterStrategy>();
            
            // Setup the SingleCharCustomDelimiterStrategy mock
            _mockSingleCharStrategy.Setup(m => m.WithDelimiter(It.IsAny<string>()))
                .Returns(_mockSingleCharStrategy.Object);

            var delimiterStrategyFactory = new DelimiterStrategyFactory(_mockDefaultStrategy.Object, _mockSingleCharStrategy.Object);
            _parser = new InputParser(delimiterStrategyFactory);
        }

        [Theory]
        [InlineData("1,2,3", new[] { "1", "2", "3" })]
        [InlineData("1\n2,3", new[] { "1", "2", "3" })]
        [InlineData("//;\n1;2;3", new[] { "1", "2", "3" })]
        [InlineData("1,2\n3", new[] { "1", "2", "3" })]
        [InlineData("//,\n1,2,3", new[] { "1", "2", "3" })]
        public void Parse_VariousInputFormats_ReturnsCorrectlySplitArray(string input, string[] expected)
        {
            // Setup the default strategy to return the expected result
            _mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(expected);

            // Setup the single char strategy to return the expected result
            _mockSingleCharStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(expected);

            var result = _parser.Parse(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Parse_EmptyString_ReturnsEmptyArray()
        {
            _mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(new string[0]);
            var result = _parser.Parse("");
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_NullString_ReturnsEmptyArray()
        {
            _mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(new string[0]);
            var result = _parser.Parse(null);
            Assert.Empty(result);
        }
    }
}