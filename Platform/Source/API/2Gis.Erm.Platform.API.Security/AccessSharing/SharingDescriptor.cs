using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security.AccessSharing
{
    public sealed class SharingDescriptor
    {
        public IUserInfo UserInfo { get; set; }
        public EntityAccessTypes AccessTypes { get; set; }
    }
}
