using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public static class WebTracerContextManagerExtension
    {
        public static void SetUserInfo(
            this ITracerContextManager tracerContextManager,
            string userAccount,
            string userSession,
            string userAddress,
            string userAgent)
        {
            tracerContextManager[TracerContextKeys.Required.UserAccount] = userAccount;
            tracerContextManager[TracerContextKeys.Optional.UserSession] = userSession;
            tracerContextManager[TracerContextKeys.Optional.UserAddress] = userAddress;
            tracerContextManager[TracerContextKeys.Optional.UserAgent] = userAgent;
        }
    }
}