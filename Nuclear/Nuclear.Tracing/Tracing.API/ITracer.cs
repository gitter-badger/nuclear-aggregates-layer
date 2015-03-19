using System;

namespace Nuclear.Tracing.API
{
    public interface ITracer
    {
        void Debug(string message);
        void Debug(string message, string methodName);
        void DebugFormat(string message, object param1);
        void DebugFormat(string message, object param1, object param2);
        void DebugFormat(string message, object param1, object param2, object param3);
        void DebugFormat(string message, params object[] args);

        void Info(string message);
        void Info(string message, string methodName);
        void InfoFormat(string message, object param1);
        void InfoFormat(string message, object param1, object param2);
        void InfoFormat(string message, object param1, object param2, object param3);
        void InfoFormat(string message, params object[] args);

        void Warn(string message);
        void Warn(string message, string methodName);
        void WarnFormat(string message, object param1);
        void WarnFormat(string message, object param1, object param2);
        void WarnFormat(string message, object param1, object param2, object param3);
        void WarnFormat(string message, params object[] args);

        void Error(string message);
        void Error(string message, string methodName);
        void Error(Exception exception, string message);
        void Error(Exception exception, string message, string methodName);
        void ErrorFormat(Exception exception, string message, object param1);
        void ErrorFormat(Exception exception, string message, object param1, object param2);
        void ErrorFormat(Exception exception, string message, object param1, object param2, object param3);
        void ErrorFormat(Exception exception, string message, params object[] args);
        void ErrorFormat(string message, object param1);
        void ErrorFormat(string message, object param1, object param2);
        void ErrorFormat(string message, object param1, object param2, object param3);
        void ErrorFormat(string message, params object[] args);

        void Fatal(string message);
        void Fatal(string message, string methodName);
        void Fatal(Exception exception, string message);
        void Fatal(Exception exception, string message, string methodName);
        void FatalFormat(Exception exception, string message, object param1);
        void FatalFormat(Exception exception, string message, object param1, object param2);
        void FatalFormat(Exception exception, string message, object param1, object param2, object param3);
        void FatalFormat(Exception exception, string message, params object[] args);
        void FatalFormat(string message, object param1);
        void FatalFormat(string message, object param1, object param2);
        void FatalFormat(string message, object param1, object param2, object param3);
        void FatalFormat(string message, params object[] args);
    }
}
