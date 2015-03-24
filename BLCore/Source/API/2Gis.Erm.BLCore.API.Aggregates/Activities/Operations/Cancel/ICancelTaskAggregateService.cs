﻿using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel
{
    public interface ICancelTaskAggregateService : IAggregateSpecificOperation<Task, CancelIdentity>
    {
        void Cancel(Task task);
    }
}
