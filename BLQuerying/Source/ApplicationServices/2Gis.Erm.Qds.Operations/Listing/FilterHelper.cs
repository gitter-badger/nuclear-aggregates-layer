using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class FilterHelper
    {
        private readonly IUserContext _userContext;
        private readonly IElasticApi _elasticApi;
        private readonly IQdsExtendedInfoFilterMetadata _extendedInfoFilterMetadata;

        public FilterHelper(IElasticApi elasticApi, IUserContext userContext, IQdsExtendedInfoFilterMetadata extendedInfoFilterMetadata)
        {
            _elasticApi = elasticApi;
            _userContext = userContext;
            _extendedInfoFilterMetadata = extendedInfoFilterMetadata;
        }

        public RemoteCollection<TDocument> Search<TDocument>(QuerySettings querySettings) where TDocument : class, IAuthorizationDoc
        {
            var descriptor = new FilterHelperDescriptor<TDocument>(querySettings);

            var filters = _extendedInfoFilterMetadata.GetExtendedInfoFilters<TDocument>(querySettings.ExtendedInfoMap);
            descriptor.AddFilters(filters);

            //descriptor.AddAuthorizationFilters();

            var searchResponse = _elasticApi.Search<TDocument>(descriptor.ApplyTo);

            var hitsMetadata = searchResponse.HitsMetaData;
            var documents = hitsMetadata.Hits.Select(HighlightDocument).ToArray();
            return new RemoteCollection<TDocument>(documents, (int)hitsMetadata.Total);
        }

        private void AddAuthorizationFilters<TDocument>(FilterHelperDescriptor<TDocument> descriptor) where TDocument : class, IAuthorizationDoc
        {
            var userDoc = _elasticApi.Get<UserAuthorizationDoc>(_userContext.Identity.Code.ToString());
            if (userDoc == null)
            {
                throw new SecurityAccessDeniedException("Cannot find user");
            }

            descriptor.AddPermissionsFilter(descriptor, userDoc);
        }

        private static TDocument HighlightDocument<TDocument>(IHit<TDocument> hit) where TDocument : class, IAuthorizationDoc
        {
            foreach (var highlight in hit.Highlights)
            {
                var propertyInfo = FilterHelperDescriptor<TDocument>.GetPropertyInfo(highlight.Key);
                var stringValue = highlight.Value.Highlights.Single();
                var convertedValue = Convert.ChangeType(stringValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(hit.Source, convertedValue);
            }

            return hit.Source;
        }
    }

    public sealed class FilterHelperDescriptor<TDocument>
            where TDocument : class, IAuthorizationDoc
    {
        private static readonly Type DocumentType = typeof(TDocument);
        private static readonly PropertyInfo[] DocumentProperties = typeof(TDocument).GetProperties();

        private readonly List<Func<FilterDescriptor<TDocument>, FilterContainer>> _filters = new List<Func<FilterDescriptor<TDocument>, FilterContainer>>();
        private readonly List<Func<QueryDescriptor<TDocument>, QueryContainer>> _queries = new List<Func<QueryDescriptor<TDocument>, QueryContainer>>();
        private readonly List<Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>>> _funcs = new List<Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>>>();

        public FilterHelperDescriptor(QuerySettings querySettings)
        {
            AddRelationalFilter(querySettings);
            AddUserInputQuery(querySettings);
            AddSortFuncs(querySettings);
        }

        public SearchDescriptor<TDocument> ApplyTo(SearchDescriptor<TDocument> searchDescriptor)
        {
            searchDescriptor = ApplyQueriesAndFilters(searchDescriptor);

            foreach (var func in _funcs)
            {
                func(searchDescriptor);
            }

            return searchDescriptor;
        }

        private SearchDescriptor<TDocument> ApplyQueriesAndFilters(SearchDescriptor<TDocument> searchDescriptor)
        {
            if (_queries.Count == 0 && _filters.Count == 0)
            {
                return searchDescriptor;
            }

            searchDescriptor = searchDescriptor.Query(query =>
            {
                if (_filters.Count != 0)
                {
                    return query.Filtered(filtered =>
                    {
                        if (_queries.Count != 0)
                        {
                            filtered.Query(ApplyQueries);
                        }

                        filtered.Filter(ApplyFilters);
                    });
                }

                return ApplyQueries(query);
            });

            return searchDescriptor;
        }

        private QueryContainer ApplyQueries(QueryDescriptor<TDocument> queryDescriptor)
        {
            switch (_queries.Count)
            {
                case 0:
                    return queryDescriptor;
                case 1:
                    return _queries[0](queryDescriptor);
                default:
                    return queryDescriptor.Bool(b => b.Should(_queries.ToArray()));
            }
        }

        private FilterContainer ApplyFilters(FilterDescriptor<TDocument> filterDescriptor)
        {
            switch (_filters.Count)
            {
                case 0:
                    return filterDescriptor;
                case 1:
                    return _filters[0](filterDescriptor);
                default:
                    return filterDescriptor.Bool(b => b.Must(_filters.ToArray()));
            }
        }

        public void AddFilter(Func<FilterDescriptor<TDocument>, FilterContainer> filter)
        {
            _filters.Add(filter);
        }

        public void AddFilters(IEnumerable<Func<FilterDescriptor<TDocument>, FilterContainer>> filters)
        {
            _filters.AddRange(filters);
        }

        private void AddSortFuncs(QuerySettings querySettings)
        {
            var sortInfos = querySettings.Sort.Select(x => new
            {
                x.Direction,
                PropertyInfo = GetPropertyInfo(x.PropertyName),
            }).ToArray();

            _funcs.AddRange(sortInfos
            .Select(x =>
            {
                var sortFieldName = x.PropertyInfo.PropertyType == typeof(string) ?
                    x.PropertyInfo.Name + ".sort" :
                    x.PropertyInfo.Name;
                sortFieldName = ToCamelCase(sortFieldName);

                Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> sortFunc;
                switch (x.Direction)
                {
                    case SortDirection.Ascending:
                        sortFunc = y => y.SortAscending(sortFieldName);
                        break;
                    case SortDirection.Descending:
                        sortFunc = y => y.SortDescending(sortFieldName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return sortFunc;
            }));

            // дополнительная сортировка по Id
            var hasIdProperty = sortInfos.Any(x => string.Equals(x.PropertyInfo.Name, "Id", StringComparison.OrdinalIgnoreCase));
            if (!hasIdProperty)
            {
                _funcs.Add(y => y.SortAscending("id"));
            }

            // paging
            if (querySettings.SkipCount != 0 || querySettings.TakeCount != 0)
            {
                _funcs.Add(y => y.From(querySettings.SkipCount).Size(querySettings.TakeCount));
            }
        }

        private void AddUserInputQuery(QuerySettings querySettings)
        {
            if (string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                return;
            }

            LambdaExpression[] lambdaExpressions;
            if (!FilteredFieldsMetadata.TryGetFieldFilter<TDocument>(out lambdaExpressions))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определены поисковые поля", DocumentType.Name));
            }

            var castedExpressions = (Expression<Func<TDocument, object>>[])lambdaExpressions;

            var userInputFilterLower = querySettings.UserInputFilter.ToLowerInvariant();
            if (userInputFilterLower.Contains("*") || userInputFilterLower.Contains("?"))
            {
                foreach (var castedExpression in castedExpressions)
                {
                    var expression = castedExpression;
                    _queries.Add(x => x.Wildcard(expression, userInputFilterLower));
                }
            }
            else
            {
                _queries.Add(x => x.MultiMatch(y => y.Query(userInputFilterLower).OnFields(castedExpressions).Type(TextQueryType.PhrasePrefix)));
            }

            var highlightedFields = castedExpressions.Select(x =>
            {
                Action<HighlightFieldDescriptor<TDocument>> action = y => y.OnField(x);
                return action;
            }).ToArray();
            if (highlightedFields.Any())
            {
                _funcs.Add(x => x.Highlight(y => y
                    .OnFields(highlightedFields)
                    .PreTags("<span class='highlightedText'>")
                    .PostTags("</span>")
                    .RequireFieldMatch(true)
                    )
                );
            }
        }

        private void AddRelationalFilter(QuerySettings querySettings)
        {
            bool filterToParent;
            if (!querySettings.TryGetExtendedProperty("filterToParent", out filterToParent))
            {
                return;
            }

            // никогда не ограничиваем по null и 0 (можно убрать после того как зарефакторим js)
            if (querySettings.ParentEntityId == null || querySettings.ParentEntityId.Value == 0)
            {
                return;
            }

            LambdaExpression lambdaExpression;
            if (!RelationalMetadata.TryGetFilterExpressionFromRelationalMap<TDocument>(querySettings.ParentEntityName, out lambdaExpression))
            {
                throw new ArgumentException(string.Format("Relational metadata should be added (Entity={0}, RelatedItem={1})", querySettings.ParentEntityName, DocumentType.Name));
            }

            Func<FilterDescriptor<TDocument>, FilterContainer> filter = x => x.Term((Expression<Func<TDocument, object>>)lambdaExpression, querySettings.ParentEntityId);
            _filters.Add(filter);
        }

        public void AddPermissionsFilter(FilterHelperDescriptor<TDocument> descriptor, UserAuthorizationDoc userDoc)
        {
            var operation = "List/" + DocumentType.Name;

            var permissions = userDoc.Permissions
                .Where(x => x.Operation.Equals(operation, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (!permissions.Any())
            {
                descriptor._filters.Add(x => x.Not(y => y.MatchAll()));
                return;
            }

            Func<FilterDescriptor<TDocument>, FilterContainer> operationFilter = x => x.Term(y => y.Authorization.Operations, operation);
            descriptor._filters.Add(operationFilter);

            var noTagsNeeded = permissions.Any(x => x.Tags.Count == 0);
            if (noTagsNeeded)
            {
                return;
            }

            var tags = permissions
                .SelectMany(x => x.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

            Func<FilterDescriptor<TDocument>, FilterContainer> tagsFilter;
            switch (tags.Length)
            {
                case 0:
                    return;
                case 1:
                    tagsFilter = x => x.Term(y => y.Authorization.Tags, tags.First());
                    break;
                default:
                    tagsFilter = x => x.Terms(y => y.Authorization.Tags, tags);
                    break;
            }

            descriptor._filters.Add(tagsFilter);
        }

        public static PropertyInfo GetPropertyInfo(string propertyName)
        {
            var propertyInfo = Array.Find(DocumentProperties, x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Для типа {0} не определено сортировочное поле {1}", DocumentType.Name, propertyName));
            }

            return propertyInfo;
        }

        private static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (!char.IsUpper(s[0]))
            {
                return s;
            }

            var str = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            if (s.Length > 1)
            {
                str = str + s.Substring(1);
            }

            return str;
        }
    }
}