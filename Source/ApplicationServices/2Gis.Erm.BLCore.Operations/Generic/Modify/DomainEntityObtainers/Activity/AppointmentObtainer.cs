﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
                                  ? new Appointment { IsActive = true }
                                  : _finder.FindOne(Specs.Find.ById<Appointment>(dto.Id));

            appointment.Header = dto.Header;
            appointment.Description = dto.Description;
            appointment.ScheduledStart = dto.ScheduledStart;
            appointment.ScheduledEnd = dto.ScheduledEnd;
            appointment.Purpose = dto.Purpose;
            appointment.Status = dto.Status;
            appointment.Location = dto.Location;
            appointment.OwnerCode = dto.OwnerRef.GetId();

            return appointment;
        }
    }
}
