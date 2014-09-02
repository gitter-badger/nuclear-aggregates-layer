using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public interface IClientReadModel : IAggregateReadModel<Client>
    {
        Client GetClient(long clientId);
        string GetClientName(long clientId);

        IEnumerable<string> GetContactEmailsByBirthDate(int month, int day);
        IReadOnlyDictionary<long, Client> GetClientsToUpdateTerritoryByFirms(IEnumerable<long> firmIds);
        IEnumerable<Client> GetClientsByMainFirmIds(IEnumerable<long> mainFirmIds);
    }
}