using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISignInService
    {
        IUserInfo SignIn();
    }
}