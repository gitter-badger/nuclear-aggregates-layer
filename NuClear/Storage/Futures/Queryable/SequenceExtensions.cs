using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Futures.Queryable
{
    public static class SequenceExtensions
    {
        public static Sequence<TResult> Map<TSource, TResult>(
            this Sequence<TSource> sequence, 
            Func<IQueryable<TSource>, IQueryable<TResult>> projector)
        {
            Func<IEnumerable<TSource>, IEnumerable<TResult>> enumerableProjector = x => projector(x.AsQueryable());
            return sequence.Map((MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>>)enumerableProjector);
        }

        public static Sequence<TResult> Map<TSource, TResult>(
            this Sequence<TSource> sequence, 
            MapSpecification<IQueryable<TSource>, IQueryable<TResult>> mapSpecification)
        {
            Func<IQueryable<TSource>, IQueryable<TResult>> projector = mapSpecification;
            return sequence.Map(projector);
        }

        public static TResult Fold<TSource, TResult>(
            this Sequence<TSource> sequence,
            Func<IQueryable<TSource>, TResult> foldFunc)
        {
            Func<IEnumerable<TSource>, TResult> enumerableFoldFunc = x => foldFunc(x.AsQueryable());
            return sequence.Fold((MapSpecification<IEnumerable<TSource>, TResult>)enumerableFoldFunc);
        }

        public static TResult Fold<TSource, TResult>(
            this Sequence<TSource> sequence,
            MapSpecification<IQueryable<TSource>, TResult> foldSpecification)
        {
            Func<IQueryable<TSource>, TResult> foldFunc = foldSpecification;
            return sequence.Fold(foldFunc);
        }
    }
}