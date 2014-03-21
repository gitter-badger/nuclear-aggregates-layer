using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Common.Extensions
{
    public static class SearchExtensions
    {
        public static SearchDescriptor<TDocument> ApplySortingPaging<TDocument>(this SearchDescriptor<TDocument> searchDescriptor, QuerySettings querySettings)
            where TDocument : class
        {
            // TODO пока выключил sorting пока не смёрджим заддачу ERM-3203
            // sorting
            if (!string.IsNullOrEmpty(querySettings.SortOrder))
            {
                var sortOrder = querySettings.SortOrder.ToCamelCase() + ".sort";
                if (string.Equals(querySettings.SortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                {
                    searchDescriptor = searchDescriptor.SortDescending(sortOrder);
                }
                else
                {
                    searchDescriptor = searchDescriptor.SortAscending(sortOrder);
                }
            }

            // paging
            if (querySettings.SkipCount != 0 || querySettings.TakeCount != 0)
            {
                searchDescriptor = searchDescriptor.Skip(querySettings.SkipCount).Take(querySettings.TakeCount);
            }

            return searchDescriptor;
        }

        public static QueryDescriptor<TDocument> ApplyQuerySettings<TDocument>(this QueryDescriptor<TDocument> queryDescriptor, QuerySettings querySettings)
            where TDocument : class
        {
            // user input
            if (!string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                var userInputExpression = DocumentMetadata.GetUserInputPropertyFor<TDocument>();

                var userInputFilterLower = querySettings.UserInputFilter.ToLowerInvariant();
                userInputFilterLower = (userInputFilterLower.Contains('*') || userInputFilterLower.Contains('?'))
                                           ? userInputFilterLower
                                           : string.Concat("*", userInputFilterLower, "*");

                queryDescriptor = (QueryDescriptor<TDocument>)queryDescriptor.Bool(x => x.Must(new Func<QueryDescriptor<TDocument>, BaseQuery>[]
                    {
                        y => y.Wildcard(userInputExpression, userInputFilterLower)
                    }));
            }
            else
            {
                queryDescriptor = (QueryDescriptor<TDocument>)queryDescriptor.MatchAll();
            }

            return queryDescriptor;
        }

        public static FilterDescriptor<TDocument> ApplyUserPermissions<TDocument>(this FilterDescriptor<TDocument> filterDescriptor, UserDoc userDoc)
            where TDocument : class, IAuthorizationDoc
        {
            if (!userDoc.Authorization.Tags.Contains("organization"))
            {
                filterDescriptor = (FilterDescriptor<TDocument>)filterDescriptor.Terms(x => x.Authorization.Tags, userDoc.Authorization.Tags);
            }

            return filterDescriptor;
        }

        public static string ToCamelCase(this string s)
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