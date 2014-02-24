using System;
using System.Globalization;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
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
            //if (!string.IsNullOrEmpty(querySettings.SortOrder))
            //{
            //    var sortOrder = querySettings.SortOrder.ToCamelCase() + ".sort";
            //    if (string.Equals(querySettings.SortDirection, "desc", StringComparison.OrdinalIgnoreCase))
            //    {
            //        searchDescriptor.SortDescending(sortOrder);
            //    }
            //    else
            //    {
            //        searchDescriptor.SortAscending(sortOrder);
            //    }
            //}

            // paging
            if (querySettings.SkipCount != 0 || querySettings.TakeCount != 0)
            {
                searchDescriptor.Skip(querySettings.SkipCount).Take(querySettings.TakeCount);
            }

            return searchDescriptor;
        }

        public static BoolQueryDescriptor<TDocument> Must<TDocument>(this BoolQueryDescriptor<TDocument> boolQueryDescriptor, Func<MustDescriptor<TDocument>, MustDescriptor<TDocument>> f)
            where TDocument : class, IAuthDoc
        {
            var mustDescriptor = f(new MustDescriptor<TDocument>());
            return boolQueryDescriptor.Must(mustDescriptor.Queries);
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