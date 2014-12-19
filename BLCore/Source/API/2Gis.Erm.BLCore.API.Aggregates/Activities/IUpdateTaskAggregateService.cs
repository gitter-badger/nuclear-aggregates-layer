using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IUpdateTaskAggregateService : IAggregateSpecificOperation<Task, UpdateIdentity>
    {
        void Update(Task task);

        void ChangeRegardingObjects(Task task,
                                    IEnumerable<TaskRegardingObject> oldReferences,
                                    IEnumerable<TaskRegardingObject> newReferences);
    }
}