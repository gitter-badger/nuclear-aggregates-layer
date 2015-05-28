using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Futures.Queryable
{
    public static class QueryableFutureExtensions
    {
        public static FutureSequence<TResult> Map<TSource, TResult>(
            this FutureSequence<TSource> futureSequence, 
            Func<IQueryable<TSource>, IQueryable<TResult>> projector)
        {
            Func<IEnumerable<TSource>, IEnumerable<TResult>> enumerableProjector = x => projector(x.AsQueryable());
            return futureSequence.Map((MapSpecification<IEnumerable<TSource>, IEnumerable<TResult>>)enumerableProjector);
        }

        public static FutureSequence<TResult> Map<TSource, TResult>(
            this FutureSequence<TSource> futureSequence, 
            MapSpecification<IQueryable<TSource>, IQueryable<TResult>> mapSpecification)
        {
            Func<IQueryable<TSource>, IQueryable<TResult>> projector = mapSpecification;
            return futureSequence.Map(projector);
        }

        public static TResult Fold<TSource, TResult>(
            this FutureSequence<TSource> futureSequence,
            Func<IQueryable<TSource>, TResult> foldFunc)
        {
            Func<IEnumerable<TSource>, TResult> enumerableFoldFunc = x => foldFunc(x.AsQueryable());
            return futureSequence.Fold((MapSpecification<IEnumerable<TSource>, TResult>)enumerableFoldFunc);
        }

        public static TResult Fold<TSource, TResult>(
            this FutureSequence<TSource> futureSequence,
            MapSpecification<IQueryable<TSource>, TResult> foldSpecification)
        {
            Func<IQueryable<TSource>, TResult> foldFunc = foldSpecification;
            return futureSequence.Fold(foldFunc);
        }
    }
}