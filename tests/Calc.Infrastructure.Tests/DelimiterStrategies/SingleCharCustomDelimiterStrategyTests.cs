using System;
using Xunit;
using Calc.Core.Interfaces;
using Calc.Infrastructure.DelimiterStrategies;

namespace Calc.Infrastructure.Tests.DelimiterStrategies
{
    public class SingleCharCustomDelimiterStrategyTests
    {
        [Fact]
        public void WithDelimiter_ValidSingleCharacter_SetsSplitDelimiter()
        {
            // Arrange
            var strategy = new SingleCharCustomDelimiterStrategy();

            // Act
            strategy.WithDelimiter(";");

            // Assert
            var result = strategy.Split("1;2;3");
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }

        [Fact]
        public void WithDelimiter_InvalidMultiCharacter_ThrowsArgumentException()
        {
            // Arrange
            var strategy = new SingleCharCustomDelimiterStrategy();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => strategy.WithDelimiter(";;"));
        }

        [Fact]
        public void Split_WithCustomDelimiter_ReturnsSplitArray()
        {
            // Arrange
            var strategy = new SingleCharCustomDelimiterStrategy().WithDelimiter("#");

            // Act
            var result = strategy.Split("1#2#3");

            // Assert
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }
    }
}