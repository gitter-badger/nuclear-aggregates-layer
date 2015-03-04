using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer
    {
        void ITracer.Debug(string message)
        {
            ((ITracer)this).Debug(message, null);
        }

        void ITracer.Debug(string message, string methodName)
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

        void ITracer.DebugFormat(string message, object param1)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1);
        }

        void ITracer.DebugFormat(string message, object param1, object param2)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1, param2);
        }

        void ITracer.DebugFormat(string message, object param1, object param2, object param3)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(message, param1, param2, param3);
        }

        void ITracer.DebugFormat(string message, params object[] args)
        {
            if (!_log.IsDebugEnabled)
            {
                return;
            }

            _log.DebugFormat(_loggingCulture, message, args);
        }
    }
}
