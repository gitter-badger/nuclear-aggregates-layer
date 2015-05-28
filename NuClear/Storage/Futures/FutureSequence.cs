using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Futures
{
    public abstract class FutureSequence<TSource>
    {
        protected FutureSequence(IEnumerable<TSource> sequence)
        {
            Sequence = sequence;
        }

        protected FutureSequence(FutureSequence<TSource> futureSequence)
        {
            Sequence = futureSequence.Sequence;
        }

        protected IEnumerable<TSource> Sequence { get; private set; }

        public abstract FutureSequence<TSource> Find(FindSpecification<TSource> findSpecification);
        public abstract FutureSequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> projector);
        
        public bool Any()
        {
            return Sequence.Any();
        }

        public virtual TSource One()
        {
            return Sequence.SingleOrDefault();
        }

        public virtual TSource Top()
        {
            return Sequence.FirstOrDefault();
        }

        public virtual IReadOnlyCollection<TSource> Many()
        {
            return Sequence.ToArray();
        }

        public virtual TResult Fold<TResult>(MapSpecification<IEnumerable<TSource>, TResult> foldSpecification)
        {
            return foldSpecification.Map(Sequence);
        }

        public virtual IReadOnlyDictionary<TKey, TValue> Map<TKey, TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            return Sequence.ToDictionary(keySelector, valueSelector);
        }
    }
}