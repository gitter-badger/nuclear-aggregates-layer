using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel
{
    public static class EavFinderActivityExtensions
    {
        [Obsolete("Use Single<TActivity>(long, Func<ActivityInstance, ICollection<ActivityPropertyInstance>, TActivity>). Will be deleted soon")]
        public static TActivity Single<TActivity>(this IFinder finder, long activityId, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter) 
            where TActivity : ActivityBase, new()
        {
            return Single(finder, activityId, activityDynamicPropertiesConverter.ConvertFromActivityInstance<TActivity>);
        }

        public static TActivity Single<TActivity>(this IFinder finder,
                                                  long activityId,
                                                  Func<ActivityInstance, ICollection<ActivityPropertyInstance>, TActivity> propertiesConverter)
            where TActivity : ActivityBase, new()
        {
            var activityInstanceDto = finder.Find<ActivityInstance, ActivityInstanceDto>(ActivitySpecs.Activity.Select.ActivityInstanceDto(),
                                                                                         Specs.Find.ById<ActivityInstance>(activityId))
                                            .Single();
            var activity = propertiesConverter(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            return SetReferenceValues(finder, activity);
        }

        public static ActivityInstanceDto Single<TActivity>(
            this IFinder finder,
            TActivity activity,
            Func<TActivity, ICollection<ActivityPropertyInstance>, long?, Tuple<ActivityInstance, ICollection<ActivityPropertyInstance>>> propertiesConverter)
            where TActivity : ActivityBase, new()
        {
            var dto = finder.Find<ActivityInstance, ActivityInstanceDto>(ActivitySpecs.Activity.Select.ActivityInstanceDto(),
                                                                         Specs.Find.ById<ActivityInstance>(activity.Id))
                            .SingleOrDefault();
            var activityPropertyInstances = dto != null ? dto.ActivityPropretyInstances : new Collection<ActivityPropertyInstance>();
            var tuple = propertiesConverter(activity, activityPropertyInstances, null);
            return new ActivityInstanceDto
                {
                    ActivityInstance = tuple.Item1,
                    ActivityPropretyInstances = tuple.Item2
                };
        }

        private static TActivity SetReferenceValues<TActivity>(IFinder finder, TActivity activity) where TActivity : ActivityBase
        {
            var activityInfo = finder.Find(Specs.Find.ById<ActivityInstance>(activity.Id))
                                     .Select(x => new
                                         {
                                             ClientName = x.Client.Name,
                                             FirmName = x.Firm.Name,
                                             ContactName = x.Contact.FullName
                                         })
                                     .Single();
            activity.ClientName = activityInfo.ClientName;
            activity.FirmName = activityInfo.FirmName;
            activity.ContactName = activityInfo.ContactName;

            return activity;
        }
    }
}