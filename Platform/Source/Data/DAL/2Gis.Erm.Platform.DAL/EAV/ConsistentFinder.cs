using System;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Core;
using NuClear.Storage.Futures;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class ConsistentFinder : IFinder
    {
        private readonly IReadDomainContextProvider _readDomainContextProvider;
        private readonly IDynamicStorageFinder _dynamicStorageFinder;
        private readonly ICompositeEntityQuery _compositeEntityQuery;
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;

        public ConsistentFinder(IReadDomainContextProvider readDomainContextProvider,
                                IDynamicStorageFinder dynamicStorageFinder,
                                ICompositeEntityQuery compositeEntityQuery,
                                IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            if (compositeEntityQuery == null)
            {
                throw new ArgumentNullException("compositeEntityQuery");
            }

            _readDomainContextProvider = readDomainContextProvider;
            _dynamicStorageFinder = dynamicStorageFinder;
            _compositeEntityQuery = compositeEntityQuery;
            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
        }

        public FutureSequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            if (typeof(TSource).AsEntityName().IsDynamic())
            {
                return new DynamicQueryableFutureSequence<TSource>(_dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification);
            }

            IEntityType entityName;
            if (typeof(TSource).TryGetEntityName(out entityName) && entityName.HasMapping())
            {
                return new MappedQueryableFutureSequence<TSource>(_compositeEntityQuery, findSpecification);
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new ConsistentQueryableFutureSequence<TSource>(queryableSource, _dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification);
        }
    }
}