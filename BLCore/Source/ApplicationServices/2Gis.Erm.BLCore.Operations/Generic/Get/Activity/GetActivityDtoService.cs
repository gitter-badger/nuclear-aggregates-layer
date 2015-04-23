using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    public abstract class GetActivityDtoService<TEntity> : GetDomainEntityDtoServiceBase<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IClientReadModel _clientReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForRegardingObjects;
        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForAttendees;

        protected GetActivityDtoService(IUserContext userContext,
                                        IAppointmentReadModel appointmentReadModel,
                                        IClientReadModel clientReadModel,
                                        IFirmReadModel firmReadModel,
                                        IDealReadModel dealReadModel,
                                        ILetterReadModel letterReadModel,
                                        IPhonecallReadModel phonecallReadModel,
                                        ITaskReadModel taskReadModel)
            : base(userContext)
        {
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _dealReadModel = dealReadModel;

            _lookupsForRegardingObjects = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Task, entityId => taskReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, ResolveRegardingObjectsFromClient },
                { EntityName.Contact, ResolveRegardingObjectsFromContact },
                { EntityName.Deal, ResolveRegardingObjectsFromDeal },
                { EntityName.Firm, ResolveRegardingObjectsFromFirm },
            };

            _lookupsForAttendees = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetAttendees(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, ResolveContactsFromClient },
                { EntityName.Contact, ResolveContactsFromContact },
                { EntityName.Deal, ResolveContactsFromDeal },
                { EntityName.Firm, ResolveContactsFromFirm },
            };
        }

        protected IEnumerable<EntityReference> GetRegardingObjects(EntityName entityName, long? entityId)
        {
            if (entityId != null && _lookupsForRegardingObjects.ContainsKey(entityName))
            {
                return _lookupsForRegardingObjects[entityName](entityId.Value);
            }

            return Enumerable.Empty<EntityReference>();
        }

        protected IEnumerable<EntityReference> GetAttandees(EntityName entityName, long? entityId)
        {
            if (entityId != null && _lookupsForAttendees.ContainsKey(entityName))
            {
                return _lookupsForAttendees[entityName](entityId.Value);
            }

            return Enumerable.Empty<EntityReference>();
        }

        private EntityReference EmbedEntityNameIfNeeded(EntityReference reference)
        {
            if (reference.Id != null && reference.Name == null)
            {
                reference.Name = ReadEntityName(reference.EntityName, reference.Id.Value);
            }

            return reference;
        }

        private string ReadEntityName(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Client:
                    return _clientReadModel.GetClientName(entityId);
                case EntityName.Contact:
                    return _clientReadModel.GetContactName(entityId);
                case EntityName.Deal:
                    return _dealReadModel.GetDeal(entityId).Name;
                case EntityName.Firm:
                    return _firmReadModel.GetFirmName(entityId);
                default:
                    throw new ArgumentOutOfRangeException("entityName");
            }
        }

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromClient(long clientId)
        {
            return
                Enumerable.Repeat(_clientReadModel.GetClient(clientId).ToEntityReference(x => x.Name), 1)
                .Concat(LookupFirmReferencesForClient(clientId))
                .Concat(LookupDealReferencesForClient(clientId));
        }

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromFirm(long firmId)
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

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromDeal(long dealId)
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

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromContact(long contactId)
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

        private IEnumerable<EntityReference> ResolveContactsFromClient(long clientId)
        {
            return LookupContactReferencesForClient(clientId);
        }

        private IEnumerable<EntityReference> ResolveContactsFromFirm(long firmId)
        {
            var client = _clientReadModel.GetClientByFirm(firmId);
            if (client != null)
            {
                return LookupContactReferencesForClient(client.Id);
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> ResolveContactsFromDeal(long dealId)
        {
            var deal = _clientReadModel.GetClientByDeal(dealId);
            if (deal != null)
            {
                return LookupContactReferencesForClient(deal.Id);
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> ResolveContactsFromContact(long contactId)
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
