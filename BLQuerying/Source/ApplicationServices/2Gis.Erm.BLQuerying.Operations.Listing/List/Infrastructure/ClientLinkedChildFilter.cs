using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public class ClientLinkedChildFilter:DescendantEntityFilter
    {
        private readonly IFinder _finder;

        public ClientLinkedChildFilter(IFinder finder)
        {
            _finder = finder;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> queryable, long clientId)
        {
            var descendantIds =
                _finder.Find(new FindSpecification<DenormalizedClientLink>(x => x.MasterClientId == clientId && x.IsLinkedDirectly ))
                       .Map(q => q.Select(x => x.ChildClientId))
                       .Many()
                       .Union(new[] { clientId });

            return Apply<TEntity>(queryable, descendantIds, "ClientId");
        }
    }
}
