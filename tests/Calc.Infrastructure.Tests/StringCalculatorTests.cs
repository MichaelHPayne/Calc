using Xunit;
using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using System.Linq;
using Moq;
using Calc.Infrastructure.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Calc.Infrastructure.Tests
{
    public class StringCalculatorTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IStringCalculator _calculator;
        private static readonly string[] DefaultDelimiters = { ",", "\n" };

        public StringCalculatorTests()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _calculator = _serviceProvider.GetRequiredService<IStringCalculator>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IStringCalculator, StringCalculator>();

            var mockDefaultStrategy = new Mock<IDefaultDelimiterStrategy>();
            mockDefaultStrategy.Setup(m => m.Split(It.IsAny<string>()))
                .Returns((string s) => s.Split(DefaultDelimiters, StringSplitOptions.None));
            services.AddSingleton(mockDefaultStrategy.Object);

            var mockSingleCharStrategy = new Mock<ISingleCharCustomDelimiterStrategy>();
            mockSingleCharStrategy.Setup(m => m.WithDelimiter(It.IsAny<string>()))
                .Returns(mockSingleCharStrategy.Object);
            mockSingleCharStrategy.Setup(m => m.Split(It.IsAny<string>()))
                .Returns((string s) => s.Split(new[] { ',' }, StringSplitOptions.None));
            services.AddSingleton(mockSingleCharStrategy.Object);

            var mockDelimiterFactory = new Mock<IDelimiterStrategyFactory>();
            mockDelimiterFactory.Setup(m => m.CreateStrategy(It.IsAny<string>()))
                .Returns(mockDefaultStrategy.Object);
            services.AddSingleton(mockDelimiterFactory.Object);
        }

        public void Dispose()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [Fact]
        public void Add_EmptyString_ShouldReturnZero()
        {
            int result = _calculator.Add("");
            Assert.Equal(0, result);
            Assert.True(result == 0, $"Adding an empty string should return 0, but it returned {result}");
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("20", 20)]
        public void Add_SingleNumber_ShouldReturnSameNumber(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
            Assert.True(result == expected, $"Adding '{input}' should return {expected}, but it returned {result}");
        }

        [Fact]
        public void Add_MissingNumber_ShouldTreatAsZero()
        {
            int result = _calculator.Add("5,");
            Assert.Equal(5, result);
            Assert.True(result == 5, $"Adding '5,' should return 5, but it returned {result}");
        }

        [Fact]
        public void Add_InvalidNumber_ShouldTreatAsZero()
        {
            int result = _calculator.Add("5,tytyt");
            Assert.Equal(5, result);
            Assert.True(result == 5, $"Adding '5,tytyt' should return 5, but it returned {result}");
        }

        [Theory]
        [InlineData("1,2,3", 6)]
        [InlineData("1,2,3,4,5", 15)]
        [InlineData("1,2,3,4,5,6,7,8,9,10,11,12", 78)]
        public void Add_MultipleNumbers_ShouldReturnSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
            Assert.True(result == expected, $"Adding '{input}' should return {expected}, but it returned {result}");
        }

        [Fact]
        public void Add_ManyNumbers_ShouldReturnCorrectSum()
        {
            string input = string.Join(",", Enumerable.Range(1, 1000));
            int result = _calculator.Add(input);
            Assert.Equal(500500, result); // Sum of numbers from 1 to 1000
            Assert.True(result == 500500, $"Adding numbers from 1 to 1000 should return 500500, but it returned {result}");
        }

        [Theory]
        [InlineData("1\n2,3", 6)]
        [InlineData("1,2\n3", 6)]
        [InlineData("1\n2\n3", 6)]
        public void Add_NewlineDelimiter_ShouldReturnSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_MixedDelimiters_ShouldReturnSum()
        {
            string input = string.Join("\n", Enumerable.Range(1, 50)) + "," + string.Join(",", Enumerable.Range(51, 50));
            int result = _calculator.Add(input);
            Assert.Equal(5050, result); // Sum of numbers from 1 to 100
        }

        [Fact]
        public void Add_NegativeNumbers_ShouldThrowNegativeNumberException()
        {
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("1,-2,3,-4,5,-6"));
            Assert.Equal(new[] { -2, -4, -6 }, exception.NegativeNumbers);
            Assert.Contains("Negatives not allowed:", exception.Message);
            Assert.Contains("-2", exception.Message);
            Assert.Contains("-4", exception.Message);
            Assert.Contains("-6", exception.Message);
        }

        [Fact]
        public void Add_SingleNegativeNumber_ShouldThrowNegativeNumberException()
        {
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("-1"));
            Assert.Equal(new[] { -1 }, exception.NegativeNumbers);
            Assert.Contains("Negatives not allowed:", exception.Message);
            Assert.Contains("-1", exception.Message);
        }

        [Fact]
        public void Add_NoNegativeNumbers_ShouldNotThrowException()
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
        public void Add_NumbersGreaterThan1000_ShouldBeIgnored(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_AllNumbersGreaterThan1000_ShouldReturnZero()
        {
            int result = _calculator.Add("1001,2000,3000");
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_MixOfValidAndInvalidNumbers_ShouldReturnCorrectSum()
        {
            int result = _calculator.Add("1,1001,2,1002,3,1003,4");
            Assert.Equal(10, result);
        }        

        [Theory]
        [InlineData("//;\n1;2", 3)]
        [InlineData("//#\n2#5", 7)]
        [InlineData("//,\n2,ff,100", 102)]
        public void Add_SingleCharCustomDelimiter_ShouldReturnSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNewlineInNumbers_ShouldReturnSum()
        {
            int result = _calculator.Add("//;\n1;2;1001;3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNegativeNumbers_ShouldThrowException()
        {
            var exception = Assert.Throws<NegativeNumberException>(
                () => _calculator.Add("//;\n1;-2;3;-4;5;-6")
            );
            Assert.Equal(new[] { -2, -4, -6 }, exception.NegativeNumbers);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNumbersOver1000_ShouldIgnoreLargeNumbers()
        {
            int result = _calculator.Add("//;\n1;2;1001;3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_EmptyInput_ShouldReturnZero()
        {
            int result = _calculator.Add("//;\n");
            Assert.Equal(0, result);
        }

        // New tests for multiple delimiters of any length (Requirement 8)

        [Fact]
        public void Add_MultipleCustomDelimiters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n11r9r22*hh*33!!44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_EmptyInput_ShouldReturnZero()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n");
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_OnlyDelimiters_ShouldReturnZero()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n*!!r9r");
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_DifferentLengths_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][!!!][r9r]\n11r9r22*33!!!44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_MixedSingleAndMultiChar_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][!!][;]\n11;22*33!!44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_WithNegativeNumbers_ShouldThrowException()
        {
            var exception = Assert.Throws<NegativeNumberException>(
                () => _calculator.Add("//[*][!!][r9r]\n11r9r22*-33!!44")
            );
            Assert.Contains(-33, exception.NegativeNumbers);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_WithNumbersOver1000_ShouldIgnoreLargeNumbers()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n11r9r1001*33!!44");
            Assert.Equal(88, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithDefault_ShouldReturnSum()
        {
            int result = _calculator.Add("1,2\n3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithSingleCustom_ShouldReturnSum()
        {
            int result = _calculator.Add("//;\n1;2;3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithBrackets_ShouldReturnSum()
        {
            int result = _calculator.Add("//[***]\n1***2***3");
            Assert.Equal(6, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_InvalidFormat_ShouldThrowException()
        {
            Assert.Throws<InvalidDelimiterException>(() => _calculator.Add("//[*][!!]11*22!!33"));
        }

// This one fails:
        [Fact]
        public void Add_MultipleCustomDelimiters_EmptyDelimiters_ShouldThrowException()
        {
            Assert.Throws<InvalidDelimiterException>(() => _calculator.Add("//[][!!][]\n11!!22!!33"));
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_RegexSpecialCharacters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][+][$]\n11*22+33$44");
            Assert.Equal(110, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_SubstringDelimiters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[**][*]\n11**22*33");
            Assert.Equal(66, result);
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_RepeatedDelimiters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][*][!!]\n11*22*33!!44");
            Assert.Equal(110, result);
        }
    }
}
