using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Calc.Core.Interfaces;
using Calc.Infrastructure;
using System;

namespace Calc.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var calculator = host.Services.GetRequiredService<IStringCalculator>();

            Console.WriteLine("String Calculator Demo");
            Console.WriteLine("---------------------");

            TestCalculator(calculator, "");
            TestCalculator(calculator, "1");
            TestCalculator(calculator, "20");
            TestCalculator(calculator, "1,5000");
            TestCalculator(calculator, "4,-3");
            TestCalculator(calculator, "5,");
            TestCalculator(calculator, "5,tytyt");

            try
            {
                TestCalculator(calculator, "1,2,3");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Input: \"1,2,3\" - Exception: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IStringCalculator, StringCalculator>();
                });

        private static void TestCalculator(IStringCalculator calculator, string input)
        {
            try
            {
                int result = calculator.Add(input);
                Console.WriteLine($"Input: \"{input}\" - Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Input: \"{input}\" - Exception: {ex.Message}");
            }
        }
    }
}