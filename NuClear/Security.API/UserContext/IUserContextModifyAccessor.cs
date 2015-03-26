using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;

namespace DoubleGis.Erm.Platform.API.Security.UserContext
{
    public interface IUserContextModifyAccessor
    {
        IUserIdentity Identity { set; }
        IUserProfile Profile { set; }
    }
}
