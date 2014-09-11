using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface ITaskReadModel : IAggregateReadModel<Task>
    {
        Task GetTask(long taskId);

        IEnumerable<TaskRegardingObject> GetRegardingObjects(long taskId);

        bool CheckIfRelatedActivitiesExists(long clientId);
    }
}