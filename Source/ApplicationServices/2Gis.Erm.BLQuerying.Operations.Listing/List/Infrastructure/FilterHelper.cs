using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
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

        public RemoteCollection<TDocument> QuerySettings<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            query = DefaultFilter(query, querySettings);
            query = RelativeFilter(query, querySettings);
            query = FieldFilter(query, querySettings);

            return SortedPaged(query, querySettings);
        }

        public IQueryable<TDocument> DefaultFilter<TDocument>(IQueryable<TDocument> queryable, QuerySettings querySettings)
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

        public IQueryable<TDocument> RelativeFilter<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            bool filterToParent;
            if (!querySettings.TryGetExtendedProperty("filterToParent", out filterToParent))
            {
                return query;
            }

            LambdaExpression lambdaExpression;
            if (!RelationalMetadata.TryGetFilterExpressionFromRelationalMap<TDocument>(querySettings.ParentEntityName, out lambdaExpression))
            {
                throw new ArgumentException(string.Format("Relational metadata should be added (Entity={0}, RelatedItem={1})", querySettings.ParentEntityName, typeof(TDocument).Name));
            }

            var bodyExpression = lambdaExpression.Body;
            var parameterExpression = lambdaExpression.Parameters.Single();
            var parentEntityIdExpression = Expression.Constant(querySettings.ParentEntityId, typeof(object));
            var equalsExpression = Expression.Equal(bodyExpression, parentEntityIdExpression);
            lambdaExpression = Expression.Lambda(equalsExpression, parameterExpression);

            var whereMethod = MethodInfos.Queryable.WhereMethodInfo.MakeGenericMethod(typeof(TDocument));
            var whereExpression = Expression.Call(whereMethod, query.Expression, lambdaExpression);
            return query.Provider.CreateQuery<TDocument>(whereExpression);
        }

        private IQueryable<TDocument> FieldFilter<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            if (string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                return query;
            }

            LambdaExpression[] lambdaExpressions;
            if (!FilteredFieldMetadata.TryGetFieldFilter<TDocument>(out lambdaExpressions))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определены поисковые поля", typeof(TDocument).Name));
            }

            var userInputExpression = CreateUserInputExpression<TDocument>(querySettings.UserInputFilter, lambdaExpressions);

            var whereMethod = MethodInfos.Queryable.WhereMethodInfo.MakeGenericMethod(typeof(TDocument));
            var whereExpression = Expression.Call(whereMethod, query.Expression, userInputExpression);

            return query.Provider.CreateQuery<TDocument>(whereExpression);
        }

        private LambdaExpression CreateUserInputExpression<TDocument>(string phrase, IEnumerable<LambdaExpression> lambdaExpressions)
        {
            Expression expression = null;
            var parameterExpression = Expression.Parameter(typeof(TDocument), "x");

            foreach (var lambdaExpression in lambdaExpressions)
            {
                var propertyInfo = GetPropertyInfo(lambdaExpression);

                if (propertyInfo.PropertyType == typeof(string))
                {
                    MethodInfo methodInfo;
                    string phraseTrimmed;

                    if (phrase.IndexOf('*') == 0)
                    {
                        phraseTrimmed = phrase.Trim('*');
                        methodInfo = MethodInfos.String.ContainsMethodInfo;
                    }
                    else
                    {
                        phraseTrimmed = phrase;
                        methodInfo = MethodInfos.String.StartsWithMethodInfo;
                    }

                    var phraseExpression = Expression.Constant(phraseTrimmed);
                    var memberExpression = Expression.Property(parameterExpression, propertyInfo);
                    var fieldFilterExpression = Expression.Call(memberExpression, methodInfo, phraseExpression);

                    if (expression != null)
                    {
                        expression = Expression.Or(expression, fieldFilterExpression);
                    }
                    else
                    {
                        expression = fieldFilterExpression;
                    }
                }
                else if (propertyInfo.PropertyType == typeof(short) ||
                        propertyInfo.PropertyType == typeof(int) ||
                        propertyInfo.PropertyType == typeof(long))
                {
                    long phraseParsed;
                    if (long.TryParse(phrase, out phraseParsed))
                    {
                        var phraseExpression = Expression.Constant(phraseParsed);
                        var memberExpression = Expression.Property(parameterExpression, propertyInfo);
                        var convertExpression = Expression.Convert(memberExpression, typeof(long));
                        var fieldFilterExpression = Expression.Equal(convertExpression, phraseExpression);

                        if (expression != null)
                        {
                            expression = Expression.Or(expression, fieldFilterExpression);
                        }
                        else
                        {
                            expression = fieldFilterExpression;
                        }
                    }
                }
            }

            if (expression == null)
            {
                throw new ArgumentException();
            }

            return Expression.Lambda(expression, parameterExpression);
        }

        private static PropertyInfo GetPropertyInfo(LambdaExpression lambdaExpression)
        {
            var body = lambdaExpression.Body;

            // Convert(expr) => expr
            var unaryExpression = body as UnaryExpression;
            if (unaryExpression != null)
            {
                body = unaryExpression.Operand;
            }

            var memberExpression = body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Property call expression must be implemented");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Property call expression must be implemented");
            }

            return propertyInfo;
        }

        private static RemoteCollection<TDocument> SortedPaged<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            var documentType = typeof(TDocument);

            PropertyInfo propertyInfo;
            if (!string.IsNullOrEmpty(querySettings.SortOrder))
            {
                // хак для сортировки по имени пользователя
                if (string.Equals(querySettings.SortOrder, "OwnerName"))
                {
                    querySettings.SortOrder = "OwnerCode";
                }

                // хак для сортировки по enum
                var sortOrderEnum = querySettings.SortOrder + "Enum";
                if (documentType.GetProperty(sortOrderEnum) != null)
                {
                    propertyInfo = documentType.GetProperty(sortOrderEnum);
                }
                else
                {
                    propertyInfo = documentType.GetProperty(querySettings.SortOrder);
                }
            }
            else
            {
                propertyInfo = documentType.GetProperty("Id");
            }
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

            var totalCount = query.Count();

            var querySorted = query.Provider.CreateQuery<TDocument>(callExpression);
            var querySortedPaged = querySorted
                .Skip(querySettings.SkipCount)
                .Take(querySettings.TakeCount)
                .ToList();

            return new RemoteCollection<TDocument>(querySortedPaged, totalCount);
        }
    }
}