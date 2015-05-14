using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class UpdateAppointmentAggregationService : IUpdateAppointmentAggregateService
    {
        private const string ActivityHasNoTheIdentityMessage = "The appointment has no the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Appointment> _repository;
        private readonly IRepository<AppointmentRegardingObject> _referenceRepository;
        private readonly IRepository<AppointmentAttendee> _attendeeRepository;
        private readonly IRepository<AppointmentOrganizer> _organizerRepository;

        public UpdateAppointmentAggregationService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Appointment> repository,
            IRepository<AppointmentRegardingObject> referenceRepository,
            IRepository<AppointmentAttendee> attendeeRepository,
            IRepository<AppointmentOrganizer> organizerRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
            _referenceRepository = referenceRepository;
            _attendeeRepository = attendeeRepository;
            _organizerRepository = organizerRepository;
        }

        public void Update(Appointment appointment)
        {
            CheckAppointment(appointment);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Appointment>())
            {
                _repository.Update(appointment);
                _repository.Save();
                
                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }

        public void UpdateAttendees(Appointment appointment, IEnumerable<AppointmentAttendee> oldAttendees, IEnumerable<AppointmentAttendee> newAttendees)
        {
            CheckAppointment(appointment);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Appointment>())
            {
                _attendeeRepository.Update<Appointment, AppointmentAttendee>(oldAttendees, newAttendees);
                _attendeeRepository.Save();
                
                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }

        public void ChangeRegardingObjects(Appointment appointment, IEnumerable<AppointmentRegardingObject> oldReferences, IEnumerable<AppointmentRegardingObject> newReferences)
        {
            CheckAppointment(appointment);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Appointment>())
            {
                _referenceRepository.Update<Appointment, AppointmentRegardingObject>(oldReferences, newReferences);

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }

        public void ChangeOrganizer(Appointment appointment, AppointmentOrganizer oldReference, AppointmentOrganizer newReference)
        {
            CheckAppointment(appointment);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Appointment>())
            {
                _organizerRepository.Update<Appointment, AppointmentOrganizer>(oldReference, newReference);

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }

        }

        private static void CheckAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }
            if (appointment.Id == 0)
            {
                throw new ArgumentException(ActivityHasNoTheIdentityMessage, "appointment");
            }
        }
    }
}