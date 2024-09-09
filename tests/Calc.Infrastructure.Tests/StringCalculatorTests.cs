using Xunit;
using Calc.Core.Interfaces;
using Calc.Infrastructure;
using System.Linq;

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

        [Theory]
        [InlineData("1,2,3", 6)]
        [InlineData("1,2,3,4,5", 15)]
        [InlineData("1,2,3,4,5,6,7,8,9,10,11,12", 78)]
        public void Add_MultipleNumbers_ReturnsSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_ManyNumbers_ReturnsCorrectSum()
        {
            string input = string.Join(",", Enumerable.Range(1, 1000));
            int result = _calculator.Add(input);
            Assert.Equal(500500, result); // Sum of numbers from 1 to 1000
        }

        [Theory]
        [InlineData("1\n2,3", 6)]
        [InlineData("1,2\n3", 6)]
        [InlineData("1\n2\n3", 6)]
        public void Add_NewlineDelimiter_ReturnsSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_MixedDelimiters_ReturnsSum()
        {
            string input = string.Join("\n", Enumerable.Range(1, 50)) + "," + string.Join(",", Enumerable.Range(51, 50));
            int result = _calculator.Add(input);
            Assert.Equal(5050, result); // Sum of numbers from 1 to 100
        }        
    }
}