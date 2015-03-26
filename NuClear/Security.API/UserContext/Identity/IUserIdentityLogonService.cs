namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserIdentityLogonService
    {
        void Logon(IUserIdentity identity);
    }
}
