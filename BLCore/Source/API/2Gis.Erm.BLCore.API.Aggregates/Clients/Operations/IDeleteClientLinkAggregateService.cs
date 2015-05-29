using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface IDeleteClientLinkAggregateService : IAggregateSpecificService<Client, DeleteIdentity>
    {
        void Delete(ClientLink client, IEnumerable<DenormalizedClientLink> currentGraph);
    }
}