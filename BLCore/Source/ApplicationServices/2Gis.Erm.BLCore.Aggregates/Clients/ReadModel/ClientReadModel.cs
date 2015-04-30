using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.ReadModel
{
    public class ClientReadModel : IClientReadModel
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public ClientReadModel(IQuery query, IFinder finder, ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _query = query;
            _finder = finder;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public Client GetClient(long clientId)
        {
            return _finder.FindOne(Specs.Find.ById<Client>(clientId));
        }

        public string GetClientName(long clientId)
        {
            return _finder.Find(Specs.Find.ById<Client>(clientId)).Select(x => x.Name).SingleOrDefault();
        }

	    public Client GetClientByContact(long contactId)
	    {
            return _finder.FindOne(ClientSpecs.Clients.Find.ByContact(contactId));
        }

	    public string GetContactName(long contactId)
	    {
			return _finder.Find(Specs.Find.ById<Contact>(contactId)).Select(x => x.FullName).Single();
		}

        public IEnumerable<string> GetContactEmailsByBirthDate(int month, int day)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Contact>() &&
                                ClientSpecs.Contacts.Find.WithWorkEmail() &&
                                ClientSpecs.Contacts.Find.ByBirthDate(month, day))
                          .Select(x => x.WorkEmail)
                          .ToArray();
        }

        public bool IsClientLinksExists(long? masterClientId, long? childClientId, bool? isDeleted)
        {
            var query = _query.For<ClientLink>();

            if (masterClientId.HasValue)
            {
                query = query.Where(c => c.MasterClientId == masterClientId);
            }

            if (childClientId.HasValue)
            {
                query = query.Where(c => c.ChildClientId == childClientId);
            }

            if (isDeleted.HasValue)
            {
                query = query.Where(c => c.IsDeleted == isDeleted);
            }

            return query.Any();
        }

        public IEnumerable<MasterClientDto> GetMasterAdvertisingAgencies(long childClientId)
        {
            return _finder.Find(Specs.Find.NotDeleted<ClientLink>() &&
                                ClientSpecs.ClientLinks.Find.ByChildClientId(childClientId) &&
                                ClientSpecs.ClientLinks.Find.WhereMasterClientIsAdvertisingAgency())
                          .Select(cl => new MasterClientDto
                              {
                                  Id = cl.MasterClientId,
                                  Name = cl.MasterClient.Name,
                                  IsAdvertisingAgency = cl.MasterClient.IsAdvertisingAgency
                              });
        }

        public IEnumerable<DenormalizedClientLink> GetCurrentDenormalizationForClientLink(long masterClientId, long childClientId)
        {
            var graphKeys = _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ByMasterAndChildNodes(masterClientId, childClientId))
                                   .Select(x => x.GraphKey)
                                   .Distinct()
                                   .ToArray();

            return _finder.FindMany(ClientSpecs.DenormalizedClientLinks.Find.ByGrapKeys(graphKeys));
        }

        public ClientLink GetClientsLink(long linkId)
        {
            return _finder.FindOne(Specs.Find.ById<ClientLink>(linkId));
        }
        public IReadOnlyDictionary<long, IEnumerable<Client>> GetClientsToUpdateTerritoryByFirms(IEnumerable<long> firmIds)
        {
            var clientIdsByFirmIds = _finder.Find(Specs.Find.ByIds<Firm>(firmIds))
                                            .Select(f => new
                                                {
                                                    FirmId = f.Id,
                                                    ClientIds = f.Clients
                                                               .Union(new[] { f.Client }.Where(c => c != null && c.MainFirmId == null))
                                                               .Select(c => c.Id)
                                                })
                                            .ToArray();

            var clients = _finder.FindMany(Specs.Find.ByIds<Client>(clientIdsByFirmIds.SelectMany(x => x.ClientIds).Distinct())).ToDictionary(x => x.Id);


            return clientIdsByFirmIds.ToDictionary(x => x.FirmId, x => x.ClientIds.Select(clientId => clients[clientId]));
        }

        public IEnumerable<Client> GetClientsByMainFirmIds(IEnumerable<long> mainFirmIds)
        {
            return _finder.FindMany(ClientSpecs.Clients.Find.ByMainFirms(mainFirmIds));
        }  

        public bool IsClientInReserve(long clientId)
        {
            var clientOwner = _finder.Find(Specs.Find.ById<Client>(clientId)).Select(client => client.OwnerCode).Single();
            return clientOwner == _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }
    }
}