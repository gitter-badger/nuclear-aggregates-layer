using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;

using NuClear.Security.API.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceUserIdentifier : IInvariantSafeCrosscuttingService
    {
        IUserInfo GetReserveUserIdentity();
        IUserInfo GetUserInfo(long? userCode);
        IUserInfo GetUserInfo(string userAccount);
        IList<long> GetUserDepartments(long? userCode, bool withChilds);
        bool UsersInSameDepartment(long firstUserCode, long secondUserCode);
        bool UsersInSameDepartmentTree(long firstUserCode, long secondUserCode);
    }
}