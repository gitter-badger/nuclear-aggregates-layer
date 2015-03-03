using log4net;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net
{
    public sealed partial class Log4NetCommonLog
    {
        void ICommonLog.Info(string message)
        {
            ((ICommonLog)this).Info(message, null);
        }

        void ICommonLog.Info(string message, string methodName)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Info(message);
            }
            }
            else
            {
                _log.Info(message);
            }
        }

        void ICommonLog.InfoFormat(string message, object param1)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1);
        }

        void ICommonLog.InfoFormat(string message, object param1, object param2)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2);
        }

        void ICommonLog.InfoFormat(string message, object param1, object param2, object param3)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2, param3);
        }

        void ICommonLog.InfoFormat(string message, params object[] args)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(_loggingCulture, message, args);
        }
    }
}
