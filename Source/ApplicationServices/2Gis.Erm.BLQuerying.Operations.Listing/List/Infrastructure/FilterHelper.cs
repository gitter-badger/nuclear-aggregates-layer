using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class FilterHelper
    {
        private readonly SubordinatesFilter _subordinatesFilter;
        private readonly DefaultFilter _defaultFilter;

        public FilterHelper(SubordinatesFilter subordinatesFilter, DefaultFilter defaultFilter)
        {
            _subordinatesFilter = subordinatesFilter;
            _defaultFilter = defaultFilter;
        }

        public IQueryable<TEntity> Filter<TEntity>(IQueryable<TEntity> query, params Expression<Func<TEntity, bool>>[] expressions)
        {
            foreach (var expression in expressions.Where(x => x != null))
            {
                query = query.Where(expression);
            }

            return query;
        }

        public IQueryable<TEntity> ForSubordinates<TEntity>(IQueryable<TEntity> queryable)
        {
            return _subordinatesFilter.Apply(queryable);
        }

        public IQueryable<TEntity> DefaultFilter<TEntity>(IQueryable<TEntity> query, QuerySettings querySettings)
        {
            return _defaultFilter.Apply(query, querySettings);
        }

        public IEnumerable<TDocument> QuerySettings<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings, out int total)
        {
            return (IEnumerable<TDocument>)QuerySettings((IQueryable)query, querySettings, out total);
        }

        private static IEnumerable QuerySettings(IQueryable query, QuerySettings querySettings, out int total)
        {
            var newQuery = RelativeFilter(query, querySettings);

            if (!string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                newQuery = System.Linq.Dynamic.DynamicQueryable.Where(newQuery, querySettings.UserInputFilter);
            }

            total = System.Linq.Dynamic.DynamicQueryable.Count(newQuery);

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

        private static IQueryable RelativeFilter(IQueryable query, QuerySettings querySettings)
        {
            bool filterToParent;
            if (!querySettings.TryGetExtendedProperty("filterToParent", out filterToParent))
            {
                return query;
            }

            string filterExpression;
            if (!RelationalMetadata.TryGetFilterExpressionFromRelationalMap(querySettings.ParentEntityName, querySettings.EntityName, out filterExpression))
            {
                throw new Exception(string.Format("Relational metadata should be added (Entity={0}, RelatedItem={1})", querySettings.ParentEntityName, querySettings.EntityName));
            }

            filterExpression = string.Format(filterExpression, querySettings.ParentEntityId);

            var newQuery = System.Linq.Dynamic.DynamicQueryable.Where(query, filterExpression);
            return newQuery;
        }
    }
}