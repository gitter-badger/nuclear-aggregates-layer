using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public sealed class TaskReadModel : ITaskReadModel
    {
        private readonly IFinder _finder;

        public TaskReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Task GetTask(long taskId)
        {
            return _finder.Find(Specs.Find.ById<Task>(taskId)).One();
        }

        public IEnumerable<TaskRegardingObject> GetRegardingObjects(long taskId)
        {
            return _finder.Find(Specs.Find.Custom<TaskRegardingObject>(x => x.SourceEntityId == taskId)).Many();
        }

        public bool CheckIfTaskExistsRegarding(IEntityType entityName, long entityId)
        {
            return _finder.Find(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId)).Any();
        }

        public bool CheckIfOpenTaskExistsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Task>() &&
                                    Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress) &&
                                    Specs.Find.ByIds<Task>(ids))
                          .Any();
        }

        public IEnumerable<Task> LookupTasksRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Task>() & Specs.Find.ByIds<Task>(ids)).Many();
        }

        public IEnumerable<Task> LookupOpenTasksRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Task>() & Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Task>(ids)).Many();
        }

        public IEnumerable<Task> LookupOpenTasksOwnedBy(long ownerCode)
        {
            return _finder.Find(Specs.Find.Owned<Task>(ownerCode) & Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress)).Many();
        }
    }
}