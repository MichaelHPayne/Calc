using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Calc.Core.Interfaces;
using Calc.Infrastructure;

namespace Calc.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddTransient<IStringCalculator, StringCalculator>())
                .Build();

            var calculator = host.Services.GetRequiredService<IStringCalculator>();

            // Example usage
            Console.WriteLine(calculator.Add(""));
        }
    }
}