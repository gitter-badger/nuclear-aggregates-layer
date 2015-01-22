using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation
{
    public interface IUserAppendRoleAggregateService : IAggregateSpecificOperation<User, AppendIdentity>
    {
        void AppendRole(User user, long roleId);
    }
}
