using System;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserInfo
    {
        String DisplayName { get; }
        Int64 Code { get; }
        String Account { get; }
    }
}