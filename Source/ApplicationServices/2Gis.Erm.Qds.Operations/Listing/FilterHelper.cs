﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class FilterHelper<TDocument> where TDocument : class, IAuthorizationDoc
    {
        private static readonly Type DocumentType = typeof(TDocument);
        private static readonly PropertyInfo[] DocumentProperties = typeof(TDocument).GetProperties();

        private readonly IUserContext _userContext;
        private readonly IElasticApi _elasticApi;
        private readonly List<Func<FilterDescriptor<TDocument>, FilterContainer>> _filters = new List<Func<FilterDescriptor<TDocument>, FilterContainer>>();
        private readonly List<Func<QueryDescriptor<TDocument>, QueryContainer>> _queries = new List<Func<QueryDescriptor<TDocument>, QueryContainer>>();
        private Action<HighlightFieldDescriptor<TDocument>>[] _highlightedFields;

        public FilterHelper(IElasticApi elasticApi, IUserContext userContext)
        {
            _elasticApi = elasticApi;
            _userContext = userContext;
        }

        public FilterHelper<TDocument> AddFilter(Func<FilterDescriptor<TDocument>, FilterContainer> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public RemoteCollection<TDocument> Search(QuerySettings querySettings)
        {
            AddRelationalFilter(querySettings);
            AddDefaultFilter(querySettings);
            AddUserInputQuery(querySettings);
            //AddAuthorizationFilters(querySettings);

            var searchResponse = _elasticApi.Search<TDocument>(x =>
            {
                ApplyFilters(x);
                ApplyQueries(x);
                ApplyHignlighting(x);

                return SortedPaged(x, querySettings);
            });
            var hitsMetadata = searchResponse.HitsMetaData;

            var documents = hitsMetadata.Hits.Select(HighlightDocument).ToArray();
            return new RemoteCollection<TDocument>(documents, (int)hitsMetadata.Total);
        }

        private static TDocument HighlightDocument(IHit<TDocument> hit)
        {
            foreach (var highlight in hit.Highlights)
            {
                var propertyInfo = GetPropertyInfo(highlight.Key);
                var stringValue = highlight.Value.Highlights.Single();
                var convertedValue = Convert.ChangeType(stringValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(hit.Source, convertedValue);
            }

            return hit.Source;
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

            _highlightedFields = castedExpressions.Select(x =>
            {
                Action<HighlightFieldDescriptor<TDocument>> action = y => y.OnField(x);
                return action;
            }).ToArray();
        }

        private void ApplyQueries(SearchDescriptor<TDocument> searchDescriptor)
        {
            switch (_queries.Count)
            {
                case 0:
                    break;
                case 1:
                    searchDescriptor.Query(_queries[0]);
                    break;
                default:
                    searchDescriptor.Query(y => y.Bool(z => z.Should(_queries.ToArray())));
                    break;
            }
        }

        private void ApplyHignlighting(SearchDescriptor<TDocument> searchDescriptor)
        {
            if (_highlightedFields == null || _highlightedFields.Length == 0)
            {
                return;
            }

            searchDescriptor.Highlight(y => y
                .OnFields(_highlightedFields)
                .PreTags("<span class='highlightedText'>")
                .PostTags("</span>")
                .RequireFieldMatch(true));
        }

        private void ApplyFilters(SearchDescriptor<TDocument> searchDescriptor)
        {
            switch (_filters.Count)
            {
                case 0:
                    break;
                case 1:
                    searchDescriptor.Filter(_filters[0]);
                    break;
                default:
                    searchDescriptor.Filter(y => y.And(_filters.ToArray()));
                    break;
            }
        }

        private void AddDefaultFilter(QuerySettings querySettings)
        {
            Func<FilterDescriptor<TDocument>, FilterContainer> filter;
            if (!QdsDefaultFilterMetadata.TryGetFilter(querySettings.FilterName, out filter))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определен фильтр по умолчанию", DocumentType.Name));
            }

            _filters.Add(filter);
        }

        private void AddRelationalFilter(QuerySettings querySettings)
        {
            bool filterToParent;
            if (!querySettings.TryGetExtendedProperty("filterToParent", out filterToParent))
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

        private void AddAuthorizationFilters(QuerySettings querySettings)
        {
            var userDoc = _elasticApi.Get<UserAuthorizationDoc>(_userContext.Identity.Code.ToString());
            if (userDoc == null)
            {
                throw new SecurityAccessDeniedException("Cannot find user");
            }

            //AddTerritoryFilter(userDoc, querySettings);
            //AddBranchFilter(userDoc, querySettings);
            AddPermissionsFilter(userDoc);
        }

        //private void AddTerritoryFilter(UserPermissionsDoc userDoc, QuerySettings querySettings)
        //{
        //    bool myTerritory;
        //    if (!querySettings.TryGetExtendedProperty("MyTerritory", out myTerritory))
        //    {
        //        return;
        //    }

        //    Func<FilterDescriptor<TDocument>, FilterContainer> filter;

        //    switch (userDoc.TerritoryIds.Count)
        //    {
        //        case 0:
        //            // TODO {m.pashuk, 01.04.2014}: если нет прав то и запрос не делать
        //            filter = x => x.Not(y => y.MatchAll());
        //            break;
        //        case 1:
        //            filter = x => x.Term("territoryId", userDoc.TerritoryIds.First());
        //            break;
        //        default:
        //            filter = x => x.Terms("territoryId", userDoc.TerritoryIds);
        //            break;
        //    }

        //    _filters.Add(filter);
        //}

        //private void AddBranchFilter(UserPermissionsDoc userDoc, QuerySettings querySettings)
        //{
        //    bool myBranch;
        //    if (!querySettings.TryGetExtendedProperty("MyBranch", out myBranch))
        //    {
        //        return;
        //    }

        //    Func<FilterDescriptor<TDocument>, FilterContainer> filter;

        //    switch (userDoc.OrganizationUnitIds.Count)
        //    {
        //        case 0:
        //            // TODO {m.pashuk, 01.04.2014}: если нет прав то и запрос не делать
        //            filter = x => x.Not(y => y.MatchAll());
        //            break;
        //        case 1:
        //            filter = x => x.Term("organizationUnitId", userDoc.OrganizationUnitIds.First());
        //            break;
        //        default:
        //            filter = x => x.Terms("organizationUnitId", userDoc.OrganizationUnitIds);
        //            break;
        //    }

        //    _filters.Add(filter);
        //}

        private void AddPermissionsFilter(UserAuthorizationDoc userDoc)
        {
            var operation = "List/" + DocumentType.Name;

            var permissions = userDoc.Permissions
                .Where(x => x.Operation.Equals(operation, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (!permissions.Any())
            {
                _filters.Add(x => x.Not(y => y.MatchAll()));
                return;
            }

            Func<FilterDescriptor<TDocument>, FilterContainer> operationFilter = x => x.Term(y => y.Authorization.Operations, operation);
            _filters.Add(operationFilter);

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

            _filters.Add(tagsFilter);
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            var propertyInfo = Array.Find(DocumentProperties, x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Для типа {0} не определено сортировочное поле {1}", DocumentType.Name, propertyName));
            }

            return propertyInfo;
        }

        private static SearchDescriptor<TDocument> SortedPaged(SearchDescriptor<TDocument> searchDescriptor, QuerySettings querySettings)
        {
            // sorting
            var sorts1 = querySettings.Sort
            .Select(x =>
            {
                var propertyInfo = GetPropertyInfo(x.PropertyName);

                var propertyName = propertyInfo.PropertyType == typeof(string) ?
                    x.PropertyName + ".sort" :
                    x.PropertyName;
                propertyName = ToCamelCase(propertyName);

                Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> sortFunc;
                switch (x.Direction)
                {
                    case SortDirection.Ascending:
                        sortFunc = y => y.SortAscending(propertyName);
                        break;
                    case SortDirection.Descending:
                        sortFunc = y => y.SortDescending(propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new
                {
                    PropertyName = propertyName,
                    SortFunc = sortFunc,
                };
            })
            .ToList();

            // дополнительная сортировка по Id
            var hasidProperty = sorts1.Any(x => string.Equals(x.PropertyName, "id", StringComparison.OrdinalIgnoreCase));
            if (!hasidProperty)
            {
                sorts1.Add(new
                {
                    PropertyName = "id",
                    SortFunc = new Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>>(x => x.SortAscending("id")),
                });
            }

            var sortFuncs = sorts1.Select(x => x.SortFunc);
            foreach (var sortFunc in sortFuncs)
            {
                sortFunc(searchDescriptor);
            }

            // paging
            if (querySettings.SkipCount != 0 || querySettings.TakeCount != 0)
            {
                searchDescriptor = searchDescriptor.From(querySettings.SkipCount).Size(querySettings.TakeCount);
            }

            return searchDescriptor;
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