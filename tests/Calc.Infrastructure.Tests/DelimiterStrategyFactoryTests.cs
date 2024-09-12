using Xunit;
using Calc.Core.Interfaces;
using Calc.Infrastructure.Factories;
using Calc.Infrastructure.DelimiterStrategies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Calc.Infrastructure.Tests
{
    public class DelimiterStrategyFactoryTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDelimiterStrategyFactory _factory;

        public DelimiterStrategyFactoryTests()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _factory = _serviceProvider.GetRequiredService<IDelimiterStrategyFactory>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDelimiterStrategyFactory, DelimiterStrategyFactory>();
            services.AddTransient<IDefaultDelimiterStrategy, DefaultDelimiterStrategy>();
            services.AddTransient<ISingleCharCustomDelimiterStrategy, SingleCharCustomDelimiterStrategy>();
        }

        public void Dispose()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public static IEnumerable<object[]> CreateStrategyTestData()
        {
            yield return new object[] { "1,2,3", typeof(DefaultDelimiterStrategy) };
            yield return new object[] { "//;\n1;2;3", typeof(SingleCharCustomDelimiterStrategy) };
            yield return new object[] { "1\n2\n3", typeof(DefaultDelimiterStrategy) };
        }

        [Theory]
        [MemberData(nameof(CreateStrategyTestData))]
        public void CreateStrategy_ReturnsCorrectStrategyType(string input, Type expectedType)
        {
            var result = _factory.CreateStrategy(input);
            Assert.True(expectedType.IsInstanceOfType(result),
                $"CreateStrategy failed to return the correct strategy type.\n" +
                $"Input: {input}\n" +
                $"Expected Type: {expectedType.Name}\n" +
                $"Actual Type: {result.GetType().Name}");
        }

        [Fact]
        public void CreateStrategy_SingleCharDelimiter_SetsSingleCharDelimiter()
        {
            var strategy = _factory.CreateStrategy("//;\n1;2;3");
            
            Assert.True(strategy is SingleCharCustomDelimiterStrategy,
                $"CreateStrategy failed to return a SingleCharCustomDelimiterStrategy.\n" +
                $"Input: //;\\n1;2;3\n" +
                $"Actual Type: {strategy?.GetType().Name ?? "null"}");

            if (strategy is SingleCharCustomDelimiterStrategy singleCharStrategy)
            {
                var splitResult = singleCharStrategy.Split("1;2;3");
                var expected = new[] { "1", "2", "3" };
                Assert.True(expected.SequenceEqual(splitResult),
                    $"SingleCharCustomDelimiterStrategy failed to split correctly.\n" +
                    $"Input: 1;2;3\n" +
                    $"Expected: [{string.Join(", ", expected)}]\n" +
                    $"Actual: [{string.Join(", ", splitResult)}]");
            }
            else
            {
                Assert.Fail("Strategy is not SingleCharCustomDelimiterStrategy, cannot proceed with split test.");
            }
        }
    }
}
