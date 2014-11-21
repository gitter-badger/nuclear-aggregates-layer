using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using log4net;
using log4net.Appender;
using log4net.Config;

namespace DoubleGis.Erm.Platform.Common.Logging
{
    public sealed class Log4NetImpl : ICommonLog
    {
        private static readonly CultureInfo LoggingCulture = new CultureInfo("ru-RU");

        private readonly ILog _log;

        public Log4NetImpl(string loggerName)
        {
            _log = LogManager.GetLogger(loggerName);
        }

        public static ICommonLog GetLogger(string logger)
        {
            return new Log4NetImpl(logger);
        }

        public static void ConfigureFromXml(string filePath, string adoNetConnectionString)
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

        #region Implementation of ICommonLog

        public void DebugEx(String message)
        {
            DebugEx(message, null);
        }

        public void DebugEx(string message, string methodName)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Debug(message);
            }
            }
            else
            {
                _log.Debug(message);
            }
        }

        public void DebugFormatEx(string message, object param1)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1);
        }

        public void DebugFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1, param2);
        }

        public void DebugFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1, param2, param3);
        }

        public void DebugFormatEx(string message, params object[] args)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(LoggingCulture, message, args);
        }

        public void InfoEx(string message)
        {
            InfoEx(message, null);
        }

        public void InfoEx(string message, string methodName)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Info(message);
            }
            }
            else
            {
                _log.Info(message);
            }
        }

        public void InfoFormatEx(string message, object param1)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1);
        }

        public void InfoFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2);
        }

        public void InfoFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2, param3);
        }

        public void InfoFormatEx(string message, params object[] args)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(LoggingCulture, message, args);
        }

        public void WarnEx(string message)
        {
            WarnEx(message, null);
        }

        public void WarnEx(string message, string methodName)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Warn(message);
            }
            }
            else
            {
                _log.Warn(message);
            }
        }

        public void WarnFormatEx(string message, object param1)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1);
        }

        public void WarnFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1, param2);
        }

        public void WarnFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1, param2, param3);
        }

        public void WarnFormatEx(string message, params object[] args)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(LoggingCulture, message, args);
        }

        public void ErrorEx(string message)
        {
            ErrorEx(message, null);
        }

        public void ErrorEx(string message, string methodName)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Error(message);
            }
            }
            else
            {
                _log.Error(message);
            }
        }

        public void ErrorEx(Exception exception, string message)
        {
            ErrorEx(exception, message, null);
        }

        public void ErrorEx(Exception exception, string message, string methodName)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Error(message, exception);
            }
            }
            else
            {
                _log.Error(message, exception);
            }
        }

        public void ErrorFormatEx(Exception exception, string message, object param1)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(LoggingCulture, message, param1), exception);
        }

        public void ErrorFormatEx(Exception exception, string message, object param1, object param2)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(LoggingCulture, message, param1, param2), exception);
        }

        public void ErrorFormatEx(Exception exception, string message, object param1, object param2, object param3)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(LoggingCulture, message, param1, param2, param3), exception);
        }

        public void ErrorFormatEx(Exception exception, string message, params object[] args)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(LoggingCulture, message, args), exception);
        }

        public void ErrorFormatEx(string message, object param1)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(message, param1);
        }

        public void ErrorFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(message, param1, param2);
        }

        public void ErrorFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(message, param1, param2, param3);
        }

        public void ErrorFormatEx(string message, params object[] args)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(LoggingCulture, message, args);
        }

        public void FatalEx(string message)
        {
            FatalEx(message, null);
        }

        public void FatalEx(string message, string methodName)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Fatal(message);
            }
            }
            else
            {
                _log.Fatal(message);
            }
        }

        public void FatalEx(Exception exception, string message)
        {
            FatalEx(exception, message, null);
        }

        public void FatalEx(Exception exception, string message, string methodName)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks[LoggerContextKeys.Optional.MethodName].Push(methodName))
                {
                    _log.Fatal(message);
            }
            }
            else
            {
                _log.Fatal(message, exception);
            }
        }

        public void FatalFormatEx(Exception exception, string message, object param1)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(LoggingCulture, message, param1), exception);
        }

        public void FatalFormatEx(Exception exception, string message, object param1, object param2)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(LoggingCulture, message, param1, param2), exception);
        }

        public void FatalFormatEx(Exception exception, string message, object param1, object param2, object param3)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(LoggingCulture, message, param1, param2, param3), exception);
        }

        public void FatalFormatEx(Exception exception, string message, params object[] args)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(LoggingCulture, message, args), exception);
        }

        public void FatalFormatEx(string message, object param1)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1);
        }

        public void FatalFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1, param2);
        }

        public void FatalFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1, param2, param3);
        }

        public void FatalFormatEx(string message, params object[] args)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(LoggingCulture, message, args);
        }

        #endregion
    }
}
