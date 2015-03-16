using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAssignAccountAggregateService : IAggregateSpecificOperation<Account, AssignIdentity>
    {
        void Assign(AssignAccountDto assignAccountDto, long ownerCode);
    }
}