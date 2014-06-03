using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal static class QueryableExtensions
    {
        internal static IQueryable ValidateQueryCorrectness(this IQueryable queryable)
        {
            return new WrappedQuery(queryable, new WrappedQueryProvider(queryable.Provider));
        }

        internal static IQueryable<T> ValidateQueryCorrectness<T>(this IQueryable<T> queryable)
        {
            return new WrappedQuery<T>(queryable, new WrappedQueryProvider(queryable.Provider));
        }
    }
}
