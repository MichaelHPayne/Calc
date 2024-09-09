using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Calc.Core.Interfaces;
using Calc.Infrastructure;
using System;
using System.Linq;

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
            TestCalculator(calculator, "1,2,3");
            TestCalculator(calculator, "1,2,3,4,5,6,7,8,9,10");
            
            // Test with 100 numbers
            string manyNumbers = string.Join(",", Enumerable.Range(1, 100));
            TestCalculator(calculator, manyNumbers);

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
                Console.WriteLine($"Input: \"{input.Substring(0, Math.Min(input.Length, 20))}{(input.Length > 20 ? "..." : "")}\" - Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Input: \"{input}\" - Exception: {ex.Message}");
            }
        }
    }
}