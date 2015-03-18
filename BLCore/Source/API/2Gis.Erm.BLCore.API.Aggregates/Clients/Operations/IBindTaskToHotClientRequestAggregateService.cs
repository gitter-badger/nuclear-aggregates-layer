using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface IBindTaskToHotClientRequestAggregateService : IAggregateSpecificOperation<Firm, BindTaskToHotClientRequestIdentity>
    {
        void BindTask(HotClientRequest hotClientRequest, long taskId);
    }
}