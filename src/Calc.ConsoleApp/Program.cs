using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
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

            // Requirement 1: Basic addition with up to 2 numbers
            TestCalculator(calculator, "");
            TestCalculator(calculator, "1");
            TestCalculator(calculator, "1,2");
            TestCalculator(calculator, "5,tytyt");

            // Requirement 2: Remove 2-number constraint
            TestCalculator(calculator, "1,2,3");
            TestCalculator(calculator, "1,2,3,4,5");
            TestCalculator(calculator, "1,2,3,4,5,6,7,8,9,10,11,12");

            // Test with many numbers
            string manyNumbers = string.Join(",", Enumerable.Range(1, 100));
            TestCalculator(calculator, manyNumbers);

            // Requirement 3: Support newline as delimiter
            TestCalculator(calculator, "1\n2,3");
            TestCalculator(calculator, "1,2\n3");
            TestCalculator(calculator, "1\n2\n3");

            // Test with mixed delimiters
            string mixedInput = string.Join("\n", Enumerable.Range(1, 5)) + "," + string.Join(",", Enumerable.Range(6, 5));
            TestCalculator(calculator, mixedInput);

            // Requirement 4: Deny negative numbers
            TestCalculator(calculator, "-1");
            TestCalculator(calculator, "1,-2,3");
            TestCalculator(calculator, "-1,-2,-3");
            TestCalculator(calculator, "1,2,-3,4,-5,6");
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