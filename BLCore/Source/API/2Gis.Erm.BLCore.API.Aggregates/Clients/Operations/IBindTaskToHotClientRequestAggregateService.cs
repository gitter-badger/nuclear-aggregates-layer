using System;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface IBindTaskToHotClientRequestAggregateService : IAggregateSpecificService<Firm, BindTaskToHotClientRequestIdentity>
    {
        void BindTask(HotClientRequest hotClientRequest, long taskId);
    }
}