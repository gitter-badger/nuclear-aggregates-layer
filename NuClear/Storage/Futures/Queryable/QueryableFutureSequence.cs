using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Futures.Queryable
{
    public sealed class QueryableFutureSequence<TSource> : FutureSequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable;

        public QueryableFutureSequence(IEnumerable<TSource> sequence)
            : base(sequence)
        {
            _queryable = sequence as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("sequence");
            }
        }

        public override FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            return new QueryableFutureSequence<TSource>(_queryable.Where(findSpecification.Predicate));
        }

        public override FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector)
        {
            return new QueryableFutureSequence<TResult>(projector.Map(_queryable));
        }
    }
}