using System;

namespace Nuclear.Tracing.API
{
    public sealed class NullLogger : ITracer
    {
        void ITracer.Debug(string message)
        {
            // do nothing
        }

        void ITracer.Debug(string message, string methodName)
        {
            // do nothing
        }

        void ITracer.DebugFormat(string message, object param1)
        {
            // do nothing
        }

        void ITracer.DebugFormat(string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.DebugFormat(string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.DebugFormat(string message, params object[] args)
        {
            // do nothing
        }

        void ITracer.Info(string message)
        {
            // do nothing
        }

        void ITracer.Info(string message, string methodName)
        {
            // do nothing
        }

        void ITracer.InfoFormat(string message, object param1)
        {
            // do nothing
        }

        void ITracer.InfoFormat(string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.InfoFormat(string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.InfoFormat(string message, params object[] args)
        {
            // do nothing
        }

        void ITracer.Warn(string message)
        {
            // do nothing
        }

        void ITracer.Warn(string message, string methodName)
        {
            // do nothing
        }

        void ITracer.WarnFormat(string message, object param1)
        {
            // do nothing
        }

        void ITracer.WarnFormat(string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.WarnFormat(string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.WarnFormat(string message, params object[] args)
        {
            // do nothing
        }

        void ITracer.Error(string message)
        {
            // do nothing
        }

        void ITracer.Error(string message, string methodName)
        {
            // do nothing
        }

        void ITracer.Error(Exception exception, string message)
        {
            // do nothing
        }

        void ITracer.Error(Exception exception, string message, string methodName)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(Exception exception, string message, object param1)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(Exception exception, string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(Exception exception, string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(Exception exception, string message, params object[] args)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(string message, object param1)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.ErrorFormat(string message, params object[] args)
        {
            // do nothing
        }

        void ITracer.Fatal(string message)
        {
            // do nothing
        }

        void ITracer.Fatal(string message, string methodName)
        {
            // do nothing
        }

        void ITracer.Fatal(Exception exception, string message)
        {
            // do nothing
        }

        void ITracer.Fatal(Exception exception, string message, string methodName)
        {
            // do nothing
        }

        void ITracer.FatalFormat(Exception exception, string message, object param1)
        {
            // do nothing
        }

        void ITracer.FatalFormat(Exception exception, string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.FatalFormat(Exception exception, string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.FatalFormat(Exception exception, string message, params object[] args)
        {
            // do nothing
        }

        void ITracer.FatalFormat(string message, object param1)
        {
            // do nothing
        }

        void ITracer.FatalFormat(string message, object param1, object param2)
        {
            // do nothing
        }

        void ITracer.FatalFormat(string message, object param1, object param2, object param3)
        {
            // do nothing
        }

        void ITracer.FatalFormat(string message, params object[] args)
        {
            // do nothing
        }
    }
}
