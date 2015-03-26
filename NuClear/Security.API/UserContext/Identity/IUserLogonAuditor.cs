namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserLogonAuditor
    {
        void LoggedIn(IUserIdentity identity);
    }
}