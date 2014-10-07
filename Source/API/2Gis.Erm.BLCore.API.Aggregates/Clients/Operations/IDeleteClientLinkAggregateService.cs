using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface IDeleteClientLinkAggregateService : IAggregateSpecificOperation<Client, DeleteIdentity>
    {
        void Delete(ClientLink client, IEnumerable<DenormalizedClientLink> currentGraph);
    }
}