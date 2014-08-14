using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.TaskService.Schedulers;
using DoubleGis.Erm.TaskService.DI;
using DoubleGis.Erm.TaskService.Settings;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.TaskService
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            var isDebuggerMode = IsDebuggerMode(args);
            if (isDebuggerMode && !Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            var settingsContainer = new TaskServiceAppSettings(BusinessModels.Supported);

            var loggerContextEntryProviders =
                new ILoggerContextEntryProvider[] 
                {
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.SessionId, Guid.Empty.ToString()),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.UserName, "ERM Windows Service"),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.UserIP, null),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.UserBrowser, null),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.SeanceCode, Guid.NewGuid().ToString()),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.Module, settingsContainer.AsSettings<IEnvironmentSettings>().EntryPointName)
                };

            LogUtils.InitializeLoggingInfrastructure(
                    settingsContainer.AsSettings<IConnectionStringSettings>().LoggingConnectionString(),
                    LogUtils.DefaultLogConfigFileFullPath,
                    loggerContextEntryProviders);

            IUnityContainer container = null;
            try
            {
                container = Bootstrapper.ConfigureUnity(settingsContainer);
                var schedulerManager = container.Resolve<ISchedulerManager>();

                if (IsConsoleMode(args))
                {
                    schedulerManager.Start();

                    Console.WriteLine("ERM сервис запущен.");
                    Console.WriteLine("Нажмите ENTER для останова...");

                    Console.ReadLine();

                    Console.WriteLine("ERM сервис останавливается...");

                    schedulerManager.Stop();

                    Console.WriteLine("ERM сервис остановлен. Нажмите ENTER для выхода...");
                    Console.ReadLine();
                }
                else
                {
                    using (var ermNtService = new ErmNtService(schedulerManager))
                    {
                        ServiceBase.Run(ermNtService);
                    }
                }
            }
            finally
            {
                if (container != null)
                {
                    container.Dispose();
                }
            }
        }

        private static bool IsConsoleMode(IEnumerable<string> args)
        {
            return args.Any(x => x.LastIndexOf("console", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool IsDebuggerMode(IEnumerable<string> args)
        {
            return args.Any(x => x.LastIndexOf("debug", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}