using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface IDetachClientFromFirmAggregateService : IAggregateSpecificOperation<Client, DetachIdentity>
    {
        void Detach(IEnumerable<Client> clientsToDetach);
    }
}