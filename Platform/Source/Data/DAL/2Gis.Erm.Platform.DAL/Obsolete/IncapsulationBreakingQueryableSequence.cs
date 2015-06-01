using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.Obsolete
{
    [Obsolete("Need to be deleted after FinderExtensions.FindObsolete refactoring")]
    internal class IncapsulationBreakingQueryableSequence<TSource> : Sequence<TSource>
    {
        private readonly IQueryable<TSource> _queryable; 

        public IncapsulationBreakingQueryableSequence(Sequence<TSource> sequence) 
            : base(sequence)
        {
            _queryable = Source as IQueryable<TSource>;
            if (_queryable == null)
            {
                throw new ArgumentException("sequence");
            }
        }

        public IQueryable<TSource> Queryable
        {
            get { return _queryable; }
        }

        public override Sequence<TSource> Filter(FindSpecification<TSource> findSpecification)
        {
            throw new NotSupportedException();
        }

        public override Sequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> mapSpecification)
        {
            throw new NotSupportedException();
        }
    }
}