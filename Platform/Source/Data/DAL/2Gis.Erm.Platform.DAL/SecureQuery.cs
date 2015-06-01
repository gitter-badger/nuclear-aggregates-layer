﻿using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public class SecureQuery : ISecureQuery
    {
        private readonly IQuery _query;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;

        public SecureQuery(IQuery query, IUserContext userContext, ISecurityServiceEntityAccessInternal entityAccessService)
        {
            _query = query;
            _userContext = userContext;
            _entityAccessService = entityAccessService;
        }

        public IQueryable For(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return RestrictQueryWhenAccessCheck<IQueryable>(_query.For(entityType));
        }

        public IQueryable<TEntity> For<TEntity>() where TEntity : class
        {
            return RestrictQueryWhenAccessCheck<IQueryable<TEntity>>(_query.For<TEntity>());
        }

        public IQueryable<TEntity> For<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class
        {
            return RestrictQueryWhenAccessCheck<IQueryable<TEntity>>(_query.For(findSpecification));
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