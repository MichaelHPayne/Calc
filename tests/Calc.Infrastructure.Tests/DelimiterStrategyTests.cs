using Xunit;
using Calc.Infrastructure.DelimiterStrategies;

namespace Calc.Infrastructure.Tests
{
    public class DelimiterStrategyTests
    {
        [Fact]
        public void DefaultDelimiterStrategy_SplitsCorrectly()
        {
            var strategy = new DefaultDelimiterStrategy();
            var result = strategy.Split("1,2\n3");
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }

        [Fact]
        public void SingleCharCustomDelimiterStrategy_SplitsCorrectly()
        {
            var strategy = new SingleCharCustomDelimiterStrategy();
            strategy.WithDelimiter(";");
            var result = strategy.Split("1;2;3");
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }
    }
}