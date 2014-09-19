using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IAppointmentReadModel : IAggregateReadModel<Appointment>
    {
        Appointment GetAppointment(long appointmentId);

        IEnumerable<AppointmentRegardingObject> GetRegardingObjects(long appointmentId);
        
        IEnumerable<AppointmentAttendee> GetAttendees(long appointmentId);

        bool CheckIfRelatedActivitiesExists(EntityName entityName, long entityId);

        bool CheckIfRelatedActiveActivitiesExists(EntityName entityName, long entityId);

        IEnumerable<Appointment> LookupRelatedActivities(EntityName entityName, long entityId);
    }
}