using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IAssignTaskAggregateService : IAggregateSpecificService<Task, AssignIdentity>
    {
        void Assign(Task task, long ownerCode);
    }
}