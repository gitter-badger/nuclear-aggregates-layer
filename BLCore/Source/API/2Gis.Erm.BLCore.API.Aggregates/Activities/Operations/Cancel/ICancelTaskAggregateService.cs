using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel
{
    public interface ICancelTaskAggregateService : IAggregateSpecificOperation<Task, CancelIdentity>
    {
        void Cancel(Task task);
    }
}
