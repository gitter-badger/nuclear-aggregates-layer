using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.Obsolete
{
    public static class FinderExtensions
    {
        [Obsolete("Need to be replaced with Find returning FutureSequence or IQuery.For")]
        public static IQueryable<TSource> FindObsolete<TSource>(
            this IFinder finder, 
            FindSpecification<TSource> findSpecification) where TSource : class, IEntity
        {
            var adaptingFutureSequence = new IncapsulationBreakingQueryableFutureSequence<TSource>(finder.Find(findSpecification));
            return adaptingFutureSequence.Queryable;
        }

        [Obsolete("Need to be replaced with Find returning FutureSequence or IQuery.For")]
        public static IQueryable<TResult> FindObsolete<TSource, TResult>(
            this IFinder finder,
            FindSpecification<TSource> findSpecification,
            SelectSpecification<TSource, TResult> selectSpecification) where TSource : class, IEntity
        {
            var adaptingFutureSequence = new IncapsulationBreakingQueryableFutureSequence<TSource>(finder.Find(findSpecification));
            return adaptingFutureSequence.Queryable.Select(selectSpecification);
        }
    }
}