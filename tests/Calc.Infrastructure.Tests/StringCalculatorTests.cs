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
            // Assert.Equal(0, result);
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
            Assert.True(result == 5, $"Adding '5,' should return 5, but it returned {result}");
        }

        [Fact]
        public void Add_InvalidNumber_ShouldTreatAsZero()
        {
            int result = _calculator.Add("5,tytyt");
            Assert.True(result == 5, $"Adding '5,tytyt' should return 5, but it returned {result}");
        }

        [Theory]
        [InlineData("1,2,3", 6)]
        [InlineData("1,2,3,4,5", 15)]
        [InlineData("1,2,3,4,5,6,7,8,9,10,11,12", 78)]
        public void Add_MultipleNumbers_ShouldReturnSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.True(result == expected, $"Adding '{input}' should return {expected}, but it returned {result}");
        }

        [Fact]
        public void Add_ManyNumbers_ShouldReturnCorrectSum()
        {
            string input = string.Join(",", Enumerable.Range(1, 1000));
            int result = _calculator.Add(input);
            Assert.True(result == 500500, $"Adding numbers from 1 to 1000 should return 500500, but it returned {result}");
        }

        [Theory]
        [InlineData("1\n2,3", 6)]
        [InlineData("1,2\n3", 6)]
        [InlineData("1\n2\n3", 6)]
        public void Add_NewlineDelimiter_ShouldReturnSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.True(result == expected, $"Adding '{input}' with newline delimiter should return {expected}, but it returned {result}");
        }

        [Fact]
        public void Add_MixedDelimiters_ShouldReturnSum()
        {
            string input = string.Join("\n", Enumerable.Range(1, 50)) + "," + string.Join(",", Enumerable.Range(51, 50));
            int result = _calculator.Add(input);
            Assert.True(result == 5050, $"Adding numbers from 1 to 100 with mixed delimiters should return 5050, but it returned {result}");
        }

        [Fact]
        public void Add_NegativeNumbers_ShouldThrowNegativeNumberException()
        {
            // Act & Assert
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("1,-2,3,-4,5,-6"));

            // Assert
            var expectedNegatives = new[] { -2, -4, -6 };
            Assert.True(
                exception.NegativeNumbers.SequenceEqual(expectedNegatives) &&
                exception.Message.Contains("Negatives not allowed:") &&
                expectedNegatives.All(n => exception.Message.Contains(n.ToString())),
                $"Exception should contain message 'Negatives not allowed:' and negative numbers {string.Join(", ", expectedNegatives)}, " +
                $"but the actual message was '{exception.Message}' " +
                $"and the actual negative numbers were [{string.Join(", ", exception.NegativeNumbers)}]"
            );
        }

        [Fact]
        public void Add_SingleNegativeNumber_ShouldThrowNegativeNumberException()
        {
            // Act & Assert
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("-1"));

            // Assert
            var expectedNegative = -1;
            Assert.True(
                exception.NegativeNumbers.SequenceEqual(new[] { expectedNegative }) &&
                exception.Message.Contains("Negatives not allowed:") &&
                exception.Message.Contains(expectedNegative.ToString()),
                $"Exception should contain message 'Negatives not allowed:' and negative number {expectedNegative}, " +
                $"but the actual message was '{exception.Message}' " +
                $"and the actual negative numbers were [{string.Join(", ", exception.NegativeNumbers)}]"
            );
        }

        [Fact]
        public void Add_NoNegativeNumbers_ShouldNotThrowException()
        {
            int result = _calculator.Add("1,2,3");
            Assert.True(result == 6, $"Adding '1,2,3' should return 6, but it returned {result}");
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
            Assert.True(result == expected, $"Adding '{input}' should return {expected}, but it returned {result}");
        }

        [Fact]
        public void Add_AllNumbersGreaterThan1000_ShouldReturnZero()
        {
            int result = _calculator.Add("1001,2000,3000");
            Assert.True(result == 0, $"Adding '1001,2000,3000' should return 0, but it returned {result}");
        }

        [Fact]
        public void Add_MixOfValidAndInvalidNumbers_ShouldReturnCorrectSum()
        {
            int result = _calculator.Add("1,1001,2,1002,3,1003,4");
            Assert.True(result == 10, $"Adding '1,1001,2,1002,3,1003,4' should return 10, but it returned {result}");
        }        

        [Theory]
        [InlineData("//;\n1;2", 3)]
        [InlineData("//#\n2#5", 7)]
        [InlineData("//,\n2,ff,100", 102)]
        public void Add_SingleCharCustomDelimiter_ShouldReturnSum(string input, int expected)
        {
            int result = _calculator.Add(input);
            Assert.True(result == expected, $"Adding '{input}' should return {expected}, but it returned {result}");
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNewlineInNumbers_ShouldReturnSum()
        {
            int result = _calculator.Add("//;\n1;2;1001;3");
            Assert.True(result == 6, $"Adding '//;\\n1;2;1001;3' should return 6, but it returned {result}");
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNegativeNumbers_ShouldThrowException()
        {
            var exception = Assert.Throws<NegativeNumberException>(
                () => _calculator.Add("//;\n1;-2;3;-4;5;-6")
            );
            Assert.True(exception.NegativeNumbers.SequenceEqual(new[] { -2, -4, -6 }),
                $"Exception should contain negative numbers [-2, -4, -6], but it contained [{string.Join(", ", exception.NegativeNumbers)}]");
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_WithNumbersOver1000_ShouldIgnoreLargeNumbers()
        {
            int result = _calculator.Add("//;\n1;2;1001;3");
            Assert.True(result == 6, $"Adding '//;\\n1;2;1001;3' should return 6 (ignoring 1001), but it returned {result}");
        }

        [Fact]
        public void Add_SingleCharCustomDelimiter_EmptyInput_ShouldReturnZero()
        {
            int result = _calculator.Add("//;\n");
            Assert.True(result == 0, $"Adding '//;\\n' (empty input) should return 0, but it returned {result}");
        }

        // New tests for multiple delimiters of any length (Requirement 8)

        [Fact]
        public void Add_MultipleCustomDelimiters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n11r9r22*hh*33!!44");
            Assert.True(result == 110, $"Adding '//[*][!!][r9r]\\n11r9r22*hh*33!!44' should return 110, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_EmptyInput_ShouldReturnZero()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n");
            Assert.True(result == 0, $"Adding '//[*][!!][r9r]\\n' (empty input) should return 0, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_OnlyDelimiters_ShouldReturnZero()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n*!!r9r");
            Assert.True(result == 0, $"Adding '//[*][!!][r9r]\\n*!!r9r' (only delimiters) should return 0, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_DifferentLengths_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][!!!][r9r]\n11r9r22*33!!!44");
            Assert.True(result == 110, $"Adding '//[*][!!!][r9r]\\n11r9r22*33!!!44' should return 110, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_MixedSingleAndMultiChar_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][!!][;]\n11;22*33!!44");
            Assert.True(result == 110, $"Adding '//[*][!!][;]\\n11;22*33!!44' should return 110, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_WithNegativeNumbers_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<NegativeNumberException>(
                () => _calculator.Add("//[*][!!][r9r]\n11r9r22*-33!!44")
            );

            // Assert
            var expectedNegative = -33;
            Assert.True(
                exception.NegativeNumbers.Contains(expectedNegative) &&
                exception.Message.Contains("Negatives not allowed:") &&
                exception.Message.Contains(expectedNegative.ToString()),
                $"Exception should contain message 'Negatives not allowed:' and negative number {expectedNegative}, " +
                $"but the actual message was '{exception.Message}' " +
                $"and the actual negative numbers were [{string.Join(", ", exception.NegativeNumbers)}]"
            );
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_WithNumbersOver1000_ShouldIgnoreLargeNumbers()
        {
            int result = _calculator.Add("//[*][!!][r9r]\n11r9r1001*33!!44");
            Assert.True(result == 88, $"Adding '//[*][!!][r9r]\\n11r9r1001*33!!44' should return 88 (ignoring 1001), but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithDefault_ShouldReturnSum()
        {
            int result = _calculator.Add("1,2\n3");
            Assert.True(result == 6, $"Adding '1,2\\n3' should return 6, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithSingleCustom_ShouldReturnSum()
        {
            int result = _calculator.Add("//;\n1;2;3");
            Assert.True(result == 6, $"Adding '//;\\n1;2;3' should return 6, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_BackwardCompatibilityWithBrackets_ShouldReturnSum()
        {
            int result = _calculator.Add("//[***]\n1***2***3");
            Assert.True(result == 6, $"Adding '//[***]\\n1***2***3' should return 6, but it returned {result}");
        }

        [Theory]
        [InlineData("//[*][!!]11*22!!33")]
        public void Add_MultipleCustomDelimiters_InvalidFormat_ShouldThrowException(string input)
        {
            var exception = Assert.Throws<InvalidDelimiterException>(() => _calculator.Add(input));
            
            Assert.True(
                exception.Message.Contains("Invalid format for multiple custom delimiters") &&
                exception.InvalidDelimiter == input,
                $"Expected exception message to contain 'Invalid format for multiple custom delimiters' and InvalidDelimiter to be '{input}', " +
                $"but got message '{exception.Message}' and InvalidDelimiter '{exception.InvalidDelimiter}'"
            );
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_EmptyDelimiters_ShouldThrowException()
        {
            // Arrange
            string input = "//[][!!][]\n11!!22!!33";

            // Act & Assert
            var exception = Assert.Throws<InvalidDelimiterException>(() => _calculator.Add(input));

            // Assert
            Assert.True(
                exception.Message.Contains("Delimiter cannot be empty") &&
                exception.InvalidDelimiter == "",
                $"Expected exception message to contain 'Delimiter cannot be empty' and InvalidDelimiter to be empty, " +
                $"but got message '{exception.Message}' and InvalidDelimiter '{exception.InvalidDelimiter}'"
            );
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_RegexSpecialCharacters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][+][$]\n11*22+33$44");
            Assert.True(result == 110, $"Adding '//[*][+][$]\\n11*22+33$44' with regex special characters as delimiters should return 110, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_SubstringDelimiters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[**][*]\n11**22*33");
            Assert.True(result == 66, $"Adding '//[**][*]\\n11**22*33' with substring delimiters should return 66, but it returned {result}");
        }

        [Fact]
        public void Add_MultipleCustomDelimiters_RepeatedDelimiters_ShouldReturnSum()
        {
            int result = _calculator.Add("//[*][*][!!]\n11*22*33!!44");
            Assert.True(result == 110, $"Adding '//[*][*][!!]\\n11*22*33!!44' with repeated delimiters should return 110, but it returned {result}");
        }
    }
}
