using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    [Obsolete("Use generic ActivityPropertiesConverter<TActivity> implementation of IDynamicEntityPropertiesConverter<TActivity, ActivityInstance, ActivityPropertyInstance> ")]
    public class ActivityDynamicPropertiesConverter : IActivityDynamicPropertiesConverter
    {
        private readonly Dictionary<Type, object> _converters = new Dictionary<Type, object>();

        public ActivityDynamicPropertiesConverter(
            IActivityPropertiesConverter<Task> taskPropertiesConverter,
            IActivityPropertiesConverter<Phonecall> phonecallPropertiesConverter,
            IActivityPropertiesConverter<Appointment> appointmentPropertiesConverter)
        {
            _converters.Add(typeof(Task), taskPropertiesConverter);
            _converters.Add(typeof(Phonecall), phonecallPropertiesConverter);
            _converters.Add(typeof(Appointment), appointmentPropertiesConverter);
        }

        public TActivity ConvertFromActivityInstance<TActivity>(ActivityInstance activityInstance, ICollection<ActivityPropertyInstance> propertyInstances) 
            where TActivity : ActivityBase, new()
        {
            object converter;
            if (!_converters.TryGetValue(typeof(TActivity), out converter))
            {
                throw new ArgumentException("Converter for entity " + typeof(TActivity) + " not found");
            }

            var typedConverter = (IActivityPropertiesConverter<TActivity>)converter;
            return typedConverter.ConvertFromDynamicEntityInstance(activityInstance, propertyInstances);
        }

        public Tuple<ActivityInstance, ICollection<ActivityPropertyInstance>> ConvertToActivityInstance<TActivity>(TActivity activity,
                                                                                                                   ICollection<ActivityPropertyInstance> propertyInstances)
            where TActivity : ActivityBase
        {
            object converter;
            if (!_converters.TryGetValue(typeof(TActivity), out converter))
            {
                throw new ArgumentException("Converter for entity " + typeof(TActivity) + " not found");
            }

            var typedConverter = (IActivityPropertiesConverter<TActivity>)converter;
            return typedConverter.ConvertToDynamicEntityInstance(activity, propertyInstances, null);
        }
    }
}