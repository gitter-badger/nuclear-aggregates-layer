using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAppointmentDtoService : GetDomainEntityDtoServiceBase<Appointment>
    {
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public GetAppointmentDtoService(IUserContext userContext,
                                        IAppointmentReadModel appointmentReadModel,
                                        IClientReadModel clientReadModel,
                                        IDealReadModel dealReadModel,
                                        IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _appointmentReadModel = appointmentReadModel;
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

                    OwnerRef = new EntityReference { Id = appointment.OwnerCode, Name =null},
                    CreatedByRef = new EntityReference { Id = appointment.CreatedBy, Name = null },
                    CreatedOn = appointment.CreatedOn,
                    ModifiedByRef = new EntityReference { Id = appointment.ModifiedBy, Name = null },
                    ModifiedOn = appointment.ModifiedOn,
                    IsActive = appointment.IsActive,
                    IsDeleted = appointment.IsDeleted,
                    Timestamp = appointment.Timestamp,
                };
        }

        protected override IDomainEntityDto<Appointment> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;
            var dto = new AppointmentDomainEntityDto
            {
                Priority = ActivityPriority.Average,
                ScheduledStart = now,
                ScheduledEnd = now.Add(TimeSpan.FromHours(1)),
                Status = ActivityStatus.InProgress,
            };

            var regardingObject = parentEntityName.CanBeRegardingObject() ? ToEntityReference(parentEntityName.Id, parentEntityId) : null;
            if (regardingObject != null)
            {
                dto.RegardingObjects = new [] {regardingObject};
            }

            var attendee = parentEntityName.CanBeContacted() ? ToEntityReference(parentEntityName.Id, parentEntityId) : null;
            if (attendee != null)
            {
                dto.Attendees = new[] { attendee };
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Appointment>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityTypeId, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(int entityTypeId, long? entityId)
        {
            if (!entityId.HasValue)
            {
                return null;
            }

            string name;
            if (entityTypeId == EntityType.Instance.Client().Id)
            {
                name = _clientReadModel.GetClientName(entityId.Value);
            }
            else if (entityTypeId == EntityType.Instance.Contact().Id)
            {
                name = _clientReadModel.GetContactName(entityId.Value);
            }
            else if (entityTypeId == EntityType.Instance.Deal().Id)
            {
                name = _dealReadModel.GetDeal(entityId.Value).Name;
            }
            else if (entityTypeId == EntityType.Instance.Firm().Id)
            {
                name = _firmReadModel.GetFirmName(entityId.Value);
            }
            else
            {
                return null;
            }
            
            return new EntityReference { Id = entityId, Name = name, EntityTypeId = entityTypeId};
        }
   }
}