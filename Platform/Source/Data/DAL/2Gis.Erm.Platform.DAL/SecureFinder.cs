using System;

using DoubleGis.Erm.Platform.API.Security;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Core;
using NuClear.Storage.Futures;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public class SecureFinder : ISecureFinder
    {
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;
        private readonly IReadDomainContextProvider _readDomainContextProvider;
        private readonly IUserContext _userContext;

        public SecureFinder(
            IReadDomainContextProvider readDomainContextProvider, 
            IUserContext userContext, 
            ISecurityServiceEntityAccessInternal entityAccessService)
        {
            if (readDomainContextProvider == null)
            {
                throw new ArgumentNullException("readDomainContextProvider");
            }

            if (userContext == null)
            {
                throw new ArgumentNullException("userContext");
            }

            if (entityAccessService == null)
            {
                throw new ArgumentNullException("entityAccessService");
            }

            _readDomainContextProvider = readDomainContextProvider;
            _userContext = userContext;
            _entityAccessService = entityAccessService;
        }

        public FutureSequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class, IEntity
        {
            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new SecureQueryableFutureSequence<TSource>(
                new QueryableFutureSequence<TSource>(queryableSource.Where(findSpecification)), 
                _userContext, 
                _entityAccessService);
        }
    }
}