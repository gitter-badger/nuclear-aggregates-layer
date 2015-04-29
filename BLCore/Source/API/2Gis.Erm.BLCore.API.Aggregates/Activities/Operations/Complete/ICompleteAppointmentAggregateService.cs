using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete
{
    public interface ICompleteAppointmentAggregateService : IAggregateSpecificOperation<Appointment, CompleteIdentity>
    {
        void Complete(Appointment appointment);
    }
}
