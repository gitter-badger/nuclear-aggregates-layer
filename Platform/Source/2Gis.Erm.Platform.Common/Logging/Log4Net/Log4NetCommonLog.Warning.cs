using log4net;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net
{
    public sealed partial class Log4NetCommonLog
    {
        void ICommonLog.WarnEx(string message)
        {
            ((ICommonLog)this).WarnEx(message, null);
        }

        void ICommonLog.WarnEx(string message, string methodName)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Warn(message);
            }
            }
            else
            {
                _log.Warn(message);
            }
        }

        void ICommonLog.WarnFormatEx(string message, object param1)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1);
        }

        void ICommonLog.WarnFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1, param2);
        }

        void ICommonLog.WarnFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1, param2, param3);
        }

        void ICommonLog.WarnFormatEx(string message, params object[] args)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(_loggingCulture, message, args);
        }
    }
}
