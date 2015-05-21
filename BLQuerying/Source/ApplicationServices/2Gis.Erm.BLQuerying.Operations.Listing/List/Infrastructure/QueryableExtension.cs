using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public static class QueryableExtension
    {
        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, FilterHelper filterHelper, params Expression<Func<TEntity, bool>>[] expressions)
        {
            return filterHelper.Filter(query, expressions);
        }

        public static IQueryable<TEntity> FilterBySpec<TEntity>(this IQueryable<TEntity> query, FilterHelper filterHelper, params FindSpecification<TEntity>[] specifications)
            where TEntity : class
        {
            return filterHelper.Filter(query, specifications);
        }

        public static RemoteCollection<TDocument> QuerySettings<TDocument>(this IQueryable<TDocument> query,
                                                                           FilterHelper filterHelper,
                                                                           QuerySettings querySettings)
        {
            return filterHelper.QuerySettings(query, querySettings);
        }
    }
}
