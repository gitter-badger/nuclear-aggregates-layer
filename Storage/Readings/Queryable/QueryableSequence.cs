using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Readings.Queryable
{
    public sealed class QueryableSequence<TSource> : Sequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable;

        public QueryableSequence(IEnumerable<TSource> source)
            : base(source)
        {
            _queryable = source as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("Source");
            }
        }

        public override Sequence<TSource> Filter(FindSpecification<TSource> findSpecification)
        {
            return new QueryableSequence<TSource>(_queryable.Where(findSpecification.Predicate));
        }

        public override Sequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> mapSpecification)
        {
            return new QueryableSequence<TResult>(mapSpecification.Map(_queryable));
        }
    }
}