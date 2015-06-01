using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Futures
{
    public abstract class Sequence<TSource>
    {
        protected Sequence(IEnumerable<TSource> source)
        {
            Source = source;
        }

        protected Sequence(Sequence<TSource> sequence)
        {
            Source = sequence.Source;
        }

        protected virtual IEnumerable<TSource> Source { get; private set; }

        public abstract Sequence<TSource> Find(FindSpecification<TSource> findSpecification);
        public abstract Sequence<TResult> Map<TResult>(MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>> mapSpecification);
        
        public bool Any()
        {
            return Source.Any();
        }

        public virtual TSource One()
        {
            return Source.SingleOrDefault();
        }

        public virtual TSource Top()
        {
            return Source.FirstOrDefault();
        }

        public virtual IReadOnlyCollection<TSource> Many()
        {
            return Source.ToArray();
        }

        public virtual TResult Fold<TResult>(MapSpecification<IEnumerable<TSource>, TResult> foldSpecification)
        {
            return foldSpecification.Map(Source);
        }

        public virtual IReadOnlyDictionary<TKey, TValue> Map<TKey, TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            return Source.ToDictionary(keySelector, valueSelector);
        }
    }
}