using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Futures;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public class DynamicQueryableSequence<TSource> : Sequence<TSource> where TSource : class
    {
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;
        private readonly IDynamicStorageFinder _dynamicStorageFinder;
        private readonly FindSpecification<TSource> _findSpecification;

        public DynamicQueryableSequence(
            IDynamicEntityMetadataProvider dynamicEntityMetadataProvider,
            IDynamicStorageFinder dynamicStorageFinder,
            FindSpecification<TSource> findSpecification)
            : base(Enumerable.Empty<TSource>())
        {
            var entityType = typeof(TSource);
            if (!entityType.AsEntityName().IsDynamic())
            {
                throw new NotSupportedException("Entity type " + entityType.Name + " is not dynamic");
            }

            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
            _dynamicStorageFinder = dynamicStorageFinder;
            _findSpecification = findSpecification;
        }

        protected override IEnumerable<TSource> Source
        {
            get { throw new InvalidOperationException("DynamicQueryableSequence class interface should be used to interact with Source"); }
        }

        public override Sequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            throw new NotSupportedException();
        }

        public override Sequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> mapSpecification)
        {
            throw new NotSupportedException();
        }

        public override TSource One()
        {
            var id = _findSpecification.ExtractEntityId();
            var specs = _dynamicEntityMetadataProvider.GetSpecifications<DictionaryEntityInstance, DictionaryEntityPropertyInstance>(typeof(TSource), new[] { id });
            return _dynamicStorageFinder.Find(specs).Cast<TSource>().SingleOrDefault();
        }

        public override TSource Top()
        {
            var id = _findSpecification.ExtractEntityId();
            var specs = _dynamicEntityMetadataProvider.GetSpecifications<DictionaryEntityInstance, DictionaryEntityPropertyInstance>(typeof(TSource), new[] { id });
            return _dynamicStorageFinder.Find(specs).Cast<TSource>().FirstOrDefault();
        }

        public override IReadOnlyCollection<TSource> Many()
        {
            var ids = _findSpecification.ExtractEntityIds();
            var specs = _dynamicEntityMetadataProvider.GetSpecifications<DictionaryEntityInstance, DictionaryEntityPropertyInstance>(typeof(TSource), ids);
            return _dynamicStorageFinder.Find(specs).Cast<TSource>().ToArray();
        }

        public override IReadOnlyDictionary<TKey, TValue> Map<TKey, TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var ids = _findSpecification.ExtractEntityIds();
            var specs = _dynamicEntityMetadataProvider.GetSpecifications<DictionaryEntityInstance, DictionaryEntityPropertyInstance>(typeof(TSource), ids);
            return _dynamicStorageFinder.Find(specs).Cast<TSource>().ToDictionary(keySelector, valueSelector);
        }
    }
}