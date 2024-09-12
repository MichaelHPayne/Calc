using Xunit;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Infrastructure.Factories;
using Calc.Core.Interfaces;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Calc.Infrastructure.Tests
{
    public class InputParserTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly InputParser _parser;
        private static readonly string[] EmptyStringArray = Array.Empty<string>();

        public InputParserTests()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _parser = _serviceProvider.GetRequiredService<InputParser>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var mockDefaultStrategy = new Mock<IDefaultDelimiterStrategy>();
            var mockSingleCharStrategy = new Mock<ISingleCharCustomDelimiterStrategy>();
            var mockDelimiterFactory = new Mock<IDelimiterStrategyFactory>();

            mockSingleCharStrategy.Setup(m => m.WithDelimiter(It.IsAny<string>()))
                .Returns(mockSingleCharStrategy.Object);

            mockDelimiterFactory.Setup(m => m.CreateStrategy(It.IsAny<string>()))
                .Returns((string input) => input.StartsWith("//") ? mockSingleCharStrategy.Object : mockDefaultStrategy.Object);

            services.AddSingleton(mockDefaultStrategy.Object);
            services.AddSingleton(mockSingleCharStrategy.Object);
            services.AddSingleton(mockDelimiterFactory.Object);
            services.AddTransient<InputParser>();
        }

        public void Dispose()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public static IEnumerable<object[]> ParseTestData()
        {
            yield return new object[] { "1,2,3", new[] { "1", "2", "3" } };
            yield return new object[] { "1\n2,3", new[] { "1", "2", "3" } };
            yield return new object[] { "//;\n1;2;3", new[] { "1", "2", "3" } };
            yield return new object[] { "1,2\n3", new[] { "1", "2", "3" } };
            yield return new object[] { "//,\n1,2,3", new[] { "1", "2", "3" } };
            yield return new object[] { "//#\n1#2\n3", new[] { "1", "2", "3" } };
        }

        [Theory]
        [MemberData(nameof(ParseTestData))]
        public void Parse_VariousInputFormats_ReturnsCorrectlySplitArray(string input, string[] expected)
        {
            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_NullOrEmptyString_ReturnsEmptyArray(string input)
        {
            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Empty(result);
        }
    }
}
