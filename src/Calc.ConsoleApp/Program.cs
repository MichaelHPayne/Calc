using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Infrastructure.Factories;
using System;
using System.Threading.Tasks;

namespace Calc.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var calculator = provider.GetRequiredService<IStringCalculator>();

            Console.WriteLine("String Calculator Demo");
            Console.WriteLine("---------------------");

            // Previous requirements
            TestCalculator(calculator, "");
            TestCalculator(calculator, "1");
            TestCalculator(calculator, "1,2");
            TestCalculator(calculator, "5,tytyt");
            TestCalculator(calculator, "1,2,3");
            TestCalculator(calculator, "1,2,3,4,5");
            TestCalculator(calculator, "1\n2,3");
            TestCalculator(calculator, "1,2\n3");
            TestCalculator(calculator, "-1");
            TestCalculator(calculator, "1,-2,3");

            // Requirement 5: Make any value greater than 1000 an invalid number
            Console.WriteLine("\nRequirement 5: Numbers greater than 1000 are ignored");
            Console.WriteLine("---------------------------------------------------");
            TestCalculator(calculator, "2,1001,6");
            TestCalculator(calculator, "1000,2,3");
            TestCalculator(calculator, "999,1001,2");
            TestCalculator(calculator, "1,2,3,1001,4,1002,5");
            TestCalculator(calculator, "5,1001");
            TestCalculator(calculator, "1001,2000,3000");
            TestCalculator(calculator, "1,1001,2,1002,3,1003,4");

            // New tests for custom delimiters
            Console.WriteLine("\nCustom Delimiter Tests");
            Console.WriteLine("---------------------");
            TestCalculator(calculator, "//;\n1;2;3");
            TestCalculator(calculator, "//#\n2#5");
            TestCalculator(calculator, "//,\n2,ff,100");
            TestCalculator(calculator, "//\n1,2,3");
            TestCalculator(calculator, "//#\n1#2\n3");


            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IStringCalculator, StringCalculator>();
                    services.AddTransient<IDelimiterStrategy, DefaultDelimiterStrategy>();
                    services.AddTransient<IDelimiterStrategy, CustomDelimiterStrategy>();
                    services.AddTransient<IDelimiterStrategyFactory, DelimiterStrategyFactory>();
                    services.AddTransient<InputParser>();
                    
                    // Add this line to register IDefaultDelimiterStrategy
                    services.AddTransient<IDefaultDelimiterStrategy, DefaultDelimiterStrategy>();
                    services.AddTransient<ISingleCharCustomDelimiterStrategy, SingleCharCustomDelimiterStrategy>();
                });

        private static void TestCalculator(IStringCalculator calculator, string input)
        {
            try
            {
                int result = calculator.Add(input);
                Console.WriteLine($"Input: \"{input}\" - Result: {result}");
            }
            catch (NegativeNumberException ex)
            {
                Console.WriteLine($"Input: \"{input}\" - NegativeNumberException: {ex.Message}");
                Console.WriteLine($"Negative numbers found: {string.Join(", ", ex.NegativeNumbers)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Input: \"{input}\" - Exception: {ex.Message}");
            }
        }
    }
}