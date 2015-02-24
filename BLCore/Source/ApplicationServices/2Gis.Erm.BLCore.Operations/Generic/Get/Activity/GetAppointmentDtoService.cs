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

        private readonly IActivityReferenceReader _activityReferenceReader;

        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public GetAppointmentDtoService(IUserContext userContext,
                                        IAppointmentReadModel appointmentReadModel,
                                        IActivityReferenceReader activityReferenceReader,
                                        IClientReadModel clientReadModel,
                                        IDealReadModel dealReadModel,
                                        IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _appointmentReadModel = appointmentReadModel;
            _activityReferenceReader = activityReferenceReader;
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
        }

        protected override IDomainEntityDto<Appointment> GetDto(long entityId)
        {
            var appointment = _appointmentReadModel.GetAppointment(entityId);
            if (appointment == null)
            {
                throw new InvalidOperationException("The appointment does not exist for the specified ID.");
            }

            var regardingObjects = _appointmentReadModel.GetRegardingObjects(entityId);
            var attendees = _appointmentReadModel.GetAttendees(entityId);

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
                    RegardingObjects = AdaptReferences(regardingObjects),
                    Attendees = AdaptReferences(attendees),

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
            };
            EntityReference regardingObject = null;
            if (parentEntityName.CanBeRegardingObject())
            {
                dto.IsNeedLookupInitialization = true;
                regardingObject = ToEntityReference(parentEntityName, parentEntityId);
            }            
            else if (parentEntityName.IsActivity() && parentEntityId.HasValue)
            {
                dto.RegardingObjects = _activityReferenceReader.GetRegardingObjects(parentEntityName, parentEntityId.Value);
                var attandees = _activityReferenceReader.GetAttendees(parentEntityName, parentEntityId.Value);
                var entityReferences = attandees as EntityReference[] ?? attandees.ToArray();
                if (entityReferences.Any() && entityReferences.Count() == 1)
                {
                    dto.Attendees = entityReferences;
                }
            }

            if (regardingObject != null)
            {
                dto.RegardingObjects = new[] { regardingObject };
            }

            var attendee = parentEntityName.CanBeContacted() ? ToEntityReference(parentEntityName, parentEntityId) : null;
            if (attendee != null)
            {
                dto.IsNeedLookupInitialization = true;
                dto.Attendees = new[] { attendee };
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Appointment>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
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