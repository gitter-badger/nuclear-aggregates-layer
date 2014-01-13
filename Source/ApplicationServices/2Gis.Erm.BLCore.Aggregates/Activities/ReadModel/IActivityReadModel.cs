using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Activities.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public interface IActivityReadModel : IAggregateReadModel<ActivityBase>
    {
        TActivity GetActivity<TActivity>(long activityId) where TActivity : ActivityBase, new();
        ActivityInstanceDto GetActivityInstanceDto<TActivity>(TActivity activity) where TActivity : ActivityBase;
        bool CheckIfRelatedActivitiesExists(long clientId);
        bool TryGetRelatedActivities(long clientId, out IEnumerable<ActivityInstance> activities);
    }
}