using System;

using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public sealed class AppointmentObtainer : IBusinessModelEntityObtainer<Appointment>, IAggregateReadModel<ActivityBase>, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IActivityDynamicPropertiesConverter _activityDynamicPropertiesConverter;

        public AppointmentObtainer(IUserContext userContext, IFinder finder, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter)
        {
            _userContext = userContext;
            _finder = finder;
            _activityDynamicPropertiesConverter = activityDynamicPropertiesConverter;
        }

        public Appointment ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AppointmentDomainEntityDto)domainEntityDto;

            var appointment = dto.IsNew() ? new Appointment { IsActive = true } : _finder.Single<Appointment>(dto.Id, _activityDynamicPropertiesConverter);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            appointment.AfterSaleServiceType = dto.AfterSaleServiceType;
            appointment.ClientId = dto.ClientRef.Id;
            appointment.ContactId = dto.ContactRef.Id;
            appointment.DealId = dto.DealRef.Id;
            appointment.Description = dto.Description;
            appointment.FirmId = dto.FirmRef.Id;
            appointment.Header = dto.Header;
            appointment.Priority = dto.Priority;
            appointment.Purpose = dto.Purpose;
            appointment.ScheduledStart = dto.ScheduledStart.Subtract(timeOffset);
            appointment.ScheduledEnd = dto.ScheduledEnd.Subtract(timeOffset);
            appointment.ActualEnd = dto.ActualEnd.HasValue ? dto.ActualEnd.Value.Subtract(timeOffset) : dto.ActualEnd;
            appointment.Status = dto.Status;
            appointment.Type = dto.Type;
            appointment.OwnerCode = dto.OwnerRef.Id.Value;
            appointment.IsActive = dto.IsActive;
            appointment.IsDeleted = dto.IsDeleted;

            appointment.Timestamp = dto.Timestamp;

            return appointment;
        }
    }
}
