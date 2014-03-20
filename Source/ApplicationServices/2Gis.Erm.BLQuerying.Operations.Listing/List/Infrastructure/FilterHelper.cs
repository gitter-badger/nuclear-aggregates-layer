using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class FilterHelper
    {
        private readonly SubordinatesFilter _subordinatesFilter;

        public FilterHelper(SubordinatesFilter subordinatesFilter)
        {
            _subordinatesFilter = subordinatesFilter;
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

        public IEnumerable<TDocument> QuerySettings<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings, out int total)
        {
            query = DefaultFilter(query, querySettings);
            query = RelativeFilter(query, querySettings);
            query = FieldFilter(query, querySettings);

            return SortedPaged(query, querySettings, out total);
        }

        private IQueryable<TDocument> DefaultFilter<TDocument>(IQueryable<TDocument> queryable, QuerySettings querySettings)
        {
            Expression expression;
            if (!DefaultFilterMetadata.TryGetFilter<TDocument>(querySettings.FilterName, out expression))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определен фильтр по умолчанию", typeof(TDocument).Name));
            }

            var whereMethod = MethodInfos.Queryable.WhereMethodInfo.MakeGenericMethod(typeof(TDocument));
            var whereExpression = Expression.Call(whereMethod, queryable.Expression, expression);

            return queryable.Provider.CreateQuery<TDocument>(whereExpression);
        }

        private IQueryable<TDocument> RelativeFilter<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            bool filterToParent;
            if (!querySettings.TryGetExtendedProperty("filterToParent", out filterToParent))
            {
                return query;
            }

            Expression expression;
            if (!RelationalMetadata.TryGetFilterExpressionFromRelationalMap<TDocument>(querySettings.ParentEntityName, querySettings.ParentEntityId, out expression))
            {
                throw new ArgumentException(string.Format("Relational metadata should be added (Entity={0}, RelatedItem={1})", querySettings.ParentEntityName, typeof(TDocument).Name));
            }

            var whereMethod = MethodInfos.Queryable.WhereMethodInfo.MakeGenericMethod(typeof(TDocument));
            var whereExpression = Expression.Call(whereMethod, query.Expression, expression);
            return query.Provider.CreateQuery<TDocument>(whereExpression);
        }

        private IQueryable<TDocument> FieldFilter<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            if (string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                return query;
            }

            Expression expression;
            if (!FilteredFieldMetadata.TryGetFieldFilter<TDocument>(querySettings.UserInputFilter, out expression))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определены поисковые поля", typeof(TDocument).Name));
            }

            var whereMethod = MethodInfos.Queryable.WhereMethodInfo.MakeGenericMethod(typeof(TDocument));
            var whereExpression = Expression.Call(whereMethod, query.Expression, expression);

            return query.Provider.CreateQuery<TDocument>(whereExpression);
        }

        private IEnumerable<TDocument> SortedPaged<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings, out int total)
        {
            total = query.Count();

            var documentType = typeof(TDocument);

            var propertyInfo = !string.IsNullOrEmpty(querySettings.SortOrder)
                               ? documentType.GetProperty(querySettings.SortOrder)
                               : documentType.GetProperty("Id");
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Для типа {0} не определены сортировочные поля", documentType.Name));
            }

            MethodInfo methodInfo;
            if (string.IsNullOrEmpty(querySettings.SortDirection) || string.Equals(querySettings.SortDirection, "ASC", StringComparison.OrdinalIgnoreCase))
            {
                methodInfo = MethodInfos.Queryable.OrderByMethodInfo.MakeGenericMethod(documentType, propertyInfo.PropertyType);
            }
            else if (string.Equals(querySettings.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase))
            {
                methodInfo = MethodInfos.Queryable.OrderByDescendingMethodInfo.MakeGenericMethod(documentType, propertyInfo.PropertyType);
            }
            else
            {
                throw new ArgumentException();
            }

            var parameterExpression = Expression.Parameter(documentType, "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
            var lambdaExpression = Expression.Lambda(propertyExpression, parameterExpression);
            var callExpression = Expression.Call(methodInfo, query.Expression, lambdaExpression);

            query = query.Provider.CreateQuery<TDocument>(callExpression);

            query = query
                .Skip(querySettings.SkipCount)
                .Take(querySettings.TakeCount);

            return query;
        }
    }
}