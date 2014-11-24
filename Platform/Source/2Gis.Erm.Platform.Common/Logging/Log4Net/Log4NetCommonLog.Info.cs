using log4net;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net
{
    public sealed partial class Log4NetCommonLog
    {
        void ICommonLog.InfoEx(string message)
        {
            ((ICommonLog)this).InfoEx(message, null);
        }

        void ICommonLog.InfoEx(string message, string methodName)
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

        void ICommonLog.InfoFormatEx(string message, object param1)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1);
        }

        void ICommonLog.InfoFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2);
        }

        void ICommonLog.InfoFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2, param3);
        }

        void ICommonLog.InfoFormatEx(string message, params object[] args)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(_loggingCulture, message, args);
        }
    }
}
