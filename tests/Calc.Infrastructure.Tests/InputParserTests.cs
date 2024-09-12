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
        private readonly Mock<IDefaultDelimiterStrategy> _mockDefaultStrategy;
        private readonly Mock<ISingleCharCustomDelimiterStrategy> _mockSingleCharStrategy;
        private readonly IDelimiterStrategyFactory _delimiterStrategyFactory;
        private readonly InputParser _parser;

        public InputParserTests()
        {
            _mockDefaultStrategy = new Mock<IDefaultDelimiterStrategy>();
            _mockSingleCharStrategy = new Mock<ISingleCharCustomDelimiterStrategy>();
            
            // Setup the SingleCharCustomDelimiterStrategy mock
            _mockSingleCharStrategy.Setup(m => m.WithDelimiter(It.IsAny<string>()))
                .Returns(_mockSingleCharStrategy.Object);

            _delimiterStrategyFactory = new DelimiterStrategyFactory(_mockDefaultStrategy.Object, _mockSingleCharStrategy.Object);
            _parser = new InputParser(_delimiterStrategyFactory);
        }

        [Theory]
        [InlineData("1,2,3", new[] { "1", "2", "3" })]
        [InlineData("1\n2,3", new[] { "1", "2", "3" })]
        [InlineData("//;\n1;2;3", new[] { "1", "2", "3" })]
        [InlineData("1,2\n3", new[] { "1", "2", "3" })]
        [InlineData("//,\n1,2,3", new[] { "1", "2", "3" })]
        [InlineData("//#\n1#2\n3", new[] { "1", "2", "3" })]
        public void Parse_VariousInputFormats_ReturnsCorrectlySplitArray(string input, string[] expected)
        {
            // Arrange
            SetupMockStrategy(input, expected);

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_NullOrEmptyString_ReturnsEmptyArray(string input)
        {
            // Arrange
            _mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(Array.Empty<string>());

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Empty(result);
        }

        private void SetupMockStrategy(string input, string[] expected)
        {
            if (input.StartsWith("//"))
            {
                _mockSingleCharStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(expected);
            }
            else
            {
                _mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns(expected);
            }
        }
    }
}
