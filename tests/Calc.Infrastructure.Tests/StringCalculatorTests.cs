using Xunit;
using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using System.Linq;
using Moq;
using Calc.Infrastructure.Factories;

namespace Calc.Infrastructure.Tests
{
    public class StringCalculatorTests
    {
        private readonly IStringCalculator _calculator;

        public StringCalculatorTests()
        {
            _calculator = CreateCalculator();
        }

        private static IStringCalculator CreateCalculator()
        {
            var mockDefaultStrategy = new Mock<IDefaultDelimiterStrategy>();
            mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns((string s) => s.Split(new[] { ',', '\n' }, StringSplitOptions.None));

            var mockSingleCharStrategy = new Mock<ISingleCharCustomDelimiterStrategy>();
            mockSingleCharStrategy.Setup(m => m.WithDelimiter(It.IsAny<string>())).Returns(mockSingleCharStrategy.Object);
            mockSingleCharStrategy.Setup(m => m.Split(It.IsAny<string>())).Returns((string s) => s.Split(new[] { ',' }, StringSplitOptions.None));

            var mockDelimiterFactory = new Mock<IDelimiterStrategyFactory>();
            mockDelimiterFactory.Setup(m => m.CreateStrategy(It.IsAny<string>())).Returns(mockDefaultStrategy.Object);

            return new StringCalculator(mockDelimiterFactory.Object);
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

        [Fact]
        public void Add_NegativeNumbers_ThrowsNegativeNumberException()
        {
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("1,-2,3,-4,5,-6"));
            Assert.Equal(new[] { -2, -4, -6 }, exception.NegativeNumbers);
            Assert.Contains("Negatives not allowed:", exception.Message);
            Assert.Contains("-2", exception.Message);
            Assert.Contains("-4", exception.Message);
            Assert.Contains("-6", exception.Message);
        }

        [Fact]
        public void Add_SingleNegativeNumber_ThrowsNegativeNumberException()
        {
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("-1"));
            Assert.Equal(new[] { -1 }, exception.NegativeNumbers);
            Assert.Contains("Negatives not allowed:", exception.Message);
            Assert.Contains("-1", exception.Message);
        }

        [Fact]
        public void Add_NoNegativeNumbers_DoesNotThrowException()
        {
            int result = _calculator.Add("1,2,3");
            Assert.Equal(6, result);
        }

        [Theory]
        [InlineData("2,1001,6", 8)]
        [InlineData("1000,2,3", 1005)]
        [InlineData("999,1001,2", 1001)]
        [InlineData("1,2,3,1001,4,1002,5", 15)]
        [InlineData("5,1001", 5)]  // New test case for two numbers with one > 1000
        public void Add_NumbersGreaterThan1000_AreIgnored(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_AllNumbersGreaterThan1000_ReturnsZero()
        {
            int result = _calculator.Add("1001,2000,3000");
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_MixOfValidAndInvalidNumbers_ReturnsCorrectSum()
        {
            int result = _calculator.Add("1,1001,2,1002,3,1003,4");
            Assert.Equal(10, result);
        }        

        [Theory]
        [InlineData("//;\n1;2", 3)]
        [InlineData("//#\n2#5", 7)]
        [InlineData("//,\n2,ff,100", 102)]
        public void Add_SingleCharCustomDelimiter_ReturnsSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNewlineInNumbers_ReturnsSum()
        {
            int result = _calculator.Add("//;\n1;2;1001;3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNegativeNumbers_ThrowsException()
        {
            var exception = Assert.Throws<NegativeNumberException>(
                () => _calculator.Add("//;\n1;-2;3;-4;5;-6")
            );
            Assert.Equal(new[] { -2, -4, -6 }, exception.NegativeNumbers);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNumbersOver1000_IgnoresLargeNumbers()
        {
            int result = _calculator.Add("//;\n1;2;1001;3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_EmptyInput_ReturnsZero()
        {
            int result = _calculator.Add("//;\n");
            Assert.Equal(0, result);
        }

        // New tests for multiple delimiters of any length (Requirement 8)

        [Fact]
        public void Add_MultipleCustomDelimiters_ReturnsSum()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n11r9r22*hh*33!!44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_EmptyInput_ReturnsZero()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n");
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_OnlyDelimiters_ReturnsZero()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n*!!r9r");
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_DifferentLengths_ReturnsSum()
        {
            int result = _calculator.Add("//[*][!!!][r9r]\n11r9r22*33!!!44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_MixedSingleAndMultiChar_ReturnsSum()
        {
            int result = _calculator.Add("//[*][!!][;]\n11;22*33!!44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_WithNegativeNumbers_ThrowsException()
        {
            var exception = Assert.Throws<NegativeNumberException>(
                () => _calculator.Add("//[*][!!][r9r]\n11r9r22*-33!!44")
            );
            Assert.Contains(-33, exception.NegativeNumbers);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_WithNumbersOver1000_IgnoresLargeNumbers()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n11r9r1001*33!!44");
            Assert.Equal(88, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithDefault_ReturnsSum()
        {
            int result = _calculator.Add("1,2\n3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithSingleCustom_ReturnsSum()
        {
            int result = _calculator.Add("//;\n1;2;3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithBrackets_ReturnsSum()
        {
            int result = _calculator.Add("//[***]\n1***2***3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_InvalidFormat_ThrowsException()
        {
            Assert.Throws<InvalidDelimiterException>(() => _calculator.Add("//[*][!!]11*22!!33"));
        }

// This one fails:
        [Fact]
        public void Add_MultipleCustomDelimiters_EmptyDelimiters_ThrowsException()
        {
            Assert.Throws<InvalidDelimiterException>(() => _calculator.Add("//[][!!][]\n11!!22!!33"));
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_RegexSpecialCharacters_ReturnsSum()
        {
            int result = _calculator.Add("//[*][+][$]\n11*22+33$44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_SubstringDelimiters_ReturnsSum()
        {
            int result = _calculator.Add("//[**][*]\n11**22*33");
            Assert.Equal(66, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_RepeatedDelimiters_ReturnsSum()
        {
            int result = _calculator.Add("//[*][*][!!]\n11*22*33!!44");
            Assert.Equal(110, result);
        }
    }
}