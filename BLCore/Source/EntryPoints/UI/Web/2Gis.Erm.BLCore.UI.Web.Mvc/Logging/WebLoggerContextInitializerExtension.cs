using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public static class WebLoggerContextManagerExtension
    {
        public static void SetUserInfo(this ILoggerContextManager loggerContextManager,
                                       string sessionId,
                                       string userName,
                                       string userIp,
                                       string userBrowser)
        {
            loggerContextManager[LoggerContextKeys.Required.SessionId] = sessionId;
            loggerContextManager[LoggerContextKeys.Required.UserName] = userName;
            loggerContextManager[LoggerContextKeys.Required.UserIP] = userIp;
            loggerContextManager[LoggerContextKeys.Required.UserBrowser] = userBrowser;
        }
    }
}