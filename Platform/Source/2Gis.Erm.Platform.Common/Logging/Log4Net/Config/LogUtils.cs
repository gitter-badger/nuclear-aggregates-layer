using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using log4net;
using log4net.Appender;
using log4net.Config;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config
{
    public static class LogUtils
    {
        public const string DefaultLogConfigFileName = "log4net.config";
        private const string EntryPointConfigLoggerName = "ConfigLogger";
        
        public static readonly CultureInfo DefaultLoggingCulture = new CultureInfo("ru-RU");

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
            ConfigureFromXml(logConfigFileFullPath, loggingConnectionString);

            // configure logger context
            var loggerContextManager = new LoggerContextManager(loggerContextEntryProviders);

            // dump configuration file to log
            var configLogger = GetLogger(EntryPointConfigLoggerName);
            var config = File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            configLogger.InfoEx(config);

            return loggerContextManager;
        }

        private static ICommonLog GetLogger(string logger)
        {
            return new Log4NetCommonLog(logger, DefaultLoggingCulture);
        }

        private static void ConfigureFromXml(string filePath, string adoNetConnectionString)
        {
            XmlConfigurator.Configure(new FileInfo(filePath));
            var repository = LogManager.GetRepository();
            if (repository == null)
            {
                Debugger.Log(1, "Log4Net", "Method: ConfigureFromXml. Error: repository is null" + Environment.NewLine);
                return;
            }

            if (!repository.Configured)
            {
                Debugger.Log(1, "Log4Net", "Method: ConfigureFromXml. Error: repository is not configured" + Environment.NewLine);
                return;
            }

            if (string.IsNullOrWhiteSpace(adoNetConnectionString))
            {
                return;
            }

            var adonetAppenders = repository.GetAppenders().OfType<AdoNetAppender>();
            foreach (var adoNetAppender in adonetAppenders)
            {
                adoNetAppender.ConnectionString = adoNetConnectionString;
                adoNetAppender.ActivateOptions();
            }
        }
    }
}
