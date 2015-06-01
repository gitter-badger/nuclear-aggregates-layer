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
    public class SecureQueryableSequenceDecorator<TSource> : Sequence<TSource>
    {
        public SecureQueryableSequenceDecorator(
            Sequence<TSource> sequence, 
            IUserContext userContext, 
            ISecurityServiceEntityAccessInternal entityAccessService)
            : base(sequence.Map(q => RestrictQueryWhenAccessCheck(q, userContext, entityAccessService)))
        {
        }

        private IQueryable<TSource> SecuredSource
        {
            get
            {
                return (IQueryable<TSource>)Source;
            }
        }
        
        public override Sequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            return new QueryableSequence<TSource>(SecuredSource.Where(findSpecification));
        }

        public override Sequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> mapSpecification)
        {
            return new QueryableSequence<TResult>(mapSpecification.Map(SecuredSource));
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