using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public sealed class LoggerContextUserLogonAuditor : IUserLogonAuditor
    {
        private readonly ITracerContextManager _tracerContextManager;

        public LoggerContextUserLogonAuditor(ITracerContextManager tracerContextManager)
        {
            _tracerContextManager = tracerContextManager;
        }

        public void LoggedIn(IUserIdentity identity)
        {
            _tracerContextManager[TracerContextKeys.Required.UserAccount] = identity.Account;
        }
    }
}