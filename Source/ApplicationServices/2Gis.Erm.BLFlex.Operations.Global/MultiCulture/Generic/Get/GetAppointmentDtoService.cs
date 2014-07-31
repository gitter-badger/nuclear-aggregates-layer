using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetAppointmentDtoService : GetDomainEntityDtoServiceBase<Appointment>, ICyprusAdapted, IChileAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly IActivityReadModel _activityReadModel;
	    private readonly IClientReadModel _clientReadModel;
	    private readonly IFirmReadModel _firmReadModel;
	    private readonly IUserContext _userContext;

        public GetAppointmentDtoService(IUserContext userContext, IActivityReadModel activityReadModel, 
			IClientReadModel clientReadModel, IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _activityReadModel = activityReadModel;
	        _clientReadModel = clientReadModel;
	        _firmReadModel = firmReadModel;
	        _userContext = userContext;
        }

        protected override IDomainEntityDto<Appointment> GetDto(long entityId)
        {
            var appointment = _activityReadModel.GetAppointment(entityId);

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
						Name = _clientReadModel.GetClientName(parentEntityId.Value)
                    };
                    break;
				case EntityName.Contact:
					dto.ContactRef = new EntityReference
					{
						Id = parentEntityId,
						Name = _clientReadModel.GetContactName(parentEntityId.Value)
					};
					break;
				case EntityName.Firm:
                    dto.FirmRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _firmReadModel.GetFirmName(parentEntityId.Value)
                    };
                    break;
            }

            return dto;
        }
    }
}