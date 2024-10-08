﻿using Microsoft.Extensions.DependencyInjection;
using Calc.Core.Interfaces;
using Calc.Core.Exceptions;
using Calc.Infrastructure;
using Calc.Infrastructure.DelimiterStrategies;
using Calc.Infrastructure.Factories;
using System;

namespace Calc.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var calculator = serviceProvider.GetRequiredService<IStringCalculator>();

                Console.WriteLine("String Calculator Demo");
                Console.WriteLine("---------------------");

                // Run all the tests
                RunTests(calculator);
            }

            // The program will exit automatically after this point
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IStringCalculator, StringCalculator>();
            services.AddTransient<IDelimiterStrategy, DefaultDelimiterStrategy>();
            services.AddTransient<IDelimiterStrategy, CustomDelimiterStrategy>();
            services.AddTransient<IDelimiterStrategyFactory, DelimiterStrategyFactory>();
            services.AddTransient<InputParser>();
            services.AddTransient<IDefaultDelimiterStrategy, DefaultDelimiterStrategy>();
            services.AddTransient<ISingleCharCustomDelimiterStrategy, SingleCharCustomDelimiterStrategy>();
        }

        private static void RunTests(IStringCalculator calculator)
        {
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

            // Custom delimiter tests
            Console.WriteLine("\nCustom Delimiter Tests");
            Console.WriteLine("---------------------");
            TestCalculator(calculator, "//;\n1;2;3");
            TestCalculator(calculator, "//#\n2#5");
            TestCalculator(calculator, "//,\n2,ff,100");
            TestCalculator(calculator, "//,\n1,2,3");
            TestCalculator(calculator, "//,1,2,3");
            TestCalculator(calculator, "//\n1,2,3");
            TestCalculator(calculator, "//#\n1#2\n3");

            // Requirement 7: Multiple custom delimiters of any length
            Console.WriteLine("\nRequirement 7: Multiple custom delimiters of any length");
            Console.WriteLine("-----------------------------------------------------");
            TestCalculator(calculator, "//[*][!!][r9r]\n11r9r22*hh*33!!44");
            TestCalculator(calculator, "//[###][@@][&&&]\n1###2@@3&&&4");
            TestCalculator(calculator, "//[;;][**][???]\n1;;2**3???4");
            TestCalculator(calculator, "//[ab][cd][ef]\n1ab2cd3ef4");
            TestCalculator(calculator, "//[123][456][789]\n1123245635678974");
            TestCalculator(calculator, "//[**][++][..]\n100**200++300..400");
            TestCalculator(calculator, "//[delimiter][separator][divider]\n1delimiter2separator3divider4");
            TestCalculator(calculator, "//[!@#][#@!][***]\n10!@#20#@!30***40");
            TestCalculator(calculator, "//[==][!=][<>]\n5==10!=15<>20");
            TestCalculator(calculator, "//[.][..][...]\n1.2..3...4");

            // New tests for Requirement 8: Multiple delimiters of any length
            Console.WriteLine("\nRequirement 8: Multiple delimiters of any length");
            Console.WriteLine("-----------------------------------------------");
            TestCalculator(calculator, "//[*][!!][r9r]\n11r9r22*hh*33!!44");
            TestCalculator(calculator, "//[delimiter1][delimiter2][delimiter3]\n1delimiter12delimiter23delimiter34");
            TestCalculator(calculator, "//[**][+++][====]\n1**2+++3====4");
            TestCalculator(calculator, "//[a][bb][ccc]\n1a2bb3ccc4");
            TestCalculator(calculator, "//[123][4567][89]\n1123245673894");
            TestCalculator(calculator, "//[.][,][$]\n1.2,3$4");
            TestCalculator(calculator, "//[||][&&][##]\n1||2&&3##4");
            
            // Test backward compatibility
            Console.WriteLine("\nBackward Compatibility Tests");
            Console.WriteLine("----------------------------");
            TestCalculator(calculator, "1,2,3");
            TestCalculator(calculator, "1\n2,3");
            TestCalculator(calculator, "//;\n1;2;3");
            TestCalculator(calculator, "//[***]\n1***2***3");

            // Test example case in requirement
            Console.WriteLine("\nexample cases in requirement");

            Console.WriteLine("\nRequirement #2: Remove the maximum constraint for numbers");
            TestCalculator(calculator, "1,2,3,4,5,6,7,8,9,10,11,12"); // returns 78

            Console.WriteLine("\nRequirement #3: Support a newline character as an alternative delimiter");
            TestCalculator(calculator, "1\n2,3"); // returns 6

            Console.WriteLine("\nRequirement #4: Deny negative numbers by throwing an exception that includes all of the negative numbersprovided");
            TestCalculator(calculator, "2,-3,6"); // Throws exception

            Console.WriteLine("\nRequirement #5: Make any value greater than 1000 an invalid number");
            TestCalculator(calculator, "2,1005,6"); // returns 8

            Console.WriteLine("\nRequirement #6: Support 1 custom delimiter of a single character using the format: //{delimiter}\n{numbers}");
            TestCalculator(calculator, "//#\n2#5"); // returns 7
            TestCalculator(calculator, "//,\n2,ff,100"); // returns 102

            Console.WriteLine("\nRequirement #7: Support 1 custom delimiter of any length using the format: //[{delimiter}]\n{numbers}");
            TestCalculator(calculator, "//[***]\n11***22***33"); // returns 66

            Console.WriteLine("\nRequirement #8: Support multiple delimiters of any length using the format: //[{delimiter1}][{delimiter2}]...\n{numbers}");
            TestCalculator(calculator, "//[*][!!][r9r]\n11r9r22*hh*33!!44");
        }

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