using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.Platform.Common.Logging
{
    public interface ICommonLog : IInvariantSafeCrosscuttingService
    {
        void DebugEx(string message);
        void DebugEx(string message, string methodName);
        void DebugFormatEx(string message, object param1);
        void DebugFormatEx(string message, object param1, object param2);
        void DebugFormatEx(string message, object param1, object param2, object param3);
        void DebugFormatEx(string message, params object[] args);

        void InfoEx(string message);
        void InfoEx(string message, string methodName);
        void InfoFormatEx(string message, object param1);
        void InfoFormatEx(string message, object param1, object param2);
        void InfoFormatEx(string message, object param1, object param2, object param3);
        void InfoFormatEx(string message, params object[] args);

        void WarnEx(string message);
        void WarnEx(string message, string methodName);
        void WarnFormatEx(string message, object param1);
        void WarnFormatEx(string message, object param1, object param2);
        void WarnFormatEx(string message, object param1, object param2, object param3);
        void WarnFormatEx(string message, params object[] args);

        void ErrorEx(string message);
        void ErrorEx(string message, string methodName);
        void ErrorEx(Exception exception, string message);
        void ErrorEx(Exception exception, string message, string methodName);
        void ErrorFormatEx(Exception exception, string message, object param1);
        void ErrorFormatEx(Exception exception, string message, object param1, object param2);
        void ErrorFormatEx(Exception exception, string message, object param1, object param2, object param3);
        void ErrorFormatEx(Exception exception, string message, params object[] args);
        void ErrorFormatEx(string message, object param1);
        void ErrorFormatEx(string message, object param1, object param2);
        void ErrorFormatEx(string message, object param1, object param2, object param3);
        void ErrorFormatEx(string message, params object[] args);

        void FatalEx(string message);
        void FatalEx(string message, string methodName);
        void FatalEx(Exception exception, string message);
        void FatalEx(Exception exception, string message, string methodName);
        void FatalFormatEx(Exception exception, string message, object param1);
        void FatalFormatEx(Exception exception, string message, object param1, object param2);
        void FatalFormatEx(Exception exception, string message, object param1, object param2, object param3);
        void FatalFormatEx(Exception exception, string message, params object[] args);
        void FatalFormatEx(string message, object param1);
        void FatalFormatEx(string message, object param1, object param2);
        void FatalFormatEx(string message, object param1, object param2, object param3);
        void FatalFormatEx(string message, params object[] args);
    }
}
