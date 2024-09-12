using Xunit;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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

        [Fact]
        public void DefaultDelimiterStrategy_SplitsCorrectly()
        {
            var strategy = _serviceProvider.GetRequiredService<IDefaultDelimiterStrategy>();
            var result = strategy.Split("1,2\n3");
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }

        [Fact]
        public void SingleCharCustomDelimiterStrategy_SplitsCorrectly()
        {
            var strategy = _serviceProvider.GetRequiredService<ISingleCharCustomDelimiterStrategy>();
            strategy.WithDelimiter(";");
            var result = strategy.Split("1;2;3");
            Assert.Equal(new[] { "1", "2", "3" }, result);
        }
    }
}
