using System;
using System.Linq;
using System.Reflection;
using System.Text;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Tests.Integration.InProc.Settings;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace DoubleGis.Erm.Tests.Integration.InProc
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var settings = new TestAPIInProcOperationsSettings();
            var logger = CreateLogger();

            logger.InfoEx("Configuring composition root " + Assembly.GetExecutingAssembly().GetName().Name);

            logger.InfoEx(new StringBuilder()
                            .AppendLine("Runtime description:")
                            .AppendLine("TargetEnvironment: " + settings.TargetEnvironment)
                            .AppendLine("TargetEnvironmentName: " + settings.TargetEnvironmentName)
                            .ToString());

            TestResultsSet testResults = null;
            ITestRunner testRunner;
            if (TestSuiteBuilder.TryBuildSuite(settings, logger, out testRunner))
            {
                logger.InfoEx("Running test suite");
                testResults = testRunner.Run();
                logger.InfoEx(testResults.ToReport());
            }

            if (!args.Any())
            {
                Console.WriteLine(@"Press ""ENTER"" to finish execution ...");
                Console.ReadLine();
            }

            Environment.ExitCode =
                testResults == null
                || testResults.Failed.Any()
                || testResults.Unhandled.Any() ? 1 : 0;
        }

        private static string ToReport(this TestResultsSet testResults)
        {
            var sb = 
                new StringBuilder()
                    .AppendLine("")
                    .AppendFormat("Test suite {0} finished. Results:\n", Assembly.GetExecutingAssembly().GetName().Name)
                    .AppendLine("\tAll=" + testResults.TotalCount)
                    .AppendLine("\tSucceeded=" + testResults.Succeeded.Count)
                    .AppendLine("\tFailed=" + testResults.Failed.Count)
                    .AppendLine("\tUnhandled=" + testResults.Unhandled.Count)
                    .AppendLine("\tUnresolved=" + testResults.Unresolved.Count)
                    .AppendLine("Passed: " + (testResults.Passed ? "YES" : "NO"));

            if (!testResults.Passed)
            {
                sb.AppendLine("Error Details:");
                if (testResults.Failed.Any())
                {
                    sb.AppendLine("\tFailed:");
                    testResults.Failed.Aggregate(sb, (builder, pair) => builder.AppendLine("\t  " + pair.Key.GetType().Name));
                }

                if (testResults.Unhandled.Any())
                {
                    sb.AppendLine("\tUnhandled:");
                    testResults.Unhandled.Aggregate(sb, (builder, pair) => builder.AppendLine("\t  " + pair.Key.GetType().Name));
                }

                if (testResults.Unresolved.Any())
                {
                    sb.AppendLine("\tUnresolved:");
                    testResults.Unresolved.Aggregate(sb, (builder, pair) => builder.AppendLine("\t  " + pair.Key.Name));
                }
            }

            return sb.ToString();
        }

        private static ICommonLog CreateLogger()
        {
            var patternLayout =
                new PatternLayout
                {
                    ConversionPattern = "%date [%thread] %-5level %message %newline %exception"
                };

            patternLayout.ActivateOptions();

            var consoleAppender = new ConsoleAppender{ Name = "Console", Layout = patternLayout, Threshold = Level.All };
            consoleAppender.ActivateOptions();

            var logger = LogManager.GetLogger(LoggerConstants.Erm);
            var coreLogger = (Logger)logger.Logger;
            coreLogger.AddAppender(consoleAppender);
            coreLogger.Hierarchy.Configured = true;

            return Log4NetImpl.GetLogger(LoggerConstants.Erm);
        }
    }
}
