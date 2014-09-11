using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
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

        public bool CheckIfRelatedActivitiesExists(long clientId)
        {
            return _finder.FindMany(Specs.Find.Custom<TaskRegardingObject>(x => x.TargetEntityId == clientId)).Any();
        }
    }
}