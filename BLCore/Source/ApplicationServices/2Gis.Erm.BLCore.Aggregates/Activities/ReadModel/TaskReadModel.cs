using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

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
            return _finder.FindOne(Specs.Find.ById<Task>(taskId));
        }

        public IEnumerable<TaskRegardingObject> GetRegardingObjects(long taskId)
        {
            return _finder.FindMany(Specs.Find.Custom<TaskRegardingObject>(x => x.SourceEntityId == taskId)).ToList();
        }

        public bool CheckIfTaskExistsRegarding(EntityName entityName, long entityId)
        {
            return _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId)).Any();
        }

        public bool CheckIfOpenTaskExistsRegarding(EntityName entityName, long entityId)
        {
            var ids = (from reference in _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId))
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.FindMany(Specs.Find.Active<Task>() &&
                                    Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress) &&
                                    Specs.Find.ByIds<Task>(ids))
                          .Any();
        }

        public IEnumerable<Task> LookupTasksRegarding(EntityName entityName, long entityId)
        {
            var ids = (from reference in _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId))
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.FindMany(Specs.Find.Active<Task>() & Specs.Find.ByIds<Task>(ids)).ToArray();
        }

        public IEnumerable<Task> LookupOpenTasksRegarding(EntityName entityName, long entityId)
        {
            var ids = (from reference in _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Task, TaskRegardingObject>(entityName, entityId))
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.FindMany(Specs.Find.Active<Task>() & Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Task>(ids)).ToArray();
        }

        public IEnumerable<long> LookupOpenTasksOwnedBy(long ownerCode)
        {
            return _finder.FindMany(Specs.Find.Owned<Task>(ownerCode) & Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress)).Select(s => s.Id).ToArray();
        }
    }
}