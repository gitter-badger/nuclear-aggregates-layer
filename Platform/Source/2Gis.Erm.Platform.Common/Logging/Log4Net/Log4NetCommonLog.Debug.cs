using log4net;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net
{
    public sealed partial class Log4NetCommonLog
    {
        void ICommonLog.DebugEx(string message)
        {
            ((ICommonLog)this).DebugEx(message, null);
        }

        void ICommonLog.DebugEx(string message, string methodName)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Debug(message);
                }
            }
            else
            {
                _log.Debug(message);
            }
        }

        void ICommonLog.DebugFormatEx(string message, object param1)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1);
        }

        void ICommonLog.DebugFormatEx(string message, object param1, object param2)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1, param2);
        }

        void ICommonLog.DebugFormatEx(string message, object param1, object param2, object param3)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1, param2, param3);
        }

        void ICommonLog.DebugFormatEx(string message, params object[] args)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(_loggingCulture, message, args);
        }
    }
}
