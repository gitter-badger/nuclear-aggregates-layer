using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface IBindCrmTaskToHotClientRequestAggregateService : IAggregateSpecificOperation<Firm, BindCrmTaskToHotClientRequestIdentity>
    {
        void BindWithCrmTask(HotClientRequest hotClientRequest, Guid taskId);
    }
}