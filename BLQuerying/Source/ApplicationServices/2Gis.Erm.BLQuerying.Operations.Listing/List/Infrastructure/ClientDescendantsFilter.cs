using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class ClientDescendantsFilter : DescendantEntityFilter
    {
        private readonly IFinder _finder;

        public ClientDescendantsFilter(IFinder finder)
        {
            _finder = finder;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> queryable, long clientId)
        {
            var descendantIds =
                _finder.Find(new FindSpecification<DenormalizedClientLink>(x => x.MasterClientId == clientId))
                       .Select(x => x.ChildClientId)
                       .ToArray()
                       .Union(new[] { clientId });

            return Apply<TEntity>(queryable, descendantIds, "ClientId");
        }
    }
}