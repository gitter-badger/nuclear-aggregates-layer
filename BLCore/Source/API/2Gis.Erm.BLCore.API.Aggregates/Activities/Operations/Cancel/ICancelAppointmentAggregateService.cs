﻿using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel
{
    public interface ICancelAppointmentAggregateService : IAggregateSpecificService<Appointment, CancelIdentity>
    {
        void Cancel(Appointment appointment);
    }
}
