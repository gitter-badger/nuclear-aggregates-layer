using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface IUserProfileService
    {
        IUserProfile GetUserProfile(long userCode);
    }
}
