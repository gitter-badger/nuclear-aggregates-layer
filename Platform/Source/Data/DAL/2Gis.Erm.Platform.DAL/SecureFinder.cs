using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public class SecureFinder : ISecureFinder
    {
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        public SecureFinder(IFinder finder, IUserContext userContext, ISecurityServiceEntityAccessInternal entityAccessService)
        {
            if (finder == null)
            {
                throw new ArgumentNullException("finder");
            }

            if (userContext == null)
            {
                throw new ArgumentNullException("userContext");
            }

            if (entityAccessService == null)
            {
                throw new ArgumentNullException("entityAccessService");
            }

            _finder = finder;
            _userContext = userContext;
            _entityAccessService = entityAccessService;
        }

        public IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            return RestrictQueryWhenAccessCheck<IQueryable<TEntity>>(_finder.Find(findSpecification));
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(
            SelectSpecification<TEntity, TOutput> selectSpecification, 
            FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            if (selectSpecification == null)
            {
                throw new ArgumentNullException("selectSpecification");
            }

            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var findResult = RestrictQueryWhenAccessCheck<IQueryable<TEntity>>(_finder.Find(findSpecification));

            return findResult.Select(selectSpecification.Selector);
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return RestrictQueryWhenAccessCheck<IQueryable<TEntity>>(_finder.Find(expression));
        }

        public TEntity FindOne<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            throw new NotSupportedException("ConsistentSecureFinderDecorator should be used");
        }

        public IEnumerable<TEntity> FindMany<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            throw new NotSupportedException("ConsistentSecureFinderDecorator should be used");
        }

        private TQueryable RestrictQueryWhenAccessCheck<TQueryable>(IQueryable querySource)
            where TQueryable : IQueryable
        {
            if (querySource == null)
            {
                throw new ArgumentNullException("querySource");
            }

            var securityControlAspect = _userContext.Identity as IUserIdentitySecurityControl;
            if (securityControlAspect != null && securityControlAspect.SkipEntityAccessCheck)
            {
                return (TQueryable)querySource;
            }

            return (TQueryable)_entityAccessService.RestrictQuery(querySource, querySource.ElementType.AsEntityName(), _userContext.Identity.Code);
        }
    }
}