using System;

using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer
    {
        void ITracer.Error(string message)
        {
            ((ITracer)this).Error(message, null);
        }

        void ITracer.Error(string message, string methodName)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Error(message);
            }
            }
            else
            {
                _log.Error(message);
            }
        }

        void ITracer.Error(Exception exception, string message)
        {
            ((ITracer)this).Error(exception, message, null);
        }

        void ITracer.Error(Exception exception, string message, string methodName)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            if (methodName != null)
            {
                using (ThreadContext.Stacks["methodName"].Push(methodName))
                {
                    _log.Error(message, exception);
            }
            }
            else
            {
                _log.Error(message, exception);
            }
        }

        void ITracer.ErrorFormat(Exception exception, string message, object param1)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(_loggingCulture, message, param1), exception);
        }

        void ITracer.ErrorFormat(Exception exception, string message, object param1, object param2)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(_loggingCulture, message, param1, param2), exception);
        }

        void ITracer.ErrorFormat(Exception exception, string message, object param1, object param2, object param3)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(_loggingCulture, message, param1, param2, param3), exception);
        }

        void ITracer.ErrorFormat(Exception exception, string message, params object[] args)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.Error(string.Format(_loggingCulture, message, args), exception);
        }

        void ITracer.ErrorFormat(string message, object param1)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(message, param1);
        }

        void ITracer.ErrorFormat(string message, object param1, object param2)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(message, param1, param2);
        }

        void ITracer.ErrorFormat(string message, object param1, object param2, object param3)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(message, param1, param2, param3);
        }

        void ITracer.ErrorFormat(string message, params object[] args)
        {
            if (!_log.IsErrorEnabled)
            {
                return;
            }

            _log.ErrorFormat(_loggingCulture, message, args);
        }
    }
}
