using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAppointmentDtoService : GetDomainEntityDtoServiceBase<Appointment>
    {
        private readonly IAppointmentReadModel _appointmentReadModel;

        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForRegardingObjects;
        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForAttendees;

        public GetAppointmentDtoService(IUserContext userContext,
                                        IAppointmentReadModel appointmentReadModel,
                                        IClientReadModel clientReadModel,
                                        IFirmReadModel firmReadModel,
                                        IDealReadModel dealReadModel,
                                        ILetterReadModel letterReadModel,
                                        IPhonecallReadModel phonecallReadModel,
                                        ITaskReadModel taskReadModel)
            : base(userContext)
        {
            _appointmentReadModel = appointmentReadModel;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _dealReadModel = dealReadModel;

            var service = new ActivityReferenceReader(clientReadModel, dealReadModel, firmReadModel);

            _lookupsForRegardingObjects = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Task, entityId => taskReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, entityId => service.ResolveRegardingObjectsFromClient(entityId) },
                { EntityName.Contact, entityId => service.ResolveRegardingObjectsFromContact(entityId) },
                { EntityName.Deal, entityId => service.ResolveRegardingObjectsFromDeal(entityId) },
                { EntityName.Firm, entityId => service.ResolveRegardingObjectsFromFirm(entityId) },
            };

            _lookupsForAttendees = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetAttendees(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, entityId => service.ResolveContactsFromClient(entityId) },
                { EntityName.Contact, entityId => service.ResolveContactsFromContact(entityId) },
                { EntityName.Deal, entityId => service.ResolveContactsFromDeal(entityId) },
                { EntityName.Firm, entityId => service.ResolveContactsFromFirm(entityId) },
            };
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
                    RegardingObjects = _lookupsForRegardingObjects.LookupElements(EntityName.Appointment, entityId),
                    Attendees = _lookupsForAttendees.LookupElements(EntityName.Appointment, entityId),

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
            
            return new AppointmentDomainEntityDto
                       {
                           Priority = ActivityPriority.Average,
                           ScheduledStart = now,
                           ScheduledEnd = now.Add(TimeSpan.FromMinutes(15)),
                           Status = ActivityStatus.InProgress,
                              
                           RegardingObjects = _lookupsForRegardingObjects.LookupElements(parentEntityName, parentEntityId),
                           Attendees = _lookupsForAttendees.LookupElements(parentEntityName, parentEntityId),
                       };
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
    }
}