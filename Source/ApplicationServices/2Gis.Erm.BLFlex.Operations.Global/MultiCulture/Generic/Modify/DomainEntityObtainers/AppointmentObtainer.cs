using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public sealed class AppointmentObtainer : IBusinessModelEntityObtainer<Appointment>, IAggregateReadModel<Appointment>, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;

        public AppointmentObtainer(IUserContext userContext, IFinder finder)
        {
            _userContext = userContext;
            _finder = finder;
        }

        public Appointment ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AppointmentDomainEntityDto)domainEntityDto;

            var appointment = dto.IsNew() ? new Appointment { IsActive = true } : _finder.FindOne(Specs.Find.ById<Appointment>(dto.Id));

            // FIXME {s.pomadin, 21.08.2014}: Смещение времени относительно UTC должно быть выполнено на клиентской части
            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            appointment.Description = dto.Description;
            appointment.Header = dto.Header;
            appointment.Priority = dto.Priority;
            appointment.Purpose = dto.Purpose;
            appointment.ScheduledStart = dto.ScheduledStart.Subtract(timeOffset);
            appointment.ScheduledEnd = dto.ScheduledEnd.Subtract(timeOffset);
            appointment.ActualEnd = dto.ActualEnd.HasValue ? dto.ActualEnd.Value.Subtract(timeOffset) : dto.ActualEnd;
            appointment.Status = dto.Status;
            appointment.OwnerCode = dto.OwnerRef.Id.Value;
            appointment.IsActive = dto.IsActive;
            appointment.IsDeleted = dto.IsDeleted;

            appointment.Timestamp = dto.Timestamp;

            return appointment;
        }
    }
}
