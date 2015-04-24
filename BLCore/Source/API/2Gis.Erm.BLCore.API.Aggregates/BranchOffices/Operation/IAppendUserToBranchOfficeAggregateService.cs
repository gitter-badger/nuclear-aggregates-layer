using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation
{
    public interface IAppendUserToBranchOfficeAggregateService : IAggregateSpecificOperation<BranchOffice, AppendIdentity>
    {
        void AppendUser(long userId, long branchOfficeId);
    }
}
