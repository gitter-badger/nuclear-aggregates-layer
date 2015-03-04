using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public static class WebLoggerContextManagerExtension
    {
        public static void SetUserInfo(
            this ILoggerContextManager loggerContextManager,
            string userAccount,
            string userSession,
            string userAddress,
            string userAgent)
        {
            loggerContextManager[LoggerContextKeys.Required.UserAccount] = userAccount;
            loggerContextManager[LoggerContextKeys.Optional.UserSession] = userSession;
            loggerContextManager[LoggerContextKeys.Optional.UserAddress] = userAddress;
            loggerContextManager[LoggerContextKeys.Optional.UserAgent] = userAgent;
        }
    }
}