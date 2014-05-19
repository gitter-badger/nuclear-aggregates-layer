using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IActivityAggregateService : IAggregateRootRepository<ActivityBase>
    {
        long CreateActivity(ActivityInstance activityInstance, IEnumerable<ActivityPropertyInstance> activityPropertyInstances);
        void UpdateActivity(ActivityInstance activityInstance, IEnumerable<ActivityPropertyInstance> activityPropertyInstances);

        void DeleteActivity(ActivityInstance activityInstance, IEnumerable<ActivityPropertyInstance> activityPropertyInstances);

        bool AssignClientRelatedActivities(IEnumerable<ActivityInstance> relatedActivities);
    }
}