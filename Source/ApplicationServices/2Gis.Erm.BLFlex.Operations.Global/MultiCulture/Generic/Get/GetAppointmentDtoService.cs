using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
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

        public GetAppointmentDtoService(IUserContext userContext,
                                        IActivityReadModel activityReadModel,
                                        IClientReadModel clientReadModel,
                                        IFirmReadModel firmReadModel)
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
            if (appointment == null)
            {
                throw new NotificationException(string.Format(BLResources.CannotFindActivity, entityId));
            }

            var regardingObjects = _activityReadModel.GetRegardingObjects<Appointment>(entityId).ToList();

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new AppointmentDomainEntityDto
            {
                Id = appointment.Id,
                CreatedByRef = new EntityReference { Id = appointment.CreatedBy, Name = null },
                CreatedOn = appointment.CreatedOn,
                ModifiedByRef = new EntityReference { Id = appointment.ModifiedBy, Name = null },
                ModifiedOn = appointment.ModifiedOn,
                IsActive = appointment.IsActive,
                IsDeleted = appointment.IsDeleted,
                Timestamp = appointment.Timestamp,
                OwnerRef = new EntityReference { Id = appointment.OwnerCode, Name = null },

                Header = appointment.Header,
                Description = appointment.Description,
                ScheduledStart = appointment.ScheduledStart.Add(timeOffset),
                ScheduledEnd = appointment.ScheduledEnd.Add(timeOffset),
                ActualEnd = appointment.ActualEnd.HasValue ? appointment.ActualEnd.Value.Add(timeOffset) : appointment.ActualEnd,
                Priority = appointment.Priority,
                Status = appointment.Status,

                ClientRef = regardingObjects.Lookup(EntityName.Client, _clientReadModel.GetClientName),
                ContactRef = regardingObjects.Lookup(EntityName.Contact, _clientReadModel.GetContactName),
                FirmRef = regardingObjects.Lookup(EntityName.Firm, _firmReadModel.GetFirmName),

                Purpose = appointment.Purpose,
                // AfterSaleServiceType = appointment.AfterSaleServiceType,
            };
        }

        protected override IDomainEntityDto<Appointment> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;
            var dto = new AppointmentDomainEntityDto
            {
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