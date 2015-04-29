using System.Collections.Generic;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface ITaskReadModel : IAggregateReadModel<Task>
    {
        Task GetTask(long taskId);
        IEnumerable<TaskRegardingObject> GetRegardingObjects(long taskId);

        bool CheckIfTaskExistsRegarding(IEntityType entityName, long entityId);
        bool CheckIfOpenTaskExistsRegarding(IEntityType entityName, long entityId);

        IEnumerable<Task> LookupTasksRegarding(IEntityType entityName, long entityId);
        IEnumerable<Task> LookupOpenTasksRegarding(IEntityType entityName, long entityId);
        IEnumerable<Task> LookupOpenTasksOwnedBy(long ownerCode);
    }
}