using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public enum ExtendedPropertyUnionType
    {
        And,
        Or
    }

    public static class QueryableExtensions2
    {
        public static bool TryGetExtendedProperty<T>(this QuerySettings querySettings, string name, out T value)
        {
            string nonParsedValue;
            if (!querySettings.ExtendedInfoMap.TryGetValue(name, out nonParsedValue))
            {
                value = default(T);
                return false;
            }

            value = (T)Convert.ChangeType(nonParsedValue, typeof(T));
            return true;
        }

        public static Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam>(
            this QuerySettings querySettings,
            string parameterName,
            Func<TParam, Expression<Func<TEntity, bool>>> action)
        {
            TParam param;
            if (!TryGetExtendedProperty(querySettings, parameterName, out param))
            {
                return null;
            }

            var expression = action(param);
            return expression;
        }

        // TODO: refactor usage
        public static Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam1, TParam2>(
            this QuerySettings querySettings,
            string parameterName1, 
            string parameterName2,
            ExtendedPropertyUnionType extendedPropertyUnionType,
            Func<TParam1, TParam2, Expression<Func<TEntity, bool>>> action)
        {
            TParam1 param1;
            var hasParam1 = querySettings.TryGetExtendedProperty(parameterName1, out param1);

            TParam2 param2;
            var hasParam2 = querySettings.TryGetExtendedProperty(parameterName2, out param2);

            switch (extendedPropertyUnionType)
            {
                case ExtendedPropertyUnionType.And:
                    if (hasParam1 && hasParam2)
                    {
                        return action(param1, param2);
                    }

                    break;
                case ExtendedPropertyUnionType.Or:
                    if (hasParam1 || hasParam2)
                    {
                        return action(param1, param2);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("extendedPropertyUnionType");
            }

            return null;
        }

        // TODO: refactor usage
        public static Expression<Func<TEntity, bool>> CreateByParentEntity<TEntity>(
            this QuerySettings querySettings,
            EntityName parentEntityName,
            Func<Expression<Func<TEntity, bool>>> action)
        {
            if (querySettings.ParentEntityName == parentEntityName && querySettings.ParentEntityId != null)
            {
                return action();
            }

            return null;
        }
    }
}