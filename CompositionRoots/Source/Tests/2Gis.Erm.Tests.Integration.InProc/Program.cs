using System;
using System.Linq;
using System.Reflection;
using System.Text;

using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config;
using DoubleGis.Erm.Platform.Common.Logging.SystemInfo;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Tests.Integration.InProc.Settings;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var settings = new TestAPIInProcOperationsSettings(BusinessModels.Supported);
            var environmentSettings = settings.AsSettings<IEnvironmentSettings>();

            var loggerContextEntryProviders =
                new ILoggerContextEntryProvider[]
                    {
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.Environment, environmentSettings.EnvironmentName),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPoint, environmentSettings.EntryPointName),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new LoggerContextSelfHostedEntryProvider(LoggerContextKeys.Required.UserAccount)
                    };

            var loggerContextManager = new LoggerContextManager(loggerContextEntryProviders);

            var logger = Log4NetLoggerBuilder.Use
                                             .Console
                                             .File(environmentSettings.EnvironmentName + "_" + environmentSettings.EntryPointName)
                                             .Build;

            logger.Info("Configuring composition root " + Assembly.GetExecutingAssembly().GetName().Name);
            logger.Info(new StringBuilder()
                            .AppendLine("Runtime description:")
                            .AppendLine("TargetEnvironment: " + environmentSettings.Type)
                            .AppendLine("TargetEnvironmentName: " + environmentSettings.EnvironmentName)
                            .ToString());

            TestResultsSet testResults = null;
            ITestRunner testRunner;
            if (TestSuiteBuilder.TryBuildSuite(settings, logger, loggerContextManager, out testRunner))
            {
                logger.Info("Running test suite");
                testResults = testRunner.Run();
                logger.Info(testResults.ToReport());
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
                    .AppendLine(string.Empty)
                    .AppendFormat("Test suite {0} finished. Results:\n", Assembly.GetExecutingAssembly().GetName().Name)
                    .AppendLine("\tAll=" + testResults.TotalCount)
                    .AppendLine("\tSucceeded=" + testResults.Succeeded.Count)
                    .AppendLine("\tFailed=" + testResults.Failed.Count)
                    .AppendLine("\tIgnored=" + testResults.Ignored.Count)
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

                if (testResults.Ignored.Any())
                {
                    sb.AppendLine("\tIgnored:");
                    testResults.Ignored.Aggregate(sb, (builder, pair) => builder.AppendLine("\t  " + pair.Key.GetType().Name));
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
    }
}
