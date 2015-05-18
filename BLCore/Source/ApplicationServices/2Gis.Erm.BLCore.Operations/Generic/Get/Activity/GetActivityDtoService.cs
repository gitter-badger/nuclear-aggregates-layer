using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    public abstract class GetActivityDtoService<TEntity> : GetDomainEntityDtoServiceBase<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IClientReadModel _clientReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly Dictionary<IEntityType, Func<long, IEnumerable<EntityReference>>> _lookupsForRegardingObjects;
        private readonly Dictionary<IEntityType, Func<long, IEnumerable<EntityReference>>> _lookupsForAttendees;

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

            _lookupsForRegardingObjects = new Dictionary<IEntityType, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityType.Instance.Appointment(), entityId => appointmentReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Letter(), entityId => letterReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Phonecall(), entityId => phonecallReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Task(), entityId => taskReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Client(), ResolveRegardingObjectsFromClient },
                { EntityType.Instance.Contact(), ResolveRegardingObjectsFromContact },
                { EntityType.Instance.Deal(), ResolveRegardingObjectsFromDeal },
                { EntityType.Instance.Firm(), ResolveRegardingObjectsFromFirm },
            };

            _lookupsForAttendees = new Dictionary<IEntityType, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityType.Instance.Appointment(), entityId => appointmentReadModel.GetAttendees(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Letter(), entityId => letterReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Phonecall(), entityId => phonecallReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityType.Instance.Client(), ResolveContactsFromClient },
                { EntityType.Instance.Contact(), ResolveContactsFromContact },
                { EntityType.Instance.Deal(), ResolveContactsFromDeal },
                { EntityType.Instance.Firm(), ResolveContactsFromFirm },
            };
        }

        protected IEnumerable<EntityReference> GetRegardingObjects(IEntityType entityName, long? entityId)
        {
            if (entityId != null && _lookupsForRegardingObjects.ContainsKey(entityName))
            {
                return _lookupsForRegardingObjects[entityName](entityId.Value);
            }

            return Enumerable.Empty<EntityReference>();
        }

        protected IEnumerable<EntityReference> GetAttandees(IEntityType entityName, long? entityId)
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
                reference.Name = ReadEntityName(reference.EntityTypeId, reference.Id.Value);
            }

            return reference;
        }

        private string ReadEntityName(int entityTypeId, long entityId)
        {
            if (entityTypeId == EntityType.Instance.Client().Id)
            {
                return _clientReadModel.GetClientName(entityId);
            }

            if (entityTypeId == EntityType.Instance.Contact().Id)
            {
                return _clientReadModel.GetContactName(entityId);
            }

            if (entityTypeId == EntityType.Instance.Deal().Id)
            {
                return _dealReadModel.GetDeal(entityId).Name;
            }

            if (entityTypeId == EntityType.Instance.Firm().Id)
            {
                return _firmReadModel.GetFirmName(entityId);
            }
            
            throw new ArgumentOutOfRangeException("entityTypeId");            
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
            return new[] { new EntityReference { EntityTypeId = EntityType.Instance.Contact().Id, Id = contactId, Name = _clientReadModel.GetContactName(contactId) } };
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
