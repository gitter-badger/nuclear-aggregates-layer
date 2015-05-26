using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Futures.Queryable
{
    public static class QueryableFutureExtensions
    {
        public static FutureSequence<TResult> Project<TSource, TResult>(
            this FutureSequence<TSource> futureSequence, 
            Func<IQueryable<TSource>, IQueryable<TResult>> projector)
        {
            Func<IEnumerable<TSource>, IEnumerable<TResult>> enumerableProjector = x => projector(x.AsQueryable());
            return futureSequence.Project((ProjectSpecification<IEnumerable<TSource>, IEnumerable<TResult>>)enumerableProjector);
        }

        public static FutureSequence<TResult> Project<TSource, TResult>(
            this FutureSequence<TSource> futureSequence, 
            ProjectSpecification<IQueryable<TSource>, IQueryable<TResult>> projectSpecification)
        {
            Func<IQueryable<TSource>, IQueryable<TResult>> projector = projectSpecification;
            return futureSequence.Project(projector);
        }
    }
}