using Xunit;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Core.Exceptions;
using System;

namespace Calc.Infrastructure.Tests.DelimiterStrategies
{
    public class MultipleCustomDelimiterStrategyTests
    {
        [Theory]
        [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44", new[] { "11", "22", "hh", "33", "44" })]
        [InlineData("//[***][#][%]\n11***22#33%44", new[] { "11", "22", "33", "44" })]
        [InlineData("//[##][!!][aaa]\n1##2!!3aaa4", new[] { "1", "2", "3", "4" })]
        [InlineData("//[!][!!][!!!]\n1!2!!3!!!4", new[] { "1", "2", "3", "4" })]  // Test with nested delimiters
        [InlineData("//[###][@@][&&&]\n1###2@@3&&&4", new[] { "1", "2", "3", "4" })]
        [InlineData("//[;;][**][???]\n1;;2**3???4", new[] { "1", "2", "3", "4" })]
        [InlineData("//[ab][cd][ef]\n1ab2cd3ef4", new[] { "1", "2", "3", "4" })]
        [InlineData("//[**][++][..]\n100**200++300..400", new[] { "100", "200", "300", "400" })]
        [InlineData("//[delimiter][separator][divider]\n1delimiter2separator3divider4", new[] { "1", "2", "3", "4" })]
        [InlineData("//[!@#][#@!][***]\n10!@#20#@!30***40", new[] { "10", "20", "30", "40" })]
        [InlineData("//[==][!=][<>]\n5==10!=15<>20", new[] { "5", "10", "15", "20" })]
        [InlineData("//[.][..][...]\n1.2..3...4", new[] { "1", "2", "3", "4" })]
        public void Split_WithMultipleCustomDelimiters_ReturnsSplitArray(string input, string[] expected)
        {
            var strategy = new MultipleCustomDelimiterStrategy(input);
            var result = strategy.Split(input.Substring(input.IndexOf('\n') + 1));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Split_EmptyString_ReturnsEmptyArray()
        {
            var strategy = new MultipleCustomDelimiterStrategy("//[*][!!][r9r]\n");
            var result = strategy.Split("");
            Assert.Empty(result);
        }

        [Fact]
        public void Split_SingleNumber_ReturnsSingleElementArray()
        {
            var strategy = new MultipleCustomDelimiterStrategy("//[*][!!][r9r]\n");
            var result = strategy.Split("42");
            Assert.Single(result);
            Assert.Equal("42", result[0]);
        }

        [Fact]
        public void Constructor_InvalidFormat_ThrowsInvalidDelimiterException()
        {
            Assert.Throws<InvalidDelimiterException>(() => new MultipleCustomDelimiterStrategy("1,2,3"));
        }

        [Fact]
        public void Constructor_WithEmptyDelimiter_ThrowsInvalidDelimiterException()
        {
            var exception = Assert.Throws<InvalidDelimiterException>(() => new MultipleCustomDelimiterStrategy("//[][abc]\n1abc2"));
            Assert.Equal("Delimiter cannot be empty or whitespace.", exception.Message);
            Assert.Equal("", exception.InvalidDelimiter);
        }

        [Fact]
        public void Constructor_WithWhitespaceDelimiter_ThrowsInvalidDelimiterException()
        {
            var exception = Assert.Throws<InvalidDelimiterException>(() => new MultipleCustomDelimiterStrategy("//[ ][abc]\n1abc2"));
            Assert.Equal("Delimiter cannot be empty or whitespace.", exception.Message);
            Assert.Equal(" ", exception.InvalidDelimiter);
        }

        [Fact]
        public void Constructor_WithFullyNumericDelimiter_ThrowsInvalidDelimiterException()
        {
            var exception = Assert.Throws<InvalidDelimiterException>(() => new MultipleCustomDelimiterStrategy("//[123][456]\n1123456"));
            Assert.Equal("Fully numeric delimiters are not allowed.", exception.Message);
            Assert.Equal("123", exception.InvalidDelimiter);
        }

        [Fact]
        public void Constructor_WithPartialNumericDelimiter_DoesNotThrow()
        {
            var strategy = new MultipleCustomDelimiterStrategy("//[a1b][c2d]\n1a1b2c2d3");
            var result = strategy.Split("1a1b2c2d3");
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }

        [Fact]
        public void Constructor_WithDuplicateDelimiters_ThrowsInvalidDelimiterException()
        {
            var exception = Assert.Throws<InvalidDelimiterException>(() => new MultipleCustomDelimiterStrategy("//[abc][def][abc]\n1abc2def3"));
            Assert.Equal("Duplicate delimiters are not allowed.", exception.Message);
            Assert.Equal("abc", exception.InvalidDelimiter);
        }

        [Fact]
        public void Split_WithMultipleOccurrencesOfSameDelimiter_SplitsCorrectly()
        {
            var strategy = new MultipleCustomDelimiterStrategy("//[**][++][..]\n");
            var result = strategy.Split("1**2**3++4..5");
            Assert.Equal(new[] { "1", "2", "3", "4", "5" }, result);
        }

        [Fact]
        public void Split_WithDelimitersContainingSpecialRegexCharacters_SplitsCorrectly()
        {
            var strategy = new MultipleCustomDelimiterStrategy("//[.*][+?][$^]\n");
            var result = strategy.Split("1.*2+?3$^4");
            Assert.Equal(new[] { "1", "2", "3", "4" }, result);
        }
    }
}