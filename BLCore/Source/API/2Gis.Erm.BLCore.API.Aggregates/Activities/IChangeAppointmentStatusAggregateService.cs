using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.CancelActivity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IChangeAppointmentStatusAggregateService : IAggregateSpecificOperation<Appointment, CancelActivityIdentity>
    {
        void Change(Appointment appointment, ActivityStatus status);
    }
}
