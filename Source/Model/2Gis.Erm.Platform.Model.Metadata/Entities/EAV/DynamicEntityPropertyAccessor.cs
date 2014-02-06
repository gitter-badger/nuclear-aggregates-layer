using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class DynamicEntityPropertyAccessor
    {
        public static Expression<Action<TDynamicEntity, ICollection<TPropertyInstance>>> BuildValueGettersExpression<TDynamicEntity, TPropertyInstance>(
            this IEnumerable<IEntityPropertyIdentity> propertyIdentities,
            Func<IEnumerable<TPropertyInstance>, int, Type, object> getPropertyValueFunc)
        {
            var parameterEntity = Expression.Parameter(typeof(TDynamicEntity), "entity");
            var parameterPropertyInstances = Expression.Parameter(typeof(ICollection<TPropertyInstance>), "propertyInstances");

            var getters = new List<Expression>();
            foreach (var propertyIdentity in propertyIdentities)
            {
                var propertyExpression = Expression.Property(parameterEntity, propertyIdentity.PropertyName);

                var valueExpression = Expression.Call(getPropertyValueFunc.Method,
                                                      parameterPropertyInstances,
                                                      Expression.Constant(propertyIdentity.Id),
                                                      Expression.Constant(propertyIdentity.PropertyType));
                getters.Add(Expression.Assign(propertyExpression, Expression.Convert(valueExpression, propertyIdentity.PropertyType)));
            }

            var blockExpression = Expression.Block(getters);
            return Expression.Lambda<Action<TDynamicEntity, ICollection<TPropertyInstance>>>(blockExpression, parameterEntity, parameterPropertyInstances);
        }

        public static Expression<Action<TDynamicEntity, long, ICollection<TPropertyInstance>>> BuildValueSettersExpression<TDynamicEntity, TPropertyInstance>(
            this IEnumerable<IEntityPropertyIdentity> propertyIdentities,
            Action<ICollection<TPropertyInstance>, int, Type, object, long> setPropertyValueAction)
        {
            var parameterEntity = Expression.Parameter(typeof(TDynamicEntity), "entity");
            var parameterEntityId = Expression.Parameter(typeof(long), "entityId");
            var parameterPropertyInstances = Expression.Parameter(typeof(ICollection<TPropertyInstance>), "propertyInstances");

            var setters = new List<Expression>();
            foreach (var propertyIdentity in propertyIdentities)
            {
                var propertyExpression = Expression.Property(parameterEntity, propertyIdentity.PropertyName);

                setters.Add(Expression.Call(Expression.Constant(setPropertyValueAction.Target),
                                            setPropertyValueAction.Method,
                                            parameterPropertyInstances,
                                            Expression.Constant(propertyIdentity.Id),
                                            Expression.Constant(propertyIdentity.PropertyType),
                                            Expression.Convert(propertyExpression, typeof(object)),
                                            parameterEntityId));
            }

            var blockExpression = Expression.Block(setters);
            return Expression.Lambda<Action<TDynamicEntity, long, ICollection<TPropertyInstance>>>(blockExpression,
                                                                                                   parameterEntity,
                                                                                                   parameterEntityId,
                                                                                                   parameterPropertyInstances);
        }
    }
}