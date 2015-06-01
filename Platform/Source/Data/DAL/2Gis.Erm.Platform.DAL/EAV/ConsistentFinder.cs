using System;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Core;
using NuClear.Storage.Futures;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class ConsistentFinder : IFinder
    {
        private readonly IReadableDomainContextProvider _readableDomainContextProvider;
        private readonly IDynamicStorageFinder _dynamicStorageFinder;
        private readonly ICompositeEntityQuery _compositeEntityQuery;
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;

        public ConsistentFinder(IReadableDomainContextProvider readableDomainContextProvider,
                                IDynamicStorageFinder dynamicStorageFinder,
                                ICompositeEntityQuery compositeEntityQuery,
                                IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            if (compositeEntityQuery == null)
            {
                throw new ArgumentNullException("compositeEntityQuery");
            }

            _readableDomainContextProvider = readableDomainContextProvider;
            _dynamicStorageFinder = dynamicStorageFinder;
            _compositeEntityQuery = compositeEntityQuery;
            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
        }

        public Sequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            if (typeof(TSource).AsEntityName().IsDynamic())
            {
                return new DynamicQueryableSequence<TSource>(_dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification);
            }

            IEntityType entityName;
            if (typeof(TSource).TryGetEntityName(out entityName) && entityName.HasMapping())
            {
                return new MappedQueryableSequence<TSource>(_compositeEntityQuery, findSpecification);
            }

            var queryableSource = _readableDomainContextProvider.Get().GetQueryableSource<TSource>();
            if (typeof(IPartable).IsAssignableFrom(typeof(TSource)))
            {
                return new ConsistentQueryableSequence<TSource>(queryableSource, _dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification);
            }

            return new QueryableSequence<TSource>(queryableSource.ValidateQueryCorrectness().Where(findSpecification));
        }
    }
}