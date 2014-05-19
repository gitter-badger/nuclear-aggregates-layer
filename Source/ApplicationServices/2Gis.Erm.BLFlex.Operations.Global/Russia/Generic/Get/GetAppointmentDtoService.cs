using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetAppointmentDtoService : GetDomainEntityDtoServiceBase<Appointment>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IActivityReadModel _activityReadModel;
        private readonly IUserContext _userContext;

        public GetAppointmentDtoService(IUserContext userContext, IFinder finder, IActivityReadModel activityReadModel)
            : base(userContext)
        {
            _finder = finder;
            _activityReadModel = activityReadModel;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Appointment> GetDto(long entityId)
        {
            var appointment = _activityReadModel.GetActivity<Appointment>(entityId);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new AppointmentDomainEntityDto
            {
                Id = appointment.Id,
                AfterSaleServiceType = appointment.AfterSaleServiceType,
                ClientRef = new EntityReference { Id = appointment.ClientId, Name = appointment.ClientName },
                ContactRef = new EntityReference { Id = appointment.ContactId, Name = appointment.ContactName },
                DealRef = new EntityReference { Id = appointment.DealId, Name = appointment.DealName },
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
                        Name = _finder.Find(Specs.Find.ById<Client>(parentEntityId.Value)).Select(x => x.Name).Single()
                    };
                    break;
                case EntityName.Deal:
                    dto.DealRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(Specs.Find.ById<Deal>(parentEntityId.Value)).Select(x => x.Name).Single()
                    };
                    break;
                case EntityName.Firm:
                    dto.FirmRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(Specs.Find.ById<Firm>(parentEntityId.Value)).Select(x => x.Name).Single()
                    };
                    break;
                case EntityName.Contact:
                    dto.ContactRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(Specs.Find.ById<Contact>(parentEntityId.Value)).Select(x => x.FullName).Single()
                    };
                    break;
            }

            return dto;
        }
    }
}