using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Futures;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.Obsolete
{
    [Obsolete("Need to be deleted after FinderExtensions.FindObsolete refactoring")]
    internal class IncapsulationBreakingQueryableFutureSequence<TSource> : FutureSequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable; 

        public IncapsulationBreakingQueryableFutureSequence(FutureSequence<TSource> futureSequence) 
            : base(futureSequence)
        {
            _queryable = Sequence as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("sequence");
            }
        }

        public IQueryable<TSource> Queryable
        {
            get { return _queryable; }
        }

        public override FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification)
        {
            throw new NotSupportedException();
        }

        public override FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector)
        {
            throw new NotSupportedException();
        }
    }
}