using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class SubordinatesFilter
    {
        private static readonly MethodInfo WhereMethodInfo = typeof(Queryable).GetMethods().First(x => x.Name == "Where");
        private static readonly MethodInfo ContainsInt64MethodInfo = typeof(Enumerable).GetMethods().First(x => x.Name == "Contains").MakeGenericMethod(typeof(long));

        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        public SubordinatesFilter(IFinder finder, IUserContext userContext)
        {
            _finder = finder;
            _userContext = userContext;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> queryable)
        {
            var descendantIds =
                    _finder.Find<UsersDescendant>(x => x.AncestorId == _userContext.Identity.Code && x.DescendantId.HasValue)
                    .Select(x => x.DescendantId.Value)
                    .ToArray();

            return Apply<TEntity>(queryable, descendantIds);
        }

        private static IQueryable<TEntity> Apply<TEntity>(IQueryable query, IEnumerable descendantIds)
        {
            var entityType = typeof(TEntity);

            var parameter = Expression.Parameter(entityType, "x");
            var ownerCodePropertyInfo = entityType.GetProperty("OwnerCode");
            if (ownerCodePropertyInfo == null)
            {
                throw new ArgumentException("В типе не определёно свойство OwnerCode");
            }

            // x.OwnerCode
            var ownerCodeProperty = Expression.Property(parameter, ownerCodePropertyInfo);
            // descendantIds
            var ownerCodesConstant = Expression.Constant(descendantIds);
            // descendantIds.Contains(x.OwnerCode)
            var ownerCodesContainsX = Expression.Call(ContainsInt64MethodInfo, ownerCodesConstant, ownerCodeProperty);

            // x => descendantIds.Contains(x.OwnerCode)
            var lambdaExpression = Expression.Lambda(ownerCodesContainsX, parameter);
            var whereMethod = WhereMethodInfo.MakeGenericMethod(entityType);
            // query.Where(x => descendantIds.Contains(x.OwnerCode))
            var whereExpression = Expression.Call(whereMethod, query.Expression, lambdaExpression);

            return query.Provider.CreateQuery<TEntity>(whereExpression);
        }
    }
}