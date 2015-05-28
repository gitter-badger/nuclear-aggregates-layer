using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Futures;
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
            : this(sequence)
        {
            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
            _dynamicStorageFinder = dynamicStorageFinder;
            _findSpecification = findSpecification;
        }

        private ConsistentQueryableFutureSequence(IEnumerable<TSource> sequence)
            : base(sequence)
        {
            _queryable = sequence as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("sequence");
            }

            _queryable = _queryable.ValidateQueryCorrectness();
        }

        public override FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            return new ConsistentQueryableFutureSequence<TSource>(_queryable, _dynamicEntityMetadataProvider, _dynamicStorageFinder, findSpecification);
        }

        public override FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector)
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TSource)))
            {
                throw new NotSupportedException();
            }

            return new ConsistentQueryableFutureSequence<TResult>(projector.Map(_queryable.Where(_findSpecification)));
        }

        public override TSource One()
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TSource)))
            {
                CheckForIEntityKeyImplementation();
                return GetInstance(q => q.SingleOrDefault());
            }

            return _queryable.SingleOrDefault();
        }

        public override TSource Top()
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TSource)))
            {
                CheckForIEntityKeyImplementation();
                return GetInstance(q => q.FirstOrDefault());
            }

            return _queryable.FirstOrDefault();
        }

        public override IReadOnlyCollection<TSource> Many()
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TSource)))
            {
                CheckForIEntityKeyImplementation();
                
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    var entities = _queryable.Where(_findSpecification).ToArray();
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

            return _queryable.ToArray();
        }

        public override IReadOnlyDictionary<TKey, TValue> Map<TKey, TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            IEnumerable<TSource> source;
            if (typeof(IPartable).IsAssignableFrom(typeof(TSource)))
            {
                CheckForIEntityKeyImplementation();

                source = Many();
            }
            else
            {
                source = _queryable;
            }

            return source.ToDictionary(keySelector, valueSelector);
        }

        private static void CheckForIEntityKeyImplementation()
        {
            var sourceType = typeof(TSource);
            if (!typeof(IEntityKey).IsAssignableFrom(sourceType))
            {
                throw new NotSupportedException("Type " + sourceType.Name + " must implement IEntityKey interface");
            }
        }

        private TSource GetInstance(Func<IQueryable<TSource>, TSource> queryExecutor)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entity = queryExecutor(_queryable.Where(_findSpecification));
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