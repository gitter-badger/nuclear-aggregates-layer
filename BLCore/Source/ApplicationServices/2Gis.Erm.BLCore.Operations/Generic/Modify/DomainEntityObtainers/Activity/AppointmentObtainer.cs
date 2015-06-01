using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AppointmentObtainer : IBusinessModelEntityObtainer<Appointment>, IAggregateReadModel<Appointment>
    {
        private readonly IFinder _finder;

        public AppointmentObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Appointment ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AppointmentDomainEntityDto)domainEntityDto;

            var appointment = dto.IsNew()
                                  ? new Appointment { IsActive = true, Status = dto.Status, OwnerCode = dto.OwnerRef.GetId() }
                                  : _finder.Find(Specs.Find.ById<Appointment>(dto.Id)).One();

            appointment.Header = dto.Header;
            appointment.Description = dto.Description;
            appointment.ScheduledStart = dto.ScheduledStart;
            appointment.ScheduledEnd = dto.ScheduledEnd;
            appointment.Priority = dto.Priority;
            appointment.Purpose = dto.Purpose;
            appointment.Location = dto.Location;
            appointment.Timestamp = dto.Timestamp;

            return appointment;
        }
    }
}
