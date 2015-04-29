using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IUpdateAppointmentAggregateService : IAggregateSpecificOperation<Appointment, UpdateIdentity>
    {
        void Update(Appointment appointment);

        void UpdateAttendees(Appointment appointment,
                             IEnumerable<AppointmentAttendee> oldAttendees,
                             IEnumerable<AppointmentAttendee> newAttendees);

        void ChangeRegardingObjects(Appointment appointment,
                                    IEnumerable<AppointmentRegardingObject> oldReferences,
                                    IEnumerable<AppointmentRegardingObject> newReferences);

        void ChangeOrganizer(Appointment appointment,
                                    AppointmentOrganizer oldReference,
                                    AppointmentOrganizer newReference);

    }
}