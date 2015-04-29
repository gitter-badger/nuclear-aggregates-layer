using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public interface IClientReadModel : IAggregateReadModel<Client>
    {
        Client GetClient(long clientId);
        string GetClientName(long clientId);

        Client GetClientByContact(long clientId);
        string GetContactName(long contactId);

        IEnumerable<string> GetContactEmailsByBirthDate(int month, int day);
        bool IsClientLinksExists(long? masterClientId, long? childClientId, bool? isDeleted);
        IEnumerable<MasterClientDto> GetMasterAdvertisingAgencies(long childClientId);
        IEnumerable<DenormalizedClientLink> GetCurrentDenormalizationForClientLink(long masterClientId, long childClientId);
        ClientLink GetClientsLink(long linkId);
        IReadOnlyDictionary<long, IEnumerable<Client>> GetClientsToUpdateTerritoryByFirms(IEnumerable<long> firmIds);
        IEnumerable<Client> GetClientsByMainFirmIds(IEnumerable<long> mainFirmIds);
        bool IsClientInReserve(long clientId);
    }
}