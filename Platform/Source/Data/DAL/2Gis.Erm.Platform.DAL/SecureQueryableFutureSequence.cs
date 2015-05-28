using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Futures;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public class SecureQueryableFutureSequence<TSource> : FutureSequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable;

        public SecureQueryableFutureSequence(
            FutureSequence<TSource> futureSequence, 
            IUserContext userContext, 
            ISecurityServiceEntityAccessInternal entityAccessService)
            : base(futureSequence.Map(q => RestrictQueryWhenAccessCheck(q, userContext, entityAccessService)))
        {
            _queryable = Sequence as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("sequence");
            }
        }

        public override FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            return new QueryableFutureSequence<TSource>(_queryable.Where(findSpecification));
        }

        public override FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector)
        {
            return new QueryableFutureSequence<TResult>(projector.Map(_queryable));
        }

        private static IQueryable<TSource> RestrictQueryWhenAccessCheck(
            IQueryable<TSource> querySource, 
            IUserContext userContext, 
            ISecurityServiceEntityAccessInternal entityAccessService)
        {
            var securityControlAspect = userContext.Identity as IUserIdentitySecurityControl;
            if (securityControlAspect != null && securityControlAspect.SkipEntityAccessCheck)
            {
                return querySource;
            }

            return (IQueryable<TSource>)entityAccessService.RestrictQuery(querySource, querySource.ElementType.AsEntityName(), userContext.Identity.Code);
        }
    }
}