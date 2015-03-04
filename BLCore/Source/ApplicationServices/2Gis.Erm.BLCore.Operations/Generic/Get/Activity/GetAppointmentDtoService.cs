using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAppointmentDtoService : GetDomainEntityDtoServiceBase<Appointment>
    {
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        private readonly IActivityReferenceReader _activityReferenceReader;

        public GetAppointmentDtoService(IUserContext userContext,
                                        IAppointmentReadModel appointmentReadModel,
                                        IActivityReferenceReader activityReferenceReader,
                                        IFirmReadModel firmReadModel,
                                        IDealReadModel dealReadModel,
                                        IClientReadModel clientReadModel,
                                        IPhonecallReadModel phonecallReadModel,
                                        ILetterReadModel letterReadModel,
                                        ITaskReadModel taskReadModel)
            : base(userContext)
        {
            _appointmentReadModel = appointmentReadModel;
            _activityReferenceReader = activityReferenceReader;
            _firmReadModel = firmReadModel;
            _dealReadModel = dealReadModel;
            _clientReadModel = clientReadModel;
            _phonecallReadModel = phonecallReadModel;
            _letterReadModel = letterReadModel;
            _taskReadModel = taskReadModel;
        }

        protected override IDomainEntityDto<Appointment> GetDto(long entityId)
        {
            var appointment = _appointmentReadModel.GetAppointment(entityId);
            if (appointment == null)
            {
                throw new InvalidOperationException("The appointment does not exist for the specified ID.");
            }

            return new AppointmentDomainEntityDto
                {
                    Id = appointment.Id,
                    Header = appointment.Header,
                    Description = appointment.Description,
                    ScheduledStart = appointment.ScheduledStart,
                    ScheduledEnd = appointment.ScheduledEnd,
                    Location = appointment.Location,
                    Priority = appointment.Priority,
                    Purpose = appointment.Purpose,
                    Status = appointment.Status,
                    RegardingObjects = ReadRegardingObjects(EntityName.Appointment, entityId),
                    Attendees = ReadAttendees(EntityName.Appointment, entityId),

                    OwnerRef = new EntityReference { Id = appointment.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = appointment.CreatedBy, Name = null },
                    CreatedOn = appointment.CreatedOn,
                    ModifiedByRef = new EntityReference { Id = appointment.ModifiedBy, Name = null },
                    ModifiedOn = appointment.ModifiedOn,
                    IsActive = appointment.IsActive,
                    IsDeleted = appointment.IsDeleted,
                    Timestamp = appointment.Timestamp,
                };
        }

        protected override IDomainEntityDto<Appointment> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;
            var dto = new AppointmentDomainEntityDto
                          {
                              Priority = ActivityPriority.Average,
                              ScheduledStart = now,
                              ScheduledEnd = now.Add(TimeSpan.FromMinutes(15)),
                              Status = ActivityStatus.InProgress,
                              
                              RegardingObjects = LookupRegardingObjects(parentEntityName, parentEntityId),
                              Attendees = LookupAttendees(parentEntityName, parentEntityId),
                          };

//            var regardingObjects = LookupRegardingObjects(parentEntityName, parentEntityId);
//            var attendees = LookupAttendees(parentEntityName, parentEntityId);
//
//            if (parentEntityName.CanBeRegardingObject())
//            {
//                var regardingObject = _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId);
//                if (regardingObject.Id != null)
//                {                    
//                    dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(regardingObject);
//                    var contact = _activityReferenceReader.FindClientContact(dto.RegardingObjects);
//                    if (contact != null)
//                    {
//                        dto.Attendees = new[] { contact };
//                    }
//                }
//            }
//            else if (parentEntityName.IsActivity() && parentEntityId.HasValue)
//            {
//                dto.RegardingObjects = _activityReferenceReader.GetRegardingObjects(parentEntityName, parentEntityId.Value);
//                var attandees = _activityReferenceReader.GetAttendees(parentEntityName, parentEntityId.Value);
//                var entityReferences = attandees as EntityReference[] ?? attandees.ToArray();
//                if (entityReferences.Any() && entityReferences.Count() == 1)
//                {
//                    dto.Attendees = entityReferences;
//                }
//            }
//
//            var attendee = parentEntityName.CanBeContacted() ? _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId) : null;
//            if (attendee != null)
//            {
//                dto.Attendees = new[] { attendee };
//                dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(attendee);
//            }
//
            return dto;
        }

        private IEnumerable<EntityReference> LookupRegardingObjects(EntityName parentEntityName, long? parentEntityId)
        {
            if (parentEntityId != null)
            {
                if (parentEntityName.IsActivity())
                {
                    return ReadRegardingObjects(parentEntityName, parentEntityId.Value);
                }

                if (parentEntityName.CanBeRegardingObject() || parentEntityName.CanBeContacted())
                {
                    return ResolveRegardingObjects(parentEntityName, parentEntityId.Value);
                }
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> LookupAttendees(EntityName parentEntityName, long? parentEntityId)
        {
            if (parentEntityName.IsActivity() && parentEntityId != null)
            {
                return ReadAttendees(parentEntityName, parentEntityId.Value);
            }

            if (parentEntityName.CanBeContacted())
            {
                return ResolveAttendees(parentEntityName, parentEntityId);
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> ReadRegardingObjects(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Appointment:
                    return AdaptReferences(_appointmentReadModel.GetRegardingObjects(entityId));
//                case EntityName.Task:
//                    return AdaptReferences(_taskReadModel.GetRegardingObjects(entityId));
//                case EntityName.Letter:
//                    return AdaptReferences(_letterReadModel.GetRegardingObjects(entityId));
//                case EntityName.Phonecall:
//                    return AdaptReferences(_phonecallReadModel.GetRegardingObjects(entityId));
                default: 
                    throw new NotSupportedException("entityName");
            }
        }

        private IEnumerable<EntityReference> ResolveRegardingObjects(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Client:
                    return ResolveRegardingObjectsFromClient(entityId);
                case EntityName.Contact:
                    return ResolveRegardingObjectsFromContact(entityId);
                case EntityName.Deal:
                    return ResolveRegardingObjectsFromDeal(entityId);
                case EntityName.Firm:
                    return ResolveRegardingObjectsFromFirm(entityId);
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> ReadAttendees(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Appointment:
                    return AdaptReferences(_appointmentReadModel.GetAttendees(entityId));
//                case EntityName.Task:
//                    return Enumerable.Empty<EntityReference>();
//                case EntityName.Letter:
//                    return AdaptReferences(new[] { _letterReadModel.GetRecipient(entityId) });
//                case EntityName.Phonecall:
//                    return AdaptReferences(new[] { _phonecallReadModel.GetRecipient(entityId) });
                default:
                    throw new NotSupportedException("entityName");
            }
        }

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromClient(long clientId)
        {
            return 
                Enumerable.Repeat(ToEntityReference(EntityName.Client, clientId), 1)
                .Concat(LookupFirmReferences(clientId).Take(1))
                .Concat(LookupDealReferences(clientId).Take(1));
        }

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromFirm(long firmId)
        {
            var firmReference = ToEntityReference(EntityName.Firm, firmId);
            var clientReference = _clientReadModel.GetClientByFirm(firmId).ToReference();
            if (clientReference.Id != null)
            {
                return
                    new[] { firmReference, clientReference }
                        .Concat(LookupDealReferences(clientReference.Id.Value).Take(1));
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromDeal(long dealId)
        {
            var dealReference = ToEntityReference(EntityName.Deal, dealId);
            var clientReference = _clientReadModel.GetClientByDeal(dealId).ToReference();
            if (clientReference.Id != null)
            {
                return
                    new[] { dealReference, clientReference }                
                        .Concat(LookupFirmReferences(clientReference.Id.Value).Take(1));
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> ResolveRegardingObjectsFromContact(long contactId)
        {            
            var clientReference = _clientReadModel.GetClientByContact(contactId).ToReference();
            if (clientReference.Id != null)
            {
                return
                    Enumerable.Repeat(clientReference, 1)
                        .Concat(LookupDealReferences(clientReference.Id.Value).Take(1))
                        .Concat(LookupFirmReferences(clientReference.Id.Value).Take(1));
            }

            return Enumerable.Empty<EntityReference>();
        }

        private IEnumerable<EntityReference> LookupFirmReferences(long clientId)
        {
            return _firmReadModel.GetFirmsForClientAndLinkedChild(clientId).ResolveReferemceAmbiguity(s => new EntityReference { EntityName = EntityName.Firm, Id = s.Id, Name = s.Name });
        }

        private IEnumerable<EntityReference> LookupDealReferences(long clientId)
        {
            return _dealReadModel.GetDealsByClientId(clientId).ResolveReferemceAmbiguity(s => new EntityReference { EntityName = EntityName.Deal, Id = s.Id, Name = s.Name });
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Appointment>> references)
        {
            return references.Select(x => _activityReferenceReader.ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(EntityName entityName, long? entityId)
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
    }
}