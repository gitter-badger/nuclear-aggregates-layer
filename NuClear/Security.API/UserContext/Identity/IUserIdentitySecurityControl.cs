namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserIdentitySecurityControl
    {
        bool SkipEntityAccessCheck { get; set; }
    }
}