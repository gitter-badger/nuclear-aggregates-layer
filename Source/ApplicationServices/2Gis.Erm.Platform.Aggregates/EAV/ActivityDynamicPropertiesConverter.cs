using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public class ActivityDynamicPropertiesConverter : IActivityDynamicPropertiesConverter
    {
        private readonly Dictionary<Type, object> _convertersFromPersistence = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _convertersToPersistence = new Dictionary<Type, object>();

        public ActivityDynamicPropertiesConverter()
        {
            BuildConverterFromPersistenceEntity<Task>();
            BuildConverterFromPersistenceEntity<Phonecall>();
            BuildConverterFromPersistenceEntity<Appointment>();

            BuildConverterToPersistenceEntity<Task>();
            BuildConverterToPersistenceEntity<Phonecall>();
            BuildConverterToPersistenceEntity<Appointment>();
        }

        public TActivity ConvertFromActivityInstance<TActivity>(ActivityInstance activityInstance, ICollection<ActivityPropertyInstance> propertyInstances) 
            where TActivity : ActivityBase, new()
        {
            object action;
            if (!_convertersFromPersistence.TryGetValue(typeof(TActivity), out action))
            {
                throw new ArgumentException("Converter for entity " + typeof(TActivity) + " not found");
            }

            var activity = new TActivity
                {
                    Id = activityInstance.Id,
                    ClientId = activityInstance.ClientId,
                    ContactId = activityInstance.ContactId,
                    FirmId = activityInstance.FirmId,
                    CreatedBy = activityInstance.CreatedBy,
                    CreatedOn = activityInstance.CreatedOn,
                    ModifiedBy = activityInstance.ModifiedBy,
                    ModifiedOn = activityInstance.ModifiedOn,
                    IsActive = activityInstance.IsActive,
                    IsDeleted = activityInstance.IsDeleted,
                    OwnerCode = activityInstance.OwnerCode,
                    Timestamp = activityInstance.Timestamp,
                    Type = (ActivityType)activityInstance.Type
                };

            var convertFromPersistenceEntityAction = (Action<TActivity, ICollection<ActivityPropertyInstance>>)action;
            convertFromPersistenceEntityAction(activity, propertyInstances);
            
            return activity;
        }

        public Tuple<ActivityInstance, ICollection<ActivityPropertyInstance>> ConvertToActivityInstance<TActivity>(TActivity activity,
                                                                                                                   ICollection<ActivityPropertyInstance> propertyInstances)
            where TActivity : ActivityBase
        {
            object action;
            if (!_convertersToPersistence.TryGetValue(typeof(TActivity), out action))
            {
                throw new ArgumentException("Converter for entity " + typeof(TActivity) + " not found");
            }

            var activityInstance = new ActivityInstance
                {
                    Id = activity.Id,
                    ClientId = activity.ClientId,
                    ContactId = activity.ContactId,
                    FirmId = activity.FirmId,
                    CreatedBy = activity.CreatedBy,
                    CreatedOn = activity.CreatedOn,
                    ModifiedBy = activity.ModifiedBy,
                    ModifiedOn = activity.ModifiedOn,
                    IsActive = activity.IsActive,
                    IsDeleted = activity.IsDeleted,
                    OwnerCode = activity.OwnerCode,
                    Timestamp = activity.Timestamp,
                    Type = (int)activity.Type
                };

            var convertToPersistenceEntityAction = (Action<TActivity, long, ICollection<ActivityPropertyInstance>>)action;
            convertToPersistenceEntityAction(activity, activityInstance.Id, propertyInstances);

            return Tuple.Create(activityInstance, propertyInstances);
        }

        private void BuildConverterFromPersistenceEntity<T>() where T : ActivityBase
        {
            var propertyIdentities = ActivityMetadataRegistry.GetPropertyIdentities<T>();
            var valueGettersExpression = propertyIdentities.BuildValueGettersExpression<T>();
            _convertersFromPersistence.Add(typeof(T), valueGettersExpression.Compile());
        }

        private void BuildConverterToPersistenceEntity<T>() where T : ActivityBase
        {
            var propertyIdentities = ActivityMetadataRegistry.GetPropertyIdentities<T>();
            var valueSettersExpression = propertyIdentities.BuildValueSettersExpression<T>();
            _convertersToPersistence.Add(typeof(T), valueSettersExpression.Compile());
        }
    }
}