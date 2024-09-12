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
            Assert.IsType(expectedType, result);
        }

        [Fact]
        public void CreateStrategy_SingleCharDelimiter_SetsSingleCharDelimiter()
        {
            var result = _factory.CreateStrategy("//;\n1;2;3") as SingleCharCustomDelimiterStrategy;
            Assert.NotNull(result);
            Assert.Equal(new[] { "1", "2", "3" }, result.Split("1;2;3"));
        }
    }
}
