using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    public class ActivityReferenceReader : IActivityReferenceReader
    {
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public ActivityReferenceReader(
            IAppointmentReadModel appointmentReadModel,
            ITaskReadModel taskReadModel,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,          
            IClientReadModel clientReadModel,
            IDealReadModel dealReadModel,
            IFirmReadModel firmReadModel)
        {
            _appointmentReadModel = appointmentReadModel;
            _taskReadModel = taskReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
        }

        public IEnumerable<EntityReference> GetRegardingObjects(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Appointment:
                    return AdaptReferences(_appointmentReadModel.GetRegardingObjects(entityId));
                case EntityName.Task:
                    return AdaptReferences(_taskReadModel.GetRegardingObjects(entityId));
                case EntityName.Letter:
                    return AdaptReferences(_letterReadModel.GetRegardingObjects(entityId));
                case EntityName.Phonecall:
                    return AdaptReferences(_phonecallReadModel.GetRegardingObjects(entityId));
                default: throw new NotSupportedException("entityName");
            }
        }

        public IEnumerable<EntityReference> GetAttendees(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Appointment:
                    return AdaptReferences(_appointmentReadModel.GetAttendees(entityId));
                case EntityName.Task:
                    return Enumerable.Empty<EntityReference>();
                case EntityName.Letter:
                    return AdaptReferences(new[] { _letterReadModel.GetRecipient(entityId) });
                case EntityName.Phonecall:
                    return AdaptReferences(new[] { _phonecallReadModel.GetRecipient(entityId) });
                default:
                    throw new NotSupportedException("entityName");
            }
        }

        public EntityReference FindClientContact(IEnumerable<EntityReference> references)
        {
            var client = references.FirstOrDefault(s => s.EntityName == EntityName.Client);
            if (client != null && client.Id.HasValue)
            {
                var contacts = _clientReadModel.GetClientContacts(client.Id.Value);
                return ConvertToEntityReference(contacts, s => new EntityReference { EntityName = EntityName.Contact, Name = s.FullName, Id = s.Id });
            }

            return null;
        }

        public IEnumerable<EntityReference> FindAutoCompleteReferences(EntityReference entity) 
        {
            if (entity == null || entity.Id == null)
            {
                return Enumerable.Empty<EntityReference>();
            }
            
            switch (entity.EntityName)
            {
                case EntityName.Client:
                    return FindAutoCompleteReferencesByClient(entity);                    
                case EntityName.Contact:
                    return FindAutoCompleteReferencesByContact(entity);                    
                case EntityName.Deal:
                    return FindAutoCompleteReferencesByDeal(entity);
                case EntityName.Firm:
                    return FindAutoCompleteReferencesByFirm(entity);
            }

            return Enumerable.Empty<EntityReference>();
        }

        public EntityReference ToEntityReference(EntityName entityName, long? entityId)
        {
            if (!entityId.HasValue)
            {
                return null;
            }

            string name;
            switch (entityName)
            {
                case EntityName.Client:
                    name = _clientReadModel.GetClientName(entityId.Value);
                    break;
                case EntityName.Contact:
                    name = _clientReadModel.GetContactName(entityId.Value);
                    break;
                case EntityName.Deal:
                    name = _dealReadModel.GetDeal(entityId.Value).Name;
                    break;
                case EntityName.Firm:
                    name = _firmReadModel.GetFirmName(entityId.Value);
                    break;
                default:
                    return null;
            }

            return new EntityReference { Id = entityId, Name = name, EntityName = entityName };
        }

        private IEnumerable<EntityReference> FindAutoCompleteReferencesByFirm(EntityReference entity)
        {
            var rval = new List<EntityReference> { entity };
            if (entity.Id != null)
            {
                var client = _clientReadModel.GetClientByFirm(entity.Id.Value);
                if (client != null)
                {
                    var clientRefs = new EntityReference { EntityName = EntityName.Client, Name = client.Name, Id = client.Id };
                    rval.Add(clientRefs);
                    AddDealReference(client.Id, rval);
                }
            }

            return rval.Where(s => s != null);
        }

        private IEnumerable<EntityReference> FindAutoCompleteReferencesByDeal(EntityReference entity)
        {
            var rval = new List<EntityReference> { entity };
            if (entity.Id != null)
            {
                var clientReference = FindClientReferenceByEntityId(_clientReadModel.GetClientByDeal, entity.Id.Value);

                if (clientReference != null && clientReference.Id.HasValue)
                {
                    rval.Add(clientReference);
                    AddFirmReference(clientReference.Id.Value, rval);
                }
            }

            return rval.Where(s => s != null);
        }

        private IEnumerable<EntityReference> FindAutoCompleteReferencesByContact(EntityReference entity)
        {
            var rval = new List<EntityReference>();
            if (entity.Id != null)
            {
                var clientReference = FindClientReferenceByEntityId(_clientReadModel.GetClientByContact, entity.Id.Value);                
                if (clientReference != null && clientReference.Id.HasValue)
                {
                    rval.Add(clientReference);
                    AddFirmReference(clientReference.Id.Value, rval);
                    AddDealReference(clientReference.Id.Value, rval);
                }
            }

            return rval.Where(s => s != null);
        }

        private IEnumerable<EntityReference> FindAutoCompleteReferencesByClient(EntityReference entity)
        {
            var rval = new List<EntityReference> { entity };
            if (entity.Id != null)
            {
                AddFirmReference(entity.Id.Value, rval);
                AddDealReference(entity.Id.Value, rval);
            }

            return rval.Where(s => s != null);
        }

        private EntityReference FindClientReferenceByEntityId(Func<long, Client> reader, long entityId)
        {
            var client = reader(entityId);
            if (client != null)
            {
                return new EntityReference { EntityName = EntityName.Client, Name = client.Name, Id = client.Id };
            }

            return null;
        }

        private void AddFirmReference(long clientId, List<EntityReference> rval)
        {
            var firms = _firmReadModel.GetFirmsForClientAndLinkedChild(clientId);
            var firmReference = ConvertToEntityReference(firms, s => new EntityReference { EntityName = EntityName.Firm, Id = s.Id, Name = s.Name });
            rval.Add(firmReference);
        }

        private void AddDealReference(long clientId, List<EntityReference> rval)
        {
            var deals = _dealReadModel.GetDealsByClientId(clientId);
            var dealReference = ConvertToEntityReference(deals, s => new EntityReference { EntityName = EntityName.Deal, Id = s.Id, Name = s.Name });
            rval.Add(dealReference);
        }       

        private EntityReference ConvertToEntityReference<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, EntityReference> convertToEntityReference) where TEntity : IEntity
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            if (!enumerable.Any())
            {
                return null;
            }

            if (enumerable.Count() == 1)
            {
                var convertEntity = enumerable.First();
                return convertToEntityReference(convertEntity);
            }

            return new EntityReference { EntityName = typeof(TEntity).AsEntityName() };
        }

        private IEnumerable<EntityReference> AdaptReferences<TEntity>(IEnumerable<EntityReference<TEntity>> references) where TEntity : IEntity
        {
            return references.Select(ToEntityReference).Where(x => x != null).ToList();
        }
       
        private EntityReference ToEntityReference<TEntity>(EntityReference<TEntity> entity) where TEntity : IEntity
        {
            if (entity == null)
            {
                return null;
            }

            return ToEntityReference(entity.TargetEntityName, entity.TargetEntityId);
        }
    }
}
