using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Activities.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public class ActivityReadModel : IActivityReadModel 
    {
        private readonly IFinder _finder;
        private readonly IActivityDynamicPropertiesConverter _activityDynamicPropertiesConverter;

        public ActivityReadModel(IFinder finder, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter)
        {
            _finder = finder;
            _activityDynamicPropertiesConverter = activityDynamicPropertiesConverter;
        }

        public TActivity GetActivity<TActivity>(long activityId) where TActivity : ActivityBase, new()
        {
            return _finder.Single<TActivity>(activityId, _activityDynamicPropertiesConverter);
        }

        public ActivityInstanceDto GetActivityInstanceDto<TActivity>(TActivity activity) where TActivity : ActivityBase
        {
            var dto = _finder.Find<ActivityInstance, ActivityInstanceDto>(ActivitySpecs.Activity.Select.ActivityInstanceDto(),
                                                                          Specs.Find.ById<ActivityInstance>(activity.Id))
                             .SingleOrDefault();

            var activityPropertyInstances = dto != null ? dto.ActivityPropretyInstances : new Collection<ActivityPropertyInstance>();
            var tuple = _activityDynamicPropertiesConverter.ConvertToActivityInstance(activity, activityPropertyInstances);
            return new ActivityInstanceDto
                {
                    ActivityInstance = tuple.Item1,
                    ActivityPropretyInstances = tuple.Item2
                };
        }

        public bool CheckIfRelatedActivitiesExists(long clientId)
        {
            var hasActivitiesInProgress = GetActivityInProgressDtosQuery(clientId).Any();

            return hasActivitiesInProgress;
        }

        public bool TryGetRelatedActivities(long clientId, out IEnumerable<ActivityInstance> activities)
        {
            activities = (from activityInstance in _finder.FindAll<ActivityInstance>()
                          join activityDto in GetActivityInProgressDtosQuery(clientId) on activityInstance.Id equals activityDto.Id
                          select activityInstance).ToArray();
            return activities.Any();
        }

        private IQueryable<ActivityDto> GetActivityInProgressDtosQuery(long clientId)
        {
            var clientRelatedEntitiesDto = _finder.Find(Specs.Find.ById<Client>(clientId))
                                                  .Select(x => new
                                                      {
                                                          FirmsIds = x.Firms.Select(y => y.Id),
                                                          DealIds = x.Deals.Select(y => y.Id),
                                                          ContactIds = x.Contacts.Select(y => y.Id),
                                                      })
                                                  .Single();

            return _finder.Find<ActivityInstance, ActivityDto>(ActivitySpecs.Activity.Select.ActivityDto(),
                                                               Specs.Find.ActiveAndNotDeleted<ActivityInstance>())
                          .Where(ActivitySpecs.Activity.Find.InProgress() &&
                                 ActivitySpecs.Activity.Find.RelatedToClient(clientId,
                                                                             clientRelatedEntitiesDto.FirmsIds,
                                                                             clientRelatedEntitiesDto.ContactIds,
                                                                             clientRelatedEntitiesDto.DealIds));
        }
    }
}