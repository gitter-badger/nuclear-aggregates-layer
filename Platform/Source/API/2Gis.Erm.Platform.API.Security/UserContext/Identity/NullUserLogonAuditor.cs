namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public sealed class NullUserLogonAuditor : IUserLogonAuditor
    {
        public void LoggedIn(IUserIdentity identity)
        {
            // do nothing
        }
    }
}