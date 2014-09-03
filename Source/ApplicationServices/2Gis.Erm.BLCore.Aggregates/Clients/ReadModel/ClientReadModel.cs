using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.ReadModel
{
    public class ClientReadModel : IClientReadModel
    {
        private readonly IFinder _finder;

        public ClientReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Client GetClient(long clientId)
        {
            return _finder.FindOne(Specs.Find.ById<Client>(clientId));
        }

        public string GetClientName(long clientId)
        {
            return _finder.Find(Specs.Find.ById<Client>(clientId)).Select(x => x.Name).Single();
        }

        public IEnumerable<string> GetContactEmailsByBirthDate(int month, int day)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Contact>() &&
                                ClientSpecs.Contacts.Find.WithWorkEmail() &&
                                ClientSpecs.Contacts.Find.ByBirthDate(month, day))
                          .Select(x => x.WorkEmail)
                          .ToArray();
        }

        public IReadOnlyDictionary<long, Client> GetClientsToUpdateTerritoryByFirms(IEnumerable<long> firmIds)
        {
            var clientIdsByFirmIds = _finder.Find(Specs.Find.ByIds<Firm>(firmIds))
                                            .SelectMany(f => f.Clients
                                                              .Union(new[] { f.Client }.Where(c => c != null && c.MainFirmId == null))
                                                              .Select(c => new { ClientId = c.Id, FirmId = f.Id }))
                                            .ToDictionary(x => x.FirmId, x => x.ClientId);

            var clients = _finder.FindMany(Specs.Find.ByIds<Client>(clientIdsByFirmIds.Values)).ToDictionary(x => x.Id);

            return clientIdsByFirmIds.ToDictionary(x => x.Key, x => clients[x.Value]);
        }

        public IEnumerable<Client> GetClientsByMainFirmIds(IEnumerable<long> mainFirmIds)
        {
            return _finder.FindMany(ClientSpecs.Clients.Find.ByMainFirms(mainFirmIds));
        }  
    }
}