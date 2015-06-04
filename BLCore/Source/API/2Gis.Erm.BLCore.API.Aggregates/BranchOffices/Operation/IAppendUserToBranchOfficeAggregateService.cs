using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation
{
    public interface IAppendUserToBranchOfficeAggregateService : IAggregateSpecificService<BranchOffice, AppendIdentity>
    {
        void AppendUser(long userId, long branchOfficeId);
    }
}
