using System.Security.Principal;

using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISignInByIdentityService
    {
        IUserInfo SignIn(IIdentity identity);
    }
}
