using System;

using log4net;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net
{
    public sealed partial class Log4NetCommonLog
    {
        void ICommonLog.FatalEx(string message)
        {
            ((ICommonLog)this).FatalEx(message, null);
        }

        void ICommonLog.FatalEx(string message, string methodName)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Fatal(message);
            }
            }
            else
            {
                _log.Fatal(message);
            }
        }

        void ICommonLog.FatalEx(Exception exception, string message)
        {
            ((ICommonLog)this).FatalEx(exception, message, null);
        }

        void ICommonLog.FatalEx(Exception exception, string message, string methodName)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Fatal(message);
            }
            }
            else
            {
                _log.Fatal(message, exception);
            }
        }

        void ICommonLog.FatalFormatEx(Exception exception, string message, object param1)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, param1), exception);
        }

        void ICommonLog.FatalFormatEx(Exception exception, string message, object param1, object param2)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, param1, param2), exception);
        }

        void ICommonLog.FatalFormatEx(Exception exception, string message, object param1, object param2, object param3)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, param1, param2, param3), exception);
        }

        void ICommonLog.FatalFormatEx(Exception exception, string message, params object[] args)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, args), exception);
        }

        void ICommonLog.FatalFormatEx(string message, object param1)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1);
        }

        void ICommonLog.FatalFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1, param2);
        }

        void ICommonLog.FatalFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1, param2, param3);
        }

        void ICommonLog.FatalFormatEx(string message, params object[] args)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(_loggingCulture, message, args);
        }
    }
}
