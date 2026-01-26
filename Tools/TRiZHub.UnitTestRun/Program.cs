#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TRiZHub.BL.Test;
using TRiZHub.BL.Test.Providers;

#endregion

namespace TRiZHub.UnitTestRun
{
    internal class Program
    {
        public static List<KeyValuePair<KeyValuePair<bool, string>, string>> TestCases;

        private static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            Console.WriteLine("Starting Test Cases...");
            Console.WriteLine();
            TestCases = new List<KeyValuePair<KeyValuePair<bool, string>, string>>();

            RunTestCasesForTestProvider(typeof (SecurityProviderTest));
            RunTestCasesForTestProvider(typeof (SettingsProviderTest));
         
            watch.Stop();
            var elapsedS = watch.Elapsed.Seconds;
            var elapsedMs = watch.Elapsed.Milliseconds;
            var elapsedM = watch.Elapsed.Minutes;
            var elapsedH = watch.Elapsed.Hours;
            Console.WriteLine();
            Console.WriteLine("Test Cases Run Complete...");
            Console.WriteLine("##########################");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: " + TestCases.Count(a => a.Key.Key));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed: " + TestCases.Count(a => !a.Key.Key));

            if (TestCases.Any(a => !a.Key.Key))
            {
                Console.WriteLine("Failed Test Cases");
                Console.WriteLine("-----");
                foreach (var testCaseFailed in TestCases.Where(a => !a.Key.Key))
                {
                    Console.WriteLine(testCaseFailed.Value);
                    Console.WriteLine("\t" + testCaseFailed.Key.Value);
                    Console.WriteLine();
                }
                Console.WriteLine("-----");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Time Excecuted: " +
                              string.Format("{0}h:{1}m:{2}s:{3}ms", elapsedH, elapsedM, elapsedS, elapsedMs));
            Console.WriteLine();
            Console.WriteLine("Closing in: ");
            for (var i = 5; i > 0; i--)
            {
                Thread.Sleep(1000);
                Console.Write(i + "... ");
            }
            if (TestCases.Any(a => !a.Key.Key))
                Environment.Exit(-1);
            Environment.Exit(0);
        }

        private static void RunTestCasesForTestProvider(Type type)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var method in type.GetMethods()
                .Where(a =>
                    a.CustomAttributes.Any(
                        b => b.AttributeType.FullName ==
                             "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute")))
            {
                ((ProviderTestBase) instance).TestInitialize();
                Console.WriteLine("###");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(method.DeclaringType.FullName);
                Console.WriteLine(method.Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("---");
                try
                {
                    method.Invoke(instance, null);
                    TestCases.Add(
                        new KeyValuePair<KeyValuePair<bool, string>, string>(
                            new KeyValuePair<bool, string>(true, "Success"),
                            string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name)));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("<COMPLETED>");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)
                {
                    TestCases.Add(
                        new KeyValuePair<KeyValuePair<bool, string>, string>(
                            new KeyValuePair<bool, string>(false, ex.InnerException.Message),
                            string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name)));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("<FAILED>");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine("---");
                Console.WriteLine();
                ((ProviderTestBase) instance).TestCleanup();
            }
        }
    }
}