using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer
    {
        void ITracer.Info(string message)
        {
            ((ITracer)this).Info(message, null);
        }

        void ITracer.Info(string message, string methodName)
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

        void ITracer.InfoFormat(string message, object param1)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1);
        }

        void ITracer.InfoFormat(string message, object param1, object param2)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2);
        }

        void ITracer.InfoFormat(string message, object param1, object param2, object param3)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(message, param1, param2, param3);
        }

        void ITracer.InfoFormat(string message, params object[] args)
        {
            if (!_log.IsInfoEnabled)
            {
                return;
            }

            _log.InfoFormat(_tracingCulture, message, args);
        }
    }
}
