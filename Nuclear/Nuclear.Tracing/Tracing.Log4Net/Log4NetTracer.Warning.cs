using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer
    {
        void ITracer.Warn(string message)
        {
            ((ITracer)this).Warn(message, null);
        }

        void ITracer.Warn(string message, string methodName)
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

        void ITracer.WarnFormat(string message, object param1)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1);
        }

        void ITracer.WarnFormat(string message, object param1, object param2)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1, param2);
        }

        void ITracer.WarnFormat(string message, object param1, object param2, object param3)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(message, param1, param2, param3);
        }

        void ITracer.WarnFormat(string message, params object[] args)
        {
            if (!_log.IsWarnEnabled)
            {
                return;
            }

            _log.WarnFormat(_tracingCulture, message, args);
        }
    }
}
