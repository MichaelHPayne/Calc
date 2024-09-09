using Xunit;
using Calc.Core.Interfaces;
using Calc.Infrastructure;

namespace Calc.Infrastructure.Tests
{
    public class StringCalculatorTests
    {
        private readonly IStringCalculator _calculator;

        public StringCalculatorTests()
        {
            _calculator = new StringCalculator();
        }

        [Fact]
        public void Add_EmptyString_ReturnsZero()
        {
            // Act
            int result = _calculator.Add("");

            // Assert
            Assert.Equal(0, result);
        }
    }
}