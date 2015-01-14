using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface ITaskReadModel : IAggregateReadModel<Task>
    {
        Task GetTask(long taskId);
        IEnumerable<TaskRegardingObject> GetRegardingObjects(long taskId);

        bool CheckIfTaskExistsRegarding(EntityName entityName, long entityId);
        bool CheckIfOpenTaskExistsRegarding(EntityName entityName, long entityId);

        IEnumerable<Task> LookupTasksRegarding(EntityName entityName, long entityId);
        IEnumerable<Task> LookupOpenTasksRegarding(EntityName entityName, long entityId);
        IEnumerable<Task> LookupOpenTasksOwnedBy(long ownerCode);
    }
}