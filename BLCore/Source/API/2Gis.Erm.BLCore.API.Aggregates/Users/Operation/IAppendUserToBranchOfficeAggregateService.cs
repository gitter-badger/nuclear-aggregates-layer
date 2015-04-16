using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation
{
    public interface IAppendUserToBranchOfficeAggregateService : IAggregateSpecificOperation<User, AppendIdentity>
    {
        void AppendUser(long userId, long branchOfficeId);
    }
}
