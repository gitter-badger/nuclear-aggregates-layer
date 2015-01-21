using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Identities.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class DynamicEntityPropertyAccessor
    {
        public static Expression<Action<TEntity, ICollection<TPropertyInstance>>> BuildValueGettersExpression<TEntity, TPropertyInstance>(
            this IEnumerable<IEntityPropertyIdentity> propertyIdentities,
            Func<IEnumerable<TPropertyInstance>, int, Type, object> getPropertyValueFunc)
        {
            var parameterEntity = Expression.Parameter(typeof(TEntity), "entity");
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
            return Expression.Lambda<Action<TEntity, ICollection<TPropertyInstance>>>(blockExpression, parameterEntity, parameterPropertyInstances);
        }

        public static Expression<Action<TEntity, long, ICollection<TPropertyInstance>, BusinessModel>> BuildValueSettersExpression<TEntity, TPropertyInstance>(
            this IEnumerable<IEntityPropertyIdentity> propertyIdentities,
            Action<ICollection<TPropertyInstance>, int, Type, object, long> setPropertyValueAction)
        {
            var parameterEntity = Expression.Parameter(typeof(TEntity), "entity");
            var parameterEntityId = Expression.Parameter(typeof(long), "entityId");
            var parameterPropertyInstances = Expression.Parameter(typeof(ICollection<TPropertyInstance>), "propertyInstances");
            var parameterBusinessModel = Expression.Parameter(typeof(BusinessModel), "businessModel");

            var setters = propertyIdentities.Select(identity => CreateMethodCallExpression(identity,
                                                                                           Expression.Property(parameterEntity, identity.PropertyName),
                                                                                           parameterPropertyInstances,
                                                                                           parameterEntityId,
                                                                                           setPropertyValueAction))
                                            .ToList();
            
            // Подмешиваем тип сущности в набор свойств
            setters.Add(CreateMethodCallExpression(EntityTypeNameIdentity.Instance,
                                                   Expression.Constant(typeof(TEntity).AsEntityName()),
                                                   parameterPropertyInstances,
                                                   parameterEntityId,
                                                   setPropertyValueAction));

            // Подмешиваем бизнес-модель в набор свойств сущности
            if (typeof(TEntity).IsAdapted())
            {
                setters.Add(CreateMethodCallExpression(BusinessModelIdentity.Instance,
                                                       parameterBusinessModel,
                                                       parameterPropertyInstances,
                                                       parameterEntityId,
                                                       setPropertyValueAction));
            }

            var blockExpression = Expression.Block(setters);
            return Expression.Lambda<Action<TEntity, long, ICollection<TPropertyInstance>, BusinessModel>>(blockExpression,
                                                                                                           parameterEntity,
                                                                                                           parameterEntityId,
                                                                                                           parameterPropertyInstances,
                                                                                                           parameterBusinessModel);
        }

        private static MethodCallExpression CreateMethodCallExpression<TPropertyInstance>(
            IEntityPropertyIdentity propertyIdentity,
            Expression propertyValueExpression,
            Expression propertyInstancesExpression,
            Expression entityIdExpression,
            Action<ICollection<TPropertyInstance>, int, Type, object, long> setPropertyValueAction)
        {

            return Expression.Call(Expression.Constant(setPropertyValueAction.Target),
                                   setPropertyValueAction.Method,
                                   propertyInstancesExpression,
                                   Expression.Constant(propertyIdentity.Id),
                                   Expression.Constant(propertyIdentity.PropertyType),
                                   Expression.Convert(propertyValueExpression, typeof(object)),
                                   entityIdExpression);
        }
    }
}