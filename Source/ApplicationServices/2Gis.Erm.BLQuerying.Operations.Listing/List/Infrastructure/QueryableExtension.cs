using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public static class QueryableExtension
    {
        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, FilterHelper filterHelper, params Expression<Func<TEntity, bool>>[] expressions)
        {
            return filterHelper.Filter(query, expressions);
        }

        public static IQueryable<TEntity> DefaultFilter<TEntity>(this IQueryable<TEntity> query, FilterHelper filterHelper, QuerySettings querySettings)
        {
            return filterHelper.DefaultFilter(query, querySettings);
        }

        public static IEnumerable<TDocument> QuerySettings<TDocument>(this IQueryable<TDocument> query,
                                                                      FilterHelper filterHelper,
                                                                      QuerySettings querySettings,
                                                                      out int total)
        {
            return filterHelper.QuerySettings(query, querySettings, out total);
        }
    }
}
