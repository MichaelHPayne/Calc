using Xunit;
using Calc.Core.Interfaces;
using Calc.Infrastructure.Factories;
using Calc.Infrastructure.DelimiterStrategies;
using Moq;

namespace Calc.Infrastructure.Tests
{
    public class DelimiterStrategyFactoryTests
    {
        private readonly Mock<IDefaultDelimiterStrategy> _mockDefaultStrategy;
        private readonly Mock<ISingleCharCustomDelimiterStrategy> _mockSingleCharStrategy;
        private readonly DelimiterStrategyFactory _factory;

        public DelimiterStrategyFactoryTests()
        {
            _mockDefaultStrategy = new Mock<IDefaultDelimiterStrategy>();
            _mockSingleCharStrategy = new Mock<ISingleCharCustomDelimiterStrategy>();
            _mockSingleCharStrategy.Setup(m => m.WithDelimiter(It.IsAny<string>())).Returns(_mockSingleCharStrategy.Object);
            _factory = new DelimiterStrategyFactory(_mockDefaultStrategy.Object, _mockSingleCharStrategy.Object);
        }

        [Fact]
        public void CreateStrategy_DefaultInput_ReturnsDefaultStrategy()
        {
            var result = _factory.CreateStrategy("1,2,3");
            Assert.Same(_mockDefaultStrategy.Object, result);
        }

        [Fact]
        public void CreateStrategy_SingleCharDelimiter_ReturnsSingleCharStrategy()
        {
            var result = _factory.CreateStrategy("//;\n1;2;3");
            Assert.Same(_mockSingleCharStrategy.Object, result);
            _mockSingleCharStrategy.Verify(m => m.WithDelimiter(";"), Times.Once);
        }

        [Fact]
        public void CreateStrategy_InputWithNewlines_ReturnsDefaultStrategy()
        {
            var result = _factory.CreateStrategy("1\n2\n3");
            Assert.Same(_mockDefaultStrategy.Object, result);
        }
                
    }
}