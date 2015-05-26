using System;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Core;
using NuClear.Storage.Futures;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class ConsistentSecureFinder : ISecureFinder
    {
        private readonly IReadDomainContextProvider _readDomainContextProvider;
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;
        private readonly IDynamicStorageFinder _dynamicStorageFinder;
        private readonly ICompositeEntityQuery _compositeEntityQuery;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;

        public ConsistentSecureFinder(IReadDomainContextProvider readDomainContextProvider,
                                      IDynamicEntityMetadataProvider dynamicEntityMetadataProvider,
                                      IDynamicStorageFinder dynamicStorageFinder,
                                      ICompositeEntityQuery compositeEntityQuery,
                                      IUserContext userContext,
                                      ISecurityServiceEntityAccessInternal entityAccessService)
        {
            _readDomainContextProvider = readDomainContextProvider;
            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
            _dynamicStorageFinder = dynamicStorageFinder;
            _compositeEntityQuery = compositeEntityQuery;
            _userContext = userContext;
            _entityAccessService = entityAccessService;
        }

        public FutureSequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            if (typeof(TSource).AsEntityName().IsDynamic())
            {
                // NOTE: we don't check entity access for dynamic entities
                return new DynamicQueryableFutureSequence<TSource>(_dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification);
            }

            IEntityType entityName;
            if (typeof(TSource).TryGetEntityName(out entityName) && entityName.HasMapping())
            {
                return new SecureQueryableFutureSequence<TSource>(
                    new MappedQueryableFutureSequence<TSource>(_compositeEntityQuery, findSpecification), 
                    _userContext, 
                    _entityAccessService);
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new SecureQueryableFutureSequence<TSource>(
                new ConsistentQueryableFutureSequence<TSource>(queryableSource, _dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification), 
                _userContext, 
                _entityAccessService);
        }
    }
}
