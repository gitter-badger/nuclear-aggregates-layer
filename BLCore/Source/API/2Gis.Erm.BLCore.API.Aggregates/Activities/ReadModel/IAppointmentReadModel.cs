using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IAppointmentReadModel : IAggregateReadModel<Appointment>
    {
        Appointment GetAppointment(long appointmentId);
        IEnumerable<AppointmentRegardingObject> GetRegardingObjects(long appointmentId);
        IEnumerable<AppointmentAttendee> GetAttendees(long appointmentId);
        AppointmentOrganizer GetOrganizer(long appointmentId);

        bool CheckIfAppointmentExistsRegarding(IEntityType entityName, long entityId);
        bool CheckIfOpenAppointmentExistsRegarding(IEntityType entityName, long entityId);

        IEnumerable<Appointment> LookupAppointmentsRegarding(IEntityType entityName, long entityId);
        IEnumerable<Appointment> LookupOpenAppointmentsRegarding(IEntityType entityName, long entityId);
        IEnumerable<Appointment> LookupOpenAppointmentsOwnedBy(long ownerCode);
    }
}