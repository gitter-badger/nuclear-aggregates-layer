using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    [Obsolete("Обертка над тремя read моделями, которая написана не по DIP понятниям.")]
    public sealed class ActivityReferenceReader
    {
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public ActivityReferenceReader(
            IClientReadModel clientReadModel,
            IDealReadModel dealReadModel,
            IFirmReadModel firmReadModel)
        {
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
        }

        public IEnumerable<EntityReference> ResolveRegardingObjectsFromClient(long clientId)
        {
            return
                Enumerable.Repeat(_clientReadModel.GetClient(clientId).ToEntityReference(x => x.Name), 1)
                .Concat(LookupFirmReferencesForClient(clientId))
                .Concat(LookupDealReferencesForClient(clientId));
        }

        public IEnumerable<EntityReference> ResolveRegardingObjectsFromFirm(long firmId)
        {
            var regardingObjects = new List<EntityReference> { _firmReadModel.GetFirm(firmId).ToEntityReference(x => x.Name) };

            var client = _clientReadModel.GetClientByFirm(firmId);
            if (client != null)
            {
                regardingObjects.Add(client.ToEntityReference(x => x.Name));
                regardingObjects.AddRange(LookupDealReferencesForClient(client.Id));
            }

            return regardingObjects;
        }

        public IEnumerable<EntityReference> ResolveRegardingObjectsFromDeal(long dealId)
        {
            var regardingObjects = new List<EntityReference> { _dealReadModel.GetDeal(dealId).ToEntityReference(x => x.Name) };

            var client = _clientReadModel.GetClientByDeal(dealId);
            if (client != null)
            {
                regardingObjects.Add(client.ToEntityReference(x => x.Name));
                regardingObjects.AddRange(LookupFirmReferencesForClient(client.Id));
            }

            return regardingObjects;
        }

        public IEnumerable<EntityReference> ResolveRegardingObjectsFromContact(long contactId)
        {
            var client = _clientReadModel.GetClientByContact(contactId);
            if (client != null)
            {
                return Enumerable.Repeat(_clientReadModel.GetClient(client.Id).ToEntityReference(x => x.Name), 1)
                    .Concat(LookupFirmReferencesForClient(client.Id))
                    .Concat(LookupDealReferencesForClient(client.Id));
            }

            return Enumerable.Empty<EntityReference>();
        }

        public IEnumerable<EntityReference> ResolveContactsFromClient(long clientId)
        {
            return LookupContactReferencesForClient(clientId);
        }

        public IEnumerable<EntityReference> ResolveContactsFromFirm(long firmId)
        {
            var client = _clientReadModel.GetClientByFirm(firmId);
            if (client != null)
            {
                return LookupContactReferencesForClient(client.Id);
            }

            return Enumerable.Empty<EntityReference>();
        }

        public IEnumerable<EntityReference> ResolveContactsFromDeal(long dealId)
        {
            var deal = _clientReadModel.GetClientByDeal(dealId);
            if (deal != null)
            {
                return LookupContactReferencesForClient(deal.Id);
            }

            return Enumerable.Empty<EntityReference>();
        }

        public IEnumerable<EntityReference> ResolveContactsFromContact(long contactId)
        {
            return new[] { new EntityReference { EntityName = EntityName.Contact, Id = contactId, Name = _clientReadModel.GetContactName(contactId) } };
        }

        private IEnumerable<EntityReference> LookupFirmReferencesForClient(long clientId)
        {
            return _firmReadModel.GetFirmsForClientAndLinkedChild(clientId).ToEntityReferencesWithNoAmbiguity(x => x.Name);
        }

        private IEnumerable<EntityReference> LookupDealReferencesForClient(long clientId)
        {
            return _dealReadModel.GetDealsByClientId(clientId).ToEntityReferencesWithNoAmbiguity(x => x.Name);
        }

        private IEnumerable<EntityReference> LookupContactReferencesForClient(long clientId)
        {
            return _clientReadModel.GetClientContacts(clientId).ToEntityReferencesWithNoAmbiguity(x => x.FullName);
        }
    }
}
