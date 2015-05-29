using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IUpdateTaskAggregateService : IAggregateSpecificService<Task, UpdateIdentity>
    {
        void Update(Task task);

        void ChangeRegardingObjects(Task task,
                                    IEnumerable<TaskRegardingObject> oldReferences,
                                    IEnumerable<TaskRegardingObject> newReferences);
    }
}