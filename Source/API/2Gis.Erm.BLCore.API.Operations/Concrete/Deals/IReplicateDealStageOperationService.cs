using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals
{
    public interface IReplicateDealStageOperationService : IOperation<ReplicateDealStageIdentity>
    {
        void Replicate(Guid dealReplicationCode, DealStage dealStage, string userDomainName);
    }
}
