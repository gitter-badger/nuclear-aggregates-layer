using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Futures;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public class MappedQueryableFutureSequence<TSource> : FutureSequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable;

        public MappedQueryableFutureSequence(ICompositeEntityQuery compositeEntityQuery, FindSpecification<TSource> findSpecification)
            : this(compositeEntityQuery.For(findSpecification))
        {
            var sourceType = typeof(TSource);
            IEntityType entityName;
            if (!sourceType.TryGetEntityName(out entityName) || !entityName.HasMapping())
            {
                throw new NotSupportedException("Entity type " + sourceType.Name + " is not mapped");
            }
        }

        private MappedQueryableFutureSequence(IEnumerable<TSource> sequence) : base(sequence)
        {
            _queryable = (IQueryable<TSource>)sequence;
        }

        public override FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            return new QueryableFutureSequence<TSource>(_queryable.Where(findSpecification));
        }

        public override FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector)
        {
            return new QueryableFutureSequence<TResult>(projector.Map(_queryable));
        }
    }
}