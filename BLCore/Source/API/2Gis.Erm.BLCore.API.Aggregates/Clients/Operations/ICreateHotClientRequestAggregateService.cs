using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface ICreateHotClientRequestAggregateService : IAggregateSpecificService<Firm, CreateIdentity>
    {
        void Create(HotClientRequest hotClientRequest);
    }
}