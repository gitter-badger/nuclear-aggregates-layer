using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
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

        public GetAppointmentDtoService(IUserContext userContext,
                                        IAppointmentReadModel appointmentReadModel,
                                        IActivityReferenceReader activityReferenceReader)
            : base(userContext)
        {
            _appointmentReadModel = appointmentReadModel;
            _activityReferenceReader = activityReferenceReader;            
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
            if (parentEntityName.CanBeRegardingObject())
            {
                EntityReference regardingObject = _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId);
                if (regardingObject.Id != null)
                {                    
                    dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(regardingObject);
                    var contact = _activityReferenceReader.FindClientContact(dto.RegardingObjects);
                    if (contact != null)
                    {
                        dto.Attendees = new[] { contact };
                    }
                }
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

            var attendee = parentEntityName.CanBeContacted() ? _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId) : null;
            if (attendee != null)
            {
                dto.Attendees = new[] { attendee };
                dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(attendee);
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Appointment>> references)
        {
            return references.Select(x => _activityReferenceReader.ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }
    }
}