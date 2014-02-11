namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class QueryableExtensions
    {
        // ����� ������ maybe, ����� ��� �����?
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Expression<Func<T, bool>> filterExpression)
        {
            if (filterExpression == null)
            {
                return query;
            }

            return query.Where(filterExpression);
        }
    }

    // TODO: ��������� � ��������� ���� ����� �����
    public static class DynamicQueryableExtensions
    {
        public static IQueryable<T> ApplyQuerySettings<T>(this IQueryable<T> query, QuerySettings querySettings, out int total)
        {
            return (IQueryable<T>)ApplyQuerySettings((IQueryable)query, querySettings, out total);
        }

        public static IQueryable ApplyQuerySettings(this IQueryable query, QuerySettings querySettings, out int total)
        {
            if (querySettings == null)
            {
                total = System.Linq.Dynamic.DynamicQueryable.Count(query);
                return query;
            }

            var newQuery = query.Filtered(querySettings);
            total = System.Linq.Dynamic.DynamicQueryable.Count(newQuery);
            newQuery = newQuery.SortedPaged(querySettings);

            return newQuery;
        }

        private static IQueryable Filtered(this IQueryable query, QuerySettings querySettings)
        {
            var newQuery = query;

            if (!string.IsNullOrEmpty(querySettings.FilterPredicate))
            {
                newQuery = System.Linq.Dynamic.DynamicQueryable.Where(newQuery, querySettings.FilterPredicate);
            }

            if (!string.IsNullOrEmpty(querySettings.DefaultFilter))
            {
                newQuery = System.Linq.Dynamic.DynamicQueryable.Where(newQuery, querySettings.DefaultFilter);
            }

            if (!string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                newQuery = System.Linq.Dynamic.DynamicQueryable.Where(newQuery, querySettings.UserInputFilter);
            }

            return newQuery;
        }

        private static IQueryable SortedPaged(this IQueryable query, QuerySettings querySettings)
        {
            var newQuery = query;

            // sort by id by default
            var ordering = "Id";
            if (!string.IsNullOrEmpty(querySettings.SortOrder))
            {
                ordering = querySettings.SortOrder;
            }

            if (!string.IsNullOrEmpty(querySettings.SortDirection))
            {
                ordering = string.Concat(ordering, " ", querySettings.SortDirection);
            }

            newQuery = System.Linq.Dynamic.DynamicQueryable.OrderBy(newQuery, ordering);
            newQuery = System.Linq.Dynamic.DynamicQueryable.Skip(newQuery, querySettings.SkipCount);
            newQuery = System.Linq.Dynamic.DynamicQueryable.Take(newQuery, querySettings.TakeCount);

            return newQuery;
        }
    }
}