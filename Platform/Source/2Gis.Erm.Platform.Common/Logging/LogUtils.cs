using System;
using System.Collections.Generic;
using System.IO;

namespace DoubleGis.Erm.Platform.Common.Logging
{
    public static class LogUtils
    {
        private const string EntryPointConfigLoggerName = "ConfigLogger";
        public const string DefaultLogConfigFileName = "log4net.config";

        public static string DefaultLogConfigFileFullPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultLogConfigFileName); }
        }

        public static ILoggerContextManager InitializeLoggingInfrastructure(
            string loggingConnectionString,
            string logConfigFileFullPath,
            IEnumerable<ILoggerContextEntryProvider> loggerContextEntryProviders)
        {
            // configure log4net
            Log4NetImpl.ConfigureFromXml(logConfigFileFullPath, loggingConnectionString);

            // configure logger context
            var loggerContextManager = new LoggerContextManager(loggerContextEntryProviders);

            // dump configuration file to log
            var configLogger = Log4NetImpl.GetLogger(EntryPointConfigLoggerName);
            var config = File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            configLogger.InfoEx(config);

            return loggerContextManager;
        }
    }
}
