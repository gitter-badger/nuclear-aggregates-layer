using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceAuthentication
    {
        IUserIdentity AuthenticateUser(string userAccount);
    }
}
