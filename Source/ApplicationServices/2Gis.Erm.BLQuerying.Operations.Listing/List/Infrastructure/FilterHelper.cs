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
            var query1 = DefaultFilter(query, querySettings);
            var query2 = RelativeFilter(query1, querySettings);
            var query3 = FieldFilter(query2, querySettings);

            return SortedPaged(query3, querySettings);
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

            var castExpression = (UnaryExpression)lambdaExpression.Body;
            var propertyExpression = castExpression.Operand;
            var parentEntityIdExpression = Expression.Constant(querySettings.ParentEntityId, propertyExpression.Type);
            var equalsExpression = Expression.Equal(propertyExpression, parentEntityIdExpression);
            var parameterExpression = lambdaExpression.Parameters.Single();
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

        private static PropertyInfo GetPropertyInfo(Type documentType, PropertyInfo[] properties, string propertyName)
        {
            // хак для сортировки по Ответственному
            if (string.Equals(propertyName, "AuthorName"))
            {
                propertyName = "AuthorId";
            }

            // хак для сортировки по имени пользователя
            if (string.Equals(propertyName, "OwnerName"))
            {
                propertyName = "OwnerCode";
            }
            else
            {
                // хак для сортировки по enum
                var enumPropertyName = propertyName + "Enum";
                var enumPropertyInfo = Array.Find(properties, x => x.Name.Equals(enumPropertyName, StringComparison.OrdinalIgnoreCase));
                if (enumPropertyInfo != null)
                {
                    return enumPropertyInfo;
                }
            }

            var propertyInfo = Array.Find(properties, x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Для типа {0} не определено поле для сортировки {1}", documentType.Name, propertyName));
            }

            return propertyInfo;
        }

        private static RemoteCollection<TDocument> SortedPaged<TDocument>(IQueryable<TDocument> query, QuerySettings querySettings)
        {
            var documentType = typeof(TDocument);
            var properties = documentType.GetProperties();

            var sorts1 = querySettings.Sort
            .Select(x => new
            {
                PropertyInfo = GetPropertyInfo(documentType, properties, x.PropertyName),
                x.Direction,
            })
            .ToList();

            // дополнительная сортировка по Id
            var hasidProperty = sorts1.Any(x => string.Equals(x.PropertyInfo.Name, "Id", StringComparison.OrdinalIgnoreCase));
            if (!hasidProperty)
            {
                sorts1.Add(new
                {
                    PropertyInfo = GetPropertyInfo(documentType, properties, "Id"),
                    Direction = SortDirection.Ascending,
                });
            }

            var sorts2 = sorts1.Select((x, index) =>
            {
                MethodInfo methodInfo;
                switch (x.Direction)
                {
                    case SortDirection.Ascending:
                        methodInfo = (index == 0) ?
                            MethodInfos.Queryable.OrderByMethodInfo.MakeGenericMethod(documentType, x.PropertyInfo.PropertyType) :
                            MethodInfos.Queryable.ThenByMethodInfo.MakeGenericMethod(documentType, x.PropertyInfo.PropertyType);
                        break;
                    case SortDirection.Descending:
                        methodInfo = (index == 0) ?
                            MethodInfos.Queryable.OrderByDescendingMethodInfo.MakeGenericMethod(documentType, x.PropertyInfo.PropertyType) :
                            MethodInfos.Queryable.ThenByDescendingMethodInfo.MakeGenericMethod(documentType, x.PropertyInfo.PropertyType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var parameterExpression = Expression.Parameter(documentType, "x");
                var propertyExpression = Expression.Property(parameterExpression, x.PropertyInfo);
                var lambdaExpression = Expression.Lambda(propertyExpression, parameterExpression);

                return new
                {
                    LambdaExpression = lambdaExpression,
                    MethodInfo = methodInfo,
                };
            });

            var querySorted = query;
            foreach (var sort2 in sorts2)
            {
                var callExpression = Expression.Call(sort2.MethodInfo, querySorted.Expression, sort2.LambdaExpression);
                querySorted = querySorted.Provider.CreateQuery<TDocument>(callExpression);
            }

            var totalCount = query.Count();

            var querySortedPaged = querySorted
                .Skip(querySettings.SkipCount)
                .Take(querySettings.TakeCount)
                .ToList();

            return new RemoteCollection<TDocument>(querySortedPaged, totalCount);
        }
    }
}