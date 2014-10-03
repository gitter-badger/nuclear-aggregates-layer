using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public abstract class DescendantEntityFilter
    {
        private static readonly MethodInfo ContainsInt64MethodInfo = MethodInfos.Enumerable.ContainsMethodInfo.MakeGenericMethod(typeof(long));

        protected static IQueryable<TEntity> Apply<TEntity>(IQueryable query, IEnumerable<long> descendantIds, string propertyName)
        {
            var entityType = typeof(TEntity);

            var parameter = Expression.Parameter(entityType, "x");
            var entityIdPropertyInfo = entityType.GetProperty(propertyName);
            if (entityIdPropertyInfo == null)
            {
                throw new ArgumentException(string.Format("В типе не определёно свойство {0}", propertyName));
            }

            // x.OwnerCode или аналогичная штука 
            var codeProperty = Expression.Property(parameter, entityIdPropertyInfo);
            var convertedCodeProperty = Expression.Convert(codeProperty, typeof(long));
            // descendantIds
            var codesConstant = Expression.Constant(descendantIds);
            // descendantIds.Contains(x.OwnerCode) или x.ClientId, или что там нужно
            var ownerCodesContainsX = Expression.Call(ContainsInt64MethodInfo, codesConstant, convertedCodeProperty);

            // x => descendantIds.Contains(x.OwnerCode) или x.ClientId, или что там нужно
            var lambdaExpression = Expression.Lambda(ownerCodesContainsX, parameter);
            var whereMethod = MethodInfos.Queryable.WhereMethodInfo.MakeGenericMethod(entityType);
            // query.Where(x => descendantIds.Contains(x.OwnerCode)) или x.ClientId, или что там нужно
            var whereExpression = Expression.Call(whereMethod, query.Expression, lambdaExpression);

            return query.Provider.CreateQuery<TEntity>(whereExpression);
        }
    }
}