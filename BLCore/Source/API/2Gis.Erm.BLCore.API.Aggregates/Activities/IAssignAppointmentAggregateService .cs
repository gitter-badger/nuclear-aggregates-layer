using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IAssignAppointmentAggregateService : IAggregateSpecificService<Appointment, AssignIdentity>
    {
        void Assign(Appointment appointment, long ownerCode);
    }
}