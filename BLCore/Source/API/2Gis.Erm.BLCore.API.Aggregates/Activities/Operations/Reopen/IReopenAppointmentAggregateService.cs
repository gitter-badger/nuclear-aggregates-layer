using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen
{
    public interface IReopenAppointmentAggregateService : IAggregateSpecificOperation<Appointment, ReopenIdentity>
    {
        void Reopen(Appointment appointment);
    }
}
