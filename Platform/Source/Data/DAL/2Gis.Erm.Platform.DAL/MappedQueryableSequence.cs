using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public class MappedQueryableSequence<TSource> : Sequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable;

        public MappedQueryableSequence(ICompositeEntityQuery compositeEntityQuery, FindSpecification<TSource> findSpecification)
            : this(compositeEntityQuery.For(findSpecification))
        {
            var sourceType = typeof(TSource);
            IEntityType entityName;
            if (!sourceType.TryGetEntityName(out entityName) || !entityName.HasMapping())
            {
                throw new NotSupportedException("Entity type " + sourceType.Name + " is not mapped");
            }
        }

        private MappedQueryableSequence(IEnumerable<TSource> sequence) : base(sequence)
        {
            _queryable = (IQueryable<TSource>)sequence;
        }

        public override Sequence<TSource> Filter(FindSpecification<TSource> findSpecification)
        {
            return new QueryableSequence<TSource>(_queryable.Where(findSpecification));
        }

        public override Sequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> mapSpecification)
        {
            return new QueryableSequence<TResult>(mapSpecification.Map(_queryable));
        }
    }
}