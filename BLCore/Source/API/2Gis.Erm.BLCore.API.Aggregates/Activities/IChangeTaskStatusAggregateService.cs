using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IChangeTaskStatusAggregateService : IAggregateSpecificOperation<Task, ChangeActivityStatusIdentity>
    {
        void Change(Task task, ActivityStatus status);
    }
}
