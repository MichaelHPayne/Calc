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
            int result = _calculator.Add("");
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("20", 20)]
        public void Add_SingleNumber_ReturnsNumber(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1,5000", 5001)]
        [InlineData("4,-3", 1)]
        public void Add_TwoNumbers_ReturnsSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_MissingNumber_TreatsAsZero()
        {
            int result = _calculator.Add("5,");
            Assert.Equal(5, result);
        }

        [Fact]
        public void Add_InvalidNumber_TreatsAsZero()
        {
            int result = _calculator.Add("5,tytyt");
            Assert.Equal(5, result);
        }

        [Fact]
        public void Add_MoreThanTwoNumbers_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _calculator.Add("1,2,3"));
        }
    }
}