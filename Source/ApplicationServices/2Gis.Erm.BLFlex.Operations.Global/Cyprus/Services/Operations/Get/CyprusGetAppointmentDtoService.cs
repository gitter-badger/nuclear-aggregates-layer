using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Activities;
using DoubleGis.Erm.BL.Operations.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Services.Operations.Get
{
    public class CyprusGetAppointmentDtoService : GetDomainEntityDtoServiceBase<Appointment>, ICyprusAdapted
    {
        private readonly IFinder _finder;
        private readonly IActivityService _activityService;
        private readonly IUserContext _userContext;

        public CyprusGetAppointmentDtoService(IUserContext userContext, IFinder finder, IActivityService activityService)
            : base(userContext)
        {
            _finder = finder;
            _activityService = activityService;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Appointment> GetDto(long entityId)
        {
            var appointment = _activityService.GetAppointment(entityId);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new AppointmentDomainEntityDto
            {
                Id = appointment.Id,
                ClientRef = new EntityReference { Id = appointment.ClientId, Name = appointment.ClientName },
                ContactRef = new EntityReference { Id = appointment.ContactId, Name = appointment.ContactName },
                Description = appointment.Description,
                FirmRef = new EntityReference { Id = appointment.FirmId, Name = appointment.FirmName },
                Header = appointment.Header,
                Priority = appointment.Priority,
                Purpose = appointment.Purpose,
                ScheduledEnd = appointment.ScheduledEnd.Add(timeOffset),
                ScheduledStart = appointment.ScheduledStart.Add(timeOffset),
                ActualEnd = appointment.ActualEnd.HasValue ? appointment.ActualEnd.Value.Add(timeOffset) : appointment.ActualEnd,
                Status = appointment.Status,
                Type = appointment.Type,
                OwnerRef = new EntityReference { Id = appointment.OwnerCode, Name = null },
                CreatedByRef = new EntityReference { Id = appointment.CreatedBy, Name = null },
                CreatedOn = appointment.CreatedOn,
                IsActive = appointment.IsActive,
                IsDeleted = appointment.IsDeleted,
                ModifiedByRef = new EntityReference { Id = appointment.ModifiedBy, Name = null },
                ModifiedOn = appointment.ModifiedOn,
                Timestamp = appointment.Timestamp
            };
        }

        protected override IDomainEntityDto<Appointment> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;
            var dto = new AppointmentDomainEntityDto
            {
                Type = ActivityType.Appointment,
                IsActive = true,
                ScheduledStart = now,
                ScheduledEnd = now.Add(TimeSpan.FromMinutes(15.0)),
                Priority = ActivityPriority.Average,
                Status = ActivityStatus.InProgress,
            };

            if (parentEntityId == null)
            {
                return dto;
            }

            switch (parentEntityName)
            {
                case EntityName.Client:
                    dto.ClientRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(GenericSpecifications.ById<Client>(parentEntityId.Value)).Select(x => x.Name).Single()
                    };
                    break;
                case EntityName.Firm:
                    dto.FirmRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(GenericSpecifications.ById<Firm>(parentEntityId.Value)).Select(x => x.Name).Single()
                    };
                    break;
                case EntityName.Contact:
                    dto.ContactRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(GenericSpecifications.ById<Contact>(parentEntityId.Value)).Select(x => x.FullName).Single()
                    };
                    break;
            }

            return dto;
        }
    }
}