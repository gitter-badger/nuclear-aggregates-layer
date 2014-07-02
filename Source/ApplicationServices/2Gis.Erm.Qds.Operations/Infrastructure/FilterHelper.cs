using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.Extensions;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Infrastructure
{
    public sealed class FilterHelper<TDocument> where TDocument : class, IAuthorizationDoc
    {
        private readonly IUserContext _userContext;
        private readonly IElasticApi _elasticApi;
        private readonly List<Func<FilterDescriptor<TDocument>, BaseFilter>> _filters = new List<Func<FilterDescriptor<TDocument>, BaseFilter>>();
        private readonly List<Func<QueryDescriptor<TDocument>, BaseQuery>> _queries = new List<Func<QueryDescriptor<TDocument>, BaseQuery>>();
        private Action<HighlightFieldDescriptor<TDocument>>[] _highlightedFields;

        public FilterHelper(IElasticApi elasticApi, IUserContext userContext)
        {
            _elasticApi = elasticApi;
            _userContext = userContext;
        }

        public FilterHelper<TDocument> AddFilter(Func<FilterDescriptor<TDocument>, BaseFilter> filter)
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

            var documents = (IList<TDocument>)searchResponse.Documents;
            return new RemoteCollection<TDocument>(documents, (int)searchResponse.Total);
        }

        private void AddUserInputQuery(QuerySettings querySettings)
        {
            if (string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                return;
            }

            LambdaExpression[] lambdaExpressions;
            if (!FilteredFieldMetadata.TryGetFieldFilter<TDocument>(out lambdaExpressions))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определены поисковые поля", typeof(TDocument).Name));
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
                _queries.Add(x => x.MultiMatch(y => y.Query(userInputFilterLower).OnFields(castedExpressions).Type(TextQueryType.PHRASE_PREFIX)));
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

            searchDescriptor.Highlight(y => y.OnFields(_highlightedFields).RequireFieldMatch(true));
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
            Func<FilterDescriptor<TDocument>, BaseFilter> filter;
            if (!QdsDefaultFilterMetadata.TryGetFilter(querySettings.FilterName, out filter))
            {
                throw new ArgumentException(string.Format("Для типа {0} не определен фильтр по умолчанию", typeof(TDocument).Name));
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
                throw new ArgumentException(string.Format("Relational metadata should be added (Entity={0}, RelatedItem={1})", querySettings.ParentEntityName, typeof(TDocument).Name));
            }

            Func<FilterDescriptor<TDocument>, BaseFilter> filter = x => x.Term((Expression<Func<TDocument, object>>)lambdaExpression, querySettings.ParentEntityId);
            _filters.Add(filter);
        }

        private void AddAuthorizationFilters(QuerySettings querySettings)
        {
            var userDoc = _elasticApi.Get<UserDoc>(_userContext.Identity.Code.ToString());
            if (userDoc == null)
            {
                throw new SecurityAccessDeniedException("Cannot find user");
            }

            AddTerritoryFilter(userDoc, querySettings);
            AddBranchFilter(userDoc, querySettings);
            AddPermissionsFilter(userDoc);
        }

        private void AddTerritoryFilter(UserDoc userDoc, QuerySettings querySettings)
        {
            bool myTerritory;
            if (!querySettings.TryGetExtendedProperty("MyTerritory", out myTerritory))
            {
                return;
            }

            Func<FilterDescriptor<TDocument>, BaseFilter> filter;

            const string Operation = "list";
            var permission = userDoc.Permissions.FirstOrDefault(x => x.Operation.Equals(Operation, StringComparison.OrdinalIgnoreCase));
            if (permission == null || permission.Tags == null)
            {
                // TODO {m.pashuk, 01.04.2014}: если нет прав то и запрос не делать
                filter = x => x.Not(y => y.MatchAll());
                _filters.Add(filter);
                return;
            }

            var allowedTags = permission.Tags.Where(x => x.StartsWith("byTerritoryId", StringComparison.OrdinalIgnoreCase));

            filter = x => x.Terms(y => y.Authorization.Tags, allowedTags);
            _filters.Add(filter);
        }

        private void AddBranchFilter(UserDoc userDoc, QuerySettings querySettings)
        {
            bool myBranch;
            if (!querySettings.TryGetExtendedProperty("MyBranch", out myBranch))
            {
                return;
            }

            Func<FilterDescriptor<TDocument>, BaseFilter> filter;

            const string Operation = "list";
            var permission = userDoc.Permissions.FirstOrDefault(x => x.Operation.Equals(Operation, StringComparison.OrdinalIgnoreCase));
            if (permission == null || permission.Tags == null)
            {
                // TODO {m.pashuk, 01.04.2014}: если нет прав то и запрос не делать
                filter = x => x.Not(y => y.MatchAll());
                _filters.Add(filter);
                return;
            }

            var allowedTags = permission.Tags.Where(x => x.StartsWith("byOrganizationUnit", StringComparison.OrdinalIgnoreCase));

            filter = x => x.Terms(y => y.Authorization.Tags, allowedTags);
            _filters.Add(filter);
        }

        private void AddPermissionsFilter(UserDoc userDoc)
        {
            var operation = "list/" + typeof(TDocument).Name;
            var permissions = userDoc.Permissions.Where(x => x.Operation.Equals(operation, StringComparison.OrdinalIgnoreCase));

            var permissionFilters = permissions.Select(permission =>
            {
                var allowedTags = new[] { operation }.AsEnumerable();
                if (permission.Tags != null)
                {
                    allowedTags = allowedTags.Concat(permission.Tags);
                }

                Func<FilterDescriptor<TDocument>, BaseFilter> tagsFilter = x => x.Terms(y => y.Authorization.Tags, allowedTags);
                return tagsFilter;
            }).ToArray();

            Func<FilterDescriptor<TDocument>, BaseFilter> filter;
            switch (permissionFilters.Length)
            {
                case 0:
                    {
                        // TODO {m.pashuk, 01.04.2014}: если нет прав то и запрос не делать
                        filter = x => x.Not(y => y.MatchAll());
                        _filters.Add(filter);
                    }
                    break;
                case 1:
                    {
                        filter = permissionFilters[0];
                        _filters.Add(filter);
                    }
                    break;
                default:
                    {
                        filter = x => x.Or(permissionFilters);
                        _filters.Add(filter);
                    }
                    break;
            }
        }

        private static PropertyInfo GetPropertyInfo(Type documentType, PropertyInfo[] properties, string propertyName)
        {
            var propertyInfo = Array.Find(properties, x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Для типа {0} не определено сортировочное поле {1}", documentType.Name, propertyName));
            }

            return propertyInfo;
        }

        private static SearchDescriptor<TDocument> SortedPaged(SearchDescriptor<TDocument> searchDescriptor, QuerySettings querySettings)
        {
            var documentType = typeof(TDocument);
            var properties = documentType.GetProperties();

            // sorting
            var sortFuncs = querySettings.Sort
            .Select(x =>
            {
                var propertyInfo = GetPropertyInfo(documentType, properties, x.PropertyName);

                var propertyName = propertyInfo.PropertyType == typeof(string) ?
                    x.PropertyName.ToCamelCase() + ".sort" :
                    x.PropertyName.ToCamelCase();

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

                return sortFunc;
            });

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
    }
}