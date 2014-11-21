using System;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal static class QueryableExtensions
    {
        internal static IQueryable ValidateQueryCorrectness(this IQueryable queryable)
        {
            var queryableType = queryable.GetType().GetInterfaces()
                                         .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>));
            if (queryableType != null)
            {
                var entityType = queryableType.GetGenericArguments().Single();
                var resultType = typeof(WrappedQuery<>).MakeGenericType(entityType);
                return (WrappedQuery)Activator.CreateInstance(resultType, queryable, new WrappedQueryProvider(queryable.Provider));
            }

            return new WrappedQuery(queryable, new WrappedQueryProvider(queryable.Provider));
        }

        internal static IQueryable<T> ValidateQueryCorrectness<T>(this IQueryable<T> queryable)
        {
            return new WrappedQuery<T>(queryable, new WrappedQueryProvider(queryable.Provider));
        }
    }
}
