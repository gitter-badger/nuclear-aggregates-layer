using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config
{
    public sealed class Log4NetLoggerBuilder
    {
        public const string DefaultLogConfigFileName = "log4net.config";
        public const string LoggingHierarchyName = "Erm";

        private readonly CultureInfo _loggingCulture = new CultureInfo("ru-RU");

        private readonly PatternLayout _localPatternLayout = new PatternLayout
        {
            ConversionPattern = "%date [%thread] %-5level %message %newline %exception"
        };

        private readonly PatternLayout _eventLogPatternLayout = new PatternLayout
        {
            ConversionPattern = "%date [%thread] %-5level%newline %message%newline%newline %exception"
        };

        private readonly IDictionary<Type, List<IAppender>> _loggerAppendersMap = new Dictionary<Type, List<IAppender>>();
        private string _xmlConfigFullPath;
        private string _dbAppenderConnectionString;

        private Log4NetLoggerBuilder()
        {
            _localPatternLayout.ActivateOptions();
            _eventLogPatternLayout.ActivateOptions();
        }

        public static Log4NetLoggerBuilder Use 
        {
            get { return new Log4NetLoggerBuilder(); }
        }

        public ICommonLog Build
        {
            get { return Create(this); }
        }

        public Log4NetLoggerBuilder Console
        {
            get
            {
                AttachOnce<ConsoleAppender>(appender =>
                {
                    appender.Name = "Console";
                    appender.Layout = _localPatternLayout;
                    appender.Threshold = Level.All;
                });

                return this;
            }
        }
        
        public Log4NetLoggerBuilder Trace
        {
            get
            {
                AttachOnce<TraceAppender>(appender =>
                {
                    appender.Name = "Trace";
                    appender.Layout = _localPatternLayout;
                    appender.Threshold = Level.All;
                });

                return this;
            }
        }

        public Log4NetLoggerBuilder EventLog
        {
            get
            {
                var rangeFilter = new LevelRangeFilter { LevelMin = Level.Warn, LevelMax = Level.Fatal, Next = new DenyAllFilter() };

                AttachOnce<EventLogAppender>(appender =>
                {
                    appender.Name = "EventLog";
                    appender.ApplicationName = "ErmNtService";
                    appender.LogName = "2Gis Erm";
                    appender.AddFilter(rangeFilter);
                    appender.AddMapping(new EventLogAppender.Level2EventLogEntryType { Level = Level.Warn, EventLogEntryType = EventLogEntryType.Warning });
                    appender.AddMapping(new EventLogAppender.Level2EventLogEntryType { Level = Level.Error, EventLogEntryType = EventLogEntryType.Error });
                    appender.AddMapping(new EventLogAppender.Level2EventLogEntryType { Level = Level.Fatal, EventLogEntryType = EventLogEntryType.Error });
                    appender.Layout = _eventLogPatternLayout;
                });

                return this;
            }
        }
        
        public Log4NetLoggerBuilder DB(string connectionString)
        {
            _dbAppenderConnectionString = connectionString;
            AttachOnce<AdoNetAppender>(
            debugAppender =>
            {
                debugAppender.Name = "DebugDB";
                debugAppender.AddFilter(new LevelMatchFilter { LevelToMatch = Level.Debug, AcceptOnMatch = true, Next = new DenyAllFilter() });
                debugAppender.BufferSize = 5;
                debugAppender.Lossy = false;
                ApplySharedSettings(debugAppender);
            },
            infoAppender =>
            {
                infoAppender.Name = "InfoDB";
                infoAppender.AddFilter(
                    new LevelMatchFilter
                        {
                            LevelToMatch = Level.Error, 
                            AcceptOnMatch = false,
                            Next = new LevelMatchFilter
                            {
                                LevelToMatch = Level.Fatal,
                                AcceptOnMatch = false,
                                Next = new LevelMatchFilter
                                {
                                    LevelToMatch = Level.Debug,
                                    AcceptOnMatch = false
                                }
                            }
                        });
                infoAppender.BufferSize = 1;
                ApplySharedSettings(infoAppender);
            },
            errorAppender =>
            {
                errorAppender.Name = "ErrorDB";
                errorAppender.AddFilter(new LevelRangeFilter { LevelMin = Level.Error, LevelMax = Level.Fatal, Next = new DenyAllFilter() });
                errorAppender.BufferSize = 1;
                ApplySharedSettings(errorAppender);
            });

            return this;
        }

        public Log4NetLoggerBuilder File(string fileName, bool alwaysCreateNew = false)
        {
            var logfileFullPath = string.Format(
                    @"{0}\2GIS\InternalLogs\{1}.log", 
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
                    fileName);

            AttachOnce<RollingFileAppender>(appender =>
            {
                appender.Name = "File";
                appender.File = logfileFullPath;
                appender.AppendToFile = !alwaysCreateNew;
                appender.ImmediateFlush = true;
                appender.Layout = _localPatternLayout;
                appender.RollingStyle = RollingFileAppender.RollingMode.Size;
                appender.MaximumFileSize = "10MB";
                appender.StaticLogFileName = true;
            });

            return this;
        }

        public Log4NetLoggerBuilder DefaultXmlConfig
        {
            get
            {
                _xmlConfigFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultLogConfigFileName);
                return this;
            }
        }

        public Log4NetLoggerBuilder XmlConfig(string xmlConfigFullPath)
        {
            _xmlConfigFullPath = xmlConfigFullPath;
            return this;
        }

        public static implicit operator Log4NetCommonLog(Log4NetLoggerBuilder builder)
        {
            return Create(builder);
        }

        private static Log4NetCommonLog Create(Log4NetLoggerBuilder builder)
        {
            var loggersHierarchy = (Hierarchy)LogManager.GetRepository();
            if (!string.IsNullOrEmpty(builder._xmlConfigFullPath))
            {
                var targetXmlConfigFileInfo = new FileInfo(builder._xmlConfigFullPath);
                if (!targetXmlConfigFileInfo.Exists)
                {
                    throw new InvalidOperationException("Can't find specified logger config file " + builder._xmlConfigFullPath); 
                }
                
                XmlConfigurator.Configure(loggersHierarchy, targetXmlConfigFileInfo);
            }
            
            foreach (var appender in builder._loggerAppendersMap.SelectMany(x => x.Value))
            {
                loggersHierarchy.Root.AddAppender(appender);
            }

            var adonetAppenders = loggersHierarchy.GetAppenders().OfType<AdoNetAppender>();
            foreach (var adoNetAppender in adonetAppenders)
            {
                adoNetAppender.ConnectionString = builder._dbAppenderConnectionString;
                adoNetAppender.ActivateOptions();
            }

            loggersHierarchy.Configured = true;
            return new Log4NetCommonLog(LoggingHierarchyName, builder._loggingCulture);
        }

        private void ApplySharedSettings(AdoNetAppender adoNetAppender)
        {
            adoNetAppender.ConnectionType = "System.Data.SqlClient.SqlConnection";
            adoNetAppender.CommandText = "[dbo].[WriteLogMessageNew]";
            adoNetAppender.CommandType = CommandType.StoredProcedure;
            adoNetAppender.ReconnectOnError = true;
            adoNetAppender.UseTransactions = false;
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@MessageDate", DbType = DbType.DateTime2, Size = 36, Precision = 7, Layout = new RawTimeStampLayout() });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@MessageLevel", DbType = DbType.String, Size = 5, Layout = new RawPropertyLayout { Key = "%level" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@MessageText", DbType = DbType.String, Size = 8000, Layout = new RawPropertyLayout { Key = "%property{message}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@ExceptionData", DbType = DbType.String, Size = 8000, Layout = new RawPropertyLayout { Key = "%exception" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@Environment", DbType = DbType.String, Size = 100, Layout = new RawPropertyLayout { Key = "%property{environment}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@EntryPoint", DbType = DbType.String, Size = 100, Layout = new RawPropertyLayout { Key = "%property{entryPointName}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@EntryPointHost", DbType = DbType.String, Size = 250, Layout = new RawPropertyLayout { Key = "%property{entryPointHost}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@EntryPointInstanceId", DbType = DbType.Guid, Layout = new RawPropertyLayout { Key = "%property{entryPointInstanceId}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@UserAccount", DbType = DbType.String, Size = 100, Layout = new RawPropertyLayout { Key = "%property{userAccount}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@UserSession", DbType = DbType.String, Size = 100, Layout = new RawPropertyLayout { Key = "%property{userSession}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@UserAddress", DbType = DbType.String, Size = 100, Layout = new RawPropertyLayout { Key = "%property{userAddress}" } });
            adoNetAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@UserAgent", DbType = DbType.String, Size = 100, Layout = new RawPropertyLayout { Key = "%property{userAgent}" } });
        }

        private void AttachOnce<TAppender>(params Action<TAppender>[] initializers) where TAppender : class, IAppender, IOptionHandler, new()
        {
            var appenderKey = typeof(TAppender);
            List<IAppender> appenders;
            if (!_loggerAppendersMap.TryGetValue(appenderKey, out appenders))
            {
                appenders = new List<IAppender>();
                foreach (var initializer in initializers)
                {
                    var appender = new TAppender();
                    initializer(appender);
                    appender.ActivateOptions();
                    appenders.Add(appender);
                }

                _loggerAppendersMap.Add(appenderKey, appenders);
            }
        }
    }
}