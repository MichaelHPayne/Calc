using Xunit;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Calc.Infrastructure.Tests
{
    public class DelimiterStrategyTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        public DelimiterStrategyTests()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
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

        public static IEnumerable<object[]> DefaultDelimiterTestData()
        {
            yield return new object[] { "1,2\n3", new[] { "1", "2", "3" } };
            yield return new object[] { "4,5,6", new[] { "4", "5", "6" } };
            yield return new object[] { "7\n8\n9", new[] { "7", "8", "9" } };
        }

        [Theory]
        [MemberData(nameof(DefaultDelimiterTestData))]
        public void DefaultDelimiterStrategy_SplitsCorrectly(string input, string[] expected)
        {
            var strategy = _serviceProvider.GetRequiredService<IDefaultDelimiterStrategy>();
            var result = strategy.Split(input);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> SingleCharCustomDelimiterTestData()
        {
            yield return new object[] { "1;2;3", new[] { "1", "2", "3" } };
            yield return new object[] { "4;5;6", new[] { "4", "5", "6" } };
            yield return new object[] { "7;8;9", new[] { "7", "8", "9" } };
        }

        [Theory]
        [MemberData(nameof(SingleCharCustomDelimiterTestData))]
        public void SingleCharCustomDelimiterStrategy_SplitsCorrectly(string input, string[] expected)
        {
            var strategy = _serviceProvider.GetRequiredService<ISingleCharCustomDelimiterStrategy>();
            strategy.WithDelimiter(";");
            var result = strategy.Split(input);
            Assert.Equal(expected, result);
        }
    }
}
