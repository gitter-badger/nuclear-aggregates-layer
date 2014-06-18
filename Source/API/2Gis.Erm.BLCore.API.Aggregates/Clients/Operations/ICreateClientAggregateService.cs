using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface ICreateClientAggregateService : IAggregateSpecificOperation<Client, CreateIdentity>
    {
        int Create(Client client, out FirmAddress mainFirmAddress);
    }
}