using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

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
            return _finder.Find(Specs.Find.ById<Client>(clientId)).One();
        }

        public string GetClientName(long clientId)
        {
            return _finder.Find(Specs.Find.ById<Client>(clientId)).Map(q => q.Select(x => x.Name)).One();
        }

        public Client GetClientByContact(long contactId)
        {
            return _finder.Find(ClientSpecs.Clients.Find.ByContact(contactId)).One();
        }

        public Client GetClientByDeal(long dealId)
        {
            return _finder.Find(ClientSpecs.Clients.Find.ByDeal(dealId)).One();
        }

        public Client GetClientByFirm(long firmId)
        {
            return _finder.Find(ClientSpecs.Clients.Find.ByFirm(firmId)).One();
        }

        public string GetContactName(long contactId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Contact>(contactId)).Select(x => x.FullName).Single();
        }

        public IEnumerable<Contact> GetClientContacts(long clientId)
        {
            var clientAndChild = _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ClientChild(clientId))
                .Map(q => q.Select(s => (long?)s.ChildClientId))
                .Many()
                .Union(new[] { (long?)clientId });
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Contact>() &&
                                ClientSpecs.Contacts.Find.IsNotFired() &&
                                ClientSpecs.Contacts.Find.ByClientIds(clientAndChild))
                          .Many();
        }

        public IEnumerable<string> GetContactEmailsByBirthDate(int month, int day)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Contact>() &&
                                ClientSpecs.Contacts.Find.WithWorkEmail() &&
                                ClientSpecs.Contacts.Find.ByBirthDate(month, day))
                          .Map(q => q.Select(x => x.WorkEmail))
                          .Many();
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
                          .Map(q => q.Select(cl => new MasterClientDto
                              {
                                  Id = cl.MasterClientId,
                                  Name = cl.MasterClient.Name,
                                  IsAdvertisingAgency = cl.MasterClient.IsAdvertisingAgency
                              }))
                          .Many();
        }

        public IEnumerable<DenormalizedClientLink> GetCurrentDenormalizationForClientLink(long masterClientId, long childClientId)
        {
            var graphKeys = _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ByMasterAndChildNodes(masterClientId, childClientId))
                                   .Map(q => q.Select(x => x.GraphKey).Distinct())
                                   .Many();

            return _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ByGrapKeys(graphKeys.ToArray())).Many();
        }

        public ClientLink GetClientsLink(long linkId)
        {
            return _finder.Find(Specs.Find.ById<ClientLink>(linkId)).One();
        }
        public IReadOnlyDictionary<long, IEnumerable<Client>> GetClientsToUpdateTerritoryByFirms(IEnumerable<long> firmIds)
        {
            var clientIdsByFirmIds = _finder.Find(Specs.Find.ByIds<Firm>(firmIds))
                                            .Map(q => q.Select(f => new
                                                {
                                                    FirmId = f.Id,
                                                    ClientIds = f.Clients
                                                                 .Union(new[] { f.Client }.Where(c => c != null && c.MainFirmId == null))
                                                                 .Select(c => c.Id)
                                                }))
                                            .Many();

            var clients = _finder.Find(Specs.Find.ByIds<Client>(clientIdsByFirmIds.SelectMany(x => x.ClientIds).Distinct())).Many().ToDictionary(x => x.Id);


            return clientIdsByFirmIds.ToDictionary(x => x.FirmId, x => x.ClientIds.Select(clientId => clients[clientId]));
        }

        public IEnumerable<Client> GetClientsByMainFirmIds(IEnumerable<long> mainFirmIds)
        {
            return _finder.Find(ClientSpecs.Clients.Find.ByMainFirms(mainFirmIds)).Many();
        }

        public bool IsClientInReserve(long clientId)
        {
            var clientOwner = _finder.FindObsolete(Specs.Find.ById<Client>(clientId)).Select(client => client.OwnerCode).Single();
            return clientOwner == _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }
    }
}