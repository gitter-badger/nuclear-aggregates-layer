using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface ICreateClientLinkAggregateService : IAggregateSpecificService<Client, CreateIdentity>
    {
        void Create(ClientLink client, IEnumerable<DenormalizedClientLink> denormalization);
    }
}