using System;

using DoubleGis.Erm.BL.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: вынесено из BL.Operations - уже копия в данном проекте, похоже на дублирование файлов в TFS из-за многочисленных merge - пока оставлены обе копии, при RI из 1.0 нужно обращать внимание какой целевой файл выбирается из 2ух
    // указан модификатор доступа internal, чтобы не подхватывался massprocessor
    internal sealed class CzechAppointmentObtainer : IBusinessModelEntityObtainer<Appointment>, IAggregateReadModel<ActivityBase>, ICzechAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IActivityDynamicPropertiesConverter _activityDynamicPropertiesConverter;

        public CzechAppointmentObtainer(IUserContext userContext, IFinder finder, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter)
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

            appointment.ClientId = dto.ClientRef.Id;
            appointment.ContactId = dto.ContactRef.Id;
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