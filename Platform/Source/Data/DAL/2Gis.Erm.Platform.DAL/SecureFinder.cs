using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            return RestrictQueryWhenAccessCheck<IQueryable<TEntity>>(_finder.Find(findSpecification));
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(
            ISelectSpecification<TEntity, TOutput> selectSpecification, 
            IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
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

        public TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey
        {
            throw new NotSupportedException("ConsistentSecureFinderDecorator should be used");
        }

        public IEnumerable<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey
        {
            throw new NotSupportedException("ConsistentSecureFinderDecorator should be used");
        }

        public IQueryable FindAll(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return RestrictQueryWhenAccessCheck<IQueryable>(_finder.FindAll(entityType));
        }

        public IQueryable<TEntity> FindAll<TEntity>() where TEntity : class, IEntity
        {
            return (IQueryable<TEntity>)FindAll(typeof(TEntity));
        }

        private TQueryable RestrictQueryWhenAccessCheck<TQueryable>(IQueryable querySource)
            where TQueryable : IQueryable
        {
            if (querySource == null)
            {
                throw new ArgumentNullException("querySource");
            }

            if (!_userContext.Identity.SkipEntityAccessCheck)
            {
                return (TQueryable)_entityAccessService.RestrictQuery(querySource, querySource.ElementType.AsEntityName(), _userContext.Identity.Code);
            }

            return (TQueryable)querySource;
        }
    }
}