using System;

using DoubleGis.Erm.Platform.API.Security;

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
        private readonly IReadableDomainContextProvider _readableDomainContextProvider;
        private readonly IUserContext _userContext;

        public SecureFinder(
            IReadableDomainContextProvider readableDomainContextProvider, 
            IUserContext userContext, 
            ISecurityServiceEntityAccessInternal entityAccessService)
        {
            if (readableDomainContextProvider == null)
            {
                throw new ArgumentNullException("readableDomainContextProvider");
            }

            if (userContext == null)
            {
                throw new ArgumentNullException("userContext");
            }

            if (entityAccessService == null)
            {
                throw new ArgumentNullException("entityAccessService");
            }

            _readableDomainContextProvider = readableDomainContextProvider;
            _userContext = userContext;
            _entityAccessService = entityAccessService;
        }

        public Sequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class
        {
            var queryableSource = _readableDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new SecureQueryableSequenceDecorator<TSource>(
                new QueryableSequence<TSource>(queryableSource.Where(findSpecification)), 
                _userContext, 
                _entityAccessService);
        }
    }
}