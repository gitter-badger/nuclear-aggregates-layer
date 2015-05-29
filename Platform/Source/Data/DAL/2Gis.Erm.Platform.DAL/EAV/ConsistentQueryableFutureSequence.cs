using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Futures;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public class ConsistentQueryableFutureSequence<TSource> : FutureSequence<TSource> 
    {
        private readonly IQueryable<TSource> _queryable;
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;
        private readonly IDynamicStorageFinder _dynamicStorageFinder;
        private readonly FindSpecification<TSource> _findSpecification;

        public ConsistentQueryableFutureSequence(
            IEnumerable<TSource> sequence, 
            IDynamicEntityMetadataProvider dynamicEntityMetadataProvider, 
            IDynamicStorageFinder dynamicStorageFinder,
            FindSpecification<TSource> findSpecification)
            : base(sequence)
        {
            _queryable = sequence as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("sequence");
            }

            var sourceType = typeof(TSource);
            if (!typeof(IEntityKey).IsAssignableFrom(sourceType))
            {
                throw new NotSupportedException("Type " + sourceType.Name + " must implement IEntityKey interface");
            }

            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
            _dynamicStorageFinder = dynamicStorageFinder;
            _findSpecification = findSpecification;
        }

        protected override IEnumerable<TSource> Sequence
        {
            get { return _queryable.Where(_findSpecification); }
        }

        public override FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            throw new NotSupportedException();
        }

        public override FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector)
        {
            if (typeof(TSource) == typeof(TResult))
            {
                var castedFindSpecification = _findSpecification as FindSpecification<TResult>;
                return new ConsistentQueryableFutureSequence<TResult>(projector.Map(_queryable), _dynamicEntityMetadataProvider, _dynamicStorageFinder, castedFindSpecification);
            }

            if (!typeof(IPartable).IsAssignableFrom(typeof(TResult)))
            {
                return new QueryableFutureSequence<TResult>(projector.Map(Sequence));
            }

            throw new NotSupportedException();
        }

        public override TSource One()
        {
            return GetInstance(q => q.SingleOrDefault());
        }

        public override TSource Top()
        {
            return GetInstance(q => q.FirstOrDefault());
        }

        public override IReadOnlyCollection<TSource> Many()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entities = Sequence.ToArray();
                var specs = _dynamicEntityMetadataProvider.GetSpecifications<BusinessEntityInstance, BusinessEntityPropertyInstance>(typeof(TSource),
                                                                                                                                     entities.OfType<IEntityKey>()
                                                                                                                                             .Select(e => e.Id));
                var parts = _dynamicStorageFinder.Find(specs).Cast<IEntityPart>().GroupBy(part => part.EntityId).ToDictionary(group => group.Key);

                foreach (var entity in entities)
                {
                    var entityKey = (IEntityKey)entity;

                    IGrouping<long, IEntityPart> entityParts;
                    if (parts.TryGetValue(entityKey.Id, out entityParts))
                    {
                        ((IPartable)entity).Parts = entityParts.ToArray();
                    }
                }

                transaction.Complete();

                return entities;
            }
        }

        public override IReadOnlyDictionary<TKey, TValue> Map<TKey, TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            return Many().ToDictionary(keySelector, valueSelector);
        }

        private TSource GetInstance(Func<IQueryable<TSource>, TSource> queryExecutor)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entity = queryExecutor(Sequence.AsQueryable());
                if (entity != null)
                {
                    var entityKey = (IEntityKey)entity;
                    var specs = _dynamicEntityMetadataProvider.GetSpecifications<BusinessEntityInstance, BusinessEntityPropertyInstance>(entity.GetType(), new[] { entityKey.Id });
                    ((IPartable)entity).Parts = _dynamicStorageFinder.Find(specs).Cast<IEntityPart>();
                }

                transaction.Complete();

                return entity;
            }
        }
    }
}