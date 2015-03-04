using System;

using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer
    {
        void ITracer.Fatal(string message)
        {
            ((ITracer)this).Fatal(message, null);
        }

        void ITracer.Fatal(string message, string methodName)
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

        void ITracer.Fatal(Exception exception, string message)
        {
            ((ITracer)this).Fatal(exception, message, null);
        }

        void ITracer.Fatal(Exception exception, string message, string methodName)
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

        void ITracer.FatalFormat(Exception exception, string message, object param1)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, param1), exception);
        }

        void ITracer.FatalFormat(Exception exception, string message, object param1, object param2)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, param1, param2), exception);
        }

        void ITracer.FatalFormat(Exception exception, string message, object param1, object param2, object param3)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, param1, param2, param3), exception);
        }

        void ITracer.FatalFormat(Exception exception, string message, params object[] args)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.Fatal(string.Format(_loggingCulture, message, args), exception);
        }

        void ITracer.FatalFormat(string message, object param1)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1);
        }

        void ITracer.FatalFormat(string message, object param1, object param2)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1, param2);
        }

        void ITracer.FatalFormat(string message, object param1, object param2, object param3)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(message, param1, param2, param3);
        }

        void ITracer.FatalFormat(string message, params object[] args)
        {
            if (!_log.IsFatalEnabled)
            {
                return;
            }

            _log.FatalFormat(_loggingCulture, message, args);
        }
    }
}
