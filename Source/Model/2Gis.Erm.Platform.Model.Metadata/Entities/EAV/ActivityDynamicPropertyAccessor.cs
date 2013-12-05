using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class ActivityDynamicPropertyAccessor
    {
        public static TProperty GetPropertyValue<TProperty>(this IEnumerable<ActivityPropertyInstance> propertyInstances, int propertyId)
        {
            var property = propertyInstances.FirstOrDefault(x => x.PropertyId == propertyId);
            var getter = ActivityDynamicPropertyMapper.GetGetter(typeof(TProperty));
            return (TProperty)getter(property);
        }

        public static void SetPropertyValue<TProperty>(this ICollection<ActivityPropertyInstance> propertyInstances, int propertyId, TProperty value, long activityId)
        {
            var property = propertyInstances.FirstOrDefault(x => x.PropertyId == propertyId);
            if (property == null)
            {
                property = new ActivityPropertyInstance { ActivityId = activityId, PropertyId = propertyId };
                propertyInstances.Add(property);
            }

            var setter = ActivityDynamicPropertyMapper.GetSetter(typeof(TProperty));
            setter(property, value);
        }

        public static Expression<Action<TActivity, ICollection<ActivityPropertyInstance>>> BuildValueGettersExpression<TActivity>(
            this IEnumerable<IEntityPropertyIdentity> propertyIdentities)
            where TActivity : ActivityBase
        {
            var parameterEntity = Expression.Parameter(typeof(TActivity), "param");
            var parameterPropertyInstances = Expression.Parameter(typeof(ICollection<ActivityPropertyInstance>), "propertyInstances");

            var getters = new List<Expression>();
            foreach (var propertyIdentity in propertyIdentities)
            {
                var propertyExpression = Expression.Property(parameterEntity, propertyIdentity.PropertyName);
                var getPropertyMethodInfo = typeof(ActivityDynamicPropertyAccessor).GetMethod("GetPropertyValue")
                                                                                  .MakeGenericMethod(new[] { propertyIdentity.PropertyType });
                var valueExpression = Expression.Call(getPropertyMethodInfo,
                                                      parameterPropertyInstances,
                                                      Expression.Constant(propertyIdentity.Id));
                getters.Add(Expression.Assign(propertyExpression, valueExpression));
            }

            var blockExpression = Expression.Block(getters);
            return Expression.Lambda<Action<TActivity, ICollection<ActivityPropertyInstance>>>(blockExpression, parameterEntity, parameterPropertyInstances);
        }

        public static Expression<Action<TActivity, long, ICollection<ActivityPropertyInstance>>> BuildValueSettersExpression<TActivity>(
            this IEnumerable<IEntityPropertyIdentity> propertyIdentities)
            where TActivity : ActivityBase
        {
            var parameterEntity = Expression.Parameter(typeof(TActivity), "param");
            var parameterActivityId = Expression.Parameter(typeof(long), "activityId");
            var parameterPropertyInstances = Expression.Parameter(typeof(ICollection<ActivityPropertyInstance>), "propertyInstances");

            var setters = new List<Expression>();
            foreach (var propertyIdentity in propertyIdentities)
            {
                var propertyExpression = Expression.Property(parameterEntity, propertyIdentity.PropertyName);

                var setPropertyMethodInfo = typeof(ActivityDynamicPropertyAccessor).GetMethod("SetPropertyValue")
                                                                                   .MakeGenericMethod(new[] { propertyIdentity.PropertyType });

                setters.Add(Expression.Call(setPropertyMethodInfo,
                                            parameterPropertyInstances,
                                            Expression.Constant(propertyIdentity.Id),
                                            propertyExpression,
                                            parameterActivityId));
            }

            var blockExpression = Expression.Block(setters);
            return Expression.Lambda<Action<TActivity, long, ICollection<ActivityPropertyInstance>>>(blockExpression,
                                                                                                     parameterEntity,
                                                                                                     parameterActivityId,
                                                                                                     parameterPropertyInstances);
        }
    }
}