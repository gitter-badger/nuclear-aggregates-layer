using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Activities.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public static class EavFinderExtensions
    {
        public static TActivity Single<TActivity>(this IFinder finder, long activityId, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter) 
            where TActivity : ActivityBase, new()
        {
            var activityInstanceDto = finder.Find<ActivityInstance, ActivityInstanceDto>(ActivitySpecs.Activity.Select.ActivityInstanceDto(),
                                                                                         Specs.Find.ById<ActivityInstance>(activityId))
                                            .Single();
            var activity = activityDynamicPropertiesConverter.ConvertFromActivityInstance<TActivity>(activityInstanceDto.ActivityInstance,
                                                                                                     activityInstanceDto.ActivityPropretyInstances);
            return SetReferenceValues(finder, activity);
        }

        private static TActivity SetReferenceValues<TActivity>(IFinder finder, TActivity activity) where TActivity : ActivityBase
        {
            var activityInfo = finder.Find(Specs.Find.ById<ActivityInstance>(activity.Id))
                                     .Select(x => new
                                         {
                                             ClientName = x.Client.Name,
                                             FirmName = x.Client.Name,
                                             ContactName = x.Client.Name
                                         })
                                     .Single();
            activity.ClientName = activityInfo.ClientName;
            activity.FirmName = activityInfo.FirmName;
            activity.ContactName = activityInfo.ContactName;

            return activity;
        }
    }
}