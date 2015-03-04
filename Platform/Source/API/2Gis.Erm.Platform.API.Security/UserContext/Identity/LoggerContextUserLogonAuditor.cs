using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public sealed class LoggerContextUserLogonAuditor : IUserLogonAuditor
    {
        private readonly ILoggerContextManager _loggerContextManager;

        public LoggerContextUserLogonAuditor(ILoggerContextManager loggerContextManager)
        {
            _loggerContextManager = loggerContextManager;
        }

        public void LoggedIn(IUserIdentity identity)
        {
            _loggerContextManager[LoggerContextKeys.Required.UserAccount] = identity.Account;
        }
    }
}