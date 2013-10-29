using System;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public static class QueryableExtensions
    {
        // тупая монада maybe, зачем она здесь?
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Expression<Func<T, bool>> filterExpression)
        {
            if (filterExpression == null)
            {
                return query;
            }

            return query.Where(filterExpression);
        }
    }

    // TODO: перенести в отдельный файл после мёрджа
    public static class DynamicQueryableExtensions
    {
        public static IQueryable<T> ApplyQuerySettings<T>(this IQueryable<T> query, QuerySettings querySettings, out int count)
        {
            return (IQueryable<T>)ApplyQuerySettings((IQueryable)query, querySettings, out count);
        }

        public static IQueryable ApplyQuerySettings(this IQueryable query, QuerySettings querySettings, out int count)
        {
            if (querySettings == null)
            {
                count = System.Linq.Dynamic.DynamicQueryable.Count(query);
                return query;
            }

            var newQuery = query.Filtered(querySettings).Sorted(querySettings);
            count = System.Linq.Dynamic.DynamicQueryable.Count(newQuery);
            newQuery = newQuery.Paged(querySettings);

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

        private static IQueryable Paged(this IQueryable query, QuerySettings querySettings)
        {
            var newQuery = query;

            if (querySettings.SkipCount > 0)
            {
                newQuery = System.Linq.Dynamic.DynamicQueryable.Skip(newQuery, querySettings.SkipCount);
            }

            if (querySettings.TakeCount > 0)
            {
                newQuery = System.Linq.Dynamic.DynamicQueryable.Take(newQuery, querySettings.TakeCount);
            }

            return newQuery;
        }

        private static IQueryable Sorted(this IQueryable query, QuerySettings querySettings)
        {
            if (string.IsNullOrEmpty(querySettings.SortOrder) || string.IsNullOrEmpty(querySettings.SortDirection))
            {
                return query;
            }

            var ordering = string.Concat(querySettings.SortOrder, " ", querySettings.SortDirection);
            var newQuery = System.Linq.Dynamic.DynamicQueryable.OrderBy(query, ordering);

            return newQuery;
        }
    }
}