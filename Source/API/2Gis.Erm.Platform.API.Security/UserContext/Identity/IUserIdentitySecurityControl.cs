using System;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserIdentitySecurityControl
    {
        Boolean SkipEntityAccessCheck { get; set; }
    }
}