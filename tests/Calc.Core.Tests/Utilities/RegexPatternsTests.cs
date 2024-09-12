using Xunit;
using Calc.Core.Utilities;
using System.Text.RegularExpressions;
using System;

namespace Calc.Core.Tests.Utilities
{
    public class RegexPatternsTests
    {
        [Theory]
        [InlineData("//[***]\n1***2***3", true)]
        [InlineData("//[*][%]\n1*2%3", true)]
        [InlineData("//[delimiter]\n1delimiter2delimiter3", true)]
        [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44", true)]
        [InlineData("//[*][!!!][r9r]\n11r9r22*33!!!44", true)]
        [InlineData("//[*][!!][;]\n11;22*33!!44", true)]
        [InlineData("1,2,3", false)]
        [InlineData("//;\n1;2;3", false)]
        [InlineData("1\n2\n3", false)]
        [InlineData("1,2,3,4,5", false)]
        [InlineData("1,2,3,4,5,6,7,8,9,10,11,12", false)]
        [InlineData("1\n2,3", false)]
        [InlineData("1,2\n3", false)]
        [InlineData("", false)]
        [InlineData("5,", false)]
        [InlineData("5,tytyt", false)]
        public void MultipleCustomDelimiter_ShouldMatchCorrectly(string input, bool expectedMatch)
        {
            var result = RegexPatterns.MultipleCustomDelimiter().IsMatch(input);
            Assert.Equal(expectedMatch, result);
        }

        [Theory]
        [InlineData("//;\n1;2;3", true)]
        [InlineData("//#\n1#2#3", true)]
        [InlineData("//,\n1,2,3", true)]
        [InlineData("1,2,3", false)]
        [InlineData("//[*]\n1*2*3", false)]
        [InlineData("1\n2\n3", false)]
        [InlineData("", false)]
        public void SingleCustomDelimiter_ShouldMatchCorrectly(string input, bool expectedMatch)
        {
            var result = RegexPatterns.SingleCustomDelimiter().IsMatch(input);
            Assert.Equal(expectedMatch, result);
        }

        [Theory]
        [InlineData("//[*][%]\n", true)]
        [InlineData("//[delimiter]\n", true)]
        [InlineData("//[*][!!][r9r]\n", true)]
        [InlineData("//;\n", false)]
        [InlineData("1,2,3", false)]
        public void MultipleCustomDelimiterFormat_ShouldMatchCorrectly(string input, bool expectedMatch)
        {
            var result = RegexPatterns.MultipleCustomDelimiterFormat().IsMatch(input);
            Assert.Equal(expectedMatch, result);
        }

        [Theory]
        [InlineData("[*][%]", new[] { "*", "%" })]
        [InlineData("[delimiter]", new[] { "delimiter" })]
        [InlineData("[*][!!][r9r]", new[] { "*", "!!", "r9r" })]
        public void ExtractMultipleDelimiters_ShouldExtractCorrectly(string input, string[] expectedDelimiters)
        {
            var matches = RegexPatterns.ExtractMultipleDelimiters().Matches(input);
            var extractedDelimiters = matches.Select(m => m.Groups[1].Value).ToArray();
            Assert.Equal(expectedDelimiters, extractedDelimiters);
        }

        [Theory]
        [InlineData("123", true)]
        [InlineData("0", true)]
        [InlineData("-123", false)]
        [InlineData("12.3", false)]
        [InlineData("a123", false)]
        [InlineData("123a", false)]
        [InlineData("", false)]
        public void FullyNumeric_ShouldMatchCorrectly(string input, bool expectedMatch)
        {
            var result = RegexPatterns.FullyNumeric().IsMatch(input);
            Assert.Equal(expectedMatch, result);
        }

        [Theory]
        [InlineData("//[*][%]\n1*2%3", "[*][%]", "1*2%3")]
        [InlineData("//[**][%%]\n1**2%%3", "[**][%%]", "1**2%%3")]
        [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44", "[*][!!][r9r]", "11r9r22*hh*33!!44")]
        [InlineData("//[delimiter]\n1delimiter2delimiter3", "[delimiter]", "1delimiter2delimiter3")]
        [InlineData("//[*]\n1*2*3", "[*]", "1*2*3")]
        [InlineData("//[*][%][&]\n1*2%3&4", "[*][%][&]", "1*2%3&4")]
        public void MultipleCustomDelimiter_ShouldCaptureCorrectGroups(string input, string expectedDelimiters, string expectedNumbers)
        {
            var match = RegexPatterns.MultipleCustomDelimiter().Match(input);
            Assert.True(match.Success, $"Regex did not match for input: {input}");
            Assert.Equal(expectedDelimiters, match.Groups[1].Value);
            Assert.Equal(expectedNumbers, match.Groups[2].Value);

            // Debug information
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Match Success: {match.Success}");
            Console.WriteLine($"Group 1 (Delimiters): {match.Groups[1].Value}");
            Console.WriteLine($"Group 2 (Numbers): {match.Groups[2].Value}");
            Console.WriteLine(new string('-', 50));
        }

        [Fact]
        public void SingleCustomDelimiter_ShouldCaptureCorrectGroups()
        {
            var input = "//;\n1;2;3";
            var match = RegexPatterns.SingleCustomDelimiter().Match(input);
            Assert.True(match.Success);
            Assert.Equal(";", match.Groups[1].Value);
            Assert.Equal("1;2;3", match.Groups[2].Value);
        }

        [Theory]
        // [InlineData("//[]\n1,2,3", true, "", "1,2,3")]
        // [InlineData("//[][]\n1,2,3", true, "[]", "1,2,3")]
        // [InlineData("//[*][]\n1*2,3", true, "[*][]", "1*2,3")]
        [InlineData("//[][*]\n1,2*3", true, "[][*]", "1,2*3")]
        // [InlineData("//[]\n", true, "", "")]
        public void MultipleCustomDelimiter_ShouldHandleEmptyDelimiters(string input, bool expectedMatch, string expectedDelimiters, string expectedNumbers)
        {
            var match = RegexPatterns.MultipleCustomDelimiter().Match(input);
            Assert.Equal(expectedMatch, match.Success);
            if (match.Success)
            {
                Assert.Equal(expectedDelimiters, match.Groups[1].Value);
                Assert.Equal(expectedNumbers, match.Groups[2].Value);
            }

            // Debug information
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Match Success: {match.Success}");
            Console.WriteLine($"Group 1 (Delimiters): {match.Groups[1].Value}");
            Console.WriteLine($"Group 2 (Numbers): {match.Groups[2].Value}");
            Console.WriteLine(new string('-', 50));
        }

        [Theory]
        [InlineData("[]", new string[] { "" })]
        [InlineData("[*][]", new string[] { "*", "" })]
        [InlineData("[][*]", new string[] { "", "*" })]
        [InlineData("[*][][][%]", new string[] { "*", "", "", "%" })]
        public void ExtractMultipleDelimiters_ShouldHandleEmptyDelimiters(string input, string[] expectedDelimiters)
        {
            var matches = RegexPatterns.ExtractMultipleDelimiters().Matches(input);
            var extractedDelimiters = matches.Select(m => m.Groups[1].Value).ToArray();
            Assert.Equal(expectedDelimiters, extractedDelimiters);

            // Debug information
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Extracted Delimiters: {string.Join(", ", extractedDelimiters)}");
            Console.WriteLine(new string('-', 50));
        }

        // new tests for requirement #8
        [Theory]
        [InlineData("//[*][%]\n1*2%3", true)]
        [InlineData("//[**][%%]\n1**2%%3", true)]
        [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44", true)]
        [InlineData("//[delimiter]\n1delimiter2delimiter3", true)]
        [InlineData("//[*]\n1*2*3", true)]
        [InlineData("//[*][%][&]\n1*2%3&4", true)]
        [InlineData("//[*][!!]11*22!!33", false)]  // Missing newline
        [InlineData("//[]\n1,2,3", false)]  // Empty delimiter
        // [InlineData("//[][]\n1,2,3", false)]  // Empty delimiters
        // [InlineData("//[*][]\n1*2,3", false)]  // One empty delimiter
        [InlineData("//\n1,2,3", false)]  // No delimiters
        [InlineData("1,2,3", false)]  // No delimiter definition
        [InlineData("//[*]\n", false)]  // No numbers after newline
        // [InlineData("//[*][!!]\n", true)]  // New test case for empty input after newline with fewer delimiters
        // [InlineData("//[delimiter]\n", true)]  // New test case for empty input after newline with single delimiter
        public void ValidMultipleCustomDelimiterFormat_ShouldMatchCorrectly(string input, bool expectedMatch)
        {
            var result = RegexPatterns.ValidMultipleCustomDelimiterFormat().IsMatch(input);
            Assert.Equal(expectedMatch, result);
        }

        // New test to ensure non-empty delimiters
        [Theory]
        [InlineData("//[*][%]\n1*2%3", new[] { "*", "%" })]
        [InlineData("//[**][%%]\n1**2%%3", new[] { "**", "%%" })]
        [InlineData("//[delimiter]\n1delimiter2", new[] { "delimiter" })]
        [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44", new[] { "*", "!!", "r9r" })]
        public void ExtractMultipleDelimiters_ShouldExtractNonEmptyDelimiters(string input, string[] expectedDelimiters)
        {
            var delimitersPart = input.Substring(2, input.IndexOf('\n') - 2);
            var matches = RegexPatterns.ExtractMultipleDelimiters().Matches(delimitersPart);
            var extractedDelimiters = matches.Select(m => m.Groups[1].Value).ToArray();
            Assert.Equal(expectedDelimiters, extractedDelimiters);
            Assert.All(extractedDelimiters, d => Assert.False(string.IsNullOrEmpty(d)));
        }

        // Helper method to debug regex matches
        private void DebugRegexMatch(Regex regex, string input)
        {
            var match = regex.Match(input);
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Match Success: {match.Success}");
            for (int i = 0; i < match.Groups.Count; i++)
            {
                Console.WriteLine($"Group {i}: {match.Groups[i].Value}");
            }
            Console.WriteLine(new string('-', 50));
        }
    }
}