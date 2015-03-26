namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserIdentity : IUserIdentitySecurityControl, IUserInfo
    {
        UserInfo UserData { get; }
    }
}